// 作者: 木枫LL
// GitHub: https://github.com/MuFengSteam/
// 小红书: https://www.xiaohongshu.com/user/profile/67c5dc1a000000000a03e5d0

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("LGUI/UIBindList")]
[RequireComponent(typeof(RectTransform))]
public class UIBindList : UIBase
{
    public enum ListMode
    {

        FullGeneration,

        LoopingList
    }

    [Header("List设置")]
    [Tooltip("列表模式")]
    public ListMode mode = ListMode.FullGeneration;

    [Tooltip("列表项模板")]
    public UIBindTemplate itemTemplate;

    [Tooltip("内容容器（ScrollRect的Content）")]
    public RectTransform contentTransform;

    [Tooltip("ScrollRect组件")]
    public ScrollRect scrollRect;

    [Header("循环列表设置")]
    [Tooltip("可见项数量")]
    public int visibleItems = 10;

    [Tooltip("项间距")]
    public float itemSpacing = 5f;

    [Tooltip("是否垂直滚动")]
    public bool vertical = true;

    [Header("其他设置")]
    [Tooltip("启动时是否清空")]
    public bool clearOnStart = true;

    public override string ComponentTypeName => "UIBindList";
    public override string BindDataType => "UIBindList";

    private List<GameObject> _activeItems = new List<GameObject>();
    private RectTransform _rectTransform;
    private float _itemSize;
    private int _totalItemCount;
    private Dictionary<int, GameObject> _itemPool = new Dictionary<int, GameObject>();
    private float _viewportSize;
    private Vector2 _lastScrollPosition;
    private int _startIndex;

    private int _instanceCounter = 0;

    private Action<int, GameObject> _legacyBindCallback;
    private System.Delegate _typedBindCallback;
    private IList _dataSource;

#if UNITY_EDITOR

    protected override void Reset()
    {

        if (string.IsNullOrEmpty(bindName))
        {
            bindName = gameObject.name;
        }

        if (contentTransform == null)
        {
            contentTransform = GetComponent<RectTransform>();
        }

        CreateScrollRectStructure();
    }
#endif

#if UNITY_EDITOR

    private void CreateScrollRectStructure()
    {

        if (scrollRect != null)
            return;

        ScrollRect existingScrollRect = GetComponentInChildren<ScrollRect>();
        if (existingScrollRect != null)
        {
            scrollRect = existingScrollRect;
            if (scrollRect.content != null)
                contentTransform = scrollRect.content;
            return;
        }

        GameObject scrollViewGO = new GameObject("ScrollView");
        scrollViewGO.transform.SetParent(transform, false);

        RectTransform scrollViewRect = scrollViewGO.AddComponent<RectTransform>();
        scrollViewRect.anchorMin = Vector2.zero;
        scrollViewRect.anchorMax = Vector2.one;
        scrollViewRect.offsetMin = Vector2.zero;
        scrollViewRect.offsetMax = Vector2.zero;

        UnityEngine.UI.Image scrollViewImage = scrollViewGO.AddComponent<UnityEngine.UI.Image>();
        scrollViewImage.color = new Color(1, 1, 1, 1);
        scrollViewImage.raycastTarget = true;

        ScrollRect newScrollRect = scrollViewGO.AddComponent<ScrollRect>();
        newScrollRect.horizontal = !vertical;
        newScrollRect.vertical = vertical;
        newScrollRect.movementType = ScrollRect.MovementType.Elastic;
        newScrollRect.elasticity = 0.1f;
        newScrollRect.inertia = true;
        newScrollRect.decelerationRate = 0.135f;
        newScrollRect.scrollSensitivity = 1f;

        GameObject viewportGO = new GameObject("Viewport");
        viewportGO.transform.SetParent(scrollViewGO.transform, false);

        RectTransform viewportRect = viewportGO.AddComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.offsetMin = Vector2.zero;
        viewportRect.offsetMax = Vector2.zero;
        viewportRect.pivot = new Vector2(0, 1);

        UnityEngine.UI.Image viewportImage = viewportGO.AddComponent<UnityEngine.UI.Image>();
        viewportImage.color = new Color(1, 1, 1, 0);
        viewportImage.raycastTarget = true;

        viewportGO.AddComponent<UnityEngine.UI.RectMask2D>();

        GameObject contentGO = new GameObject("Content");
        contentGO.transform.SetParent(viewportGO.transform, false);

        RectTransform contentRect = contentGO.AddComponent<RectTransform>();

        if (vertical)
        {
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.pivot = new Vector2(0.5f, 1);
        }
        else
        {
            contentRect.anchorMin = new Vector2(0, 0);
            contentRect.anchorMax = new Vector2(0, 1);
            contentRect.pivot = new Vector2(0, 0.5f);
        }
        contentRect.offsetMin = Vector2.zero;
        contentRect.offsetMax = Vector2.zero;
        contentRect.sizeDelta = Vector2.zero;

        ContentSizeFitter sizeFitter = contentGO.AddComponent<ContentSizeFitter>();
        sizeFitter.horizontalFit = vertical ? ContentSizeFitter.FitMode.Unconstrained : ContentSizeFitter.FitMode.PreferredSize;
        sizeFitter.verticalFit = vertical ? ContentSizeFitter.FitMode.PreferredSize : ContentSizeFitter.FitMode.Unconstrained;

        if (vertical)
        {
            VerticalLayoutGroup layoutGroup = contentGO.AddComponent<VerticalLayoutGroup>();
            layoutGroup.spacing = itemSpacing;
            layoutGroup.childForceExpandWidth = true;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childControlWidth = true;
            layoutGroup.childControlHeight = false;
            layoutGroup.childAlignment = TextAnchor.UpperCenter;
        }
        else
        {
            HorizontalLayoutGroup layoutGroup = contentGO.AddComponent<HorizontalLayoutGroup>();
            layoutGroup.spacing = itemSpacing;
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.childForceExpandHeight = true;
            layoutGroup.childControlWidth = false;
            layoutGroup.childControlHeight = true;
            layoutGroup.childAlignment = TextAnchor.MiddleLeft;
        }

        newScrollRect.viewport = viewportRect;
        newScrollRect.content = contentRect;

        scrollRect = newScrollRect;
        contentTransform = contentRect;

        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.EditorUtility.SetDirty(gameObject);
    }
#endif

