using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class UIDropTarget : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler
{
    [Header("高亮设置")]
    [Tooltip("是否在拖拽悬停时高亮")]
    [SerializeField] private bool _enableHighlight = true;

    [Tooltip("高亮颜色")]
    [SerializeField] private Color _highlightColor = new Color(0.5f, 1f, 0.5f, 0.3f);

    [Tooltip("正常颜色")]
    [SerializeField] private Color _normalColor = new Color(1f, 1f, 1f, 0f);

    [Header("数据")]
    [Tooltip("目标携带的数据ID（如任务TaskId）")]
    [SerializeField] private int _targetId;

    public event Action<UIDraggableCard> OnCardDropped;

    public event Action<UIDraggableCard> OnCardEnter;

    public event Action<UIDraggableCard> OnCardExit;

    private Image _highlightImage;
    private bool _isHighlighted = false;

    public int TargetId
    {
        get => _targetId;
        set => _targetId = value;
    }

    private void Awake()
    {

        _highlightImage = GetComponent<Image>();
        if (_highlightImage != null && _enableHighlight)
        {
            _highlightImage.color = _normalColor;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

        var draggingCard = eventData.pointerDrag?.GetComponent<UIDraggableCard>();
        if (draggingCard != null && draggingCard.IsDragging)
        {
            SetHighlight(true);
            OnCardEnter?.Invoke(draggingCard);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetHighlight(false);

        var draggingCard = eventData.pointerDrag?.GetComponent<UIDraggableCard>();
        if (draggingCard != null)
        {
            OnCardExit?.Invoke(draggingCard);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        SetHighlight(false);

        if (eventData.pointerDrag == null) return;

        var droppedCard = eventData.pointerDrag.GetComponent<UIDraggableCard>();
        if (droppedCard != null && droppedCard.gameObject != null)
        {
            Debug.Log($"[UIDropTarget] 卡片 {droppedCard.DataId} 放入目标 {_targetId}");
            OnCardDropped?.Invoke(droppedCard);
        }
    }

    private void SetHighlight(bool highlight)
    {
        if (!_enableHighlight || _highlightImage == null) return;

        _isHighlighted = highlight;
        _highlightImage.color = highlight ? _highlightColor : _normalColor;
    }

    public void SetTargetId(int targetId)
    {
        _targetId = targetId;
    }
}
