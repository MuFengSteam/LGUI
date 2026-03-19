using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[AddComponentMenu("LGUI/UIScrollView")]
public class UIScrollView : ScrollRect
{
    [Header("扩展设置")]
    [Tooltip("滚动动画时长")]
    [SerializeField] private float _scrollDuration = 0.3f;

    [Tooltip("启用滚动到顶部/底部时的回调")]
    [SerializeField] private bool _enableEdgeCallbacks = false;

    public event Action OnScrollToTop;
    public event Action OnScrollToBottom;
    public event Action OnScrollToLeft;
    public event Action OnScrollToRight;
    public event Action<Vector2> OnScrolling;

    private bool _isScrolling = false;
    private Vector2 _targetPosition;
    private float _scrollStartTime;
    private Vector2 _scrollStartPosition;

    #region 公共属性

    public float ScrollDuration
    {
        get => _scrollDuration;
        set => _scrollDuration = Mathf.Max(0f, value);
    }

    public bool IsScrolling => _isScrolling;

    #endregion

    #region 滚动控制

    public void ScrollToTop(bool animated = true)
    {
        ScrollTo(new Vector2(normalizedPosition.x, 1f), animated);
    }

    public void ScrollToBottom(bool animated = true)
    {
        ScrollTo(new Vector2(normalizedPosition.x, 0f), animated);
    }

    public void ScrollToLeft(bool animated = true)
    {
        ScrollTo(new Vector2(0f, normalizedPosition.y), animated);
    }

    public void ScrollToRight(bool animated = true)
    {
        ScrollTo(new Vector2(1f, normalizedPosition.y), animated);
    }

    public void ScrollTo(Vector2 normalizedPos, bool animated = true)
    {
        normalizedPos.x = Mathf.Clamp01(normalizedPos.x);
        normalizedPos.y = Mathf.Clamp01(normalizedPos.y);

        if (!animated || _scrollDuration <= 0)
        {
            normalizedPosition = normalizedPos;
            return;
        }

        _targetPosition = normalizedPos;
        _scrollStartPosition = normalizedPosition;
        _scrollStartTime = Time.unscaledTime;
        _isScrolling = true;
    }

    public void ScrollToChild(RectTransform child, bool animated = true)
    {
        if (child == null || content == null) return;

        Vector2 contentSize = content.rect.size;
        Vector2 viewportSize = viewport != null ? viewport.rect.size : ((RectTransform)transform).rect.size;

        if (contentSize.x <= viewportSize.x && contentSize.y <= viewportSize.y)
        {

            return;
        }

        Vector2 childLocalPos = content.InverseTransformPoint(child.position);
        Vector2 normalizedPos = new Vector2(
            contentSize.x > viewportSize.x ? Mathf.Clamp01(-childLocalPos.x / (contentSize.x - viewportSize.x)) : 0f,
            contentSize.y > viewportSize.y ? Mathf.Clamp01(childLocalPos.y / (contentSize.y - viewportSize.y)) : 1f
        );

        ScrollTo(normalizedPos, animated);
    }

    public void StopScrolling()
    {
        _isScrolling = false;
        velocity = Vector2.zero;
    }

    #endregion

    #region Unity生命周期

    protected override void LateUpdate()
    {
        base.LateUpdate();

        if (_isScrolling)
        {
            float elapsed = Time.unscaledTime - _scrollStartTime;
            float t = Mathf.Clamp01(elapsed / _scrollDuration);

            t = EaseOutCubic(t);

            normalizedPosition = Vector2.Lerp(_scrollStartPosition, _targetPosition, t);

            if (t >= 1f)
            {
                _isScrolling = false;
                normalizedPosition = _targetPosition;
            }
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        onValueChanged.AddListener(OnScrollValueChanged);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        onValueChanged.RemoveListener(OnScrollValueChanged);
    }

    #endregion

    #region 事件处理

    private void OnScrollValueChanged(Vector2 value)
    {
        OnScrolling?.Invoke(value);

        if (!_enableEdgeCallbacks) return;

        if (vertical)
        {
            if (value.y >= 0.99f)
            {
                OnScrollToTop?.Invoke();
            }
            else if (value.y <= 0.01f)
            {
                OnScrollToBottom?.Invoke();
            }
        }

        if (horizontal)
        {
            if (value.x <= 0.01f)
            {
                OnScrollToLeft?.Invoke();
            }
            else if (value.x >= 0.99f)
            {
                OnScrollToRight?.Invoke();
            }
        }
    }

    #endregion

    #region 辅助方法

    private float EaseOutCubic(float t)
    {
        return 1f - Mathf.Pow(1f - t, 3f);
    }

    public void RefreshContentSize()
    {
        if (content != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        }
    }

    public float GetVerticalScrollPercent()
    {
        return normalizedPosition.y;
    }

    public float GetHorizontalScrollPercent()
    {
        return normalizedPosition.x;
    }

    #endregion

#if UNITY_EDITOR
    protected override void Reset()
    {
        base.Reset();

        movementType = MovementType.Elastic;
        elasticity = 0.1f;
        inertia = true;
        decelerationRate = 0.135f;
        scrollSensitivity = 1f;
    }
#endif
}
