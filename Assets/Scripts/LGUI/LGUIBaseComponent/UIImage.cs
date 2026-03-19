using System;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("LGUI/UIImage")]
public class UIImage : Image
{

    private string _currentImagePath;
    private int _currentImageConfigId;
    private bool _isRegisteredForLanguageChange = false;

    #region 本地化图片加载

    public void SetSpriteByPath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogWarning($"[UIImage] 路径为空");
            return;
        }

        _currentImagePath = path;
        _currentImageConfigId = 0;

        LoadAndSetSprite(path);
    }

    public void SetSpriteById(int imageConfigId)
    {
        if (imageConfigId <= 0)
        {
            Debug.LogWarning($"[UIImage] 无效的ImageConfig Id: {imageConfigId}");
            return;
        }

        _currentImageConfigId = imageConfigId;
        _currentImagePath = null;

        LoadAndSetSprite(imageConfigId.ToString());
    }

    private void LoadAndSetSprite(string path)
    {

            LoadSpriteFromAddress(path);

    }

    private void LoadSpriteFromAddress(string address)
    {
        string normalizedAddress = NormalizeResourcesPath(address);
        Sprite loadedSprite = Resources.Load<Sprite>(normalizedAddress);

        if (loadedSprite != null)
        {
            sprite = loadedSprite;
        }
        else
        {
            Debug.LogWarning($"[UIImage] 无法加载图片资源: {normalizedAddress}");
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

    #endregion

    #region 语言变更监听

    #endregion

    #region 扩展功能

    public void SetNativeSizeSprite(Sprite newSprite)
    {
        sprite = newSprite;
        SetNativeSize();
    }

    public void SetAlpha(float alpha)
    {
        var c = color;
        c.a = Mathf.Clamp01(alpha);
        color = c;
    }

    public void FadeAlpha(float targetAlpha, float duration)
    {
        if (duration <= 0)
        {
            SetAlpha(targetAlpha);
            return;
        }

        StartCoroutine(FadeAlphaCoroutine(targetAlpha, duration));
    }

    private System.Collections.IEnumerator FadeAlphaCoroutine(float targetAlpha, float duration)
    {
        float startAlpha = color.a;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            SetAlpha(Mathf.Lerp(startAlpha, targetAlpha, t));
            yield return null;
        }

        SetAlpha(targetAlpha);
    }

    public void SetFillAmountValue(float amount)
    {
        type = Type.Filled;
        fillAmount = Mathf.Clamp01(amount);
    }

    public void SetColorRGB(Color newColor)
    {
        var c = color;
        c.r = newColor.r;
        c.g = newColor.g;
        c.b = newColor.b;
        color = c;
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
