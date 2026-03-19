using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("LGUI/UIBindProgress")]
[RequireComponent(typeof(Image))]
public class UIBindProgress : UIBase
{
    [Header("进度设置")]
    [Tooltip("最小值")]
    [SerializeField] private float _minValue = 0f;

    [Tooltip("最大值")]
    [SerializeField] private float _maxValue = 1f;

    [Tooltip("当前值")]
    [SerializeField] private float _currentValue = 0f;

    [Tooltip("是否反向填充")]
    [SerializeField] private bool _inverse = false;

    private Image _imageComponent;

    public override string ComponentTypeName => "UIBindProgress";
    public override string BindDataType => "float";

    public float Progress
    {
        get => _currentValue;
        set => SetProgress(value);
    }

#if UNITY_EDITOR
    protected override void Reset()
    {
        base.Reset();
        EnsureImageComponentExists();
        SetupImageForFill();
    }
#endif

    protected override void Initialize()
    {
        if (_initialized) return;
        EnsureImageComponentExists();
        base.Initialize();
    }

    private void EnsureImageComponentExists()
    {
        if (_imageComponent == null)
        {
            _imageComponent = GetComponent<Image>();
        }
    }

    private void SetupImageForFill()
    {
        if (_imageComponent != null && _imageComponent.type != Image.Type.Filled)
        {
            _imageComponent.type = Image.Type.Filled;
            _imageComponent.fillMethod = Image.FillMethod.Horizontal;
            _imageComponent.fillOrigin = 0;
        }
    }

    public Image GetImageComponent()
    {
        EnsureImageComponentExists();
        return _imageComponent;
    }

    public void SetProgress(float value)
    {
        _currentValue = Mathf.Clamp(value, _minValue, _maxValue);
        ApplyProgress();
    }

    public void SetProgress(float current, float max)
    {
        if (max <= 0)
        {
            SetProgress(0);
            return;
        }
        SetProgress(current / max);
    }

    public void SetProgress(float current, float min, float max)
    {
        float range = max - min;
        if (range <= 0)
        {
            SetProgress(0);
            return;
        }
        SetProgress((current - min) / range);
    }

    private void ApplyProgress()
    {
        EnsureImageComponentExists();
        if (_imageComponent == null) return;

        float normalizedValue = (_currentValue - _minValue) / (_maxValue - _minValue);
        normalizedValue = Mathf.Clamp01(normalizedValue);

        if (_inverse)
        {
            normalizedValue = 1f - normalizedValue;
        }

        _imageComponent.fillAmount = normalizedValue;
    }

    public float GetProgress()
    {
        return _currentValue;
    }

    public float GetNormalizedProgress()
    {
        return (_currentValue - _minValue) / (_maxValue - _minValue);
    }

    public void SetRange(float min, float max)
    {
        _minValue = min;
        _maxValue = max;
        ApplyProgress();
    }

    public void SetInverse(bool inverse)
    {
        _inverse = inverse;
        ApplyProgress();
    }

    public void SetColor(Color color)
    {
        EnsureImageComponentExists();
        if (_imageComponent != null)
        {
            _imageComponent.color = color;
        }
    }

    public void SetFillMethod(Image.FillMethod method, int origin = 0)
    {
        EnsureImageComponentExists();
        if (_imageComponent != null)
        {
            _imageComponent.fillMethod = method;
            _imageComponent.fillOrigin = origin;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {

        EnsureImageComponentExists();
        ApplyProgress();
    }

    public override string GetValidationError()
    {
        if (!HasValidBindName)
        {
            return $"UIBindProgress组件 [{gameObject.name}] 的bindName未设置";
        }

        var image = GetComponent<Image>();
        if (image == null)
        {
            return $"UIBindProgress组件 [{gameObject.name}] 没有Image组件";
        }

        var bindImage = GetComponent<UIBindImage>();
        if (bindImage != null)
        {
            return $"UIBindProgress组件 [{gameObject.name}] 不能与UIBindImage共存于同一节点，请移除UIBindImage";
        }

        return null;
    }
#endif
}
