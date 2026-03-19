using UnityEngine;
using UnityEditor;

/// <summary>
/// UILayout 自定义编辑器
/// 容器类组件，隐藏 BindName 和 BindId
/// </summary>
[CustomEditor(typeof(UILayout))]
public class UILayoutEditor : Editor
{
    private SerializedProperty _layoutTypeProp;
    private SerializedProperty _paddingProp;
    private SerializedProperty _spacingProp;
    private SerializedProperty _childAlignmentProp;
    private SerializedProperty _childControlWidthProp;
    private SerializedProperty _childControlHeightProp;
    private SerializedProperty _childScaleWidthProp;
    private SerializedProperty _childScaleHeightProp;
    private SerializedProperty _childForceExpandWidthProp;
    private SerializedProperty _childForceExpandHeightProp;
    private SerializedProperty _reverseArrangementProp;

    private void OnEnable()
    {
        _layoutTypeProp = serializedObject.FindProperty("m_LayoutType");
        _paddingProp = serializedObject.FindProperty("m_Padding");
        _spacingProp = serializedObject.FindProperty("m_Spacing");
        _childAlignmentProp = serializedObject.FindProperty("m_ChildAlignment");
        _childControlWidthProp = serializedObject.FindProperty("m_ChildControlWidth");
        _childControlHeightProp = serializedObject.FindProperty("m_ChildControlHeight");
        _childScaleWidthProp = serializedObject.FindProperty("m_ChildScaleWidth");
        _childScaleHeightProp = serializedObject.FindProperty("m_ChildScaleHeight");
        _childForceExpandWidthProp = serializedObject.FindProperty("m_ChildForceExpandWidth");
        _childForceExpandHeightProp = serializedObject.FindProperty("m_ChildForceExpandHeight");
        _reverseArrangementProp = serializedObject.FindProperty("m_ReverseArrangement");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Space();

        // 布局设置
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("布局设置", EditorStyles.boldLabel);
        
        EditorGUILayout.PropertyField(_layoutTypeProp, new GUIContent("布局类型"));
        EditorGUILayout.PropertyField(_paddingProp, new GUIContent("内边距"));
        EditorGUILayout.PropertyField(_spacingProp, new GUIContent("间距"));
        EditorGUILayout.PropertyField(_childAlignmentProp, new GUIContent("子元素对齐"));
        EditorGUILayout.PropertyField(_reverseArrangementProp, new GUIContent("反转排列"));
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // 控制子元素
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("控制子元素", EditorStyles.boldLabel);
        
        EditorGUILayout.PropertyField(_childControlWidthProp, new GUIContent("控制宽度"));
        EditorGUILayout.PropertyField(_childControlHeightProp, new GUIContent("控制高度"));
        EditorGUILayout.PropertyField(_childScaleWidthProp, new GUIContent("使用宽度缩放"));
        EditorGUILayout.PropertyField(_childScaleHeightProp, new GUIContent("使用高度缩放"));
        EditorGUILayout.PropertyField(_childForceExpandWidthProp, new GUIContent("强制扩展宽度"));
        EditorGUILayout.PropertyField(_childForceExpandHeightProp, new GUIContent("强制扩展高度"));
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // 状态信息
        UILayout layout = target as UILayout;
        if (layout != null)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("状态信息", EditorStyles.boldLabel);
            
            int childCount = layout.transform.childCount;
            EditorGUILayout.LabelField($"子对象数量: {childCount}");
            
            if (GUILayout.Button("立即刷新布局"))
            {
                layout.LayoutNow();
                EditorUtility.SetDirty(target);
            }
            EditorGUILayout.EndVertical();
        }

        serializedObject.ApplyModifiedProperties();
    }
}

