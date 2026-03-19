using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

[CustomEditor(typeof(UIBindSlider))]
public class UIBindSliderEditor : Editor
{
    private SerializedProperty _bindNameProp;
    private SerializedProperty _defaultValueProp;
    private SerializedProperty _minValueProp;
    private SerializedProperty _maxValueProp;
    private SerializedProperty _setDefaultOnStartProp;

    private void OnEnable()
    {
        _bindNameProp = serializedObject.FindProperty("_bindName");
        _defaultValueProp = serializedObject.FindProperty("defaultValue");
        _minValueProp = serializedObject.FindProperty("minValue");
        _maxValueProp = serializedObject.FindProperty("maxValue");
        _setDefaultOnStartProp = serializedObject.FindProperty("setDefaultOnStart");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Space();

        // 绑定设置
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("绑定设置", EditorStyles.boldLabel);
        
        if (_bindNameProp != null)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_bindNameProp, new GUIContent("绑定名称"));
            if (EditorGUI.EndChangeCheck())
            {
                string newName = _bindNameProp.stringValue;
                if (!string.IsNullOrEmpty(newName))
                {
                    if (!char.IsLetter(newName[0]))
                    {
                        _bindNameProp.stringValue = "slider" + newName;
                    }
                }
            }

            if (string.IsNullOrEmpty(_bindNameProp.stringValue))
            {
                EditorGUILayout.HelpBox("绑定名称不能为空", MessageType.Error);
            }
            else if (!char.IsLetter(_bindNameProp.stringValue[0]))
            {
                EditorGUILayout.HelpBox("绑定名称必须以字母开头", MessageType.Error);
            }
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // 滑动条设置
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("滑动条设置", EditorStyles.boldLabel);

        if (_minValueProp != null && _maxValueProp != null)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_minValueProp, new GUIContent("最小值"));
            EditorGUILayout.PropertyField(_maxValueProp, new GUIContent("最大值"));
            if (EditorGUI.EndChangeCheck())
            {
                // 确保最小值小于最大值
                if (_minValueProp.floatValue > _maxValueProp.floatValue)
                {
                    _maxValueProp.floatValue = _minValueProp.floatValue;
                }
            }

            // 使用滑动条显示默认值
            if (_defaultValueProp != null)
            {
                float defaultValue = EditorGUILayout.Slider("默认值", _defaultValueProp.floatValue, 
                    _minValueProp.floatValue, _maxValueProp.floatValue);
                if (defaultValue != _defaultValueProp.floatValue)
                {
                    _defaultValueProp.floatValue = defaultValue;
                }
            }
        }

        if (_setDefaultOnStartProp != null)
        {
            EditorGUILayout.PropertyField(_setDefaultOnStartProp, new GUIContent("启动时设置默认值"));
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // 组件状态
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("组件状态", EditorStyles.boldLabel);

        UIBindSlider uiSlider = (UIBindSlider)target;
        Transform targetTransform = uiSlider.transform;
        Slider slider = uiSlider.GetComponent<Slider>();
        
        if (slider == null)
        {
            EditorGUILayout.HelpBox("缺少 Slider 组件，将在运行时自动添加", MessageType.Info);
        }
        else
        {
            GUI.color = Color.green;
            EditorGUILayout.HelpBox("Slider 组件已配置", MessageType.Info);
            GUI.color = Color.white;

            // 显示当前值
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.Slider("当前值", slider.value, slider.minValue, slider.maxValue);
            EditorGUI.EndDisabledGroup();

            // 检查必要的子对象
            bool hasBackground = targetTransform.Find("Background") != null;
            bool hasFillArea = targetTransform.Find("Fill Area") != null;
            bool hasHandle = targetTransform.Find("Handle Slide Area") != null;

            if (!hasBackground || !hasFillArea || !hasHandle)
            {
                EditorGUILayout.HelpBox("缺少必要的子对象，将在运行时自动创建", MessageType.Warning);
            }
        }

        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
} 