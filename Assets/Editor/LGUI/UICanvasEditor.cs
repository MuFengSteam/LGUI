// 作者: 木枫LL
// GitHub: https://github.com/MuFengSteam/

using UnityEngine;
using UnityEditor;

/// <summary>
/// UICanvas 自定义编辑器
/// 容器类组件，隐藏 BindName 和 BindId
/// </summary>
[CustomEditor(typeof(UICanvas))]
public class UICanvasEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Space();

        // 组件说明
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("UICanvas 组件", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "UICanvas 是 LGUI 的画布容器组件\n" +
            "• 自动添加 Canvas、CanvasScaler、GraphicRaycaster\n" +
            "• 用于包装 Unity 的 Canvas 组件", MessageType.Info);
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // Canvas 状态
        UICanvas uiCanvas = target as UICanvas;
        if (uiCanvas != null)
        {
            Canvas canvas = uiCanvas.GetComponent<Canvas>();
            if (canvas != null)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Canvas 状态", EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"渲染模式: {canvas.renderMode}");
                EditorGUILayout.LabelField($"排序层: {canvas.sortingLayerName}");
                EditorGUILayout.LabelField($"排序顺序: {canvas.sortingOrder}");
                EditorGUILayout.EndVertical();
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}