    protected override void OnEnable()
    {
        base.OnEnable();

        _rectTransform = GetComponent<RectTransform>();

        if (scrollRect == null)
            scrollRect = GetComponent<ScrollRect>();

        if (contentTransform == null && scrollRect != null)
            contentTransform = scrollRect.content;

        HideTemplate();

        if (clearOnStart)
            ClearItems();

        if (scrollRect != null && mode == ListMode.LoopingList)
            scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
    }

    private void HideTemplate()
    {
        if (itemTemplate != null && itemTemplate.gameObject.activeSelf)
        {
            itemTemplate.gameObject.SetActive(false);
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (scrollRect != null)
            scrollRect.onValueChanged.RemoveListener(OnScrollValueChanged);
    }

    public void ClearItems()
    {
        foreach (var item in _activeItems)
        {
            if (item != null)
                DestroyImmediate(item);
        }

        _activeItems.Clear();
        _itemPool.Clear();
        _dataSource = null;
        _typedBindCallback = null;
        _legacyBindCallback = null;
        _instanceCounter = 0;
    }

    public void SetList<T>(List<T> dataList, Action<int, T, UIBindTemplate> bindItemFunc)
    {
        ClearItems();

        if (dataList == null || dataList.Count == 0 || itemTemplate == null)
            return;

        HideTemplate();

        _dataSource = dataList;
        _typedBindCallback = bindItemFunc;
        _totalItemCount = dataList.Count;

        if (mode == ListMode.FullGeneration)
        {
            GenerateFullItems<T>(dataList, bindItemFunc);
        }
        else
        {
            SetupLoopingList<T>(dataList, bindItemFunc);
        }
    }

    public void SetList(int count, Action<int, UIBindTemplate> bindItemFunc)
    {
        ClearItems();

        if (count <= 0 || itemTemplate == null)
            return;

        HideTemplate();

        _totalItemCount = count;

        Action<int, GameObject> legacyCallback = (index, go) => {
            var template = go.GetComponent<UIBindTemplate>();
            if (template != null)
            {
                bindItemFunc(index, template);
            }
        };

        _legacyBindCallback = legacyCallback;

        if (mode == ListMode.FullGeneration)
        {
            GenerateFullItems(count);
        }
        else
        {
            SetupLoopingList(count);
        }
    }

    private void GenerateFullItems<T>(List<T> dataList, Action<int, T, UIBindTemplate> bindItemFunc)
    {
        for (int i = 0; i < dataList.Count; i++)
        {
            GameObject item = CreateItem();
            _activeItems.Add(item);

            var template = item.GetComponent<UIBindTemplate>();
            if (template != null && bindItemFunc != null)
            {
                bindItemFunc(i, dataList[i], template);
            }
        }

        if (contentTransform != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentTransform);
    }

    private void GenerateFullItems(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject item = CreateItem();
            _activeItems.Add(item);

            if (_legacyBindCallback != null)
                _legacyBindCallback(i, item);
        }

        if (contentTransform != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentTransform);
    }

