using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class UILayout : UIBase, ILayoutGroup
{

    public enum LayoutType
    {
        Vertical = 0,
        Horizontal = 1
    }

    [Header("布局设置")]
    [Tooltip("布局类型")]
    [SerializeField] private LayoutType m_LayoutType = LayoutType.Vertical;

    [Tooltip("内边距")]
    [SerializeField] private RectOffset m_Padding = new RectOffset();

    [Tooltip("子元素间距")]
    [SerializeField] private float m_Spacing = 0;

    [Tooltip("子元素对齐方式")]
    [SerializeField] private TextAnchor m_ChildAlignment = TextAnchor.UpperLeft;

    [Header("控制子元素")]
    [Tooltip("控制子元素宽度")]
    [SerializeField] private bool m_ChildControlWidth = false;

    [Tooltip("控制子元素高度")]
    [SerializeField] private bool m_ChildControlHeight = false;

    [Tooltip("使用子元素的宽度缩放")]
    [SerializeField] private bool m_ChildScaleWidth = false;

    [Tooltip("使用子元素的高度缩放")]
    [SerializeField] private bool m_ChildScaleHeight = false;

    [Tooltip("强制子元素扩展宽度")]
    [SerializeField] private bool m_ChildForceExpandWidth = false;

    [Tooltip("强制子元素扩展高度")]
    [SerializeField] private bool m_ChildForceExpandHeight = false;

    [Tooltip("是否反转排列")]
    [SerializeField] private bool m_ReverseArrangement = false;

    private RectTransform _rectTransform;
    private List<RectTransform> _children = new List<RectTransform>();
    private bool _layoutDirty = false;
    private bool _isLayoutting = false;

    public override string ComponentTypeName => "UILayout";
    public override string BindDataType => "UILayout";

    #region 公共属性

    public LayoutType layoutType
    {
        get => m_LayoutType;
        set { m_LayoutType = value; SetLayoutDirty(); }
    }

    public RectOffset padding
    {
        get => m_Padding;
        set { m_Padding = value; SetLayoutDirty(); }
    }

    public float spacing
    {
        get => m_Spacing;
        set { m_Spacing = value; SetLayoutDirty(); }
    }

    public TextAnchor childAlignment
    {
        get => m_ChildAlignment;
        set { m_ChildAlignment = value; SetLayoutDirty(); }
    }

    public bool childControlWidth
    {
        get => m_ChildControlWidth;
        set { m_ChildControlWidth = value; SetLayoutDirty(); }
    }

    public bool childControlHeight
    {
        get => m_ChildControlHeight;
        set { m_ChildControlHeight = value; SetLayoutDirty(); }
    }

    public bool childScaleWidth
    {
        get => m_ChildScaleWidth;
        set { m_ChildScaleWidth = value; SetLayoutDirty(); }
    }

    public bool childScaleHeight
    {
        get => m_ChildScaleHeight;
        set { m_ChildScaleHeight = value; SetLayoutDirty(); }
    }

    public bool childForceExpandWidth
    {
        get => m_ChildForceExpandWidth;
        set { m_ChildForceExpandWidth = value; SetLayoutDirty(); }
    }

    public bool childForceExpandHeight
    {
        get => m_ChildForceExpandHeight;
        set { m_ChildForceExpandHeight = value; SetLayoutDirty(); }
    }

    public bool reverseArrangement
    {
        get => m_ReverseArrangement;
        set { m_ReverseArrangement = value; SetLayoutDirty(); }
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
            CalculateLayout();
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

    public void LayoutNow()
    {
        CollectChildren();
        CalculateLayout();
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

        if (m_ReverseArrangement)
        {
            _children.Reverse();
        }
    }

    private void CalculateLayout()
    {
        if (_isLayoutting) return;
        if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
        if (_rectTransform == null) return;

        _isLayoutting = true;

        CollectChildren();

        if (_children.Count == 0)
        {
            _isLayoutting = false;
            return;
        }

        if (m_LayoutType == LayoutType.Vertical)
        {
            CalculateVerticalLayout();
        }
        else
        {
            CalculateHorizontalLayout();
        }

        _isLayoutting = false;
    }

    private void CalculateVerticalLayout()
    {
        float width = _rectTransform.rect.width;
        float height = _rectTransform.rect.height;

        float availableWidth = width - m_Padding.left - m_Padding.right;
        float availableHeight = height - m_Padding.top - m_Padding.bottom;

        float totalHeight = 0;
        float totalFlexible = 0;
        List<float> childHeights = new List<float>();

        foreach (var child in _children)
        {
            float childHeight = child.rect.height;
            if (m_ChildScaleHeight)
            {
                childHeight *= child.localScale.y;
            }
            childHeights.Add(childHeight);
            totalHeight += childHeight;

            if (m_ChildForceExpandHeight)
            {
                totalFlexible += 1;
            }
        }

        totalHeight += m_Spacing * (_children.Count - 1);

        float extraHeight = availableHeight - totalHeight;
        float extraPerChild = totalFlexible > 0 ? extraHeight / totalFlexible : 0;

        float startY = GetAlignmentOffset(availableHeight, totalHeight, true);

        float currentY = m_Padding.top + startY;

        for (int i = 0; i < _children.Count; i++)
        {
            var child = _children[i];
            float childHeight = childHeights[i];

            if (m_ChildForceExpandHeight && extraPerChild > 0)
            {
                childHeight += extraPerChild;
            }

            child.anchorMin = new Vector2(0, 1);
            child.anchorMax = new Vector2(0, 1);
            child.pivot = new Vector2(0, 1);

            float xPos = m_Padding.left + GetAlignmentOffset(availableWidth, child.rect.width, false);

            child.anchoredPosition = new Vector2(xPos, -currentY);

            if (m_ChildControlWidth)
            {
                float newWidth = m_ChildForceExpandWidth ? availableWidth : child.rect.width;
                child.sizeDelta = new Vector2(newWidth, m_ChildControlHeight ? childHeight : child.sizeDelta.y);
            }
            else if (m_ChildControlHeight)
            {
                child.sizeDelta = new Vector2(child.sizeDelta.x, childHeight);
            }

            currentY += childHeight + m_Spacing;
        }
    }

    private void CalculateHorizontalLayout()
    {
        float width = _rectTransform.rect.width;
        float height = _rectTransform.rect.height;

        float availableWidth = width - m_Padding.left - m_Padding.right;
        float availableHeight = height - m_Padding.top - m_Padding.bottom;

        float totalWidth = 0;
        float totalFlexible = 0;
        List<float> childWidths = new List<float>();

        foreach (var child in _children)
        {
            float childWidth = child.rect.width;
            if (m_ChildScaleWidth)
            {
                childWidth *= child.localScale.x;
            }
            childWidths.Add(childWidth);
            totalWidth += childWidth;

            if (m_ChildForceExpandWidth)
            {
                totalFlexible += 1;
            }
        }

        totalWidth += m_Spacing * (_children.Count - 1);

        float extraWidth = availableWidth - totalWidth;
        float extraPerChild = totalFlexible > 0 ? extraWidth / totalFlexible : 0;

        float startX = GetAlignmentOffset(availableWidth, totalWidth, false);

        float currentX = m_Padding.left + startX;

        for (int i = 0; i < _children.Count; i++)
        {
            var child = _children[i];
            float childWidth = childWidths[i];

            if (m_ChildForceExpandWidth && extraPerChild > 0)
            {
                childWidth += extraPerChild;
            }

            child.anchorMin = new Vector2(0, 1);
            child.anchorMax = new Vector2(0, 1);
            child.pivot = new Vector2(0, 1);

            float yPos = m_Padding.top + GetAlignmentOffset(availableHeight, child.rect.height, true);

            child.anchoredPosition = new Vector2(currentX, -yPos);

            if (m_ChildControlHeight)
            {
                float newHeight = m_ChildForceExpandHeight ? availableHeight : child.rect.height;
                child.sizeDelta = new Vector2(m_ChildControlWidth ? childWidth : child.sizeDelta.x, newHeight);
            }
            else if (m_ChildControlWidth)
            {
                child.sizeDelta = new Vector2(childWidth, child.sizeDelta.y);
            }

            currentX += childWidth + m_Spacing;
        }
    }

    private float GetAlignmentOffset(float availableSize, float contentSize, bool isVertical)
    {
        if (contentSize >= availableSize) return 0;

        float remainingSpace = availableSize - contentSize;

        if (isVertical)
        {
            switch (m_ChildAlignment)
            {
                case TextAnchor.MiddleLeft:
                case TextAnchor.MiddleCenter:
                case TextAnchor.MiddleRight:
                    return remainingSpace * 0.5f;
                case TextAnchor.LowerLeft:
                case TextAnchor.LowerCenter:
                case TextAnchor.LowerRight:
                    return remainingSpace;
            }
        }
        else
        {
            switch (m_ChildAlignment)
            {
                case TextAnchor.UpperCenter:
                case TextAnchor.MiddleCenter:
                case TextAnchor.LowerCenter:
                    return remainingSpace * 0.5f;
                case TextAnchor.UpperRight:
                case TextAnchor.MiddleRight:
                case TextAnchor.LowerRight:
                    return remainingSpace;
            }
        }

        return 0;
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        m_Spacing = Mathf.Max(0, m_Spacing);
        SetLayoutDirty();

        UnityEditor.EditorApplication.delayCall += () =>
        {
            if (this != null && !Application.isPlaying)
            {
                CalculateLayout();
            }
        };
    }
#endif
}
