using UnityEngine;
using UnityEditor;

/// <summary>
/// UIPanel 自定义编辑器
/// 容器类组件，隐藏 BindName 和 BindId
/// </summary>
[CustomEditor(typeof(UIPanel))]
public class UIPanelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Space();

        // 组件说明
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("UIPanel 组件", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "UIPanel 是 LGUI 的面板容器组件\n" +
            "• 用于组织和管理子 UI 元素\n" +
            "• 可作为 UI 面板的根节点", MessageType.Info);
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // 状态信息
        UIPanel panel = target as UIPanel;
        if (panel != null)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("状态信息", EditorStyles.boldLabel);
            
            int childCount = panel.transform.childCount;
            EditorGUILayout.LabelField($"子对象数量: {childCount}");
            
            RectTransform rect = panel.GetComponent<RectTransform>();
            if (rect != null)
            {
                EditorGUILayout.LabelField($"尺寸: {rect.rect.width} x {rect.rect.height}");
            }
            EditorGUILayout.EndVertical();
        }

        serializedObject.ApplyModifiedProperties();
    }
}

