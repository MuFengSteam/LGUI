using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[AddComponentMenu("LGUI/UIButton")]
public class UIButton : Button, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    #region 常量

    private const float HOVER_SCALE = 0.95f;

    private const float HOVER_ANIM_DURATION = 0.1f;

    private const float HOVER_OFFSET_RATIO = 0.15f;

    #endregion

    [Header("悬停效果设置")]
    [Tooltip("启用悬停缩放动画")]
    [SerializeField] private bool _enableHoverScale = true;

    [Tooltip("启用悬停偏移（向上偏移节点高度的15%）")]
    [SerializeField] private bool _enableHoverOffset = false;

    [Header("扩展设置")]
    [Tooltip("启用长按功能")]
    [SerializeField] private bool _enableLongPress = false;

    [Tooltip("长按触发时间（秒）")]
    [SerializeField] private float _longPressTime = 0.5f;

    [Tooltip("启用双击功能")]
    [SerializeField] private bool _enableDoubleClick = false;

    [Tooltip("双击间隔时间（秒）")]
    [SerializeField] private float _doubleClickInterval = 0.3f;

    [Tooltip("按钮音效（可选）")]
    [SerializeField] private AudioClip _clickSound;

    public event Action OnLongPress;
    public event Action OnDoubleClick;
    public event Action OnPointerDownEvent;
    public event Action OnPointerUpEvent;

    private bool _isPointerDown = false;
    private float _pointerDownTime = 0f;
    private bool _longPressTriggered = false;
    private float _lastClickTime = 0f;
    private int _clickCount = 0;

    private RectTransform _rectTransform;
    private Vector3 _originalScale;
    private Vector2 _originalAnchoredPosition;
    private bool _isHovering = false;
    private Coroutine _scaleCoroutine;
    private bool _hoverInitialized = false;

    #region 公共属性

    public bool EnableHoverScale
    {
        get => _enableHoverScale;
        set
        {
            _enableHoverScale = value;
            if (!value && _isHovering)
            {
                _isHovering = false;
                PlayHoverAnimation(false);
            }
        }
    }

    public bool EnableHoverOffset
    {
        get => _enableHoverOffset;
        set
        {
            _enableHoverOffset = value;
            if (!value && _isHovering)
            {
                _isHovering = false;
                PlayHoverAnimation(false);
            }
        }
    }

    public bool EnableLongPress
    {
        get => _enableLongPress;
        set => _enableLongPress = value;
    }

    public float LongPressTime
    {
        get => _longPressTime;
        set => _longPressTime = Mathf.Max(0.1f, value);
    }

    public bool EnableDoubleClick
    {
        get => _enableDoubleClick;
        set => _enableDoubleClick = value;
    }

    public float DoubleClickInterval
    {
        get => _doubleClickInterval;
        set => _doubleClickInterval = Mathf.Max(0.1f, value);
    }

    #endregion

    #region 指针事件

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        _isPointerDown = true;
        _pointerDownTime = Time.unscaledTime;
        _longPressTriggered = false;

        OnPointerDownEvent?.Invoke();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);

        _isPointerDown = false;

        OnPointerUpEvent?.Invoke();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        if ((!_enableHoverScale && !_enableHoverOffset) || !interactable) return;

        EnsureHoverInitialized();

        if (!_isHovering)
        {
            UpdateBasePosition();
        }

        _isHovering = true;
        PlayHoverAnimation(true);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        if (!_enableHoverScale && !_enableHoverOffset) return;

        _isHovering = false;
        PlayHoverAnimation(false);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {

        if (_longPressTriggered)
        {
            _longPressTriggered = false;
            return;
        }

        PlayClickSound();

        if (_enableDoubleClick)
        {
            float currentTime = Time.unscaledTime;

            if (currentTime - _lastClickTime <= _doubleClickInterval)
            {
                _clickCount++;
                if (_clickCount >= 2)
                {
                    OnDoubleClick?.Invoke();
                    _clickCount = 0;
                    _lastClickTime = 0f;
                    return;
                }
            }
            else
            {
                _clickCount = 1;
            }

            _lastClickTime = currentTime;
        }

        base.OnPointerClick(eventData);
    }

    #endregion

    #region Unity生命周期

    protected override void Awake()
    {
        base.Awake();
        _rectTransform = GetComponent<RectTransform>();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (_rectTransform != null && _hoverInitialized)
        {
            _rectTransform.localScale = _originalScale;
            _rectTransform.anchoredPosition = _originalAnchoredPosition;
        }
        _isHovering = false;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (_scaleCoroutine != null)
        {
            StopCoroutine(_scaleCoroutine);
            _scaleCoroutine = null;
        }

        if (_rectTransform != null && _hoverInitialized)
        {
            _rectTransform.localScale = _originalScale;
            _rectTransform.anchoredPosition = _originalAnchoredPosition;
        }
        _isHovering = false;
    }

    private void Update()
    {

        if (_enableLongPress && _isPointerDown && !_longPressTriggered)
        {
            if (Time.unscaledTime - _pointerDownTime >= _longPressTime)
            {
                _longPressTriggered = true;
                OnLongPress?.Invoke();
            }
        }
    }

    #endregion

    #region 悬停效果

    private void EnsureHoverInitialized()
    {
        if (_hoverInitialized) return;

        if (_rectTransform == null)
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        if (_rectTransform != null)
        {
            _originalScale = _rectTransform.localScale;
            _originalAnchoredPosition = _rectTransform.anchoredPosition;
            _hoverInitialized = true;
        }
    }

    private void UpdateBasePosition()
    {
        if (_rectTransform != null && _scaleCoroutine == null)
        {
            _originalAnchoredPosition = _rectTransform.anchoredPosition;
        }
    }

    private void PlayHoverAnimation(bool isHovering)
    {
        if (_rectTransform == null) return;

        if (_scaleCoroutine != null)
        {
            StopCoroutine(_scaleCoroutine);
        }

        _scaleCoroutine = StartCoroutine(HoverAnimationCoroutine(isHovering));
    }

    private IEnumerator HoverAnimationCoroutine(bool isHovering)
    {
        Vector3 startScale = _rectTransform.localScale;
        Vector2 startPosition = _rectTransform.anchoredPosition;

        float targetScaleMultiplier = (_enableHoverScale && isHovering) ? HOVER_SCALE : 1f;
        Vector3 targetScale = _originalScale * targetScaleMultiplier;

        Vector2 targetPosition = CalculateTargetPosition(targetScaleMultiplier, isHovering);

        float elapsed = 0f;

        while (elapsed < HOVER_ANIM_DURATION)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / HOVER_ANIM_DURATION);

            t = EaseOutCubic(t);

            _rectTransform.localScale = Vector3.Lerp(startScale, targetScale, t);
            _rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);

            yield return null;
        }

        _rectTransform.localScale = targetScale;
        _rectTransform.anchoredPosition = targetPosition;
        _scaleCoroutine = null;
    }

    private Vector2 CalculateTargetPosition(float scaleFactor, bool isHovering)
    {
        Vector2 targetPosition = _originalAnchoredPosition;

        if (_enableHoverScale && scaleFactor != 1f)
        {
            Vector2 pivot = _rectTransform.pivot;
            Vector2 sizeDelta = _rectTransform.sizeDelta;
            Vector2 pivotToCenter = new Vector2(0.5f - pivot.x, 0.5f - pivot.y);
            Vector2 scaleOffset = pivotToCenter * sizeDelta * (1f - scaleFactor);
            targetPosition += scaleOffset;
        }

        if (_enableHoverOffset && isHovering)
        {
            float height = _rectTransform.rect.height;
            float offsetY = height * HOVER_OFFSET_RATIO;
            targetPosition += new Vector2(0, offsetY);
        }

        return targetPosition;
    }

    private float EaseOutCubic(float t)
    {
        return 1f - Mathf.Pow(1f - t, 3f);
    }

    #endregion

    #region 辅助方法

    private void PlayClickSound()
    {
        if (_clickSound != null)
        {
            AudioSource.PlayClipAtPoint(_clickSound, Camera.main.transform.position);
        }
    }

    public void SetInteractable(bool interactable)
    {
        this.interactable = interactable;
    }

    public void SimulateClick()
    {
        if (interactable)
        {
            onClick?.Invoke();
            PlayClickSound();
        }
    }

    public void AddClickListener(UnityEngine.Events.UnityAction action)
    {
        onClick.AddListener(action);
    }

    public void RemoveAllClickListeners()
    {
        onClick.RemoveAllListeners();
    }

    public void ResetHoverState()
    {
        if (_scaleCoroutine != null)
        {
            StopCoroutine(_scaleCoroutine);
            _scaleCoroutine = null;
        }

        _isHovering = false;

        if (_rectTransform != null && _hoverInitialized)
        {
            _rectTransform.localScale = _originalScale;
            _rectTransform.anchoredPosition = _originalAnchoredPosition;
        }
    }

    public Vector2 GetOriginalAnchoredPosition()
    {
        return _hoverInitialized ? _originalAnchoredPosition : _rectTransform.anchoredPosition;
    }

    #endregion

#if UNITY_EDITOR
    protected override void Reset()
    {
        base.Reset();

        transition = Transition.ColorTint;

        var image = GetComponent<Image>();
        if (image == null)
        {
            image = gameObject.AddComponent<Image>();
        }

        if (image != null)
        {
            targetGraphic = image;
        }
    }
#endif
}