    private void SetupLoopingList<T>(List<T> dataList, Action<int, T, UIBindTemplate> bindItemFunc)
    {

        GameObject sizeReference = CreateItem(false);
        RectTransform itemRect = sizeReference.GetComponent<RectTransform>();
        _itemSize = vertical ? itemRect.rect.height : itemRect.rect.width;
        DestroyImmediate(sizeReference);

        if (contentTransform != null)
        {
            if (vertical)
            {
                contentTransform.sizeDelta = new Vector2(
                    contentTransform.sizeDelta.x,
                    _itemSize * _totalItemCount + itemSpacing * (_totalItemCount - 1)
                );
            }
            else
            {
                contentTransform.sizeDelta = new Vector2(
                    _itemSize * _totalItemCount + itemSpacing * (_totalItemCount - 1),
                    contentTransform.sizeDelta.y
                );
            }
        }

        _viewportSize = vertical ? _rectTransform.rect.height : _rectTransform.rect.width;
        int visibleCount = Mathf.Min(visibleItems, _totalItemCount);

        for (int i = 0; i < visibleCount; i++)
        {
            GameObject item = CreateItem();
            _activeItems.Add(item);
            _itemPool[i] = item;

            PositionItem(item, i);

            var template = item.GetComponent<UIBindTemplate>();
            if (template != null && bindItemFunc != null)
            {
                bindItemFunc(i, dataList[i], template);
            }
        }

        _lastScrollPosition = scrollRect != null ? scrollRect.normalizedPosition : Vector2.one;
    }

    private void SetupLoopingList(int count)
    {
        _totalItemCount = count;

        GameObject sizeReference = CreateItem(false);
        RectTransform itemRect = sizeReference.GetComponent<RectTransform>();
        _itemSize = vertical ? itemRect.rect.height : itemRect.rect.width;
        DestroyImmediate(sizeReference);

        if (contentTransform != null)
        {
            if (vertical)
            {
                contentTransform.sizeDelta = new Vector2(
                    contentTransform.sizeDelta.x,
                    _itemSize * count + itemSpacing * (count - 1)
                );
            }
            else
            {
                contentTransform.sizeDelta = new Vector2(
                    _itemSize * count + itemSpacing * (count - 1),
                    contentTransform.sizeDelta.y
                );
            }
        }

        _viewportSize = vertical ? _rectTransform.rect.height : _rectTransform.rect.width;
        int visibleCount = Mathf.Min(visibleItems, count);

        for (int i = 0; i < visibleCount; i++)
        {
            GameObject item = CreateItem();
            _activeItems.Add(item);
            _itemPool[i] = item;

            PositionItem(item, i);

            if (_legacyBindCallback != null)
                _legacyBindCallback(i, item);
        }

        _lastScrollPosition = scrollRect != null ? scrollRect.normalizedPosition : Vector2.one;
    }

    private void PositionItem(GameObject item, int index)
    {
        RectTransform rt = item.GetComponent<RectTransform>();
        if (vertical)
        {
            rt.anchoredPosition = new Vector2(
                rt.anchoredPosition.x,
                -index * (_itemSize + itemSpacing)
            );
        }
        else
        {
            rt.anchoredPosition = new Vector2(
                index * (_itemSize + itemSpacing),
                rt.anchoredPosition.y
            );
        }
    }

