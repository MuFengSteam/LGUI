// 作者: 木枫LL
// GitHub: https://github.com/MuFengSteam/
// 小红书: https://www.xiaohongshu.com/user/profile/67c5dc1a000000000a03e5d0

using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Canvas))]
public class LGUIRoot : MonoBehaviour
{
    [Header("代码生成设置")]
    [Tooltip("绑定数据类名，默认根据预制体名称自动生成")]
    [SerializeField] private string _bindDataClassName = "";

    [Tooltip("UI脚本名，默认根据预制体名称自动生成")]
    [SerializeField] private string _uiScriptName = "";

    [Header("层级设置")]
    [Tooltip("UI渲染深度，数值越大越靠前")]
    [SerializeField] private int _depth = 0;

    public string bindDataClassName
    {
        get
        {
            if (string.IsNullOrEmpty(_bindDataClassName))
            {
                AutoGenerateNames();
            }
            return _bindDataClassName;
        }
        set => _bindDataClassName = value;
    }

    public string uiScriptName
    {
        get
        {
            if (string.IsNullOrEmpty(_uiScriptName))
            {
                AutoGenerateNames();
            }
            return _uiScriptName;
        }
        set => _uiScriptName = value;
    }

    public int Depth
    {
        get => _depth;
        set => _depth = value;
    }

    private Canvas _canvas;

    private void Awake()
    {
        Initialize();
        EnsureUIScriptComponent();
    }

    private void Reset()
    {
        AutoGenerateNames();
        Initialize();
    }

    private void AutoGenerateNames()
    {
        string objectName = gameObject.name;

        if (objectName.EndsWith("(Clone)"))
        {
            objectName = objectName.Substring(0, objectName.Length - 7);
        }

        if (objectName.EndsWith("Panel"))
        {
            _uiScriptName = objectName;
            string baseName = objectName.Substring(0, objectName.Length - 5);
            _bindDataClassName = baseName + "PanelBindData";
        }
        else
        {
            _uiScriptName = objectName + "Panel";
            _bindDataClassName = objectName + "PanelBindData";
        }
    }

    private void Initialize()
    {
        _canvas = GetComponent<Canvas>();
        if (_canvas == null)
        {
            _canvas = gameObject.AddComponent<Canvas>();
        }

        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = _depth;

        if (GetComponent<UnityEngine.UI.CanvasScaler>() == null)
        {
            gameObject.AddComponent<UnityEngine.UI.CanvasScaler>();
        }

        if (GetComponent<UnityEngine.UI.GraphicRaycaster>() == null)
        {
            gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        }
    }

    private void EnsureUIScriptComponent()
    {
        if (Application.isPlaying && !string.IsNullOrEmpty(uiScriptName))
        {
            Component existingScript = GetComponent(uiScriptName);
            if (existingScript == null)
            {
                System.Type scriptType = FindScriptType(uiScriptName);
                if (scriptType != null)
                {
                    gameObject.AddComponent(scriptType);
                }
                else
                {
                    Debug.LogWarning($"[LGUI] 找不到UI脚本类型: {uiScriptName}，请确保脚本已生成并编译。");
                }
            }
        }
    }

    private System.Type FindScriptType(string className)
    {
        System.Type type = System.Type.GetType(className);
        if (type != null)
        {
            return type;
        }

        foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
        {
            type = assembly.GetType(className);
            if (type != null)
            {
                return type;
            }
        }

        return null;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        Initialize();
    }

    public List<UIBase> CollectBindComponents()
    {
        List<UIBase> bindComponents = new List<UIBase>();
        List<string> validationErrors = new List<string>();

        UIBase[] allComponents = GetComponentsInChildren<UIBase>(true);

        foreach (var comp in allComponents)
        {

            if (comp is UIBindTemplate template)
            {

                var parentList = comp.GetComponentInParent<UIBindList>();
                if (parentList != null && parentList.itemTemplate == template)
                {
                    continue;
                }
            }

            string error = comp.GetValidationError();
            if (error != null)
            {
                validationErrors.Add(error);
                Debug.LogWarning($"[LGUI] ⚠️ {error}");
                continue;
            }

            if (comp.HasValidBinding)
            {
                bindComponents.Add(comp);
            }
            else
            {

                Debug.LogWarning($"[LGUI] ⚠️ {comp.ComponentTypeName}组件 [{comp.gameObject.name}] 没有设置bindName或bindId，已跳过");
            }
        }

        if (validationErrors.Count > 0)
        {
            Debug.LogError($"[LGUI] 发现 {validationErrors.Count} 个配置问题，请修复后重新生成：");
            foreach (var err in validationErrors)
            {
                Debug.LogError($"  - {err}");
            }
        }

        return bindComponents;
    }

