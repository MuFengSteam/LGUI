using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UIBindPosition))]
public class UIBindPositionEditor : Editor
{
    private SerializedProperty _bindNameProp;
    private SerializedProperty _positionModeProp;

    private void OnEnable()
    {
        _bindNameProp = serializedObject.FindProperty("_bindName");
        _positionModeProp = serializedObject.FindProperty("positionMode");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(_bindNameProp, new GUIContent("绑定名称", "绑定变量的名称，用于在代码中通过 bindData.xxx 访问"));
        EditorGUILayout.PropertyField(_positionModeProp, new GUIContent("位置模式", "Anchored: 相对于锚点的位置\nLocal: 本地坐标"));

        // 显示当前位置信息
        UIBindPosition comp = (UIBindPosition)target;
        RectTransform rectTransform = comp.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("当前位置信息", EditorStyles.boldLabel);
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.Vector2Field("Anchored Position", rectTransform.anchoredPosition);
            EditorGUILayout.Vector3Field("Local Position", rectTransform.localPosition);
            EditorGUI.EndDisabledGroup();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