    private void OnScrollValueChanged(Vector2 position)
    {
        if (mode != ListMode.LoopingList || _totalItemCount <= 0)
            return;

        float scrollDelta = vertical ?
            _lastScrollPosition.y - position.y :
            position.x - _lastScrollPosition.x;

        if (Mathf.Abs(scrollDelta) > 0.001f)
            UpdateVisibleItems();

        _lastScrollPosition = position;
    }

    private void UpdateVisibleItems()
    {
        if (contentTransform == null || _totalItemCount <= 0)
            return;

        float viewportStart, viewportEnd;

        if (vertical)
        {
            float normalizedPos = 1f - scrollRect.normalizedPosition.y;
            viewportStart = normalizedPos * (contentTransform.rect.height - _viewportSize);
            viewportEnd = viewportStart + _viewportSize;
        }
        else
        {
            float normalizedPos = scrollRect.normalizedPosition.x;
            viewportStart = normalizedPos * (contentTransform.rect.width - _viewportSize);
            viewportEnd = viewportStart + _viewportSize;
        }

        int startIndex = Mathf.FloorToInt(viewportStart / (_itemSize + itemSpacing));
        int endIndex = Mathf.CeilToInt(viewportEnd / (_itemSize + itemSpacing));

        startIndex = Mathf.Max(0, startIndex - 1);
        endIndex = Mathf.Min(_totalItemCount - 1, endIndex + 1);

        _startIndex = startIndex;

        List<int> toRemove = new List<int>();
        foreach (var kvp in _itemPool)
        {
            if (kvp.Key < startIndex || kvp.Key > endIndex)
                toRemove.Add(kvp.Key);
        }

        foreach (int index in toRemove)
        {
            if (_itemPool.TryGetValue(index, out GameObject item))
            {
                if (item != null)
                    item.SetActive(false);
                _itemPool.Remove(index);
            }
        }

        for (int i = startIndex; i <= endIndex; i++)
        {
            if (!_itemPool.ContainsKey(i) && i >= 0 && i < _totalItemCount)
            {
                GameObject item = GetRecycledItem();

                if (item == null)
                    item = CreateItem();

                _itemPool[i] = item;
                item.SetActive(true);

                PositionItem(item, i);

                if (_legacyBindCallback != null)
                {
                    _legacyBindCallback(i, item);
                }
                else if (_typedBindCallback != null && _dataSource != null)
                {
                    var template = item.GetComponent<UIBindTemplate>();
                    if (template != null)
                    {

                        _typedBindCallback.DynamicInvoke(i, _dataSource[i], template);
                    }
                }
            }
        }
    }

    private GameObject GetRecycledItem()
    {
        foreach (var item in _activeItems)
        {
            if (item != null && !item.activeInHierarchy)
                return item;
        }
        return null;
    }

    private GameObject CreateItem(bool addToActive = true)
    {
        if (itemTemplate == null || contentTransform == null)
            return null;

        GameObject item = Instantiate(itemTemplate.gameObject, contentTransform);

        _instanceCounter++;
        string baseName = itemTemplate.gameObject.name;
        item.name = $"{baseName}_{_instanceCounter}";

        item.SetActive(true);

        if (addToActive && !_activeItems.Contains(item))
            _activeItems.Add(item);

        return item;
    }

    public UIBindTemplate GetItem(int index)
    {
        GameObject go = null;

        if (mode == ListMode.FullGeneration)
        {
            if (index >= 0 && index < _activeItems.Count)
            {
                go = _activeItems[index];
            }
        }
        else
        {
            _itemPool.TryGetValue(index, out go);
        }

        return go?.GetComponent<UIBindTemplate>();
    }

    public void RefreshItem(int index)
    {
        var item = GetItem(index);
        if (item == null) return;

        if (_legacyBindCallback != null)
        {
            _legacyBindCallback(index, item.gameObject);
        }
        else if (_typedBindCallback != null && _dataSource != null && index < _dataSource.Count)
        {
            _typedBindCallback.DynamicInvoke(index, _dataSource[index], item);
        }
    }

    public void RefreshAll()
    {
        for (int i = 0; i < _activeItems.Count; i++)
        {
            RefreshItem(i);
        }
    }

