using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("LGUI/UICanvas")]
[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasScaler))]
[RequireComponent(typeof(GraphicRaycaster))]
public class UICanvas : MonoBehaviour
{
    [Header("画布设置")]
    [Tooltip("参考分辨率宽度")]
    [SerializeField] private float _referenceWidth = 1920f;

    [Tooltip("参考分辨率高度")]
    [SerializeField] private float _referenceHeight = 1080f;

    [Tooltip("屏幕匹配模式（0=宽度，1=高度，0.5=匹配）")]
    [Range(0f, 1f)]
    [SerializeField] private float _matchWidthOrHeight = 0.5f;

    private Canvas _canvas;
    private CanvasScaler _canvasScaler;
    private GraphicRaycaster _graphicRaycaster;

    public Canvas Canvas
    {
        get
        {
            if (_canvas == null)
            {
                _canvas = GetComponent<Canvas>();
            }
            return _canvas;
        }
    }

    public Vector2 ReferenceResolution
    {
        get => new Vector2(_referenceWidth, _referenceHeight);
        set
        {
            _referenceWidth = value.x;
            _referenceHeight = value.y;
            UpdateCanvasScaler();
        }
    }

    public float MatchWidthOrHeight
    {
        get => _matchWidthOrHeight;
        set
        {
            _matchWidthOrHeight = Mathf.Clamp01(value);
            UpdateCanvasScaler();
        }
    }

    public CanvasScaler CanvasScaler
    {
        get
        {
            if (_canvasScaler == null)
            {
                _canvasScaler = GetComponent<CanvasScaler>();
            }
            return _canvasScaler;
        }
    }

    public GraphicRaycaster GraphicRaycaster
    {
        get
        {
            if (_graphicRaycaster == null)
            {
                _graphicRaycaster = GetComponent<GraphicRaycaster>();
            }
            return _graphicRaycaster;
        }
    }

    public int SortingOrder
    {
        get => Canvas != null ? Canvas.sortingOrder : 0;
        set
        {
            if (Canvas != null)
            {
                Canvas.sortingOrder = value;
            }
        }
    }

    public RenderMode RenderMode
    {
        get => Canvas != null ? Canvas.renderMode : RenderMode.ScreenSpaceOverlay;
        set
        {
            if (Canvas != null)
            {
                Canvas.renderMode = value;
            }
        }
    }

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _canvasScaler = GetComponent<CanvasScaler>();
        _graphicRaycaster = GetComponent<GraphicRaycaster>();
        UpdateCanvasScaler();
    }

    public void UpdateCanvasScaler()
    {
        if (_canvasScaler == null)
        {
            _canvasScaler = GetComponent<CanvasScaler>();
        }

        if (_canvasScaler != null)
        {
            _canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            _canvasScaler.referenceResolution = new Vector2(_referenceWidth, _referenceHeight);
            _canvasScaler.matchWidthOrHeight = _matchWidthOrHeight;
        }
    }

    public void SetScreenSpaceOverlay()
    {
        if (Canvas != null)
        {
            Canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }
    }

    public void SetScreenSpaceCamera(Camera camera)
    {
        if (Canvas != null)
        {
            Canvas.renderMode = RenderMode.ScreenSpaceCamera;
            Canvas.worldCamera = camera;
        }
    }

    public void SetWorldSpace(Camera camera)
    {
        if (Canvas != null)
        {
            Canvas.renderMode = RenderMode.WorldSpace;
            Canvas.worldCamera = camera;
        }
    }

    public void SetSortingLayer(string layerName)
    {
        if (Canvas != null)
        {
            Canvas.sortingLayerName = layerName;
        }
    }

    public void SetSortingOrder(int order)
    {
        if (Canvas != null)
        {
            Canvas.sortingOrder = order;
        }
    }

    public void SetRaycastEnabled(bool enabled)
    {
        if (_graphicRaycaster == null)
        {
            _graphicRaycaster = GetComponent<GraphicRaycaster>();
        }

        if (_graphicRaycaster != null)
        {
            _graphicRaycaster.enabled = enabled;
        }
    }

    public float GetScaleFactor()
    {
        return Canvas != null ? Canvas.scaleFactor : 1f;
    }

    public Vector2 ScreenToCanvasPosition(Vector2 screenPosition)
    {
        RectTransform canvasRect = GetComponent<RectTransform>();
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, screenPosition, Canvas?.worldCamera, out localPoint);
        return localPoint;
    }

#if UNITY_EDITOR
    private void Reset()
    {
        _canvas = GetComponent<Canvas>();
        if (_canvas != null)
        {
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }

        var scaler = GetComponent<CanvasScaler>();
        if (scaler != null)
        {
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(_referenceWidth, _referenceHeight);
            scaler.matchWidthOrHeight = _matchWidthOrHeight;
        }
    }

    private void OnValidate()
    {
        UpdateCanvasScaler();
    }
#endif
}
