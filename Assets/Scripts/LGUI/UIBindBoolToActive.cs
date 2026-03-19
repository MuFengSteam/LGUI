// 作者: 木枫LL
// GitHub: https://github.com/MuFengSteam/
// 小红书: https://www.xiaohongshu.com/user/profile/67c5dc1a000000000a03e5d0

using UnityEngine;

public class UIBindBoolToActive : UIBase
{
    public enum HideMode
    {
        [Tooltip("使用SetActive控制显隐")]
        Active,
        [Tooltip("通过移动到屏幕外控制显隐")]
        Position,
        [Tooltip("通过缩放控制显隐")]
        Scale
    }

    [Header("显隐设置")]
    [Tooltip("显隐模式")]
    public HideMode hideMode = HideMode.Active;

    [Tooltip("反选：勾选后逻辑反转，true时隐藏，false时显示")]
    [SerializeField]
    private bool _invert = false;

    private readonly Vector2 _hidePosition = new Vector2(3000f, 3000f);

    private RectTransform _rectTransform;
    private Vector2 _originalPosition;
    private Vector3 _originalScale;
    private bool _isShowing;
    private bool _isShowingInitialized = false;

    public bool Invert
    {
        get => _invert;
        set => _invert = value;
    }

    public override string ComponentTypeName => "UIBindBoolToActive";
    public override string BindDataType => "bool";

#if UNITY_EDITOR
    protected override void Reset()
    {
        base.Reset();
        InitializeRectTransform();
    }
#endif

    private void Start()
    {

    }

    protected override void Initialize()
    {
        if (_initialized) return;
        InitializeRectTransform();
        base.Initialize();
    }

    private void InitializeRectTransform()
    {
        _rectTransform = GetComponent<RectTransform>();
        if (_rectTransform == null)
        {
            _rectTransform = gameObject.AddComponent<RectTransform>();
        }

        _originalPosition = _rectTransform.anchoredPosition;
        _originalScale = _rectTransform.localScale;
    }

    public void SetShow(bool show)
    {
        EnsureInitialized();

        bool actualShow = _invert ? !show : show;

        if (_isShowingInitialized && _isShowing == actualShow) return;

        _isShowing = actualShow;
        _isShowingInitialized = true;

        switch (hideMode)
        {
            case HideMode.Active:
                gameObject.SetActive(actualShow);
                break;

            case HideMode.Position:
                _rectTransform.anchoredPosition = actualShow ? _originalPosition : _hidePosition;
                break;

            case HideMode.Scale:
                _rectTransform.localScale = actualShow ? _originalScale : Vector3.zero;
                break;
        }
    }

    public bool GetShow()
    {
        EnsureInitialized();
        return _isShowing;
    }

    public void Toggle()
    {
        SetShow(!GetShow());
    }

#if UNITY_EDITOR
    public override string GetValidationError()
    {
        if (!HasValidBindName)
        {
            return $"UIBindBoolToActive组件 [{gameObject.name}] 的bindName未设置";
        }
        return null;
    }
#endif
}
