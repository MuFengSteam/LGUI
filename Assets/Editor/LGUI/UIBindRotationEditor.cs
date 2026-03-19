using UnityEngine;
using UnityEditor;

/// <summary>
/// UIBindRotation 自定义编辑器
/// 隐藏 BindId，只显示 BindName 和旋转设置
/// </summary>
[CustomEditor(typeof(UIBindRotation))]
public class UIBindRotationEditor : Editor
{
    private SerializedProperty _bindNameProp;
    private SerializedProperty _rotationModeProp;
    private SerializedProperty _defaultRotationProp;
    private SerializedProperty _setDefaultOnStartProp;

    private void OnEnable()
    {
        _bindNameProp = serializedObject.FindProperty("_bindName");
        _rotationModeProp = serializedObject.FindProperty("rotationMode");
        _defaultRotationProp = serializedObject.FindProperty("defaultRotation");
        _setDefaultOnStartProp = serializedObject.FindProperty("setDefaultOnStart");
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
                "在生成的代码中使用的变量名称，用于控制旋转"));
            
            if (string.IsNullOrEmpty(_bindNameProp.stringValue))
            {
                EditorGUILayout.HelpBox("请输入绑定变量名称！", MessageType.Warning);
            }
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // 旋转设置
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("旋转设置", EditorStyles.boldLabel);
        
        EditorGUILayout.PropertyField(_rotationModeProp, new GUIContent("旋转模式"));
        EditorGUILayout.PropertyField(_defaultRotationProp, new GUIContent("默认旋转角度(Z轴)"));
        EditorGUILayout.PropertyField(_setDefaultOnStartProp, new GUIContent("Start时设置默认值"));
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // 使用说明
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("使用说明", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "设置Z轴旋转：bindData.rotateAngle = 45f\n" +
            "或使用方法：GetRotationByBindName(\"xxx\").SetRotationZ(45f)", MessageType.Info);
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
}

