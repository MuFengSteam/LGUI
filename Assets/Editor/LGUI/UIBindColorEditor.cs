using UnityEngine;
using UnityEditor;

/// <summary>
/// UIBindColor 自定义编辑器
/// 隐藏 BindId，只显示 BindName 和颜色设置
/// </summary>
[CustomEditor(typeof(UIBindColor))]
public class UIBindColorEditor : Editor
{
    private SerializedProperty _bindNameProp;
    private SerializedProperty _defaultColorProp;
    private SerializedProperty _setDefaultOnStartProp;
    private SerializedProperty _affectChildrenProp;
    private SerializedProperty _colorPresetsProp;

    private void OnEnable()
    {
        _bindNameProp = serializedObject.FindProperty("_bindName");
        _defaultColorProp = serializedObject.FindProperty("defaultColor");
        _setDefaultOnStartProp = serializedObject.FindProperty("setDefaultOnStart");
        _affectChildrenProp = serializedObject.FindProperty("affectChildren");
        _colorPresetsProp = serializedObject.FindProperty("colorPresets");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Space();

        // 绑定设置
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("绑定设置", EditorStyles.boldLabel);
        
        if (_bindNameProp != null)
        {
            EditorGUILayout.PropertyField(_bindNameProp, new GUIContent("绑定变量名称", 
                "在生成的代码中使用的变量名称，用于控制颜色"));
            
            if (string.IsNullOrEmpty(_bindNameProp.stringValue))
            {
                EditorGUILayout.HelpBox("请输入绑定变量名称！", MessageType.Warning);
            }
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // 颜色设置
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("颜色设置", EditorStyles.boldLabel);
        
        EditorGUILayout.PropertyField(_defaultColorProp, new GUIContent("默认颜色"));
        EditorGUILayout.PropertyField(_setDefaultOnStartProp, new GUIContent("Start时设置默认颜色"));
        EditorGUILayout.PropertyField(_affectChildrenProp, new GUIContent("影响子节点", 
            "勾选后会同时修改所有子节点的颜色"));
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // 颜色预设
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("颜色预设", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_colorPresetsProp, new GUIContent("预设列表", 
            "可通过键值快速设置颜色，如：bindData.myColor = \"error\""));
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // 使用说明
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("使用说明", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "设置颜色的方式：\n" +
            "• bindData.变量名 = Color.red;\n" +
            "• bindData.变量名 = \"#FF0000\";\n" +
            "• bindData.变量名 = \"预设键值\";", 
            MessageType.Info);
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
}

