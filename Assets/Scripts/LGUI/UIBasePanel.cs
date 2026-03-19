using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;

public class UIBasePanel : MonoBehaviour
{
    private LGUIRoot _root;
    private Dictionary<uint, MethodInfo> _buttonMethods = new Dictionary<uint, MethodInfo>();
    private Dictionary<uint, MethodInfo> _buttonIndexMethods = new Dictionary<uint, MethodInfo>();
    private Dictionary<uint, MethodInfo> _buttonPressUpMethods = new Dictionary<uint, MethodInfo>();
    private Dictionary<uint, MethodInfo> _buttonPressUpIndexMethods = new Dictionary<uint, MethodInfo>();
    private bool _initialized = false;

    private Dictionary<string, UIBindText> _textCache;
    private Dictionary<string, UIBindImage> _imageCache;
    private Dictionary<string, UIBindTexture> _textureCache;
    private Dictionary<string, UIBindBoolToActive> _boolToActiveCache;
    private Dictionary<string, List<UIBindBoolToActive>> _boolToActiveListCache;
    private Dictionary<string, UIBindToggle> _toggleCache;
    private Dictionary<string, UIBindSlider> _sliderCache;
    private Dictionary<string, UIBindList> _listCache;
    private Dictionary<string, UIBindButton> _buttonCache;
    private Dictionary<string, UIBindAlpha> _alphaCache;
    private Dictionary<string, UIBindRotation> _rotationCache;
    private Dictionary<string, UIReference> _referenceCache;
    private Dictionary<string, UIReferenceComponent> _referenceComponentCache;
    private Dictionary<string, UIBindEffect> _effectCache;
    private Dictionary<string, UIBindInputField> _inputFieldCache;
    private Dictionary<string, UIBindProgress> _progressCache;

    protected virtual void Awake()
    {
        Initialize();
        OnAwake();
    }

    private void OnEnable()
    {
        OnShow();
    }

    protected virtual void Initialize()
    {
        if (_initialized) return;

        _root = GetComponent<LGUIRoot>();
        if (_root == null)
        {
            Debug.LogWarning($"[UIBasePanel] 在 {gameObject.name} 上没有找到LGUIRoot组件！");
            return;
        }

        CacheAllComponents();

        CacheButtonMethods();

        BindAllButtons();

        _initialized = true;
    }

