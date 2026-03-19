using UnityEngine;
using System;

public class UIReferenceComponent : UIBase
{

    [HideInInspector]
    [SerializeField] private Component _targetComponent;

    [HideInInspector]
    [SerializeField] private string _componentTypeName = "";

    public override string ComponentTypeName => "UIReferenceComponent";
    public override string BindDataType => GetActualBindDataType();

    public bool IsTargetGameObject => _componentTypeName == "GameObject";

    private string GetActualBindDataType()
    {
        if (!string.IsNullOrEmpty(_componentTypeName))
        {
            return _componentTypeName;
        }
        if (_targetComponent != null)
        {
            return _targetComponent.GetType().Name;
        }
        return "Component";
    }

    public Component TargetComponent
    {
        get => _targetComponent;
        set
        {
            _targetComponent = value;
            if (value != null)
            {
                _componentTypeName = value.GetType().Name;
            }
            else
            {
                _componentTypeName = "";
            }
        }
    }

    public GameObject TargetGameObject => gameObject;

    public string componentTypeName => _componentTypeName;

    public void SetTargetAsGameObject()
    {
        _targetComponent = null;
        _componentTypeName = "GameObject";
    }

    public new T GetComponent<T>() where T : Component
    {

        if (_targetComponent is T typedComponent)
        {
            return typedComponent;
        }
        return gameObject.GetComponent<T>();
    }

    public Component GetConfiguredComponent()
    {
        if (_targetComponent != null)
        {
            return _targetComponent;
        }

        Debug.LogWarning($"UIReferenceComponent: [{gameObject.name}] 未设置目标组件");
        return null;
    }

    public new T GetComponentInChildren<T>(bool includeInactive = false) where T : Component
    {
        return gameObject.GetComponentInChildren<T>(includeInactive);
    }

    public new T[] GetComponentsInChildren<T>(bool includeInactive = false) where T : Component
    {
        return gameObject.GetComponentsInChildren<T>(includeInactive);
    }

    public T AddComponent<T>() where T : Component
    {
        return gameObject.AddComponent<T>();
    }

#if UNITY_EDITOR
    public override string GetValidationError()
    {
        if (!HasValidBindName)
        {
            return $"UIReferenceComponent组件 [{gameObject.name}] 的bindName未设置";
        }
        return null;
    }
#endif
}
