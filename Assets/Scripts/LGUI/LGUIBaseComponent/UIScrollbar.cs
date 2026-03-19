using System;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("LGUI/UIScrollbar")]
public class UIScrollbar : Scrollbar
{

    public event Action<float> OnValueChangedAction;

    protected override void Awake()
    {
        base.Awake();
        onValueChanged.AddListener(OnScrollbarValueChanged);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        onValueChanged.RemoveListener(OnScrollbarValueChanged);
    }

    private void OnScrollbarValueChanged(float value)
    {
        OnValueChangedAction?.Invoke(value);
    }

    #region 公共方法

    public void SetValueWithoutNotify(float newValue)
    {
        SetValueWithoutNotify(newValue);
    }

    public void SetValue(float newValue)
    {
        value = newValue;
    }

    public float GetValue()
    {
        return value;
    }

    public void SetSize(float newSize)
    {
        size = Mathf.Clamp01(newSize);
    }

    public float GetSize()
    {
        return size;
    }

    public void SetNumberOfSteps(int steps)
    {
        numberOfSteps = steps;
    }

    public void SetInteractable(bool interactable)
    {
        this.interactable = interactable;
    }

    public void AddValueChangedListener(UnityEngine.Events.UnityAction<float> action)
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
        direction = Direction.LeftToRight;
        size = 0.2f;
    }
#endif
}