    private string GetDirectoryName(string className)
    {
        string nameWithoutPanel = className.Replace("Panel", "");
        var matches = Regex.Matches(nameWithoutPanel, @"[A-Z][a-z]*");

        if (matches.Count <= 1)
        {
            return matches.Count > 0 ? matches[0].Value : nameWithoutPanel;
        }
        else
        {
            return matches[0].Value;
        }
    }

    private string GenerateBindDataClassCode(List<UIBase> components)
    {
        if (components.Count == 0)
        {
            Debug.LogWarning("[LGUI] 没有找到有效的绑定组件！");
            return string.Empty;
        }

        StringBuilder sb = new StringBuilder();

        sb.AppendLine("// 作者: 木枫LL");
        sb.AppendLine("// ============================================");
        sb.AppendLine("// 此文件由LGUI自动生成，请勿手动修改");
        sb.AppendLine("// ============================================");
        sb.AppendLine("using UnityEngine;");
        sb.AppendLine("using System;");
        sb.AppendLine();

        sb.AppendLine($"[Serializable]");
        sb.AppendLine($"public class {bindDataClassName}");
        sb.AppendLine("{");
        sb.AppendLine($"    private {uiScriptName} _panel;");
        sb.AppendLine();

        sb.AppendLine($"    public void Initialize({uiScriptName} panel)");
        sb.AppendLine("    {");
        sb.AppendLine("        _panel = panel;");
        sb.AppendLine("    }");
        sb.AppendLine();

        HashSet<string> generatedNames = new HashSet<string>();

        foreach (var comp in components)
        {
            string bindName = comp.bindName;

            if (string.IsNullOrEmpty(bindName))
            {
                continue;
            }

            if (generatedNames.Contains(bindName))
            {
                Debug.LogWarning($"[LGUI] 重复的bindName: {bindName}，已跳过");
                continue;
            }
            generatedNames.Add(bindName);

            GenerateBindProperty(sb, comp, bindName);
        }

        sb.AppendLine("}");
        return sb.ToString();
    }

    private string GetButtonMethodName(UIBindButton button)
    {

        if (!string.IsNullOrEmpty(button.bindName))
        {
            return char.ToUpper(button.bindName[0]) + button.bindName.Substring(1);
        }

        string goName = button.gameObject.name;

        string cleanName = Regex.Replace(goName, @"[^a-zA-Z0-9_]", "");
        if (string.IsNullOrEmpty(cleanName))
        {

            return $"Button_{button.buttonId}";
        }

        cleanName = char.ToUpper(cleanName[0]) + cleanName.Substring(1);
        return $"Button_{cleanName}";
    }

