using UnityEngine;

public class UIBindImage : UIBase
{
    private UIImage _imageComponent;

    public override string ComponentTypeName => "UIBindImage";
    public override string BindDataType => "int";

#if UNITY_EDITOR
    protected override void Reset()
    {
        base.Reset();
        EnsureImageComponentExists();
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

        if (this == null || gameObject == null)
        {
            return;
        }

        if (_imageComponent == null)
        {
            _imageComponent = GetComponent<UIImage>();
        }

        if (_imageComponent == null)
        {
            _imageComponent = gameObject.AddComponent<UIImage>();

            if (_imageComponent != null)
            {
                _imageComponent.color = Color.white;
            }
        }
    }

    public UIImage GetImageComponent()
    {
        EnsureInitialized();
        return _imageComponent;
    }

    public void SetImageById(int imageConfigId)
    {
        EnsureInitialized();

        if (_imageComponent != null)
        {
            _imageComponent.SetSpriteById(imageConfigId);
        }
    }

    public void SetSpriteByPath(string path)
    {
        EnsureInitialized();

        if (_imageComponent != null)
        {
            _imageComponent.SetSpriteByPath(path);
        }
    }

    public void SetSprite(Sprite value)
    {
        EnsureInitialized();

        if (_imageComponent != null)
        {
            _imageComponent.sprite = value;
        }
    }

    public Sprite GetSprite()
    {
        EnsureInitialized();

        if (_imageComponent != null)
        {
            return _imageComponent.sprite;
        }
        return null;
    }

    public void SetColor(Color color)
    {
        EnsureInitialized();

        if (_imageComponent != null)
        {
            _imageComponent.color = color;
        }
    }

    public Color GetColor()
    {
        EnsureInitialized();

        if (_imageComponent != null)
        {
            return _imageComponent.color;
        }
        return Color.white;
    }

    public void SetFillAmount(float amount)
    {
        EnsureInitialized();

        if (_imageComponent != null)
        {
            _imageComponent.SetFillAmountValue(amount);
        }
    }

    public float GetFillAmount()
    {
        EnsureInitialized();

        if (_imageComponent != null)
        {
            return _imageComponent.fillAmount;
        }
        return 0f;
    }

    public void SetAlpha(float alpha)
    {
        EnsureInitialized();

        if (_imageComponent != null)
        {
            _imageComponent.SetAlpha(alpha);
        }
    }

#if UNITY_EDITOR
    public override string GetValidationError()
    {
        if (!HasValidBindName)
        {
            return $"UIBindImage组件 [{gameObject.name}] 的bindName未设置";
        }
        return null;
    }
#endif
}
