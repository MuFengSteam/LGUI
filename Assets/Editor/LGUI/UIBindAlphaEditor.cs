using UnityEngine;
using UnityEditor;

/// <summary>
/// UIBindAlpha 自定义编辑器
/// 隐藏 BindId，只显示 BindName 和透明度设置
/// </summary>
[CustomEditor(typeof(UIBindAlpha))]
public class UIBindAlphaEditor : Editor
{
    private SerializedProperty _bindNameProp;
    private SerializedProperty _alphaModeProp;
    private SerializedProperty _defaultAlphaProp;
    private SerializedProperty _setDefaultOnStartProp;

    private void OnEnable()
    {
        _bindNameProp = serializedObject.FindProperty("_bindName");
        _alphaModeProp = serializedObject.FindProperty("alphaMode");
        _defaultAlphaProp = serializedObject.FindProperty("defaultAlpha");
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
                "在生成的代码中使用的变量名称，用于控制透明度"));
            
            if (string.IsNullOrEmpty(_bindNameProp.stringValue))
            {
                EditorGUILayout.HelpBox("请输入绑定变量名称！", MessageType.Warning);
            }
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // 透明度设置
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("透明度设置", EditorStyles.boldLabel);
        
        EditorGUILayout.PropertyField(_alphaModeProp, new GUIContent("透明度模式"));
        EditorGUILayout.PropertyField(_defaultAlphaProp, new GUIContent("默认透明度"));
        EditorGUILayout.PropertyField(_setDefaultOnStartProp, new GUIContent("Start时设置默认值"));
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // 使用说明
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("使用说明", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("通过 bindData.变量名 = 0.5f 设置透明度\n例如：bindData.fadeAlpha = 0.5f;", MessageType.Info);
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
}

