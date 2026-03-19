using UnityEngine;
using UnityEditor;

/// <summary>
/// UIScrollView 自定义编辑器
/// 容器类组件，隐藏 BindName 和 BindId
/// </summary>
[CustomEditor(typeof(UIScrollView))]
public class UIScrollViewEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 绘制默认的 ScrollRect Inspector，但不包含 UIBase 的字段
        serializedObject.Update();

        EditorGUILayout.Space();

        // 组件说明
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("UIScrollView 组件", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "UIScrollView 是 LGUI 的滚动视图组件\n" +
            "继承自 ScrollRect，提供滚动功能", MessageType.Info);
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // 绘制 ScrollRect 相关属性
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("滚动设置", EditorStyles.boldLabel);
        
        SerializedProperty contentProp = serializedObject.FindProperty("m_Content");
        SerializedProperty horizontalProp = serializedObject.FindProperty("m_Horizontal");
        SerializedProperty verticalProp = serializedObject.FindProperty("m_Vertical");
        SerializedProperty movementTypeProp = serializedObject.FindProperty("m_MovementType");
        SerializedProperty elasticityProp = serializedObject.FindProperty("m_Elasticity");
        SerializedProperty inertiaProp = serializedObject.FindProperty("m_Inertia");
        SerializedProperty decelerationRateProp = serializedObject.FindProperty("m_DecelerationRate");
        SerializedProperty scrollSensitivityProp = serializedObject.FindProperty("m_ScrollSensitivity");
        SerializedProperty viewportProp = serializedObject.FindProperty("m_Viewport");
        
        if (contentProp != null) EditorGUILayout.PropertyField(contentProp, new GUIContent("Content"));
        if (viewportProp != null) EditorGUILayout.PropertyField(viewportProp, new GUIContent("Viewport"));
        if (horizontalProp != null) EditorGUILayout.PropertyField(horizontalProp, new GUIContent("水平滚动"));
        if (verticalProp != null) EditorGUILayout.PropertyField(verticalProp, new GUIContent("垂直滚动"));
        if (movementTypeProp != null) EditorGUILayout.PropertyField(movementTypeProp, new GUIContent("移动类型"));
        if (elasticityProp != null) EditorGUILayout.PropertyField(elasticityProp, new GUIContent("弹性"));
        if (inertiaProp != null) EditorGUILayout.PropertyField(inertiaProp, new GUIContent("惯性"));
        if (decelerationRateProp != null) EditorGUILayout.PropertyField(decelerationRateProp, new GUIContent("减速率"));
        if (scrollSensitivityProp != null) EditorGUILayout.PropertyField(scrollSensitivityProp, new GUIContent("滚动灵敏度"));
        
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
}

