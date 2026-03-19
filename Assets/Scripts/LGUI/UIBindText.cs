// 作者: 木枫LL
// GitHub: https://github.com/MuFengSteam/
// 小红书: https://www.xiaohongshu.com/user/profile/67c5dc1a000000000a03e5d0

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

            var tmpText = GetComponent<TMPro.TMP_Text>();

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
    }
#endif
}
