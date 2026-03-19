using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

/// <summary>
/// UIImage 自定义编辑器
/// 继承自 ImageEditor 保留原有功能，方便后续扩展
/// </summary>
[CustomEditor(typeof(UIImage), true)]
[CanEditMultipleObjects]
public class UIImageEditor : ImageEditor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space();

        // 组件说明
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("UIImage 组件", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("LGUI 图片组件，支持通过路径或 ImageConfig ID 加载图片", MessageType.Info);
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // 绘制原始 Image Inspector
        base.OnInspectorGUI();
    }
}

