using UnityEngine;
using UnityEditor;

/// <summary>
/// UIBindBoolToActive 自定义编辑器
/// 隐藏 BindId，只显示 BindName、显隐模式和反选设置
/// </summary>
[CustomEditor(typeof(UIBindBoolToActive))]
public class UIBindBoolToActiveEditor : Editor
{
    private SerializedProperty _bindNameProp;
    private SerializedProperty _hideModeProp;
    private SerializedProperty _invertProp;

    private readonly Color successColor = new Color(0.8f, 1f, 0.8f);

    private void OnEnable()
    {
        _bindNameProp = serializedObject.FindProperty("_bindName");
        _hideModeProp = serializedObject.FindProperty("hideMode");
        _invertProp = serializedObject.FindProperty("_invert");
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
                "在生成的代码中使用的变量名称，用于控制显隐"));
            
            if (string.IsNullOrEmpty(_bindNameProp.stringValue))
            {
                EditorGUILayout.HelpBox("请输入绑定变量名称！", MessageType.Warning);
            }
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // 显隐设置
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("显隐设置", EditorStyles.boldLabel);
        
        EditorGUILayout.PropertyField(_hideModeProp, new GUIContent("显隐模式"));
        EditorGUILayout.PropertyField(_invertProp, new GUIContent("反选", 
            "勾选后逻辑反转：true时隐藏，false时显示"));
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // 使用说明
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("使用说明", EditorStyles.boldLabel);
        
        bool isInvert = _invertProp != null && _invertProp.boolValue;
        string example = isInvert 
            ? "反选模式：bindData.变量名 = true 时隐藏，false 时显示" 
            : "正常模式：bindData.变量名 = true 时显示，false 时隐藏";
        
        GUI.backgroundColor = successColor;
        EditorGUILayout.HelpBox($"{example}\n例如：bindData.isShowNpcImg = true;", MessageType.Info);
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
}

