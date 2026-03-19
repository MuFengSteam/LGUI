using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIBindSlider : UIBase
{
    [Header("Slider设置")]
    [Tooltip("默认值")]
    public float defaultValue = 0f;

    [Tooltip("最小值")]
    public float minValue = 0f;

    [Tooltip("最大值")]
    public float maxValue = 1f;

    [Tooltip("是否在Start时设置默认值")]
    public bool setDefaultOnStart = true;

    private UISlider _uiSliderComponent;
    private Slider _sliderComponent;
    private UnityAction<float> _onValueChanged;
    private System.Action<float> _steppedCallback;

    public override string ComponentTypeName => "UIBindSlider";
    public override string BindDataType => "float";

#if UNITY_EDITOR
    protected override void Reset()
    {
        base.Reset();
        EnsureSliderComponentExists();
    }
#endif

    private void Start()
    {
        if (setDefaultOnStart)
        {
            SetValue(defaultValue);
        }
    }

    private void OnDestroy()
    {
        if (_sliderComponent != null && _onValueChanged != null)
        {
            _sliderComponent.onValueChanged.RemoveListener(_onValueChanged);
        }
    }

    protected override void Initialize()
    {
        if (_initialized) return;
        EnsureSliderComponentExists();
        base.Initialize();
    }

    private void EnsureSliderComponentExists()
    {

        if (this == null || gameObject == null)
        {
            return;
        }

        if (_uiSliderComponent == null)
        {
            _uiSliderComponent = GetComponent<UISlider>();
        }
        if (_uiSliderComponent != null)
        {
            _sliderComponent = _uiSliderComponent;
            return;
        }

        if (_sliderComponent == null)
        {
            _sliderComponent = GetComponent<Slider>();
        }
        if (_sliderComponent == null)
        {

            _uiSliderComponent = gameObject.AddComponent<UISlider>();

            if (_uiSliderComponent != null)
            {
                _sliderComponent = _uiSliderComponent;
                _sliderComponent.minValue = minValue;
                _sliderComponent.maxValue = maxValue;
                _sliderComponent.value = defaultValue;
                CreateSliderVisuals();
            }
        }
    }

    private void CreateSliderVisuals()
    {

        GameObject background = new GameObject("Background");
        background.transform.SetParent(transform, false);
        Image backgroundImage = background.AddComponent<Image>();
        backgroundImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        RectTransform backgroundRect = background.GetComponent<RectTransform>();
        backgroundRect.anchorMin = Vector2.zero;
        backgroundRect.anchorMax = Vector2.one;
        backgroundRect.sizeDelta = Vector2.zero;
        _sliderComponent.targetGraphic = backgroundImage;

        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(transform, false);
        RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.sizeDelta = Vector2.zero;
        fillAreaRect.offsetMin = new Vector2(5, 0);
        fillAreaRect.offsetMax = new Vector2(-5, 0);

        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = new Color(0.1f, 0.5f, 0.9f, 1f);
        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.sizeDelta = Vector2.zero;

        GameObject handle = new GameObject("Handle Slide Area");
        handle.transform.SetParent(transform, false);
        RectTransform handleRect = handle.AddComponent<RectTransform>();
        handleRect.anchorMin = Vector2.zero;
        handleRect.anchorMax = Vector2.one;
        handleRect.sizeDelta = Vector2.zero;
        handleRect.offsetMin = new Vector2(10, 0);
        handleRect.offsetMax = new Vector2(-10, 0);

        GameObject handleButton = new GameObject("Handle");
        handleButton.transform.SetParent(handle.transform, false);
        Image handleImage = handleButton.AddComponent<Image>();
        handleImage.color = new Color(0.9f, 0.9f, 0.9f, 1f);
        RectTransform handleButtonRect = handleButton.GetComponent<RectTransform>();
        handleButtonRect.sizeDelta = new Vector2(20, 0);
        handleButtonRect.anchorMin = Vector2.zero;
        handleButtonRect.anchorMax = Vector2.one;

        _sliderComponent.fillRect = fillRect;
        _sliderComponent.handleRect = handleButtonRect;
        _sliderComponent.targetGraphic = handleImage;
    }

    public void SetValue(float value)
    {
        EnsureInitialized();

        if (_uiSliderComponent != null)
        {
            _uiSliderComponent.SetValue(value);
        }
        else if (_sliderComponent != null)
        {
            _sliderComponent.value = Mathf.Clamp(value, minValue, maxValue);
        }
    }

    public float GetValue()
    {
        EnsureInitialized();

        if (_uiSliderComponent != null)
        {
            return _uiSliderComponent.GetSteppedValue();
        }
        return _sliderComponent != null ? _sliderComponent.value : defaultValue;
    }

    public void SetRange(float min, float max)
    {
        EnsureInitialized();

        minValue = min;
        maxValue = max;

        if (_sliderComponent != null)
        {
            _sliderComponent.minValue = min;
            _sliderComponent.maxValue = max;
        }
    }

    public void SetStep(float stepValue)
    {
        EnsureInitialized();

        if (_uiSliderComponent != null)
        {
            _uiSliderComponent.SetStep(stepValue);
        }
    }

    public float GetStep()
    {
        EnsureInitialized();

        if (_uiSliderComponent != null)
        {
            return _uiSliderComponent.GetStep();
        }
        return 0;
    }

    public void AddValueChangedListener(UnityAction<float> callback)
    {
        EnsureInitialized();

        if (callback == null) return;

        _onValueChanged = callback;

        if (_uiSliderComponent != null && _uiSliderComponent.Step > 0)
        {

            _steppedCallback = (value) => callback(value);
            _uiSliderComponent.OnSteppedValueChangedAction += _steppedCallback;
        }
        else if (_sliderComponent != null)
        {
            _sliderComponent.onValueChanged.AddListener(callback);
        }
    }

    public void RemoveValueChangedListener(UnityAction<float> callback)
    {
        if (_uiSliderComponent != null && _steppedCallback != null)
        {
            _uiSliderComponent.OnSteppedValueChangedAction -= _steppedCallback;
            _steppedCallback = null;
        }

        if (_sliderComponent != null && _onValueChanged != null)
        {
            _sliderComponent.onValueChanged.RemoveListener(_onValueChanged);
        }

        _onValueChanged = null;
    }

#if UNITY_EDITOR
    public override string GetValidationError()
    {
        if (!HasValidBindName)
        {
            return $"UIBindSlider组件 [{gameObject.name}] 的bindName未设置";
        }
        return null;
    }
#endif
}
