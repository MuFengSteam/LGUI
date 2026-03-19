// 作者: 木枫LL
// GitHub: https://github.com/MuFengSteam/
// 小红书: https://www.xiaohongshu.com/user/profile/67c5dc1a000000000a03e5d0

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIBindToggle : UIBase
{
    [Header("Toggle设置")]
    [Tooltip("默认是否选中")]
    public bool defaultValue = false;

    [Tooltip("是否在Start时自动设置默认值")]
    public bool setDefaultOnStart = true;

    private Toggle _toggleComponent;

    public override string ComponentTypeName => "UIBindToggle";
    public override string BindDataType => "bool";

#if UNITY_EDITOR
    protected override void Reset()
    {
        base.Reset();
        EnsureToggleComponentExists();
    }
#endif

    private void Start()
    {
        if (setDefaultOnStart)
        {
            SetValue(defaultValue);
        }
    }

    protected override void Initialize()
    {
        if (_initialized) return;
        EnsureToggleComponentExists();
        base.Initialize();
    }

    private void EnsureToggleComponentExists()
    {

        if (this == null || gameObject == null)
        {
            return;
        }

        if (_toggleComponent == null)
        {
            _toggleComponent = GetComponent<Toggle>();
        }

        if (_toggleComponent == null)
        {
            _toggleComponent = gameObject.AddComponent<Toggle>();

            if (_toggleComponent != null)
            {
                _toggleComponent.isOn = defaultValue;

                GameObject checkmark = new GameObject("Checkmark");
                checkmark.transform.SetParent(transform);
                Image checkmarkImage = checkmark.AddComponent<Image>();
                _toggleComponent.graphic = checkmarkImage;

                GameObject background = new GameObject("Background");
                background.transform.SetParent(transform);
                background.transform.SetAsFirstSibling();
                background.AddComponent<Image>();
            }
        }
    }

    public void SetValue(bool value)
    {
        EnsureInitialized();

        if (_toggleComponent != null)
        {
            _toggleComponent.isOn = value;
        }
    }

    public bool GetValue()
    {
        EnsureInitialized();

        if (_toggleComponent != null)
        {
            return _toggleComponent.isOn;
        }
        return false;
    }

    public void AddValueChangedListener(UnityAction<bool> callback)
    {
        EnsureInitialized();

        if (_toggleComponent != null)
        {
            _toggleComponent.onValueChanged.AddListener(callback);
        }
    }

    public void RemoveValueChangedListener(UnityAction<bool> callback)
    {
        if (_toggleComponent != null)
        {
            _toggleComponent.onValueChanged.RemoveListener(callback);
        }
    }

#if UNITY_EDITOR
    public override string GetValidationError()
    {
        if (!HasValidBindName)
        {
            return $"UIBindToggle组件 [{gameObject.name}] 的bindName未设置";
        }
        return null;
    }
#endif
}
