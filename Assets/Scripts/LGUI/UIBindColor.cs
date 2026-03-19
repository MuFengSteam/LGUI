// 作者: 木枫LL
// GitHub: https://github.com/MuFengSteam/
// 小红书: https://www.xiaohongshu.com/user/profile/67c5dc1a000000000a03e5d0

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UIBindColor : UIBase
{
    [System.Serializable]
    public class ColorPreset
    {
        public string key;
        public Color color = Color.white;
    }

    [Header("颜色设置")]
    [Tooltip("默认颜色")]
    public Color defaultColor = Color.white;

    [Tooltip("是否在Start时设置默认颜色")]
    public bool setDefaultOnStart = true;

    [Tooltip("是否影响子节点")]
    public bool affectChildren = false;

    [Header("颜色预设")]
    [Tooltip("预设颜色列表，可通过键值快速设置")]
    public List<ColorPreset> colorPresets = new List<ColorPreset>();

    private List<Graphic> _graphics = new List<Graphic>();
    private Color _currentColor = Color.white;

    public override string ComponentTypeName => "UIBindColor";
    public override string BindDataType => "Color";

#if UNITY_EDITOR
    protected override void Reset()
    {
        base.Reset();
        CacheGraphics();
    }
#endif

    private void Start()
    {
        if (setDefaultOnStart)
        {
            SetColor(defaultColor);
        }
    }

    protected override void Initialize()
    {
        if (_initialized) return;
        CacheGraphics();
        base.Initialize();
    }

    private void CacheGraphics()
    {
        _graphics.Clear();

        if (affectChildren)
        {
            _graphics.AddRange(GetComponentsInChildren<Graphic>(true));
        }
        else
        {
            var graphic = GetComponent<Graphic>();
            if (graphic != null)
            {
                _graphics.Add(graphic);
            }
        }
    }

    public void SetColor(Color color)
    {
        EnsureInitialized();

        _currentColor = color;

        foreach (var graphic in _graphics)
        {
            if (graphic != null)
            {
                graphic.color = color;
            }
        }
    }

    public void SetColor(string hexColor)
    {

        var preset = colorPresets.Find(p => p.key == hexColor);
        if (preset != null)
        {
            SetColor(preset.color);
            return;
        }

        if (ColorUtility.TryParseHtmlString(hexColor, out Color color))
        {
            SetColor(color);
        }
        else if (ColorUtility.TryParseHtmlString("#" + hexColor, out color))
        {
            SetColor(color);
        }
    }

    public void SetColorByPreset(string presetKey)
    {
        var preset = colorPresets.Find(p => p.key == presetKey);
        if (preset != null)
        {
            SetColor(preset.color);
        }
    }

    public Color GetColor()
    {
        return _currentColor;
    }

    public void SetRGB(Color color)
    {
        color.a = _currentColor.a;
        SetColor(color);
    }

    public void SetAlpha(float alpha)
    {
        Color color = _currentColor;
        color.a = Mathf.Clamp01(alpha);
        SetColor(color);
    }

    public void TweenColor(Color targetColor, float duration = 0.3f)
    {
        StartCoroutine(TweenColorCoroutine(targetColor, duration));
    }

    private System.Collections.IEnumerator TweenColorCoroutine(Color targetColor, float duration)
    {
        Color startColor = _currentColor;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            SetColor(Color.Lerp(startColor, targetColor, t));
            yield return null;
        }

        SetColor(targetColor);
    }

#if UNITY_EDITOR
    public override string GetValidationError()
    {
        if (!HasValidBindName)
        {
            return $"UIBindColor组件 [{gameObject.name}] 的bindName未设置";
        }
        return null;
    }

    private void OnValidate()
    {

        if (!Application.isPlaying)
        {
            CacheGraphics();
            foreach (var graphic in _graphics)
            {
                if (graphic != null)
                {
                    graphic.color = defaultColor;
                }
            }
        }
    }
#endif
}
