using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

[CustomEditor(typeof(UIBindToggle))]
public class UIBindToggleEditor : Editor
{
    private UIBindToggle _target;
    private SerializedProperty _bindNameProp;
    private SerializedProperty _defaultValueProp;
    private SerializedProperty _setDefaultOnStartProp;

    private readonly Color warningColor = new Color(1f, 0.9f, 0.6f);
    private readonly Color successColor = new Color(0.8f, 1f, 0.8f);

    private void OnEnable()
    {
        _target = (UIBindToggle)target;
        _bindNameProp = serializedObject.FindProperty("bindName");
        _defaultValueProp = serializedObject.FindProperty("defaultValue");
        _setDefaultOnStartProp = serializedObject.FindProperty("setDefaultOnStart");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Space();

        // 绑定名称区域
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        {
            EditorGUILayout.LabelField("绑定设置", EditorStyles.boldLabel);

            // 绑定名称输入框
            EditorGUI.BeginChangeCheck();
            string newBindName = EditorGUILayout.TextField(new GUIContent("绑定变量名称", 
                "在生成的代码中使用的变量名称，用于访问此Toggle组件"), _bindNameProp.stringValue);
            if (EditorGUI.EndChangeCheck())
            {
                _bindNameProp.stringValue = newBindName;
            }

            // 显示警告或提示
            if (string.IsNullOrEmpty(_bindNameProp.stringValue))
            {
                EditorGUILayout.HelpBox("请输入绑定变量名称！", MessageType.Warning);
            }
            else if (!char.IsLetter(_bindNameProp.stringValue[0]))
            {
                EditorGUILayout.HelpBox("变量名称必须以字母开头！", MessageType.Error);
            }
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // Toggle设置区域
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        {
            EditorGUILayout.LabelField("Toggle设置", EditorStyles.boldLabel);

            // 默认值设置
            EditorGUILayout.PropertyField(_defaultValueProp, new GUIContent("默认值", 
                "Toggle的默认选中状态"));

            // 自动设置默认值
            EditorGUILayout.PropertyField(_setDefaultOnStartProp, new GUIContent("自动设置默认值", 
                "是否在Start时自动设置默认值"));

            // 预览当前值
            Toggle toggleComponent = _target.GetComponent<Toggle>();
            if (toggleComponent != null)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.Toggle("当前值", toggleComponent.isOn);
                EditorGUI.EndDisabledGroup();
            }
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // 组件状态区域
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        {
            EditorGUILayout.LabelField("组件状态", EditorStyles.boldLabel);

            // 显示当前检测到的组件类型
            Toggle toggleComponent = _target.GetComponent<Toggle>();

            // 显示组件状态
            if (toggleComponent != null)
            {
                GUI.backgroundColor = successColor;
                EditorGUILayout.HelpBox("已检测到 Toggle 组件", MessageType.Info);

                // 检查必要的子组件
                bool hasBackground = false;
                bool hasCheckmark = false;
                foreach (Transform child in _target.transform)
                {
                    if (child.name == "Background") hasBackground = true;
                    if (child.name == "Checkmark") hasCheckmark = true;
                }

                if (!hasBackground || !hasCheckmark)
                {
                    GUI.backgroundColor = warningColor;
                    if (!hasBackground)
                        EditorGUILayout.HelpBox("缺少 Background 子对象！", MessageType.Warning);
                    if (!hasCheckmark)
                        EditorGUILayout.HelpBox("缺少 Checkmark 子对象！", MessageType.Warning);
                }
            }
            else
            {
                GUI.backgroundColor = warningColor;
                EditorGUILayout.HelpBox("未检测到 Toggle 组件！将自动添加", MessageType.Warning);
            }
            GUI.backgroundColor = Color.white;
        }
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
} 