    private void CacheAllComponents()
    {
        _textCache = new Dictionary<string, UIBindText>();
        _imageCache = new Dictionary<string, UIBindImage>();
        _textureCache = new Dictionary<string, UIBindTexture>();
        _boolToActiveCache = new Dictionary<string, UIBindBoolToActive>();
        _boolToActiveListCache = new Dictionary<string, List<UIBindBoolToActive>>();
        _toggleCache = new Dictionary<string, UIBindToggle>();
        _sliderCache = new Dictionary<string, UIBindSlider>();
        _listCache = new Dictionary<string, UIBindList>();
        _buttonCache = new Dictionary<string, UIBindButton>();
        _alphaCache = new Dictionary<string, UIBindAlpha>();
        _rotationCache = new Dictionary<string, UIBindRotation>();
        _referenceCache = new Dictionary<string, UIReference>();
        _referenceComponentCache = new Dictionary<string, UIReferenceComponent>();
        _effectCache = new Dictionary<string, UIBindEffect>();
        _inputFieldCache = new Dictionary<string, UIBindInputField>();
        _progressCache = new Dictionary<string, UIBindProgress>();

        foreach (var comp in GetComponentsInChildren<UIBindText>(true))
        {
            if (!string.IsNullOrEmpty(comp.bindName))
                _textCache[comp.bindName] = comp;
        }

        foreach (var comp in GetComponentsInChildren<UIBindImage>(true))
        {
            if (!string.IsNullOrEmpty(comp.bindName))
                _imageCache[comp.bindName] = comp;
        }

        foreach (var comp in GetComponentsInChildren<UIBindTexture>(true))
        {
            if (!string.IsNullOrEmpty(comp.bindName))
                _textureCache[comp.bindName] = comp;
        }

        foreach (var comp in GetComponentsInChildren<UIBindBoolToActive>(true))
        {
            if (!string.IsNullOrEmpty(comp.bindName))
            {

                _boolToActiveCache[comp.bindName] = comp;

                if (!_boolToActiveListCache.TryGetValue(comp.bindName, out var list))
                {
                    list = new List<UIBindBoolToActive>();
                    _boolToActiveListCache[comp.bindName] = list;
                }
                list.Add(comp);
            }
        }

        foreach (var comp in GetComponentsInChildren<UIBindToggle>(true))
        {
            if (!string.IsNullOrEmpty(comp.bindName))
                _toggleCache[comp.bindName] = comp;
        }

        foreach (var comp in GetComponentsInChildren<UIBindSlider>(true))
        {
            if (!string.IsNullOrEmpty(comp.bindName))
                _sliderCache[comp.bindName] = comp;
        }

        foreach (var comp in GetComponentsInChildren<UIBindList>(true))
        {
            if (!string.IsNullOrEmpty(comp.bindName))
                _listCache[comp.bindName] = comp;
        }

        foreach (var comp in GetComponentsInChildren<UIBindButton>(true))
        {
            if (!string.IsNullOrEmpty(comp.bindName))
                _buttonCache[comp.bindName] = comp;
        }

        foreach (var comp in GetComponentsInChildren<UIBindAlpha>(true))
        {
            if (!string.IsNullOrEmpty(comp.bindName))
                _alphaCache[comp.bindName] = comp;
        }

        foreach (var comp in GetComponentsInChildren<UIBindRotation>(true))
        {
            if (!string.IsNullOrEmpty(comp.bindName))
                _rotationCache[comp.bindName] = comp;
        }

        foreach (var comp in GetComponentsInChildren<UIReference>(true))
        {
            if (!string.IsNullOrEmpty(comp.bindName))
                _referenceCache[comp.bindName] = comp;
        }

        foreach (var comp in GetComponentsInChildren<UIReferenceComponent>(true))
        {
            if (!string.IsNullOrEmpty(comp.bindName))
                _referenceComponentCache[comp.bindName] = comp;
        }

        foreach (var comp in GetComponentsInChildren<UIBindEffect>(true))
        {
            if (!string.IsNullOrEmpty(comp.bindName))
                _effectCache[comp.bindName] = comp;
        }

        foreach (var comp in GetComponentsInChildren<UIBindInputField>(true))
        {
            if (!string.IsNullOrEmpty(comp.bindName))
                _inputFieldCache[comp.bindName] = comp;
        }

        foreach (var comp in GetComponentsInChildren<UIBindProgress>(true))
        {
            if (!string.IsNullOrEmpty(comp.bindName))
                _progressCache[comp.bindName] = comp;
        }
    }

    protected virtual void OnAwake()
    {

    }

    protected virtual void OnShow()
    {

    }

    protected virtual void OnClose()
    {

    }

    private void CacheButtonMethods()
    {
        _buttonMethods.Clear();
        _buttonIndexMethods.Clear();
        _buttonPressUpMethods.Clear();
        _buttonPressUpIndexMethods.Clear();

        MethodInfo[] methods = GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        Type buttonIDType = null;
        var nestedTypes = GetType().GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic);
        foreach (var nestedType in nestedTypes)
        {
            if (nestedType.IsEnum && nestedType.Name == "ButtonID")
            {
                buttonIDType = nestedType;
                break;
            }
        }

