using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.UI;

[CustomEditor(typeof(UIBindInputField))]
public class UIBindInputFieldEditor : Editor
{
    private SerializedProperty _bindNameProp;
    private SerializedProperty _maxCharacterLimitProp;
    private SerializedProperty _placeholderTextProp;
    private SerializedProperty _isPasswordProp;

    private void OnEnable()
    {
        _bindNameProp = serializedObject.FindProperty("_bindName");
        _maxCharacterLimitProp = serializedObject.FindProperty("maxCharacterLimit");
        _placeholderTextProp = serializedObject.FindProperty("placeholderText");
        _isPasswordProp = serializedObject.FindProperty("isPassword");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Space();

        // 绑定设置（不显示BindId）
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
                        _bindNameProp.stringValue = "input" + newName;
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

        // InputField设置
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("InputField设置", EditorStyles.boldLabel);

        if (_maxCharacterLimitProp != null)
        {
            EditorGUILayout.PropertyField(_maxCharacterLimitProp, new GUIContent("最大字符数", "0表示不限制"));
        }
        
        if (_placeholderTextProp != null)
        {
            EditorGUILayout.PropertyField(_placeholderTextProp, new GUIContent("占位符文本"));
        }
        
        if (_isPasswordProp != null)
        {
            EditorGUILayout.PropertyField(_isPasswordProp, new GUIContent("密码输入"));
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // 组件状态
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("组件状态", EditorStyles.boldLabel);

        UIBindInputField uiInputField = (UIBindInputField)target;
        TMP_InputField tmpInputField = uiInputField.GetComponent<TMP_InputField>();
        InputField legacyInputField = uiInputField.GetComponent<InputField>();
        
        if (tmpInputField != null)
        {
            GUI.color = Color.green;
            EditorGUILayout.HelpBox("TMP_InputField 组件已配置", MessageType.Info);
            GUI.color = Color.white;
        }
        else if (legacyInputField != null)
        {
            GUI.color = Color.yellow;
            EditorGUILayout.HelpBox("使用 Legacy InputField，建议改用 TMP_InputField", MessageType.Warning);
            GUI.color = Color.white;
        }
        else
        {
            EditorGUILayout.HelpBox("缺少 InputField 组件，请手动添加 TMP_InputField", MessageType.Error);
            
            if (GUILayout.Button("添加 TMP_InputField"))
            {
                // 添加TMP_InputField需要一些额外设置
                EditorGUILayout.HelpBox("请通过 GameObject > UI > Input Field - TextMeshPro 创建", MessageType.Info);
            }
        }

        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
}

