using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

[CustomEditor(typeof(UIBindProgress))]
public class UIBindProgressEditor : Editor
{
    private SerializedProperty _bindNameProp;
    private SerializedProperty _minValueProp;
    private SerializedProperty _maxValueProp;
    private SerializedProperty _currentValueProp;
    private SerializedProperty _inverseProp;

    private void OnEnable()
    {
        _bindNameProp = serializedObject.FindProperty("_bindName");
        _minValueProp = serializedObject.FindProperty("_minValue");
        _maxValueProp = serializedObject.FindProperty("_maxValue");
        _currentValueProp = serializedObject.FindProperty("_currentValue");
        _inverseProp = serializedObject.FindProperty("_inverse");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // 绑定名称
        EditorGUILayout.PropertyField(_bindNameProp, new GUIContent("绑定名称"));

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("进度设置", EditorStyles.boldLabel);

        // 范围设置
        EditorGUILayout.PropertyField(_minValueProp, new GUIContent("最小值"));
        EditorGUILayout.PropertyField(_maxValueProp, new GUIContent("最大值"));

        // 当前值（使用滑动条）
        float min = _minValueProp.floatValue;
        float max = _maxValueProp.floatValue;
        _currentValueProp.floatValue = EditorGUILayout.Slider(
            new GUIContent("当前值"), 
            _currentValueProp.floatValue, 
            min, 
            max
        );

        // 反向
        EditorGUILayout.PropertyField(_inverseProp, new GUIContent("反向填充"));

        // 检查Image组件
        var progress = target as UIBindProgress;
        var image = progress?.GetComponent<Image>();
        
        EditorGUILayout.Space();
        
        if (image == null)
        {
            EditorGUILayout.HelpBox("需要Image组件", MessageType.Error);
        }
        else if (image.type != Image.Type.Filled)
        {
            EditorGUILayout.HelpBox("Image需要设置为Filled类型才能正确显示进度", MessageType.Warning);
            if (GUILayout.Button("自动设置为Filled"))
            {
                Undo.RecordObject(image, "Set Image Type to Filled");
                image.type = Image.Type.Filled;
                image.fillMethod = Image.FillMethod.Horizontal;
                image.fillOrigin = 0;
                EditorUtility.SetDirty(image);
            }
        }
        else
        {
            // 显示当前填充信息
            EditorGUILayout.LabelField($"填充方式: {image.fillMethod}");
            EditorGUILayout.LabelField($"填充量: {image.fillAmount:P0}");
        }

        serializedObject.ApplyModifiedProperties();
    }
}

