using UnityEngine;
using UnityEngine.UI;

public class UIBindAlpha : UIBase
{
    public enum AlphaMode
    {
        [Tooltip("使用CanvasGroup控制透明度（推荐，可以影响所有子节点）")]
        CanvasGroup,
        [Tooltip("使用Graphic组件控制透明度（仅影响当前节点的UI组件）")]
        Graphic
    }

    [Header("透明度设置")]
    [Tooltip("透明度控制模式")]
    public AlphaMode alphaMode = AlphaMode.CanvasGroup;

    [Tooltip("默认透明度")]
    [Range(0f, 1f)]
    public float defaultAlpha = 1f;

    [Tooltip("是否在Start时设置默认值")]
    public bool setDefaultOnStart = true;

    private CanvasGroup _canvasGroup;
    private Graphic _graphic;
    private float _currentAlpha = 1f;

    public override string ComponentTypeName => "UIBindAlpha";
    public override string BindDataType => "float";

#if UNITY_EDITOR
    protected override void Reset()
    {
        base.Reset();
        EnsureComponentExists();
    }
#endif

    private void Start()
    {
        if (setDefaultOnStart)
        {
            SetAlpha(defaultAlpha);
        }
    }

    protected override void Initialize()
    {
        if (_initialized) return;
        EnsureComponentExists();
        base.Initialize();
    }

    private void EnsureComponentExists()
    {
        if (alphaMode == AlphaMode.CanvasGroup)
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
            {
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }
        else
        {
            _graphic = GetComponent<Graphic>();

        }
    }

    public void SetAlpha(float alpha)
    {
        EnsureInitialized();

        _currentAlpha = Mathf.Clamp01(alpha);

        if (alphaMode == AlphaMode.CanvasGroup && _canvasGroup != null)
        {
            _canvasGroup.alpha = _currentAlpha;
        }
        else if (alphaMode == AlphaMode.Graphic && _graphic != null)
        {
            Color color = _graphic.color;
            color.a = _currentAlpha;
            _graphic.color = color;
        }
    }

    public float GetAlpha()
    {
        EnsureInitialized();

        if (alphaMode == AlphaMode.CanvasGroup && _canvasGroup != null)
        {
            return _canvasGroup.alpha;
        }
        else if (alphaMode == AlphaMode.Graphic && _graphic != null)
        {
            return _graphic.color.a;
        }
        return _currentAlpha;
    }

    public void FadeIn(float duration = 0.3f)
    {
        StartCoroutine(FadeCoroutine(1f, duration));
    }

    public void FadeOut(float duration = 0.3f)
    {
        StartCoroutine(FadeCoroutine(0f, duration));
    }

    private System.Collections.IEnumerator FadeCoroutine(float targetAlpha, float duration)
    {
        float startAlpha = GetAlpha();
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            SetAlpha(Mathf.Lerp(startAlpha, targetAlpha, t));
            yield return null;
        }

        SetAlpha(targetAlpha);
    }

#if UNITY_EDITOR
    public override string GetValidationError()
    {
        if (!HasValidBindName)
        {
            return $"UIBindAlpha组件 [{gameObject.name}] 的bindName未设置";
        }
        return null;
    }
#endif
}
