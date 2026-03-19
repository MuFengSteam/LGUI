// 作者: 木枫LL
// GitHub: https://github.com/MuFengSteam/
// 小红书: https://www.xiaohongshu.com/user/profile/67c5dc1a000000000a03e5d0

using System;
using UnityEngine;
using UnityEngine.UI;

public class UIBindTexture : UIBase
{

    private const string TEXTURE_BASE_PATH = "Resources/Texture/";

    private const string DEFAULT_EXTENSION = ".png";

    private RawImage _rawImageComponent;
    private CanvasGroup _canvasGroup;

    private string _currentTexturePath;

    public override string ComponentTypeName => "UIBindTexture";
    public override string BindDataType => "string";

#if UNITY_EDITOR
    protected override void Reset()
    {
        base.Reset();
        EnsureRawImageComponentExists();
    }
#endif

    protected override void Initialize()
    {
        if (_initialized) return;
        EnsureRawImageComponentExists();
        base.Initialize();
    }

    private void EnsureRawImageComponentExists()
    {

        if (this == null || gameObject == null)
        {
            return;
        }

        if (_rawImageComponent == null)
        {
            _rawImageComponent = GetComponent<RawImage>();
        }

        if (_rawImageComponent == null)
        {
            _rawImageComponent = gameObject.AddComponent<RawImage>();

            if (_rawImageComponent != null)
            {
                _rawImageComponent.color = Color.white;
            }
        }
    }

    public void SetTextureByPath(string path)
    {
        EnsureInitialized();

        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        _currentTexturePath = path;

        string fullPath = BuildFullTexturePath(path);
        LoadAndSetTexture(fullPath);
    }

    private string BuildFullTexturePath(string simplePath)
    {

        string normalizedPath = simplePath.Replace('\\', '/');

        normalizedPath = normalizedPath.TrimStart('/');

        string lowerPath = normalizedPath.ToLowerInvariant();
        if (lowerPath.StartsWith("resources/") || lowerPath.Contains("localization"))
        {

            return EnsureExtension(normalizedPath);
        }

        string fullPath = TEXTURE_BASE_PATH + normalizedPath;
        return EnsureExtension(fullPath);
    }

    private string EnsureExtension(string path)
    {

        string lowerPath = path.ToLowerInvariant();
        if (lowerPath.EndsWith(".png") || lowerPath.EndsWith(".jpg") ||
            lowerPath.EndsWith(".jpeg") || lowerPath.EndsWith(".tga") ||
            lowerPath.EndsWith(".bmp") || lowerPath.EndsWith(".psd"))
        {
            return path;
        }

        return path + DEFAULT_EXTENSION;
    }

    private void LoadAndSetTexture(string path)
    {

            LoadTextureFromAddress(path);

    }

    private void LoadTextureFromAddress(string address)
    {
        string normalizedAddress = NormalizeResourcesPath(address);
        Texture loadedTexture = Resources.Load<Texture>(normalizedAddress);

        if (loadedTexture != null)
        {
            SetTexture(loadedTexture);
        }
    }

    private static string NormalizeResourcesPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return string.Empty;
        }

        string normalizedPath = path.Replace('\\', '/').Trim();

        if (normalizedPath.StartsWith("Assets/Resources/", StringComparison.OrdinalIgnoreCase))
        {
            normalizedPath = normalizedPath.Substring("Assets/Resources/".Length);
        }
        else if (normalizedPath.StartsWith("Resources/", StringComparison.OrdinalIgnoreCase))
        {
            normalizedPath = normalizedPath.Substring("Resources/".Length);
        }

        int extensionIndex = normalizedPath.LastIndexOf('.');
        if (extensionIndex > normalizedPath.LastIndexOf('/'))
        {
            normalizedPath = normalizedPath.Substring(0, extensionIndex);
        }

        return normalizedPath;
    }

    private bool IsLocalizationPath(string path)
    {
        if (string.IsNullOrEmpty(path))
            return false;

        string normalizedPath = path.Replace('\\', '/').ToLowerInvariant();
        return normalizedPath.Contains("localization");
    }

    public void RefreshOnLanguageChanged()
    {
        if (!string.IsNullOrEmpty(_currentTexturePath))
        {

            string fullPath = BuildFullTexturePath(_currentTexturePath);
            LoadAndSetTexture(fullPath);
        }
    }

    public void SetTexture(Texture value)
    {
        EnsureInitialized();

        if (_rawImageComponent != null)
        {
            _rawImageComponent.texture = value;
        }
    }

    public Texture GetTexture()
    {
        EnsureInitialized();

        if (_rawImageComponent != null)
        {
            return _rawImageComponent.texture;
        }
        return null;
    }

    public void SetColor(Color color)
    {
        EnsureInitialized();

        if (_rawImageComponent != null)
        {
            _rawImageComponent.color = color;
        }
    }

    public Color GetColor()
    {
        EnsureInitialized();

        if (_rawImageComponent != null)
        {
            return _rawImageComponent.color;
        }
        return Color.white;
    }

    public void SetUVRect(Rect uvRect)
    {
        EnsureInitialized();

        if (_rawImageComponent != null)
        {
            _rawImageComponent.uvRect = uvRect;
        }
    }

    public void SetAlpha(float alpha)
    {
        EnsureInitialized();

        if (_rawImageComponent != null)
        {
            Color color = _rawImageComponent.color;
            color.a = Mathf.Clamp01(alpha);
            _rawImageComponent.color = color;
        }
    }

#if UNITY_EDITOR
    public override string GetValidationError()
    {
        if (!HasValidBindName)
        {
            return $"UIBindTexture组件 [{gameObject.name}] 的bindName未设置";
        }
        return null;
    }
#endif
}
