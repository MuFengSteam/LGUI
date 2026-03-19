using UnityEngine;
using UnityEditor;
using TMPro;

/// <summary>
/// UIBindText的自定义编辑器
/// 提供文本绑定的可视化配置界面
/// </summary>
[CustomEditor(typeof(UIBindText))]
public class UIBindTextEditor : Editor
{
    private SerializedProperty _bindNameProp;
    private SerializedProperty _formatStringProp;

    private readonly Color warningColor = new Color(1f, 0.9f, 0.6f);
    private readonly Color successColor = new Color(0.8f, 1f, 0.8f);
    private readonly Color errorColor = new Color(1f, 0.6f, 0.6f);
    
    private bool _propertiesInitialized = false;

    private void OnEnable()
    {
        RefreshSerializedProperties();
    }

    /// <summary>
    /// 刷新序列化属性引用
    /// </summary>
    private void RefreshSerializedProperties()
    {
        if (serializedObject == null || serializedObject.targetObject == null)
        {
            _propertiesInitialized = false;
            return;
        }

        try
        {
            // 使用正确的序列化字段名
            _bindNameProp = serializedObject.FindProperty("_bindName");
            _formatStringProp = serializedObject.FindProperty("formatString");
            _propertiesInitialized = true;
        }
        catch (System.Exception)
        {
            _propertiesInitialized = false;
        }
    }

