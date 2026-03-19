using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

/// <summary>
/// UIToggle 自定义编辑器
/// 继承自 ToggleEditor 保留原有功能，方便后续扩展
/// </summary>
[CustomEditor(typeof(UIToggle), true)]
[CanEditMultipleObjects]
public class UIToggleEditor : ToggleEditor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space();

        // 组件说明
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("UIToggle 组件", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("LGUI 开关组件，继承自 Toggle", MessageType.Info);
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // 绘制原始 Toggle Inspector
        base.OnInspectorGUI();
    }
}

