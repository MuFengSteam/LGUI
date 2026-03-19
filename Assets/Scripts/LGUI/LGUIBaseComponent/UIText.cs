using UnityEngine;
using TMPro;
using System.Collections;

[AddComponentMenu("LGUI/UIText")]
public class UIText : TextMeshProUGUI
{
    #region 翻译功能

    [SerializeField]
    [Tooltip("是否启用翻译功能")]
    private bool _enableTranslation = false;

    [SerializeField]
    [Tooltip("翻译ID，由导出工具自动分配")]
    private int _translationId = 0;

    [SerializeField]
    [Tooltip("原始文本内容")]
    private string _originalText = "";

    [SerializeField]
    [Tooltip("节点路径")]
    private string _nodePath = "";

    #endregion

    #region 打字机效果

    [Header("打字机效果")]
    [SerializeField]
    [Tooltip("是否启用打字机效果")]
    private bool _enableTypeWriter = false;

    [SerializeField]
    [Tooltip("打字机速度（每秒显示的字符数）")]
    [Range(1f, 200f)]
    private float _typeWriterSpeed = 30f;

    [SerializeField]
    [Tooltip("打字机完成后的回调延迟（秒）")]
    private float _typeWriterCompleteDelay = 0f;

    public bool enableTypeWriter
    {
        get => _enableTypeWriter;
        set => _enableTypeWriter = value;
    }

    public float typeWriterSpeed
    {
        get => _typeWriterSpeed;
        set => _typeWriterSpeed = Mathf.Max(1f, value);
    }

    public float typeWriterCompleteDelay
    {
        get => _typeWriterCompleteDelay;
        set => _typeWriterCompleteDelay = Mathf.Max(0f, value);
    }

    public bool IsTypeWriterPlaying => _typeWriterCoroutine != null;

    public System.Action OnTypeWriterComplete;

    private Coroutine _typeWriterCoroutine;

    private string _targetText;

    #endregion

    #region 翻译功能属性

    public bool EnableTranslation
    {
        get => _enableTranslation;
        set => _enableTranslation = value;
    }

    public int TranslationId => _translationId;

    public string OriginalText => _originalText;

    public string NodePath => _nodePath;

    public bool HasValidTranslationId => _enableTranslation && _translationId >= 600000;

    public bool NeedsTranslation => _enableTranslation && _translationId > 0;

    private bool _isRegisteredForLanguageChange = false;

    protected override void Start()
    {
        base.Start();

    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        StopTypeWriter();
        OnTypeWriterComplete = null;
    }

    #endregion

    #region 编辑器方法（供导出工具使用）

#if UNITY_EDITOR

    public void SetTranslationId(int id)
    {
        _translationId = id;
    }

    public void ClearTranslation()
    {
        _translationId = 0;
        _originalText = text;
        _nodePath = "";
    }

    public void UpdateOriginalText()
    {
        _originalText = text;
    }

    public void GenerateNodePath()
    {
        _nodePath = GetNodePath();
    }

    private string GetNodePath()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        Transform current = transform;

        while (current != null)
        {
            if (sb.Length > 0)
            {
                sb.Insert(0, "-");
            }
            sb.Insert(0, current.name);
            current = current.parent;
        }

        return sb.ToString();
    }
