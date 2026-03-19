using System;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("LGUI/UISlider")]
public class UISlider : Slider
{
    [Header("扩展设置")]
    [Tooltip("滑动音效")]
    [SerializeField] private AudioClip _slideSound;

    [Tooltip("是否在拖动时播放音效")]
    [SerializeField] private bool _playSoundOnDrag = false;

    [Tooltip("步长，0表示连续滑动，>0表示每次变化的最小单位")]
    [SerializeField] private float _step = 0f;

    public event Action<float> OnValueChangedAction;

    public event Action<float> OnDragEndAction;

    public event Action<float> OnSteppedValueChangedAction;

    private bool _isDragging = false;
    private float _lastSteppedValue;

    public float Step
    {
        get => _step;
        set => _step = Mathf.Max(0, value);
    }

    protected override void Awake()
    {
        base.Awake();
        onValueChanged.AddListener(OnSliderValueChanged);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        onValueChanged.RemoveListener(OnSliderValueChanged);
    }

    private void OnSliderValueChanged(float rawValue)
    {
        if (_playSoundOnDrag || !_isDragging)
        {
            PlaySlideSound();
        }

        if (_step > 0)
        {
            float steppedValue = ApplyStep(rawValue);

            if (!Mathf.Approximately(steppedValue, _lastSteppedValue))
            {
                _lastSteppedValue = steppedValue;

                if (!Mathf.Approximately(value, steppedValue))
                {
                    SetValueWithoutNotify(steppedValue);
                }

                OnSteppedValueChangedAction?.Invoke(steppedValue);
            }
        }

        OnValueChangedAction?.Invoke(rawValue);
    }

    private float ApplyStep(float rawValue)
    {
        if (_step <= 0)
            return rawValue;

        float steppedValue = Mathf.Round((rawValue - minValue) / _step) * _step + minValue;
        return Mathf.Clamp(steppedValue, minValue, maxValue);
    }

    public float GetSteppedValue()
    {
        return _step > 0 ? ApplyStep(value) : value;
    }

    public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        _isDragging = true;
    }

    public override void OnPointerUp(UnityEngine.EventSystems.PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        _isDragging = false;
        OnDragEndAction?.Invoke(value);
        PlaySlideSound();
    }

    private void PlaySlideSound()
    {
        if (_slideSound != null && Camera.main != null)
        {
            AudioSource.PlayClipAtPoint(_slideSound, Camera.main.transform.position);
        }
    }

    #region 公共方法

    public new void SetValueWithoutNotify(float newValue)
    {
        base.SetValueWithoutNotify(newValue);
        if (_step > 0)
        {
            _lastSteppedValue = ApplyStep(newValue);
        }
    }

    public void SetValue(float newValue)
    {
        if (_step > 0)
        {
            value = ApplyStep(newValue);
        }
        else
        {
            value = newValue;
        }
    }

    public void SetStep(float stepValue)
    {
        _step = Mathf.Max(0, stepValue);

        if (_step > 0)
        {
            _lastSteppedValue = ApplyStep(value);
            SetValueWithoutNotify(_lastSteppedValue);
        }
    }

    public float GetStep()
    {
        return _step;
    }

    public float GetValue()
    {
        return value;
    }

    public void SetRange(float min, float max)
    {
        minValue = min;
        maxValue = max;
    }

    public float GetNormalizedValue()
    {
        return normalizedValue;
    }

    public void SetNormalizedValue(float normalizedValue)
    {
        this.normalizedValue = normalizedValue;
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
        wholeNumbers = false;
        minValue = 0f;
        maxValue = 1f;
    }
#endif
}
