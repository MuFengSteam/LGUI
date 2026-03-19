using UnityEngine;

public class UIBindText : UIBase
{
    [Header("Text设置")]
    [Tooltip("文本格式化字符串，例如：'得分：{0}'")]
    public string formatString = "";

    private UIText _textComponent;

    public override string ComponentTypeName => "UIBindText";
    public override string BindDataType => "string";

#if UNITY_EDITOR
    protected override void Reset()
    {
        base.Reset();
        EnsureTextComponentExists();
    }
#endif

    protected override void Initialize()
    {
        if (_initialized) return;
        EnsureTextComponentExists();
        base.Initialize();
    }

    private void EnsureTextComponentExists()
    {
        _textComponent = GetComponent<UIText>();
        if (_textComponent == null)
        {

            var uguiText = GetComponent<UnityEngine.UI.Text>();
            if (uguiText != null)
            {
                Debug.LogWarning($"[UIBindText] 节点 '{gameObject.name}' 使用了UGUI原生Text，建议改用UIText组件");
            }

            var tmpText = GetComponent<TMPro.TMP_Text>();
            if (tmpText != null && !(tmpText is UIText))
            {
                Debug.LogWarning($"[UIBindText] 节点 '{gameObject.name}' 使用了TextMeshPro原生组件，建议改用UIText组件");
            }

            _textComponent = gameObject.AddComponent<UIText>();
        }
    }

    public UIText GetTextComponent()
    {
        EnsureInitialized();
        return _textComponent;
    }

    public void SetText(string value)
    {
        EnsureInitialized();

        string finalText = !string.IsNullOrEmpty(formatString) ?
            string.Format(formatString, value) : value;

        if (_textComponent != null)
        {
            _textComponent.SetText(finalText);
        }
    }

    public void SetTextImmediate(string value)
    {
        EnsureInitialized();

        string finalText = !string.IsNullOrEmpty(formatString) ?
            string.Format(formatString, value) : value;

        if (_textComponent != null)
        {
            _textComponent.SetTextImmediate(finalText);
        }
    }

    public string GetText()
    {
        EnsureInitialized();

        if (_textComponent != null)
        {
            return _textComponent.text;
        }
        return string.Empty;
    }

    public void SetAlpha(float alpha)
    {
        EnsureInitialized();

        if (_textComponent != null)
        {
            _textComponent.SetAlpha(alpha);
        }
    }

    public void SetColor(Color color)
    {
        EnsureInitialized();

        if (_textComponent != null)
        {
            _textComponent.color = color;
        }
    }

#if UNITY_EDITOR
    public override string GetValidationError()
    {
        if (!HasValidBindName)
        {
            return $"UIBindText组件 [{gameObject.name}] 的bindName未设置";
        }

        var uguiText = GetComponent<UnityEngine.UI.Text>();
        if (uguiText != null)
        {
            return $"UIBindText组件 [{gameObject.name}] 检测到UGUI Text，请改用UIText组件";
        }

        return null;
    }

    protected override void OnValidate()
    {
        base.OnValidate();

        var uguiText = GetComponent<UnityEngine.UI.Text>();
        if (uguiText != null)
        {
            Debug.LogError($"[UIBindText] 节点 '{gameObject.name}' 使用了UGUI Text组件，请改用UIText组件！");
        }
    }
#endif
}
