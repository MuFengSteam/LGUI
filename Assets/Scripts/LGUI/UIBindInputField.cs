// 作者: 木枫LL
// GitHub: https://github.com/MuFengSteam/
// 小红书: https://www.xiaohongshu.com/user/profile/67c5dc1a000000000a03e5d0

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIBindInputField : UIBase
{
    [Header("InputField设置")]
    [Tooltip("最大字符数（0表示不限制）")]
    public int maxCharacterLimit = 0;

    [Tooltip("占位符文本")]
    public string placeholderText = "";

    [Tooltip("是否密码输入")]
    public bool isPassword = false;

    private TMP_InputField _tmpInputField;
    private InputField _legacyInputField;

    private Action<string> _onValueChangedCallback;
    private Action<string> _onEndEditCallback;

    public override string ComponentTypeName => "UIBindInputField";
    public override string BindDataType => "UIBindInputField";

#if UNITY_EDITOR
    protected override void Reset()
    {
        base.Reset();
        EnsureInputFieldExists();
    }
#endif

    protected override void Initialize()
    {
        if (_initialized) return;
        EnsureInputFieldExists();
        base.Initialize();

        ApplySettings();

        BindEvents();
    }

    private void EnsureInputFieldExists()
    {

        _tmpInputField = GetComponent<TMP_InputField>();
        if (_tmpInputField != null)
        {
            return;
        }

        _legacyInputField = GetComponent<InputField>();
        if (_legacyInputField != null)
        {
            return;
        }
    }

    private void ApplySettings()
    {
        if (_tmpInputField != null)
        {
            if (maxCharacterLimit > 0)
                _tmpInputField.characterLimit = maxCharacterLimit;

            if (!string.IsNullOrEmpty(placeholderText) && _tmpInputField.placeholder != null)
            {
                var placeholderTmp = _tmpInputField.placeholder as TMP_Text;
                if (placeholderTmp != null)
                    placeholderTmp.text = placeholderText;
            }

            if (isPassword)
                _tmpInputField.contentType = TMP_InputField.ContentType.Password;
        }
        else if (_legacyInputField != null)
        {
            if (maxCharacterLimit > 0)
                _legacyInputField.characterLimit = maxCharacterLimit;

            if (!string.IsNullOrEmpty(placeholderText) && _legacyInputField.placeholder != null)
            {
                var placeholderText = _legacyInputField.placeholder as Text;
                if (placeholderText != null)
                    placeholderText.text = this.placeholderText;
            }

            if (isPassword)
                _legacyInputField.contentType = InputField.ContentType.Password;
        }
    }

    private void BindEvents()
    {
        if (_tmpInputField != null)
        {
            _tmpInputField.onValueChanged.AddListener(OnValueChanged);
            _tmpInputField.onEndEdit.AddListener(OnEndEdit);
        }
        else if (_legacyInputField != null)
        {
            _legacyInputField.onValueChanged.AddListener(OnValueChanged);
            _legacyInputField.onEndEdit.AddListener(OnEndEdit);
        }
    }

    private void OnValueChanged(string value)
    {
        _onValueChangedCallback?.Invoke(value);
    }

    private void OnEndEdit(string value)
    {
        _onEndEditCallback?.Invoke(value);
    }

    #region 公共方法

    public string GetText()
    {
        EnsureInitialized();

        if (_tmpInputField != null)
            return _tmpInputField.text;

        if (_legacyInputField != null)
            return _legacyInputField.text;

        return string.Empty;
    }

    public void SetText(string value)
    {
        EnsureInitialized();

        if (_tmpInputField != null)
            _tmpInputField.text = value ?? string.Empty;
        else if (_legacyInputField != null)
            _legacyInputField.text = value ?? string.Empty;
    }

    public void Clear()
    {
        SetText(string.Empty);
    }

    public void SetPlaceholder(string text)
    {
        EnsureInitialized();

        if (_tmpInputField != null && _tmpInputField.placeholder != null)
        {
            var placeholderTmp = _tmpInputField.placeholder as TMP_Text;
            if (placeholderTmp != null)
                placeholderTmp.text = text;
        }
        else if (_legacyInputField != null && _legacyInputField.placeholder != null)
        {
            var placeholderText = _legacyInputField.placeholder as Text;
            if (placeholderText != null)
                placeholderText.text = text;
        }
    }

    public void SetInteractable(bool interactable)
    {
        EnsureInitialized();

        if (_tmpInputField != null)
            _tmpInputField.interactable = interactable;
        else if (_legacyInputField != null)
            _legacyInputField.interactable = interactable;
    }

    public bool GetInteractable()
    {
        EnsureInitialized();

        if (_tmpInputField != null)
            return _tmpInputField.interactable;

        if (_legacyInputField != null)
            return _legacyInputField.interactable;

        return false;
    }

    public void SetCharacterLimit(int limit)
    {
        EnsureInitialized();

        if (_tmpInputField != null)
            _tmpInputField.characterLimit = limit;
        else if (_legacyInputField != null)
            _legacyInputField.characterLimit = limit;
    }

    public void AddValueChangedListener(Action<string> callback)
    {
        _onValueChangedCallback += callback;
    }

    public void AddEndEditListener(Action<string> callback)
    {
        _onEndEditCallback += callback;
    }

    public void RemoveAllListeners()
    {
        _onValueChangedCallback = null;
        _onEndEditCallback = null;
    }

    public void ActivateInputField()
    {
        EnsureInitialized();

        if (_tmpInputField != null)
            _tmpInputField.ActivateInputField();
        else if (_legacyInputField != null)
            _legacyInputField.ActivateInputField();
    }

    public void DeactivateInputField()
    {
        EnsureInitialized();

        if (_tmpInputField != null)
            _tmpInputField.DeactivateInputField();
        else if (_legacyInputField != null)
            _legacyInputField.DeactivateInputField();
    }

    #endregion

    protected override void OnDisable()
    {
        base.OnDisable();

        DeactivateInputField();
    }

    private void OnDestroy()
    {

        if (_tmpInputField != null)
        {
            _tmpInputField.onValueChanged.RemoveListener(OnValueChanged);
            _tmpInputField.onEndEdit.RemoveListener(OnEndEdit);
        }
        else if (_legacyInputField != null)
        {
            _legacyInputField.onValueChanged.RemoveListener(OnValueChanged);
            _legacyInputField.onEndEdit.RemoveListener(OnEndEdit);
        }

        RemoveAllListeners();
    }

#if UNITY_EDITOR
    public override string GetValidationError()
    {
        if (!HasValidBindName)
        {
            return $"UIBindInputField组件 [{gameObject.name}] 的bindName未设置";
        }

        var tmpInput = GetComponent<TMP_InputField>();
        var legacyInput = GetComponent<InputField>();

        if (tmpInput == null && legacyInput == null)
        {
            return $"UIBindInputField组件 [{gameObject.name}] 没有InputField组件";
        }

        return null;
    }
#endif
}