    private void GenerateBindProperty(StringBuilder sb, UIBase comp, string bindName)
    {
        string fieldName = "_" + bindName.ToLower();

        switch (comp)
        {
            case UIBindText _:
                sb.AppendLine($"    // UIBindText: {comp.gameObject.name}");
                sb.AppendLine($"    private string {fieldName};");
                sb.AppendLine($"    public string {bindName}");
                sb.AppendLine("    {");
                sb.AppendLine($"        get => {fieldName};");
                sb.AppendLine("        set");
                sb.AppendLine("        {");
                sb.AppendLine($"            {fieldName} = value;");
                sb.AppendLine($"            _panel?.GetTextByBindName(\"{bindName}\")?.SetText(value);");
                sb.AppendLine("        }");
                sb.AppendLine("    }");
                sb.AppendLine();
                break;

            case UIBindImage _:
                sb.AppendLine($"    // UIBindImage: {comp.gameObject.name}");
                sb.AppendLine($"    private int {fieldName};");
                sb.AppendLine($"    /// <summary>");
                sb.AppendLine($"    /// 设置图片（通过ImageConfig Id）");
                sb.AppendLine($"    /// </summary>");
                sb.AppendLine($"    public int {bindName}");
                sb.AppendLine("    {");
                sb.AppendLine($"        get => {fieldName};");
                sb.AppendLine("        set");
                sb.AppendLine("        {");
                sb.AppendLine($"            {fieldName} = value;");
                sb.AppendLine($"            _panel?.GetImageByBindName(\"{bindName}\")?.SetImageById(value);");
                sb.AppendLine("        }");
                sb.AppendLine("    }");
                sb.AppendLine();
                break;

            case UIBindTexture _:
                sb.AppendLine($"    // UIBindTexture: {comp.gameObject.name}");
                sb.AppendLine($"    private string {fieldName};");
                sb.AppendLine($"    /// <summary>");
                sb.AppendLine($"    /// 设置Texture（通过Resources路径）");
                sb.AppendLine($"    /// </summary>");
                sb.AppendLine($"    public string {bindName}");
                sb.AppendLine("    {");
                sb.AppendLine($"        get => {fieldName};");
                sb.AppendLine("        set");
                sb.AppendLine("        {");
                sb.AppendLine($"            {fieldName} = value;");
                sb.AppendLine($"            _panel?.GetTextureByBindName(\"{bindName}\")?.SetTextureByPath(value);");
                sb.AppendLine("        }");
                sb.AppendLine("    }");
                sb.AppendLine();
                break;

            case UIBindBoolToActive _:
                sb.AppendLine($"    // UIBindBoolToActive: {comp.gameObject.name}");
                sb.AppendLine($"    private bool {fieldName};");
                sb.AppendLine($"    /// <summary>");
                sb.AppendLine($"    /// 设置显隐状态（支持同名多节点）");
                sb.AppendLine($"    /// </summary>");
                sb.AppendLine($"    public bool {bindName}");
                sb.AppendLine("    {");
                sb.AppendLine($"        get => {fieldName};");
                sb.AppendLine("        set");
                sb.AppendLine("        {");
                sb.AppendLine($"            {fieldName} = value;");
                sb.AppendLine($"            _panel?.SetBoolToActiveShow(\"{bindName}\", value);");
                sb.AppendLine("        }");
                sb.AppendLine("    }");
                sb.AppendLine();
                break;

            case UIBindToggle _:
                sb.AppendLine($"    // UIBindToggle: {comp.gameObject.name}");
                sb.AppendLine($"    private bool {fieldName};");
                sb.AppendLine($"    public bool {bindName}");
                sb.AppendLine("    {");
                sb.AppendLine($"        get => {fieldName};");
                sb.AppendLine("        set");
                sb.AppendLine("        {");
                sb.AppendLine($"            {fieldName} = value;");
                sb.AppendLine($"            _panel?.GetToggleByBindName(\"{bindName}\")?.SetValue(value);");
                sb.AppendLine("        }");
                sb.AppendLine("    }");
                sb.AppendLine();
                break;

            case UIBindSlider _:
                sb.AppendLine($"    // UIBindSlider: {comp.gameObject.name}");
                sb.AppendLine($"    private float {fieldName};");
                sb.AppendLine($"    public float {bindName}");
                sb.AppendLine("    {");
                sb.AppendLine($"        get => {fieldName};");
                sb.AppendLine("        set");
                sb.AppendLine("        {");
                sb.AppendLine($"            {fieldName} = value;");
                sb.AppendLine($"            _panel?.GetSliderByBindName(\"{bindName}\")?.SetValue(value);");
                sb.AppendLine("        }");
                sb.AppendLine("    }");
                sb.AppendLine();
                break;

            case UIBindAlpha _:
                sb.AppendLine($"    // UIBindAlpha: {comp.gameObject.name}");
                sb.AppendLine($"    private float {fieldName};");
                sb.AppendLine($"    public float {bindName}");
                sb.AppendLine("    {");
                sb.AppendLine($"        get => {fieldName};");
                sb.AppendLine("        set");
                sb.AppendLine("        {");
                sb.AppendLine($"            {fieldName} = value;");
                sb.AppendLine($"            _panel?.GetAlphaByBindName(\"{bindName}\")?.SetAlpha(value);");
                sb.AppendLine("        }");
                sb.AppendLine("    }");
                sb.AppendLine();
                break;

            case UIBindRotation _:
                sb.AppendLine($"    // UIBindRotation: {comp.gameObject.name}");
                sb.AppendLine($"    private float {fieldName};");
                sb.AppendLine($"    /// <summary>");
                sb.AppendLine($"    /// 设置Z轴旋转角度");
                sb.AppendLine($"    /// </summary>");
                sb.AppendLine($"    public float {bindName}");
                sb.AppendLine("    {");
                sb.AppendLine($"        get => {fieldName};");
                sb.AppendLine("        set");
                sb.AppendLine("        {");
                sb.AppendLine($"            {fieldName} = value;");
                sb.AppendLine($"            _panel?.GetRotationByBindName(\"{bindName}\")?.SetRotationZ(value);");
                sb.AppendLine("        }");
                sb.AppendLine("    }");
                sb.AppendLine();
                break;

            case UIBindList _:
                sb.AppendLine($"    // UIBindList: {comp.gameObject.name}");
                sb.AppendLine($"    public UIBindList {bindName} => _panel?.GetListByBindName(\"{bindName}\");");
                sb.AppendLine();
                break;

            case UIReference _:
                sb.AppendLine($"    // UIReference: {comp.gameObject.name}");
                sb.AppendLine($"    public GameObject {bindName} => _panel?.GetReferenceByBindName(\"{bindName}\")?.GetGameObject();");
                sb.AppendLine();
                break;

            case UIReferenceComponent refComp:
                string componentTypeName = refComp.componentTypeName;
                if (string.IsNullOrEmpty(componentTypeName))
                {
                    componentTypeName = "Component";
                }
                sb.AppendLine($"    // UIReferenceComponent: {comp.gameObject.name} -> {componentTypeName}");
                if (componentTypeName == "GameObject")
                {

                    sb.AppendLine($"    public GameObject {bindName} => _panel?.GetReferenceComponentByBindName(\"{bindName}\")?.TargetGameObject;");
                }
                else
                {

                    sb.AppendLine($"    public {componentTypeName} {bindName} => _panel?.GetReferenceComponentByBindName(\"{bindName}\")?.TargetComponent as {componentTypeName};");
                }
                sb.AppendLine();
                break;

            case UIBindEffect _:
                sb.AppendLine($"    // UIBindEffect: {comp.gameObject.name}");
                sb.AppendLine($"    public UIBindEffect {bindName} => _panel?.GetEffectByBindName(\"{bindName}\");");
                sb.AppendLine();
                break;

            case UIBindInputField _:
                sb.AppendLine($"    // UIBindInputField: {comp.gameObject.name}");
                sb.AppendLine($"    public UIBindInputField {bindName} => _panel?.GetInputFieldByBindName(\"{bindName}\");");
                sb.AppendLine();
                break;

            case UIBindProgress _:
                sb.AppendLine($"    // UIBindProgress: {comp.gameObject.name}");
                sb.AppendLine($"    private float {fieldName};");
                sb.AppendLine($"    /// <summary>");
                sb.AppendLine($"    /// 设置进度（0-1）");
                sb.AppendLine($"    /// </summary>");
                sb.AppendLine($"    public float {bindName}");
                sb.AppendLine("    {");
                sb.AppendLine($"        get => {fieldName};");
                sb.AppendLine("        set");
                sb.AppendLine("        {");
                sb.AppendLine($"            {fieldName} = value;");
                sb.AppendLine($"            _panel?.GetProgressByBindName(\"{bindName}\")?.SetProgress(value);");
                sb.AppendLine("        }");
                sb.AppendLine("    }");
                sb.AppendLine();
                break;

            case UIBindButton _:

                break;

            default:

                Debug.LogWarning($"[LGUI] ⚠️ 未处理的组件类型: {comp.GetType().Name} [{comp.gameObject.name}] bindName='{bindName}'");
                break;
        }
    }