    public void ScrollToIndex(int index, bool immediate = false)
    {
        if (_totalItemCount <= 0 || index < 0 || index >= _totalItemCount) return;

        float position;

        if (vertical)
        {
            position = 1.0f - Mathf.Clamp01((float)index / Mathf.Max(1, _totalItemCount - 1));

            if (immediate)
            {
                scrollRect.normalizedPosition = new Vector2(scrollRect.normalizedPosition.x, position);
            }
            else
            {
                StartCoroutine(SmoothScrollTo(new Vector2(scrollRect.normalizedPosition.x, position)));
            }
        }
        else
        {
            position = Mathf.Clamp01((float)index / Mathf.Max(1, _totalItemCount - 1));

            if (immediate)
            {
                scrollRect.normalizedPosition = new Vector2(position, scrollRect.normalizedPosition.y);
            }
            else
            {
                StartCoroutine(SmoothScrollTo(new Vector2(position, scrollRect.normalizedPosition.y)));
            }
        }

        if (mode == ListMode.LoopingList)
        {
            UpdateVisibleItems();
        }
    }

    private IEnumerator SmoothScrollTo(Vector2 targetPos)
    {
        float duration = 0.3f;
        float elapsed = 0;
        Vector2 startPos = scrollRect.normalizedPosition;

        while (elapsed < duration)
        {
            scrollRect.normalizedPosition = Vector2.Lerp(startPos, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        scrollRect.normalizedPosition = targetPos;
    }

    public int GetItemCount()
    {
        return _activeItems.Count;
    }

    public int GetTotalCount()
    {
        return _totalItemCount;
    }

    #region 动态列表项管理

    public UIBindTemplate AddItem(Action<UIBindTemplate> bindItemFunc = null)
    {
        GameObject item = CreateItem(true);
        if (item == null)
            return null;

        var template = item.GetComponent<UIBindTemplate>();
        bindItemFunc?.Invoke(template);

        _totalItemCount = _activeItems.Count;

        if (contentTransform != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentTransform);

        return template;
    }

    public bool RemoveItem(int index)
    {
        if (index < 0 || index >= _activeItems.Count)
            return false;

        GameObject item = _activeItems[index];
        _activeItems.RemoveAt(index);

        if (item != null)
            Destroy(item);

        _totalItemCount = _activeItems.Count;

        if (contentTransform != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentTransform);

        return true;
    }

    public bool RemoveItem(UIBindTemplate template)
    {
        if (template == null)
            return false;

        int index = _activeItems.IndexOf(template.gameObject);
        if (index < 0)
            return false;

        return RemoveItem(index);
    }

    public void SetFirst(int index)
    {
        SetIndex(index, 0);
    }

    public void SetFirst(UIBindTemplate template)
    {
        if (template == null)
            return;

        int index = _activeItems.IndexOf(template.gameObject);
        if (index >= 0)
            SetFirst(index);
    }

    public void SetLast(int index)
    {
        SetIndex(index, _activeItems.Count - 1);
    }

    public void SetLast(UIBindTemplate template)
    {
        if (template == null)
            return;

        int index = _activeItems.IndexOf(template.gameObject);
        if (index >= 0)
            SetLast(index);
    }

    public void SetIndex(int fromIndex, int toIndex)
    {
        if (fromIndex < 0 || fromIndex >= _activeItems.Count)
            return;
        if (toIndex < 0 || toIndex >= _activeItems.Count)
            return;
        if (fromIndex == toIndex)
            return;

        GameObject item = _activeItems[fromIndex];
        _activeItems.RemoveAt(fromIndex);
        _activeItems.Insert(toIndex, item);

        if (item != null)
        {
            item.transform.SetSiblingIndex(toIndex);
        }

        if (contentTransform != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentTransform);
    }

    public void SetIndex(UIBindTemplate template, int toIndex)
    {
        if (template == null)
            return;

        int fromIndex = _activeItems.IndexOf(template.gameObject);
        if (fromIndex >= 0)
            SetIndex(fromIndex, toIndex);
    }

    public int IndexOf(UIBindTemplate template)
    {
        if (template == null)
            return -1;
        return _activeItems.IndexOf(template.gameObject);
    }

    #endregion

#if UNITY_EDITOR
    public override string GetValidationError()
    {
        if (!HasValidBindName)
        {
            return $"UIBindList组件 [{gameObject.name}] 的bindName未设置";
        }
        if (itemTemplate == null)
        {
            return $"UIBindList组件 [{gameObject.name}] 的itemTemplate未设置";
        }
        return null;
    }
#endif
}
