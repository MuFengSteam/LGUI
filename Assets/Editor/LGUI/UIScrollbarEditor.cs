using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

/// <summary>
/// UIScrollbar 自定义编辑器
/// 容器类组件，隐藏 BindName 和 BindId
/// 继承自 ScrollbarEditor 保留原有功能
/// </summary>
[CustomEditor(typeof(UIScrollbar), true)]
[CanEditMultipleObjects]
public class UIScrollbarEditor : ScrollbarEditor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space();

        // 组件说明
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("UIScrollbar 组件", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("LGUI 滚动条组件，继承自 Scrollbar", MessageType.Info);
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // 绘制原始 Scrollbar Inspector
        base.OnInspectorGUI();
    }
}

