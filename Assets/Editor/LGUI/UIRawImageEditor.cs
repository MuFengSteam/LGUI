using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

/// <summary>
/// UIRawImage 自定义编辑器
/// 继承自 RawImageEditor 保留原有功能，方便后续扩展
/// </summary>
[CustomEditor(typeof(UIRawImage), true)]
[CanEditMultipleObjects]
public class UIRawImageEditor : RawImageEditor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space();

        // 组件说明
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("UIRawImage 组件", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("LGUI 原始图片组件，支持通过路径加载 Texture", MessageType.Info);
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // 绘制原始 RawImage Inspector
        base.OnInspectorGUI();
    }
}

