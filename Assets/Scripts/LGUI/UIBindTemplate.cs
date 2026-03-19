using UnityEngine;
using System;
using System.Collections.Generic;

public class UIBindTemplate : UIBase
{

    private Dictionary<string, UIBase> _bindComponents;

    public override string ComponentTypeName => "UIBindTemplate";
    public override string BindDataType => "UIBindTemplate";

    protected override void Initialize()
    {
        if (_initialized) return;
        CacheBindComponents();
        base.Initialize();
    }

    private void CacheBindComponents()
    {
        _bindComponents = new Dictionary<string, UIBase>();

        UIBase[] components = GetComponentsInChildren<UIBase>(true);
        foreach (var comp in components)
        {
            if (comp == this) continue;
            if (comp.HasValidBindName)
            {
                _bindComponents[comp.bindName] = comp;
            }
        }
    }

    public GameObject Clone()
    {
        GameObject clone = Instantiate(gameObject, transform.parent);
        clone.name = gameObject.name + "_Clone";
        clone.SetActive(true);
        return clone;
    }

    public GameObject Clone(Transform parent)
    {
        GameObject clone = Instantiate(gameObject, parent);
        clone.name = gameObject.name + "_Clone";
        clone.SetActive(true);
        return clone;
    }

    public T GetBindComponent<T>(string bindName) where T : UIBase
    {
        EnsureInitialized();

        if (_bindComponents.TryGetValue(bindName, out UIBase comp))
        {
            return comp as T;
        }
        return null;
    }

    public void SetText(string bindName, string value)
    {
        var textComp = GetBindComponent<UIBindText>(bindName);
        if (textComp != null)
        {
            textComp.SetText(value);
        }
    }

    public void SetImage(string bindName, int imageConfigId)
    {
        var imageComp = GetBindComponent<UIBindImage>(bindName);
        if (imageComp != null)
        {
            imageComp.SetImageById(imageConfigId);
        }
    }

    public void SetSprite(string bindName, Sprite sprite)
    {
        var imageComp = GetBindComponent<UIBindImage>(bindName);
        if (imageComp != null)
        {
            imageComp.SetSprite(sprite);
        }
    }

    public void SetActive(string bindName, bool active)
    {
        var activeComp = GetBindComponent<UIBindBoolToActive>(bindName);
        if (activeComp != null)
        {
            activeComp.SetShow(active);
        }
    }

    public void SetPosition(string bindName, Vector2 position)
    {
        var posComp = GetBindComponent<UIBindPosition>(bindName);
        if (posComp != null)
        {
            posComp.SetPosition(position);
        }
    }

    public void SetPosition(string bindName, float x, float y)
    {
        SetPosition(bindName, new Vector2(x, y));
    }

    public void SetPosition(string bindName, float[] position)
    {
        var posComp = GetBindComponent<UIBindPosition>(bindName);
        if (posComp != null)
        {
            posComp.SetPosition(position);
        }
    }

    public Vector2 GetPosition(string bindName)
    {
        var posComp = GetBindComponent<UIBindPosition>(bindName);
        if (posComp != null)
        {
            return posComp.GetPosition();
        }
        return Vector2.zero;
    }

    public void SetToggle(string bindName, bool value)
    {
        var toggleComp = GetBindComponent<UIBindToggle>(bindName);
        if (toggleComp != null)
        {
            toggleComp.SetValue(value);
        }
    }

    public void SetSlider(string bindName, float value)
    {
        var sliderComp = GetBindComponent<UIBindSlider>(bindName);
        if (sliderComp != null)
        {
            sliderComp.SetValue(value);
        }
    }

    #region 按钮点击事件

    public void SetButtonClick(Action onClick)
    {
        var button = gameObject.GetComponent<UIButton>();
        if (button != null)
        {
            button.RemoveAllClickListeners();
            button.AddClickListener(() => onClick?.Invoke());
        }
    }

    public void SetButtonClick(int index, Action<int> onClick)
    {
        var button = gameObject.GetComponent<UIButton>();
        if (button != null)
        {
            button.RemoveAllClickListeners();
            int capturedIndex = index;
            button.AddClickListener(() => onClick?.Invoke(capturedIndex));
        }
    }

