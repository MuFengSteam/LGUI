using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(Image))]
public class UIDraggableCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("拖拽设置")]
    [Tooltip("拖拽时的透明度")]
    [SerializeField] private float _dragAlpha = 0.8f;

    [Tooltip("拖拽时是否禁用射线检测（允许检测下方目标）")]
    [SerializeField] private bool _blockRaycastsOnDrag = false;

    [Tooltip("拖拽结束后是否返回原位")]
    [SerializeField] private bool _returnOnEnd = true;

    [Tooltip("返回动画时长")]
    [SerializeField] private float _returnDuration = 0.2f;

    [Tooltip("拖拽时移动到指定父节点（脱离 ScrollRect）")]
    [SerializeField] private bool _moveToParentOnDrag = true;

    [Tooltip("拖拽时的父节点（为空则使用 Canvas 根节点）")]
    [SerializeField] private Transform _dragParent;

    [Header("数据")]
    [Tooltip("卡片携带的数据ID（如员工NpcId）")]
    [SerializeField] private int _dataId;

    public event Action<UIDraggableCard> OnDragBegin;

    public event Action<UIDraggableCard, Vector2> OnDragging;

    public event Action<UIDraggableCard> OnDragEnd;

    public event Action<UIDraggableCard, GameObject> OnDropped;

    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private Canvas _canvas;
    private Transform _originalParent;
    private int _originalSiblingIndex;
    private Vector2 _originalAnchoredPosition;
    private Vector3 _originalScale;
    private Vector3 _originalWorldPosition;
    private float _originalAlpha;
    private bool _isDragging = false;
    private bool _dropSucceeded = false;
    private UIButton _uiButton;
    private ScrollRect _parentScrollRect;
    private Vector3 _pointerOffset;

    public bool IsDragging => _isDragging;

    public void SetDropSucceeded(bool succeeded)
    {
        _dropSucceeded = succeeded;
    }

    public int DataId
    {
        get => _dataId;
        set => _dataId = value;
    }

    public Transform DragParent
    {
        get => _dragParent;
        set => _dragParent = value;
    }

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
        {
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        _uiButton = GetComponent<UIButton>();

        _canvas = GetComponentInParent<Canvas>()?.rootCanvas;

        _parentScrollRect = GetComponentInParent<ScrollRect>();

        var image = GetComponent<Image>();
        if (image != null)
        {
            image.raycastTarget = true;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }

    public void OnPointerUp(PointerEventData eventData)
    {

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _isDragging = true;

        _originalParent = transform.parent;
        _originalSiblingIndex = transform.GetSiblingIndex();
        _originalScale = _rectTransform.localScale;
        _originalWorldPosition = _rectTransform.position;
        _originalAlpha = _canvasGroup.alpha;

        if (_uiButton != null)
        {
            _originalAnchoredPosition = _uiButton.GetOriginalAnchoredPosition();
            _uiButton.interactable = false;
        }
        else
        {
            _originalAnchoredPosition = _rectTransform.anchoredPosition;
        }

        _canvasGroup.alpha = _dragAlpha;
        _canvasGroup.blocksRaycasts = _blockRaycastsOnDrag;

        Camera eventCamera = GetEventCamera();

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _rectTransform, eventData.pressPosition, eventCamera, out localPoint);

        Transform targetParent = null;
        if (_moveToParentOnDrag)
        {
            targetParent = _dragParent != null ? _dragParent : (_canvas != null ? _canvas.transform : null);

            if (targetParent != null)
            {

                transform.SetParent(targetParent, false);

                Vector3 worldPos;
                RectTransform parentRect = targetParent as RectTransform;
                if (parentRect != null && RectTransformUtility.ScreenPointToWorldPointInRectangle(
                    parentRect, eventData.pressPosition, eventCamera, out worldPos))
                {

                    Vector2 parentLocalPoint;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        parentRect, eventData.pressPosition, eventCamera, out parentLocalPoint);

                    _rectTransform.localPosition = new Vector3(
                        parentLocalPoint.x - localPoint.x,
                        parentLocalPoint.y - localPoint.y,
                        0);
                }
            }
        }

        Vector3 pointerWorldPos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
            _rectTransform, eventData.position, eventCamera, out pointerWorldPos))
        {
            _pointerOffset = _rectTransform.position - pointerWorldPos;
        }
        else
        {
            _pointerOffset = Vector3.zero;
        }

        transform.SetAsLastSibling();

        OnDragBegin?.Invoke(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isDragging) return;

        Vector3 pointerWorldPos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
            _rectTransform, eventData.position, GetEventCamera(), out pointerWorldPos))
        {
            _rectTransform.position = pointerWorldPos + _pointerOffset;
        }

        OnDragging?.Invoke(this, eventData.position);
    }

    private Camera GetEventCamera()
    {
        if (_canvas == null) return null;
        return _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera;
    }

    public void OnEndDrag(PointerEventData eventData)
    {

        if (this == null || gameObject == null)
        {
            return;
        }

        _isDragging = false;
        _dropSucceeded = false;

        if (_canvasGroup != null)
        {
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.alpha = _originalAlpha;
        }

        GameObject dropTarget = GetDropTarget(eventData);

        if (dropTarget != null)
        {

            OnDropped?.Invoke(this, dropTarget);
        }

        if (_uiButton != null)
        {
            _uiButton.interactable = true;
        }

        if (this == null || gameObject == null)
        {
            return;
        }

        if (_returnOnEnd && !_dropSucceeded)
        {
            ReturnToOriginalPosition();
        }

        OnDragEnd?.Invoke(this);
    }

    private GameObject GetDropTarget(PointerEventData eventData)
    {

        _canvasGroup.blocksRaycasts = false;

        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        _canvasGroup.blocksRaycasts = true;

        foreach (var result in results)
        {
            var dropTarget = result.gameObject.GetComponent<UIDropTarget>();
            if (dropTarget != null && dropTarget.gameObject != gameObject)
            {
                return result.gameObject;
            }
        }

        return null;
    }

    public void ReturnToOriginalPosition()
    {

        if (this == null || gameObject == null)
        {
            return;
        }

        if (_returnDuration > 0)
        {
            StartCoroutine(AnimateReturn());
        }
        else
        {

            Vector3 currentWorldPos = _rectTransform.position;
            transform.SetParent(_originalParent, true);
            _rectTransform.position = currentWorldPos;
            transform.SetSiblingIndex(_originalSiblingIndex);

            _rectTransform.anchoredPosition = _originalAnchoredPosition;
            _rectTransform.localScale = _originalScale;
        }
    }

    private System.Collections.IEnumerator AnimateReturn()
    {

        if (this == null || gameObject == null || _rectTransform == null)
        {
            yield break;
        }

        Vector3 currentWorldPos = _rectTransform.position;
        transform.SetParent(_originalParent, true);
        _rectTransform.position = currentWorldPos;
        transform.SetSiblingIndex(_originalSiblingIndex);

        Vector2 startAnchoredPos = _rectTransform.anchoredPosition;
        Vector3 startScale = _rectTransform.localScale;
        float elapsed = 0f;

        while (elapsed < _returnDuration)
        {

            if (this == null || gameObject == null || _rectTransform == null)
            {
                yield break;
            }

            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / _returnDuration;
            t = 1 - Mathf.Pow(1 - t, 3);

            _rectTransform.anchoredPosition = Vector2.Lerp(startAnchoredPos, _originalAnchoredPosition, t);
            _rectTransform.localScale = Vector3.Lerp(startScale, _originalScale, t);

            yield return null;
        }

        if (this != null && gameObject != null && _rectTransform != null)
        {

            _rectTransform.anchoredPosition = _originalAnchoredPosition;
            _rectTransform.localScale = _originalScale;
        }
    }

    public void SetData(int dataId)
    {
        _dataId = dataId;
    }
}
