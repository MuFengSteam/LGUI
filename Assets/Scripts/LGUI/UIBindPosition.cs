// 作者: 木枫LL
// GitHub: https://github.com/MuFengSteam/
// 小红书: https://www.xiaohongshu.com/user/profile/67c5dc1a000000000a03e5d0

using UnityEngine;

[AddComponentMenu("LGUI/UIBindPosition")]
[RequireComponent(typeof(RectTransform))]
public class UIBindPosition : UIBase
{
    public enum PositionMode
    {
        [Tooltip("使用 anchoredPosition（相对于锚点的位置）")]
        Anchored,
        [Tooltip("使用 localPosition（本地坐标）")]
        Local
    }

    [Header("位置设置")]
    [Tooltip("位置模式")]
    public PositionMode positionMode = PositionMode.Anchored;

    private RectTransform _rectTransform;

    public override string ComponentTypeName => "UIBindPosition";
    public override string BindDataType => "Vector2";

#if UNITY_EDITOR
    protected override void Reset()
    {
        base.Reset();
        InitializeRectTransform();
    }
#endif

    protected override void Initialize()
    {
        if (_initialized) return;
        InitializeRectTransform();
        base.Initialize();
    }

    private void InitializeRectTransform()
    {
        if (_rectTransform == null)
        {
            _rectTransform = GetComponent<RectTransform>();
        }
    }

    public void SetPosition(Vector2 position)
    {
        EnsureInitialized();

        if (_rectTransform == null) return;

        switch (positionMode)
        {
            case PositionMode.Anchored:
                _rectTransform.anchoredPosition = position;
                break;
            case PositionMode.Local:
                _rectTransform.localPosition = new Vector3(position.x, position.y, _rectTransform.localPosition.z);
                break;
        }
    }

    public void SetPosition(float x, float y)
    {
        SetPosition(new Vector2(x, y));
    }

    public void SetPosition(float[] position)
    {
        if (position == null || position.Length < 2)
        {
            return;
        }
        SetPosition(new Vector2(position[0], position[1]));
    }

    public Vector2 GetPosition()
    {
        EnsureInitialized();

        if (_rectTransform == null) return Vector2.zero;

        switch (positionMode)
        {
            case PositionMode.Anchored:
                return _rectTransform.anchoredPosition;
            case PositionMode.Local:
                return new Vector2(_rectTransform.localPosition.x, _rectTransform.localPosition.y);
            default:
                return Vector2.zero;
        }
    }

    public void SetX(float x)
    {
        var pos = GetPosition();
        pos.x = x;
        SetPosition(pos);
    }

    public void SetY(float y)
    {
        var pos = GetPosition();
        pos.y = y;
        SetPosition(pos);
    }

    public void Offset(Vector2 offset)
    {
        SetPosition(GetPosition() + offset);
    }

    public void Offset(float x, float y)
    {
        Offset(new Vector2(x, y));
    }

#if UNITY_EDITOR
    public override string GetValidationError()
    {
        if (!HasValidBindName)
        {
            return $"UIBindPosition组件 [{gameObject.name}] 的bindName未设置";
        }
        return null;
    }
#endif
}