    public void SetButtonClick<T>(T data, Action<T> onClick)
    {
        var button = gameObject.GetComponent<UIButton>();
        if (button != null)
        {
            button.RemoveAllClickListeners();
            T capturedData = data;
            button.AddClickListener(() => onClick?.Invoke(capturedData));
        }
    }

    public void SetButtonClick(string bindName, Action onClick)
    {

        var buttonComp = GetBindComponent<UIBindButton>(bindName);
        if (buttonComp != null)
        {
            var button = buttonComp.GetComponent<UIButton>();
            if (button != null)
            {
                button.RemoveAllClickListeners();
                button.AddClickListener(() => onClick?.Invoke());
                return;
            }
        }

        var anyBindComp = GetBindComponent<UIBase>(bindName);
        if (anyBindComp != null)
        {
            var button = anyBindComp.GetComponent<UIButton>();
            if (button != null)
            {
                button.RemoveAllClickListeners();
                button.AddClickListener(() => onClick?.Invoke());
                return;
            }

            var unityButton = anyBindComp.GetComponent<UnityEngine.UI.Button>();
            if (unityButton != null)
            {
                unityButton.onClick.RemoveAllListeners();
                unityButton.onClick.AddListener(() => onClick?.Invoke());
                return;
            }

            Debug.LogWarning($"[UIBindTemplate] 绑定组件 '{bindName}' 上没有 Button 组件");
        }
        else
        {
            Debug.LogWarning($"[UIBindTemplate] 找不到绑定组件: {bindName}");
        }
    }

    public void SetButtonClick(string bindName, int index, Action<int> onClick)
    {

        var bindComp = GetBindComponent<UIBase>(bindName);
        if (bindComp != null)
        {
            var button = bindComp.GetComponent<UIButton>();
            if (button != null)
            {
                button.RemoveAllClickListeners();
                int capturedIndex = index;
                button.AddClickListener(() => onClick?.Invoke(capturedIndex));
            }
        }
    }

    public UIButton GetButton()
    {
        return gameObject.GetComponent<UIButton>();
    }

    public UIButton GetButton(string bindName)
    {
        var buttonComp = GetBindComponent<UIBindButton>(bindName);
        return buttonComp?.GetComponent<UIButton>();
    }

    #endregion

    #region Hover事件

    private UnityEngine.EventSystems.EventTrigger _eventTrigger;

    private UnityEngine.EventSystems.EventTrigger EnsureEventTrigger()
    {
        if (_eventTrigger == null)
        {
            _eventTrigger = gameObject.GetComponent<UnityEngine.EventSystems.EventTrigger>();
            if (_eventTrigger == null)
            {
                _eventTrigger = gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
            }
        }
        return _eventTrigger;
    }

    public void SetHoverCallback(Action onEnter, Action onExit)
    {
        var trigger = EnsureEventTrigger();

        trigger.triggers.RemoveAll(e => e.eventID == UnityEngine.EventSystems.EventTriggerType.PointerEnter);

        trigger.triggers.RemoveAll(e => e.eventID == UnityEngine.EventSystems.EventTriggerType.PointerExit);

        if (onEnter != null)
        {
            var entryEnter = new UnityEngine.EventSystems.EventTrigger.Entry();
            entryEnter.eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter;
            entryEnter.callback.AddListener((data) => onEnter?.Invoke());
            trigger.triggers.Add(entryEnter);
        }

        if (onExit != null)
        {
            var entryExit = new UnityEngine.EventSystems.EventTrigger.Entry();
            entryExit.eventID = UnityEngine.EventSystems.EventTriggerType.PointerExit;
            entryExit.callback.AddListener((data) => onExit?.Invoke());
            trigger.triggers.Add(entryExit);
        }
    }

    public void SetHoverCallback(int index, Action<int> onEnter, Action<int> onExit)
    {
        int capturedIndex = index;
        SetHoverCallback(
            onEnter != null ? () => onEnter(capturedIndex) : (Action)null,
            onExit != null ? () => onExit(capturedIndex) : (Action)null
        );
    }

    #endregion

#if UNITY_EDITOR
    protected override void Reset()
    {
        base.Reset();

        if (GetComponent<RectTransform>() == null)
        {
            gameObject.AddComponent<RectTransform>();
        }
    }

    public override string GetValidationError()
    {

        return null;
    }
#endif
}