    public override void OnInspectorGUI()
    {
        // 确保序列化对象有效
        if (serializedObject == null || serializedObject.targetObject == null)
        {
            EditorGUILayout.HelpBox("目标对象无效", MessageType.Warning);
            return;
        }

        // 如果属性未初始化，重新初始化
        if (!_propertiesInitialized)
        {
            RefreshSerializedProperties();
        }

        serializedObject.Update();

        UIBindText bindText = target as UIBindText;
        if (bindText == null)
        {
            EditorGUILayout.HelpBox("无法获取UIBindText组件", MessageType.Error);
            return;
        }

        EditorGUILayout.Space();

        // 绑定名称区域
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        {
            EditorGUILayout.LabelField("绑定设置", EditorStyles.boldLabel);

            // 绑定名称输入框
            if (_bindNameProp != null)
            {
                EditorGUI.BeginChangeCheck();
                string newBindName = EditorGUILayout.TextField(new GUIContent("绑定变量名称", 
                    "在生成的代码中使用的变量名称，用于访问此文本组件"), _bindNameProp.stringValue);
                if (EditorGUI.EndChangeCheck())
                {
                    _bindNameProp.stringValue = newBindName;
                }

                // 显示警告或提示
                if (string.IsNullOrEmpty(_bindNameProp.stringValue))
                {
                    EditorGUILayout.HelpBox("请输入绑定变量名称！", MessageType.Warning);
                }
                else if (_bindNameProp.stringValue.Length > 0 && !char.IsLetter(_bindNameProp.stringValue[0]))
                {
                    EditorGUILayout.HelpBox("变量名称必须以字母开头！", MessageType.Error);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("无法找到bindName属性", MessageType.Error);
            }
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // 格式化字符串区域
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        {
            EditorGUILayout.LabelField("文本格式设置", EditorStyles.boldLabel);

            if (_formatStringProp != null)
            {
                // 格式化字符串输入框
                EditorGUI.BeginChangeCheck();
                string newFormatString = EditorGUILayout.TextField(new GUIContent("格式化字符串",
                    "使用 {0} 作为占位符，例如：'得分：{0}分'"), _formatStringProp.stringValue);
                if (EditorGUI.EndChangeCheck())
                {
                    _formatStringProp.stringValue = newFormatString;
                }

                // 格式化字符串预览
                if (!string.IsNullOrEmpty(_formatStringProp.stringValue))
                {
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    EditorGUILayout.LabelField("预览效果：");
                    try
                    {
                        string preview = string.Format(_formatStringProp.stringValue, "示例");
                        EditorGUILayout.LabelField(preview, EditorStyles.boldLabel);
                        GUI.backgroundColor = successColor;
                        EditorGUILayout.HelpBox("格式化字符串有效", MessageType.Info);
                    }
                    catch (System.Exception)
                    {
                        GUI.backgroundColor = warningColor;
                        EditorGUILayout.HelpBox("格式化字符串无效！请检查格式是否正确。", MessageType.Error);
                    }
                    GUI.backgroundColor = Color.white;
                    EditorGUILayout.EndVertical();
                }
            }
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // 组件状态区域
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        {
            EditorGUILayout.LabelField("组件状态", EditorStyles.boldLabel);

            // 检查文本组件类型
            TMP_Text tmpText = bindText.GetComponent<TMP_Text>();
            UnityEngine.UI.Text uguiText = bindText.GetComponent<UnityEngine.UI.Text>();

            // 显示组件状态
            if (tmpText != null)
            {
                GUI.backgroundColor = successColor;
                string tmpType = tmpText is TextMeshProUGUI ? "TextMeshPro - Text (UI)" : "TextMeshPro - Text";
                EditorGUILayout.HelpBox($"✓ 已检测到 {tmpType} 组件", MessageType.Info);
                GUI.backgroundColor = Color.white;
            }
            else if (uguiText != null)
            {
                GUI.backgroundColor = errorColor;
                EditorGUILayout.HelpBox("✗ 检测到 UGUI Text 组件！\n\n项目统一使用TextMeshPro，请删除UGUI Text并添加TextMeshProUGUI组件。", MessageType.Error);
                GUI.backgroundColor = Color.white;
                
                // 提供快速修复按钮
                if (GUILayout.Button("替换为 TextMeshProUGUI"))
                {
                    ReplaceWithTMP(bindText.gameObject, uguiText);
                }
            }
            else
            {
                GUI.backgroundColor = warningColor;
                EditorGUILayout.HelpBox("未检测到文本组件！\n将自动添加 TextMeshProUGUI 组件", MessageType.Warning);
                GUI.backgroundColor = Color.white;
            }
        }
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }

    /// <summary>
    /// 将UGUI Text替换为TextMeshProUGUI
    /// </summary>
    private void ReplaceWithTMP(GameObject go, UnityEngine.UI.Text uguiText)
    {
        // 保存原始属性
        string text = uguiText.text;
        Color color = uguiText.color;
        int fontSize = uguiText.fontSize;
        TextAnchor alignment = uguiText.alignment;
        
        // 删除UGUI Text
        Undo.DestroyObjectImmediate(uguiText);
        
        // 添加TMP
        TextMeshProUGUI tmpText = Undo.AddComponent<TextMeshProUGUI>(go);
        
        // 恢复属性
        tmpText.text = text;
        tmpText.color = color;
        tmpText.fontSize = fontSize;
        
        // 转换对齐方式
        switch (alignment)
        {
            case TextAnchor.UpperLeft:
                tmpText.alignment = TextAlignmentOptions.TopLeft;
                break;
            case TextAnchor.UpperCenter:
                tmpText.alignment = TextAlignmentOptions.Top;
                break;
            case TextAnchor.UpperRight:
                tmpText.alignment = TextAlignmentOptions.TopRight;
                break;
            case TextAnchor.MiddleLeft:
                tmpText.alignment = TextAlignmentOptions.MidlineLeft;
                break;
            case TextAnchor.MiddleCenter:
                tmpText.alignment = TextAlignmentOptions.Center;
                break;
            case TextAnchor.MiddleRight:
                tmpText.alignment = TextAlignmentOptions.MidlineRight;
                break;
            case TextAnchor.LowerLeft:
                tmpText.alignment = TextAlignmentOptions.BottomLeft;
                break;
            case TextAnchor.LowerCenter:
                tmpText.alignment = TextAlignmentOptions.Bottom;
                break;
            case TextAnchor.LowerRight:
                tmpText.alignment = TextAlignmentOptions.BottomRight;
                break;
        }
        
        EditorUtility.SetDirty(go);
        Debug.Log($"[UIBindText] 已将节点 '{go.name}' 的UGUI Text替换为TextMeshProUGUI");
    }
}
