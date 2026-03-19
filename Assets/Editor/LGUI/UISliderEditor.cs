using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

/// <summary>
/// UISlider 自定义编辑器
/// 继承自 SliderEditor 保留原有功能，方便后续扩展
/// </summary>
[CustomEditor(typeof(UISlider), true)]
[CanEditMultipleObjects]
public class UISliderEditor : SliderEditor
{
    private SerializedProperty _stepProp;
    private SerializedProperty _slideSoundProp;
    private SerializedProperty _playSoundOnDragProp;
    
    protected override void OnEnable()
    {
        base.OnEnable();
        _stepProp = serializedObject.FindProperty("_step");
        _slideSoundProp = serializedObject.FindProperty("_slideSound");
        _playSoundOnDragProp = serializedObject.FindProperty("_playSoundOnDrag");
    }
    
    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space();

        // 组件说明
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("UISlider 组件", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("LGUI 滑块组件，继承自 Slider", MessageType.Info);
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // 绘制原始 Slider Inspector
        base.OnInspectorGUI();
        
        EditorGUILayout.Space();
        
        // 扩展设置
        serializedObject.Update();
        
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("扩展设置", EditorStyles.boldLabel);
        
        // 步长设置
        if (_stepProp != null)
        {
            EditorGUI.BeginChangeCheck();
            float stepValue = EditorGUILayout.FloatField(
                new GUIContent("步长", "0表示连续滑动，>0表示每次变化的最小单位"), 
                _stepProp.floatValue);
            if (EditorGUI.EndChangeCheck())
            {
                _stepProp.floatValue = Mathf.Max(0, stepValue);
            }
            
            if (_stepProp.floatValue > 0)
            {
                EditorGUILayout.HelpBox($"步进模式：每次变化 {_stepProp.floatValue} 单位", MessageType.Info);
            }
        }
        
        EditorGUILayout.Space(5);
        
        // 音效设置
        if (_slideSoundProp != null)
        {
            EditorGUILayout.PropertyField(_slideSoundProp, new GUIContent("滑动音效"));
        }
        
        if (_playSoundOnDragProp != null)
        {
            EditorGUILayout.PropertyField(_playSoundOnDragProp, new GUIContent("拖动时播放音效"));
        }
        
        EditorGUILayout.EndVertical();
        
        serializedObject.ApplyModifiedProperties();
    }
}