    private string GenerateUIScriptCode(List<UIBase> components)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("// 作者: 木枫LL");
        sb.AppendLine("// GitHub: https://github.com/MuFengSteam/");
        sb.AppendLine("// 小红书: https://www.xiaohongshu.com/user/profile/67c5dc1a000000000a03e5d0");
        sb.AppendLine("// ============================================");
        sb.AppendLine("// 此文件由LGUI自动生成");
        sb.AppendLine("// 可以在此文件中添加业务逻辑");
        sb.AppendLine("// ============================================");
        sb.AppendLine("using UnityEngine;");
        sb.AppendLine();

        sb.AppendLine($"public partial class {uiScriptName} : UIBasePanel");
        sb.AppendLine("{");

        sb.AppendLine($"    private {bindDataClassName} _bindData;");
        sb.AppendLine();

        sb.AppendLine($"    public {bindDataClassName} bindData");
        sb.AppendLine("    {");
        sb.AppendLine("        get");
        sb.AppendLine("        {");
        sb.AppendLine("            if (_bindData == null)");
        sb.AppendLine("            {");
        sb.AppendLine($"                _bindData = new {bindDataClassName}();");
        sb.AppendLine("                _bindData.Initialize(this);");
        sb.AppendLine("            }");
        sb.AppendLine("            return _bindData;");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine();

