using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[AddComponentMenu("LGUI/UIDropdown")]
public class UIDropdown : TMP_Dropdown
{
    [Header("扩展设置")]
    [Tooltip("选择音效")]
    [SerializeField] private AudioClip _selectSound;

    public event Action<int> OnValueChangedAction;

    public event Action<string> OnValueChangedTextAction;

    protected override void Awake()
    {
        base.Awake();
        onValueChanged.AddListener(OnDropdownValueChanged);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        onValueChanged.RemoveListener(OnDropdownValueChanged);
    }

    private void OnDropdownValueChanged(int index)
    {
        PlaySelectSound();
        OnValueChangedAction?.Invoke(index);

        if (options != null && index >= 0 && index < options.Count)
        {
            OnValueChangedTextAction?.Invoke(options[index].text);
        }
    }

    private void PlaySelectSound()
    {
        if (_selectSound != null && Camera.main != null)
        {
            AudioSource.PlayClipAtPoint(_selectSound, Camera.main.transform.position);
        }
    }

    #region 公共方法

    public void SetOptions(List<string> optionTexts)
    {
        ClearOptions();
        AddOptions(optionTexts);
    }

    public void SetOptions(List<OptionData> newOptions)
    {
        ClearOptions();
        AddOptions(newOptions);
    }

    public void AddOption(string text, Sprite sprite = null)
    {
        options.Add(new OptionData(text, sprite));
        RefreshShownValue();
    }

    public void RemoveOption(int index)
    {
        if (index >= 0 && index < options.Count)
        {
            options.RemoveAt(index);
            RefreshShownValue();
        }
    }

    public void SetValueWithoutNotify(int newValue)
    {
        SetValueWithoutNotify(newValue);
    }

    public void SetValue(int newValue)
    {
        value = newValue;
    }

    public int GetValue()
    {
        return value;
    }

    public string GetSelectedText()
    {
        if (options != null && value >= 0 && value < options.Count)
        {
            return options[value].text;
        }
        return string.Empty;
    }

    public bool SelectByText(string text)
    {
        for (int i = 0; i < options.Count; i++)
        {
            if (options[i].text == text)
            {
                value = i;
                return true;
            }
        }
        return false;
    }

    public void SetInteractable(bool interactable)
    {
        this.interactable = interactable;
    }

    public void AddValueChangedListener(UnityEngine.Events.UnityAction<int> action)
    {
        onValueChanged.AddListener(action);
    }

    public void RemoveAllValueChangedListeners()
    {
        onValueChanged.RemoveAllListeners();
    }

    #endregion

#if UNITY_EDITOR
    protected override void Reset()
    {
        base.Reset();
    }
#endif
}