#endif

    #endregion

    #region 文本功能

    public void SetText(string value)
    {
        if (_enableTypeWriter)
        {
            StartTypeWriter(value);
        }
        else
        {
            text = value;
        }
    }

    public void SetTextImmediate(string value)
    {
        StopTypeWriter();
        text = value;
    }

    public void SetText(string format, params object[] args)
    {
        SetText(string.Format(format, args));
    }

    public void SetTextImmediate(string format, params object[] args)
    {
        SetTextImmediate(string.Format(format, args));
    }

    public void SetNumber(int value)
    {
        text = value.ToString();
    }

    public void SetNumber(int value, string format)
    {
        text = value.ToString(format);
    }

    public void SetNumber(float value, int decimalPlaces = 2)
    {
        text = value.ToString($"F{decimalPlaces}");
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

    public void SetColorRGB(Color newColor)
    {
        var c = color;
        c.r = newColor.r;
        c.g = newColor.g;
        c.b = newColor.b;
        color = c;
    }

    public void EnableRichText(bool enable = true)
    {
        richText = enable;
    }

    public float GetPreferredWidth()
    {
        return preferredWidth;
    }

    public float GetPreferredHeight()
    {
        return preferredHeight;
    }

    public void FitToText()
    {
        ForceMeshUpdate();
        rectTransform.sizeDelta = new Vector2(preferredWidth, preferredHeight);
    }

    #endregion

    #region 打字机效果方法

    public void StartTypeWriter(string content, System.Action onComplete = null)
    {
        StopTypeWriter();
        _targetText = content;

        if (onComplete != null)
        {
            OnTypeWriterComplete = onComplete;
        }

        if (string.IsNullOrEmpty(content))
        {
            text = "";
            OnTypeWriterComplete?.Invoke();
            return;
        }

        _typeWriterCoroutine = StartCoroutine(TypeWriterCoroutine());
    }

    public void StartTypeWriter(string content, float speed, System.Action onComplete = null)
    {
        _typeWriterSpeed = Mathf.Max(1f, speed);
        StartTypeWriter(content, onComplete);
    }

    public void StopTypeWriter(bool showFullText = false)
    {
        if (_typeWriterCoroutine != null)
        {
            StopCoroutine(_typeWriterCoroutine);
            _typeWriterCoroutine = null;

            if (showFullText && !string.IsNullOrEmpty(_targetText))
            {
                text = _targetText;
            }
        }
    }

    public void SkipTypeWriter()
    {
        if (_typeWriterCoroutine != null)
        {
            StopCoroutine(_typeWriterCoroutine);
            _typeWriterCoroutine = null;

            if (!string.IsNullOrEmpty(_targetText))
            {
                text = _targetText;
            }

            OnTypeWriterComplete?.Invoke();
        }
    }

    private bool _isTypeWriterPaused = false;

    public void PauseTypeWriter()
    {
        _isTypeWriterPaused = true;
    }

    public void ResumeTypeWriter()
    {
        _isTypeWriterPaused = false;
    }

    public bool IsTypeWriterPaused => _isTypeWriterPaused;

    private IEnumerator TypeWriterCoroutine()
    {
        text = "";
        int currentCharIndex = 0;
        int totalLength = _targetText.Length;
        float charInterval = 1f / _typeWriterSpeed;
        float timer = 0f;

        while (currentCharIndex < totalLength)
        {

            while (_isTypeWriterPaused)
            {
                yield return null;
            }

            timer += Time.deltaTime;

            while (timer >= charInterval && currentCharIndex < totalLength)
            {
                timer -= charInterval;
                currentCharIndex++;

                currentCharIndex = SkipRichTextTags(_targetText, currentCharIndex);

                text = _targetText.Substring(0, currentCharIndex);
            }

            yield return null;
        }

        text = _targetText;
        _typeWriterCoroutine = null;

        if (_typeWriterCompleteDelay > 0)
        {
            yield return new WaitForSeconds(_typeWriterCompleteDelay);
        }

        OnTypeWriterComplete?.Invoke();
    }

    private int SkipRichTextTags(string content, int currentIndex)
    {
        if (currentIndex >= content.Length)
            return currentIndex;

        int lastTagStart = content.LastIndexOf('<', currentIndex - 1);
        if (lastTagStart >= 0)
        {
            int tagEnd = content.IndexOf('>', lastTagStart);
            if (tagEnd >= currentIndex)
            {

                return tagEnd + 1;
            }
        }

        return currentIndex;
    }

    public float GetTypeWriterProgress()
    {
        if (string.IsNullOrEmpty(_targetText) || _targetText.Length == 0)
            return 1f;

        return (float)text.Length / _targetText.Length;
    }

    #endregion

#if UNITY_EDITOR
    protected override void Reset()
    {
        base.Reset();

        raycastTarget = false;
        enableWordWrapping = true;
        overflowMode = TextOverflowModes.Ellipsis;
    }

    protected override void OnValidate()
    {
        base.OnValidate();
    }
#endif
}
