using UnityEngine;

[ExecuteInEditMode]
public abstract class UIBase : MonoBehaviour
{
    [Header("绑定设置")]
    [Tooltip("绑定变量的名称，用于在代码中通过 bindData.xxx 访问")]
    [SerializeField] protected string _bindName = "";

    [Tooltip("绑定ID，用于按钮等需要ID标识的组件")]
    [SerializeField] protected int _bindId = 0;

    public string bindName
    {
        get => _bindName;
        set => _bindName = value;
    }

    public int bindId
    {
        get => _bindId;
        set => _bindId = value;
    }

    public bool HasValidBindName => !string.IsNullOrEmpty(_bindName);

    public bool HasValidBindId => _bindId > 0;

    public bool HasValidBinding => HasValidBindName || HasValidBindId;

    public abstract string ComponentTypeName { get; }

    public abstract string BindDataType { get; }

    protected bool _initialized = false;

    protected virtual void Awake()
    {
        Initialize();
    }

    protected virtual void OnEnable()
    {
    }

    protected virtual void OnDisable()
    {
    }

    protected virtual void Initialize()
    {
        if (_initialized) return;
        _initialized = true;
    }

    protected void EnsureInitialized()
    {
        if (!_initialized)
        {
            Initialize();
        }
    }

#if UNITY_EDITOR
    protected virtual void Reset()
    {

    }

    protected virtual void OnValidate()
    {

    }

    public virtual string GetValidationError()
    {

        return null;
    }
#endif
}
