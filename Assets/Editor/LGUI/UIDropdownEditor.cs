using UnityEngine;
using UnityEditor;
using TMPro.EditorUtilities;

/// <summary>
/// UIDropdown 自定义编辑器
/// 继承自 TMP_DropdownEditor 保留原有功能，方便后续扩展
/// </summary>
[CustomEditor(typeof(UIDropdown), true)]
[CanEditMultipleObjects]
public class UIDropdownEditor : DropdownEditor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space();

        // 组件说明
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("UIDropdown 组件", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("LGUI 下拉框组件，继承自 TMP_Dropdown", MessageType.Info);
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // 绘制原始 Dropdown Inspector
        base.OnInspectorGUI();
    }
}

