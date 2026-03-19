using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class UIGrid : UIBase, ILayoutGroup
{

    public enum Corner
    {
        UpperLeft = 0,
        UpperRight = 1,
        LowerLeft = 2,
        LowerRight = 3
    }

    public enum Axis
    {
        Horizontal = 0,
        Vertical = 1
    }

    public enum Constraint
    {
        Flexible = 0,
        FixedColumnCount = 1,
        FixedRowCount = 2
    }

    [Header("布局设置")]
    [Tooltip("起始角落")]
    [SerializeField] private Corner m_StartCorner = Corner.UpperLeft;

    [Tooltip("排列方向")]
    [SerializeField] private Axis m_StartAxis = Axis.Horizontal;

    [Tooltip("单元格大小")]
    [SerializeField] private Vector2 m_CellSize = new Vector2(100, 100);

    [Tooltip("单元格间距")]
    [SerializeField] private Vector2 m_Spacing = Vector2.zero;

    [Tooltip("约束模式")]
    [SerializeField] private Constraint m_Constraint = Constraint.Flexible;

    [Tooltip("约束数量（列数或行数）")]
    [SerializeField] private int m_ConstraintCount = 2;

    [Header("对齐设置")]
    [Tooltip("子元素对齐方式")]
    [SerializeField] private TextAnchor m_ChildAlignment = TextAnchor.UpperLeft;

    [Tooltip("内边距")]
    [SerializeField] private RectOffset m_Padding = new RectOffset();

    private RectTransform _rectTransform;
    private List<RectTransform> _children = new List<RectTransform>();
    private bool _layoutDirty = false;
    private bool _isLayoutting = false;

    public override string ComponentTypeName => "UIGrid";
    public override string BindDataType => "UIGrid";

    #region 公共属性

    public Corner startCorner
    {
        get => m_StartCorner;
        set { m_StartCorner = value; SetLayoutDirty(); }
    }

    public Axis startAxis
    {
        get => m_StartAxis;
        set { m_StartAxis = value; SetLayoutDirty(); }
    }

    public Vector2 cellSize
    {
        get => m_CellSize;
        set { m_CellSize = value; SetLayoutDirty(); }
    }

    public Vector2 spacing
    {
        get => m_Spacing;
        set { m_Spacing = value; SetLayoutDirty(); }
    }

    public Constraint constraint
    {
        get => m_Constraint;
        set { m_Constraint = value; SetLayoutDirty(); }
    }

    public int constraintCount
    {
        get => m_ConstraintCount;
        set { m_ConstraintCount = Mathf.Max(1, value); SetLayoutDirty(); }
    }

    public TextAnchor childAlignment
    {
        get => m_ChildAlignment;
        set { m_ChildAlignment = value; SetLayoutDirty(); }
    }

    public RectOffset padding
    {
        get => m_Padding;
        set { m_Padding = value; SetLayoutDirty(); }
    }

    #endregion

    protected override void Awake()
    {
        base.Awake();
        _rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        SetLayoutDirty();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        SetLayoutDirty();
    }

    private void OnTransformChildrenChanged()
    {
        SetLayoutDirty();
    }

    private void LateUpdate()
    {
        if (_layoutDirty && !_isLayoutting)
        {
            LayoutGrid();
            _layoutDirty = false;
        }
    }

    #region ILayoutGroup 实现

    public void SetLayoutHorizontal()
    {
        SetLayoutDirty();
    }

    public void SetLayoutVertical()
    {
        SetLayoutDirty();
    }

    #endregion

    public void SetLayoutDirty()
    {
        _layoutDirty = true;
    }

    public void LayoutGrid()
    {
        if (_isLayoutting) return;
        if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
        if (_rectTransform == null) return;

        _isLayoutting = true;

        CollectChildren();
        CalculateLayout();

        _isLayoutting = false;
    }

    private void CollectChildren()
    {
        _children.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (!child.gameObject.activeSelf) continue;

            RectTransform childRect = child.GetComponent<RectTransform>();
            if (childRect != null)
            {
                _children.Add(childRect);
            }
        }
    }

    private void CalculateLayout()
    {
        if (_children.Count == 0) return;

        float width = _rectTransform.rect.width;
        float height = _rectTransform.rect.height;

        int cellCountX = 1;
        int cellCountY = 1;

        float availableWidth = width - m_Padding.horizontal;
        float availableHeight = height - m_Padding.vertical;

        if (m_Constraint == Constraint.FixedColumnCount)
        {
            cellCountX = m_ConstraintCount;
            cellCountY = Mathf.CeilToInt((float)_children.Count / cellCountX);
        }
        else if (m_Constraint == Constraint.FixedRowCount)
        {
            cellCountY = m_ConstraintCount;
            cellCountX = Mathf.CeilToInt((float)_children.Count / cellCountY);
        }
        else
        {

            if (m_CellSize.x + m_Spacing.x <= 0)
                cellCountX = int.MaxValue;
            else
                cellCountX = Mathf.Max(1, Mathf.FloorToInt((availableWidth + m_Spacing.x + 0.001f) / (m_CellSize.x + m_Spacing.x)));

            if (m_CellSize.y + m_Spacing.y <= 0)
                cellCountY = int.MaxValue;
            else
                cellCountY = Mathf.Max(1, Mathf.FloorToInt((availableHeight + m_Spacing.y + 0.001f) / (m_CellSize.y + m_Spacing.y)));
        }

        int actualColumns = cellCountX;
        int actualRows = cellCountY;

        if (m_StartAxis == Axis.Horizontal)
        {
            actualRows = Mathf.CeilToInt((float)_children.Count / actualColumns);
        }
        else
        {
            actualColumns = Mathf.CeilToInt((float)_children.Count / actualRows);
        }

        Vector2 startOffset = GetStartOffset(actualColumns, actualRows);

        for (int i = 0; i < _children.Count; i++)
        {
            int positionX, positionY;

            if (m_StartAxis == Axis.Horizontal)
            {
                positionX = i % actualColumns;
                positionY = i / actualColumns;
            }
            else
            {
                positionX = i / actualRows;
                positionY = i % actualRows;
            }

            if (m_StartCorner == Corner.UpperRight || m_StartCorner == Corner.LowerRight)
            {
                positionX = actualColumns - 1 - positionX;
            }
            if (m_StartCorner == Corner.LowerLeft || m_StartCorner == Corner.LowerRight)
            {
                positionY = actualRows - 1 - positionY;
            }

            SetChildPosition(_children[i], positionX, positionY, startOffset);
        }
    }

    private Vector2 GetStartOffset(int columns, int rows)
    {
        float width = _rectTransform.rect.width;
        float height = _rectTransform.rect.height;

        float requiredWidth = columns * m_CellSize.x + (columns - 1) * m_Spacing.x;
        float requiredHeight = rows * m_CellSize.y + (rows - 1) * m_Spacing.y;

        float offsetX = m_Padding.left;
        float offsetY = m_Padding.top;

        switch (m_ChildAlignment)
        {
            case TextAnchor.UpperCenter:
            case TextAnchor.MiddleCenter:
            case TextAnchor.LowerCenter:
                offsetX = (width - requiredWidth) * 0.5f;
                break;
            case TextAnchor.UpperRight:
            case TextAnchor.MiddleRight:
            case TextAnchor.LowerRight:
                offsetX = width - requiredWidth - m_Padding.right;
                break;
        }

        switch (m_ChildAlignment)
        {
            case TextAnchor.MiddleLeft:
            case TextAnchor.MiddleCenter:
            case TextAnchor.MiddleRight:
                offsetY = (height - requiredHeight) * 0.5f;
                break;
            case TextAnchor.LowerLeft:
            case TextAnchor.LowerCenter:
            case TextAnchor.LowerRight:
                offsetY = height - requiredHeight - m_Padding.bottom;
                break;
        }

        return new Vector2(offsetX, offsetY);
    }

    private void SetChildPosition(RectTransform child, int posX, int posY, Vector2 startOffset)
    {
        float xPos = startOffset.x + posX * (m_CellSize.x + m_Spacing.x);
        float yPos = startOffset.y + posY * (m_CellSize.y + m_Spacing.y);

        child.anchorMin = new Vector2(0, 1);
        child.anchorMax = new Vector2(0, 1);
        child.pivot = new Vector2(0, 1);

        child.anchoredPosition = new Vector2(xPos, -yPos);

        child.sizeDelta = m_CellSize;
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();

        m_ConstraintCount = Mathf.Max(1, m_ConstraintCount);

        m_Spacing.x = Mathf.Max(0, m_Spacing.x);
        m_Spacing.y = Mathf.Max(0, m_Spacing.y);

        m_CellSize.x = Mathf.Max(0, m_CellSize.x);
        m_CellSize.y = Mathf.Max(0, m_CellSize.y);

        SetLayoutDirty();

        UnityEditor.EditorApplication.delayCall += () =>
        {
            if (this != null && !Application.isPlaying)
            {
                LayoutGrid();
            }
        };
    }

    protected override void Reset()
    {
        base.Reset();
    }
#endif
}
