using System;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("LGUI/UIRawImage")]
public class UIRawImage : RawImage
{
    private string _currentTexturePath;
    private bool _isRegisteredForLanguageChange = false;

    #region 本地化加载

    public void SetTextureByPath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogWarning("[UIRawImage] 路径为空");
            return;
        }

        _currentTexturePath = path;

        LoadAndSetTexture(path);
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
            texture = loadedTexture;
        }
        else
        {
            Debug.LogWarning($"[UIRawImage] 无法加载纹理资源: {normalizedAddress}");
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
        if (string.IsNullOrEmpty(path)) return false;
        return path.Replace('\\', '/').ToLowerInvariant().Contains("localization");
    }

    #endregion

    #region 语言变更监听

    protected override void OnDestroy()
    {
        base.OnDestroy();

    }

    #endregion

    #region 扩展功能

    public void SetAlpha(float alpha)
    {
        var c = color;
        c.a = Mathf.Clamp01(alpha);
        color = c;
    }

    public void SetUVRectValue(Rect rect)
    {
        uvRect = rect;
    }

    #endregion

#if UNITY_EDITOR
    protected override void Reset()
    {
        base.Reset();
        raycastTarget = false;
    }
#endif
}