        if (buttonIDType != null)
        {
            var enumValues = System.Enum.GetValues(buttonIDType);
            var enumNames = System.Enum.GetNames(buttonIDType);

            Debug.Log($"[UIBasePanel] CacheButtonMethods: 找到 ButtonID 枚举，共 {enumNames.Length} 个值");

            for (int i = 0; i < enumNames.Length; i++)
            {
                string enumName = enumNames[i];
                uint buttonId = (uint)System.Convert.ToInt32(enumValues.GetValue(i));

                Debug.Log($"[UIBasePanel] 查找方法: {enumName} (ID={buttonId})");

                foreach (MethodInfo method in methods)
                {

                    if (method.Name == enumName)
                    {
                        var parameters = method.GetParameters();
                        if (parameters.Length == 0)
                        {
                            _buttonMethods[buttonId] = method;
                            Debug.Log($"[UIBasePanel] 找到方法: {enumName} -> {method.Name}");
                        }
                        else if (parameters.Length == 1 && parameters[0].ParameterType == typeof(int))
                        {
                            _buttonIndexMethods[buttonId] = method;
                        }
                    }

                    else if (method.Name == enumName + "Up")
                {
                    var parameters = method.GetParameters();
                    if (parameters.Length == 0)
                    {
                            _buttonPressUpMethods[buttonId] = method;
                    }
                    else if (parameters.Length == 1 && parameters[0].ParameterType == typeof(int))
                    {
                            _buttonPressUpIndexMethods[buttonId] = method;
                        }
                    }
                }
            }
        }
    }

    private void BindAllButtons()
    {
        UIBindButton[] buttons = GetComponentsInChildren<UIBindButton>(true);

        Debug.Log($"[UIBasePanel] BindAllButtons: 找到 {buttons.Length} 个按钮, 缓存了 {_buttonMethods.Count} 个方法");

        foreach (UIBindButton button in buttons)
        {
                Debug.Log($"[UIBasePanel] 绑定按钮: {button.name}, buttonId={button.buttonId}, 方法存在={_buttonMethods.ContainsKey(button.buttonId)}");
                button.RemoveAllListeners();

            if (button.IsLongPressEnabled())
            {

                if (button.index >= 0)
                {

                    if (_buttonIndexMethods.TryGetValue(button.buttonId, out MethodInfo pressDownMethod))
                    {
                        button.AddPressDownListener((idx) => pressDownMethod.Invoke(this, new object[] { idx }));
                    }
                    else if (_buttonMethods.TryGetValue(button.buttonId, out MethodInfo pressDownMethodNoIndex))
                    {
                        button.AddPressDownListener(() => pressDownMethodNoIndex.Invoke(this, null));
            }

                    uint upEventId = button.GetLongPressUpEventId();
                    if (_buttonPressUpIndexMethods.TryGetValue(button.buttonId, out MethodInfo pressUpMethod))
            {
                        button.AddPressUpListener((idx) => pressUpMethod.Invoke(this, new object[] { idx }));
                    }
                    else if (_buttonPressUpMethods.TryGetValue(button.buttonId, out MethodInfo pressUpMethodNoIndex))
                    {
                        button.AddPressUpListener(() => pressUpMethodNoIndex.Invoke(this, null));
                    }
            }
            else
            {

                    if (_buttonMethods.TryGetValue(button.buttonId, out MethodInfo pressDownMethod))
        {
                        button.AddPressDownListener(() => pressDownMethod.Invoke(this, null));
        }

                    if (_buttonPressUpMethods.TryGetValue(button.buttonId, out MethodInfo pressUpMethod))
        {
                        button.AddPressUpListener(() => pressUpMethod.Invoke(this, null));
                    }
                }
            }
            else
            {

                if (button.index >= 0 && _buttonIndexMethods.TryGetValue(button.buttonId, out MethodInfo indexMethod))
                {
                    button.AddClickListener((idx) => indexMethod.Invoke(this, new object[] { idx }));
                }
                else if (_buttonMethods.TryGetValue(button.buttonId, out MethodInfo method))
        {
                    button.AddClickListener(() => method.Invoke(this, null));
            }
        }
        }
    }

    #region 组件获取方法

    public UIBindText GetTextByBindName(string bindName)
    {
        if (_textCache != null && _textCache.TryGetValue(bindName, out var comp))
            return comp;
        return null;
    }

    public UIBindImage GetImageByBindName(string bindName)
    {
        if (_imageCache != null && _imageCache.TryGetValue(bindName, out var comp))
            return comp;
        return null;
    }

    public UIBindTexture GetTextureByBindName(string bindName)
        {
        if (_textureCache != null && _textureCache.TryGetValue(bindName, out var comp))
            return comp;
        return null;
    }

    public UIBindBoolToActive GetBoolToActiveByBindName(string bindName)
    {
        if (_boolToActiveCache != null && _boolToActiveCache.TryGetValue(bindName, out var comp))
            return comp;
        return null;
    }

    public List<UIBindBoolToActive> GetAllBoolToActiveByBindName(string bindName)
    {
        if (_boolToActiveListCache != null && _boolToActiveListCache.TryGetValue(bindName, out var list))
            return list;
        return null;
    }

    public void SetBoolToActiveShow(string bindName, bool show)
    {
        if (_boolToActiveListCache != null && _boolToActiveListCache.TryGetValue(bindName, out var list))
        {
            foreach (var comp in list)
            {
                comp?.SetShow(show);
            }
        }
    }

    public UIBindToggle GetToggleByBindName(string bindName)
    {
        if (_toggleCache != null && _toggleCache.TryGetValue(bindName, out var comp))
            return comp;
        return null;
    }

    public UIBindSlider GetSliderByBindName(string bindName)
        {
        if (_sliderCache != null && _sliderCache.TryGetValue(bindName, out var comp))
            return comp;
        return null;
    }

    public UIBindList GetListByBindName(string bindName)
    {
        if (_listCache != null && _listCache.TryGetValue(bindName, out var comp))
            return comp;
        return null;
    }

    public UIBindButton GetButtonByBindName(string bindName)
    {
        if (_buttonCache != null && _buttonCache.TryGetValue(bindName, out var comp))
            return comp;
        return null;
    }

    public UIBindAlpha GetAlphaByBindName(string bindName)
    {
        if (_alphaCache != null && _alphaCache.TryGetValue(bindName, out var comp))
            return comp;
        return null;
    }

    public UIBindRotation GetRotationByBindName(string bindName)
    {
        if (_rotationCache != null && _rotationCache.TryGetValue(bindName, out var comp))
            return comp;
        return null;
    }

    public UIReference GetReferenceByBindName(string bindName)
    {
        if (_referenceCache != null && _referenceCache.TryGetValue(bindName, out var comp))
            return comp;
        return null;
    }

    public UIReferenceComponent GetReferenceComponentByBindName(string bindName)
    {
        if (_referenceComponentCache != null && _referenceComponentCache.TryGetValue(bindName, out var comp))
            return comp;
        return null;
    }

    public UIBindEffect GetEffectByBindName(string bindName)
    {
        if (_effectCache != null && _effectCache.TryGetValue(bindName, out var comp))
            return comp;
        return null;
    }

    public UIBindInputField GetInputFieldByBindName(string bindName)
    {
        if (_inputFieldCache != null && _inputFieldCache.TryGetValue(bindName, out var comp))
            return comp;
        return null;
    }

    public UIBindProgress GetProgressByBindName(string bindName)
    {
        if (_progressCache != null && _progressCache.TryGetValue(bindName, out var comp))
            return comp;
        return null;
    }

    protected UIBindButton GetButton(uint buttonId)
    {
        UIBindButton[] buttons = GetComponentsInChildren<UIBindButton>(true);

        foreach (UIBindButton button in buttons)
        {
            if (button.buttonId == buttonId)
            {
                return button;
            }
        }

        return null;
    }

    #endregion

    #region 公共方法

    protected void TriggerButtonClick(uint buttonId)
    {
        if (_buttonMethods.TryGetValue(buttonId, out MethodInfo method))
        {
            method.Invoke(this, null);
        }
        else if (_buttonIndexMethods.TryGetValue(buttonId, out MethodInfo indexMethod))
        {
            UIBindButton button = GetButton(buttonId);
            if (button != null)
            {
                indexMethod.Invoke(this, new object[] { button.index });
            }
            else
            {
                indexMethod.Invoke(this, new object[] { -1 });
            }
        }
    }

    protected void SetButtonInteractable(uint buttonId, bool interactable)
    {
        UIBindButton button = GetButton(buttonId);
        if (button != null)
        {
            button.SetInteractable(interactable);
        }
    }

    public virtual void Open()
    {
        gameObject.SetActive(true);
        OnOpen();
    }

    public virtual void Close()
    {
        OnClose();
        gameObject.SetActive(false);
    }

    protected virtual void OnOpen()
    {

    }

    #endregion

    #region PanelManager支持

    public virtual void OnLanguageChanged()
    {

    }

    #endregion
}
