using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

/// <summary>
/// UIButton 自定义编辑器
/// 继承自 ButtonEditor 以保留原有 Button 功能，同时添加悬停缩放设置
/// </summary>
[CustomEditor(typeof(UIButton), true)]
[CanEditMultipleObjects]
public class UIButtonEditor : ButtonEditor
{
    private SerializedProperty _enableHoverScaleProp;
    private SerializedProperty _enableHoverOffsetProp;
    private SerializedProperty _enableLongPressProp;
    private SerializedProperty _longPressTimeProp;
    private SerializedProperty _enableDoubleClickProp;
    private SerializedProperty _doubleClickIntervalProp;
    private SerializedProperty _clickSoundProp;

    protected override void OnEnable()
    {
        base.OnEnable();
        
        _enableHoverScaleProp = serializedObject.FindProperty("_enableHoverScale");
        _enableHoverOffsetProp = serializedObject.FindProperty("_enableHoverOffset");
        _enableLongPressProp = serializedObject.FindProperty("_enableLongPress");
        _longPressTimeProp = serializedObject.FindProperty("_longPressTime");
        _enableDoubleClickProp = serializedObject.FindProperty("_enableDoubleClick");
        _doubleClickIntervalProp = serializedObject.FindProperty("_doubleClickInterval");
        _clickSoundProp = serializedObject.FindProperty("_clickSound");
    }

    public override void OnInspectorGUI()
    {
        // 先绘制原始 Button 的 Inspector
        base.OnInspectorGUI();

        serializedObject.Update();

        EditorGUILayout.Space();

        // 悬停效果设置
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("悬停效果设置", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_enableHoverScaleProp, new GUIContent("启用悬停缩放", "鼠标悬停时按钮缩放动画（缩放到0.95）"));
        EditorGUILayout.PropertyField(_enableHoverOffsetProp, new GUIContent("启用悬停偏移", "鼠标悬停时向上偏移节点高度的15%"));
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // 扩展设置
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("扩展设置", EditorStyles.boldLabel);
        
        // 长按设置
        EditorGUILayout.PropertyField(_enableLongPressProp, new GUIContent("启用长按功能"));
        if (_enableLongPressProp.boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_longPressTimeProp, new GUIContent("长按触发时间(秒)"));
            EditorGUI.indentLevel--;
        }

        // 双击设置
        EditorGUILayout.PropertyField(_enableDoubleClickProp, new GUIContent("启用双击功能"));
        if (_enableDoubleClickProp.boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_doubleClickIntervalProp, new GUIContent("双击间隔时间(秒)"));
            EditorGUI.indentLevel--;
        }

        // 音效
        EditorGUILayout.PropertyField(_clickSoundProp, new GUIContent("按钮音效"));
        
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
}

