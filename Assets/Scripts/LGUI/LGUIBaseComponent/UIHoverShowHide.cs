using UnityEngine;
using UnityEngine.EventSystems;

public class UIHoverShowHide : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("目标设置")]
    [Tooltip("要控制显隐的目标节点")]
    [SerializeField] private GameObject _targetObject;

    [Tooltip("如果未指定目标，使用 bindName 查找")]
    [SerializeField] private string _targetBindName;

    [Header("行为设置")]
    [Tooltip("初始状态是否显示")]
    [SerializeField] private bool _showOnStart = false;

    [Tooltip("悬停时是否显示（false则悬停时隐藏）")]
    [SerializeField] private bool _showOnHover = true;

    private UIBindBoolToActive _boolToActive;
    private bool _isHovering = false;

    private void Awake()
    {

        if (_targetObject == null && !string.IsNullOrEmpty(_targetBindName))
        {
            var template = GetComponent<UIBindTemplate>();
            if (template != null)
            {
                _boolToActive = template.GetBindComponent<UIBindBoolToActive>(_targetBindName);
                if (_boolToActive != null)
                {
                    _targetObject = _boolToActive.gameObject;
                }
            }
        }
    }

    private void Start()
    {

        SetShow(_showOnStart);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isHovering = true;
        SetShow(_showOnHover);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isHovering = false;
        SetShow(!_showOnHover);
    }

    private void SetShow(bool show)
    {
        if (_boolToActive != null)
        {
            _boolToActive.SetShow(show);
        }
        else if (_targetObject != null)
        {
            _targetObject.SetActive(show);
        }
    }

    public void SetTarget(GameObject target)
    {
        _targetObject = target;
    }

    public void SetTargetBindName(string bindName)
    {
        _targetBindName = bindName;
    }

    public bool IsHovering => _isHovering;
}
