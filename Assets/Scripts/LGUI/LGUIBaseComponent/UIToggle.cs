using System;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("LGUI/UIToggle")]
public class UIToggle : Toggle
{
    [Header("扩展设置")]
    [Tooltip("开关音效")]
    [SerializeField] private AudioClip _toggleSound;

    public event Action<bool> OnValueChangedAction;

    protected override void Awake()
    {
        base.Awake();
        onValueChanged.AddListener(OnToggleValueChanged);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        onValueChanged.RemoveListener(OnToggleValueChanged);
    }

    private void OnToggleValueChanged(bool value)
    {
        PlayToggleSound();
        OnValueChangedAction?.Invoke(value);
    }

    private void PlayToggleSound()
    {
        if (_toggleSound != null && Camera.main != null)
        {
            AudioSource.PlayClipAtPoint(_toggleSound, Camera.main.transform.position);
        }
    }

    #region 公共方法

    public void SetValueWithoutNotify(bool value)
    {
        SetIsOnWithoutNotify(value);
    }

    public void SetValue(bool value)
    {
        isOn = value;
    }

    public bool GetValue()
    {
        return isOn;
    }

    public void ToggleValue()
    {
        isOn = !isOn;
    }

    public void SetInteractable(bool interactable)
    {
        this.interactable = interactable;
    }

    public void AddValueChangedListener(UnityEngine.Events.UnityAction<bool> action)
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
        transition = Transition.ColorTint;
    }
#endif
}
