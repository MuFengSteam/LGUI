using System;
using UnityEngine;
using TMPro;

[AddComponentMenu("LGUI/UIInputField")]
public class UIInputField : TMP_InputField
{
    [Header("扩展设置")]
    [Tooltip("最大字符数（0表示无限制）")]
    [SerializeField] private int _maxCharacterCount = 0;

    [Tooltip("输入完成音效")]
    [SerializeField] private AudioClip _submitSound;

    public event Action<string> OnTextChangedAction;

    public event Action<string> OnSubmitAction;

    public event Action OnFocusAction;

    public event Action OnBlurAction;

    protected override void Awake()
    {
        base.Awake();

        if (_maxCharacterCount > 0)
        {
            characterLimit = _maxCharacterCount;
        }

        onValueChanged.AddListener(OnInputValueChanged);
        onSubmit.AddListener(OnInputSubmit);
        onSelect.AddListener(OnInputSelect);
        onDeselect.AddListener(OnInputDeselect);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        onValueChanged.RemoveListener(OnInputValueChanged);
        onSubmit.RemoveListener(OnInputSubmit);
        onSelect.RemoveListener(OnInputSelect);
        onDeselect.RemoveListener(OnInputDeselect);
    }

    private void OnInputValueChanged(string value)
    {
        OnTextChangedAction?.Invoke(value);
    }

    private void OnInputSubmit(string value)
    {
        PlaySubmitSound();
        OnSubmitAction?.Invoke(value);
    }

    private void OnInputSelect(string value)
    {
        OnFocusAction?.Invoke();
    }

    private void OnInputDeselect(string value)
    {
        OnBlurAction?.Invoke();
    }

    private void PlaySubmitSound()
    {
        if (_submitSound != null && Camera.main != null)
        {
            AudioSource.PlayClipAtPoint(_submitSound, Camera.main.transform.position);
        }
    }

    #region 公共方法

    public void SetText(string value)
    {
        text = value;
    }

    public void SetTextWithoutNotify(string value)
    {
        SetTextWithoutNotify(value);
    }

    public string GetText()
    {
        return text;
    }

    public void ClearText()
    {
        text = string.Empty;
    }

    public void SetPlaceholder(string placeholderText)
    {
        if (placeholder is TMP_Text tmpPlaceholder)
        {
            tmpPlaceholder.text = placeholderText;
        }
    }

    public void SetMaxCharacterCount(int max)
    {
        _maxCharacterCount = max;
        characterLimit = max > 0 ? max : 0;
    }

    public void Focus()
    {
        ActivateInputField();
    }

    public void Blur()
    {
        DeactivateInputField();
    }

    public void SelectAllText()
    {
        SelectAll();
    }

    public void SetInteractable(bool interactable)
    {
        this.interactable = interactable;
    }

    public void SetReadOnly(bool readOnly)
    {
        this.readOnly = readOnly;
    }

    public void AddTextChangedListener(UnityEngine.Events.UnityAction<string> action)
    {
        onValueChanged.AddListener(action);
    }

    public void AddSubmitListener(UnityEngine.Events.UnityAction<string> action)
    {
        onSubmit.AddListener(action);
    }

    public void RemoveAllListeners()
    {
        onValueChanged.RemoveAllListeners();
        onSubmit.RemoveAllListeners();
        onSelect.RemoveAllListeners();
        onDeselect.RemoveAllListeners();
    }

    #endregion

#if UNITY_EDITOR
    protected override void Reset()
    {
        base.Reset();
        lineType = LineType.SingleLine;
    }
#endif
}