        sb.AppendLine("    protected override void OnAwake()");
        sb.AppendLine("    {");
        sb.AppendLine("        base.OnAwake();");
        sb.AppendLine("        // 在这里初始化面板");
        sb.AppendLine("        ");
        sb.AppendLine("        // 事件监听示例（取消注释并修改EventId和回调函数）:");
        sb.AppendLine("        // EventManager.AddListenerMessage<int>(EventId.None, OnEventCallback);");
        sb.AppendLine("    }");
        sb.AppendLine();

        sb.AppendLine("    private void OnDestroy()");
        sb.AppendLine("    {");
        sb.AppendLine("        // 在这里清理资源");
        sb.AppendLine("        ");
        sb.AppendLine("        // 移除事件监听示例（与OnAwake中的监听对应）:");
        sb.AppendLine("        // EventManager.RemoveListenerMessage<int>(EventId.None, OnEventCallback);");
        sb.AppendLine("    }");
        var buttons = components.OfType<UIBindButton>().ToList();
        if (buttons.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("    #region 按钮事件");
            sb.AppendLine();
            sb.AppendLine("    public enum ButtonID");
            sb.AppendLine("    {");

            foreach (var button in buttons)
            {
                string enumName = GetButtonMethodName(button);
                sb.AppendLine($"        {enumName} = {button.buttonId},");
            }

            sb.AppendLine("    }");
            sb.AppendLine();

            foreach (var button in buttons)
            {
                string methodName = GetButtonMethodName(button);

                sb.AppendLine($"    /// <summary>");
                sb.AppendLine($"    /// {button.gameObject.name} 按钮点击事件");
                sb.AppendLine($"    /// ButtonID.{methodName} = {button.buttonId}");
                sb.AppendLine($"    /// </summary>");
                sb.AppendLine($"    public void {methodName}()");
                sb.AppendLine("    {");
                sb.AppendLine("        // 在这里添加按钮点击逻辑");
                sb.AppendLine("    }");
                sb.AppendLine();
            }

            sb.AppendLine("    #endregion");
        }

        sb.AppendLine("}");

        return sb.ToString();
    }

    public void GenerateCode()
    {
        if (string.IsNullOrEmpty(bindDataClassName) || string.IsNullOrEmpty(uiScriptName))
        {
            Debug.LogError("[LGUI] 请先设置 bindDataClassName 和 uiScriptName！");
            return;
        }

        var components = CollectBindComponents();
        if (components.Count == 0)
        {
            Debug.LogWarning("[LGUI] 没有找到有效的绑定组件，代码生成已取消");
            return;
        }

        string directoryName = GetDirectoryName(uiScriptName);
        string baseDirectory = "Assets/Scripts/Generated";
        string fullDirectory = Path.Combine(baseDirectory, directoryName);

        if (!Directory.Exists(baseDirectory))
        {
            Directory.CreateDirectory(baseDirectory);
        }
        if (!Directory.Exists(fullDirectory))
        {
            Directory.CreateDirectory(fullDirectory);
        }

        string bindDataCode = GenerateBindDataClassCode(components);
        if (!string.IsNullOrEmpty(bindDataCode))
        {
            string bindDataPath = Path.Combine(fullDirectory, $"{bindDataClassName}.cs");
            File.WriteAllText(bindDataPath, bindDataCode);
        }

        string uiScriptPath = Path.Combine(fullDirectory, $"{uiScriptName}.cs");
        if (!File.Exists(uiScriptPath))
        {
            string scriptCode = GenerateUIScriptCode(components);
            if (!string.IsNullOrEmpty(scriptCode))
            {
                File.WriteAllText(uiScriptPath, scriptCode);
            }
        }

        AssetDatabase.Refresh();
    }
#endif
}
