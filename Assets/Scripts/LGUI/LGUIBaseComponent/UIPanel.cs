using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("LGUI/UIPanel")]
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasRenderer))]
public class UIPanel : Graphic
{
    [Header("面板设置")]
    [Tooltip("面板深度，数值越大越靠前")]
    [SerializeField] private int _depth = 0;

    [Tooltip("是否接收射线检测")]
    [SerializeField] private bool _blockRaycast = false;

    public int Depth
    {
        get => _depth;
        set
        {
            if (_depth != value)
            {
                _depth = value;
                UpdateSortingOrder();
            }
        }
    }

    public bool BlockRaycast
    {
        get => _blockRaycast;
        set
        {
            _blockRaycast = value;
            raycastTarget = value;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        raycastTarget = _blockRaycast;
    }

    protected override void Start()
    {
        base.Start();
        UpdateSortingOrder();
    }

    public void UpdateSortingOrder()
    {

        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.sortingOrder = _depth;
        }

        Canvas[] childCanvases = GetComponentsInChildren<Canvas>(true);
        foreach (var childCanvas in childCanvases)
        {
            if (childCanvas.gameObject != gameObject)
            {
                childCanvas.sortingOrder = _depth + childCanvas.sortingOrder;
            }
        }
    }

    public void BringToFront()
    {
        transform.SetAsLastSibling();
    }

    public void SendToBack()
    {
        transform.SetAsFirstSibling();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {

        vh.Clear();
    }

#if UNITY_EDITOR
    protected override void Reset()
    {
        base.Reset();
        raycastTarget = false;
        color = new Color(1, 1, 1, 0);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        raycastTarget = _blockRaycast;
    }
#endif
}
