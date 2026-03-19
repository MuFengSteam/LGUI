using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class UIBindButton : UIBase, IPointerDownHandler, IPointerUpHandler
{
    [Header("Button设置")]
    [Tooltip("按钮ID，用于与UIBasePanel中的方法关联")]
    public uint buttonId = 1;

    [Tooltip("索引参数，传入点击方法的参数值")]
    public int index = -1;

    [Header("长按设置")]
    [Tooltip("是否启用长按模式")]
    public bool enableLongPress = false;

    [Tooltip("长按抬起事件ID（仅在长按模式下使用）")]
    public uint longPressUpEventId = 0;

    [Tooltip("长按触发时间（秒）")]
    public float longPressTime = 0.5f;

    private UIButton _button;
    private bool _isPressed = false;
    private float _pressStartTime;
    private bool _longPressTriggered = false;

    private Action _onClickCallback;
    private Action<int> _onClickWithIndexCallback;
    private Action _onPressDownCallback;
    private Action _onPressUpCallback;
    private Action<int> _onPressDownWithIndexCallback;
    private Action<int> _onPressUpWithIndexCallback;

    public override string ComponentTypeName => "UIBindButton";
    public override string BindDataType => "UIBindButton";

#if UNITY_EDITOR
    protected override void Reset()
    {
        base.Reset();
        _bindId = (int)buttonId;
        EnsureButtonComponentExists();
    }
#endif

    protected override void Initialize()
    {
        if (_initialized) return;

        EnsureButtonComponentExists();
        base.Initialize();
    }

    private void EnsureButtonComponentExists()
    {
        _button = GetComponent<UIButton>();
        if (_button == null)
        {
            _button = gameObject.AddComponent<UIButton>();
        }
    }

    public UIButton GetButtonComponent()
    {
        EnsureInitialized();
        return _button;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        EnsureInitialized();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    private void Update()
    {

        if (enableLongPress && _isPressed && !_longPressTriggered)
        {
            if (Time.time - _pressStartTime >= longPressTime)
            {
                _longPressTriggered = true;
            }
        }
    }

    #region IPointerDownHandler / IPointerUpHandler

    public void OnPointerDown(PointerEventData eventData)
    {
        if (enableLongPress)
        {
            _isPressed = true;
            _pressStartTime = Time.time;
            _longPressTriggered = false;
            TriggerPressDown();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (enableLongPress && _isPressed)
        {
            _isPressed = false;
            TriggerPressUp();
        }
    }

    #endregion

    #region 事件触发

    private void TriggerPressDown()
    {
        _onPressDownCallback?.Invoke();

        if (index >= 0)
        {
            _onPressDownWithIndexCallback?.Invoke(index);
        }
    }

    private void TriggerPressUp()
    {
        _onPressUpCallback?.Invoke();

        if (index >= 0)
        {
            _onPressUpWithIndexCallback?.Invoke(index);
        }
    }

    #endregion

    #region 公共方法

    public void AddClickListener(Action callback)
    {
        EnsureInitialized();
        _onClickCallback = callback;

        if (!enableLongPress && _button != null)
        {
            _button.onClick.AddListener(() => callback?.Invoke());
        }
    }

    public void AddClickListener(Action<int> callback)
    {
        EnsureInitialized();
        _onClickWithIndexCallback = callback;

        if (!enableLongPress && _button != null)
        {
            _button.onClick.AddListener(() => callback?.Invoke(index));
        }
    }

    public void AddPressDownListener(Action callback)
    {
        _onPressDownCallback = callback;
    }

    public void AddPressDownListener(Action<int> callback)
    {
        _onPressDownWithIndexCallback = callback;
    }

    public void AddPressUpListener(Action callback)
    {
        _onPressUpCallback = callback;
    }

    public void AddPressUpListener(Action<int> callback)
    {
        _onPressUpWithIndexCallback = callback;
    }

    public void RemoveAllListeners()
    {
        EnsureInitialized();
        if (_button != null)
        {
            _button.onClick.RemoveAllListeners();
        }
        _onClickCallback = null;
        _onClickWithIndexCallback = null;
        _onPressDownCallback = null;
        _onPressUpCallback = null;
        _onPressDownWithIndexCallback = null;
        _onPressUpWithIndexCallback = null;
    }

    public void SetInteractable(bool interactable)
    {
        EnsureInitialized();
        if (_button != null)
        {
            _button.interactable = interactable;
        }
    }

    public bool GetInteractable()
    {
        EnsureInitialized();
        return _button != null && _button.interactable;
    }

    public void SimulateClick()
    {
        EnsureInitialized();

        if (enableLongPress)
        {
            TriggerPressDown();
            TriggerPressUp();
        }
        else if (_button != null)
        {
            _button.onClick.Invoke();
        }
    }

    public uint GetLongPressUpEventId()
    {
        return longPressUpEventId;
    }

    public bool IsLongPressEnabled()
    {
        return enableLongPress;
    }

    #endregion

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        _bindId = (int)buttonId;
    }

    public override string GetValidationError()
    {
        if (buttonId <= 0)
        {
            return $"UIBindButton组件 [{gameObject.name}] 的buttonId未设置或为0";
        }
        if (enableLongPress && longPressUpEventId <= 0)
        {
            return $"UIBindButton组件 [{gameObject.name}] 启用了长按但longPressUpEventId未设置";
        }
        return null;
    }
#endif
}
