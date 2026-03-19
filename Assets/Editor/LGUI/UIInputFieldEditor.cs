using UnityEngine;
using UnityEditor;
using TMPro.EditorUtilities;

/// <summary>
/// UIInputField 自定义编辑器
/// 继承自 TMP_InputFieldEditor 保留原有功能，方便后续扩展
/// </summary>
[CustomEditor(typeof(UIInputField), true)]
[CanEditMultipleObjects]
public class UIInputFieldEditor : TMP_InputFieldEditor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space();

        // 组件说明
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("UIInputField 组件", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("LGUI 输入框组件，继承自 TMP_InputField", MessageType.Info);
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // 绘制原始 InputField Inspector
        base.OnInspectorGUI();
    }
}

