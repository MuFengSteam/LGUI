using UnityEngine;
using UnityEditor;

/// <summary>
/// UIBindButton 自定义编辑器
/// 隐藏基类的 BindName 和 BindId，只显示 UIBindButton 特有属性
/// 悬停缩放功能已移至 UIButton 组件
/// </summary>
[CustomEditor(typeof(UIBindButton))]
public class UIBindButtonEditor : Editor
{
    private SerializedProperty _buttonIdProp;
    private SerializedProperty _indexProp;
    private SerializedProperty _enableLongPressProp;
    private SerializedProperty _longPressUpEventIdProp;
    private SerializedProperty _longPressTimeProp;

    private void OnEnable()
    {
        _buttonIdProp = serializedObject.FindProperty("buttonId");
        _indexProp = serializedObject.FindProperty("index");
        _enableLongPressProp = serializedObject.FindProperty("enableLongPress");
        _longPressUpEventIdProp = serializedObject.FindProperty("longPressUpEventId");
        _longPressTimeProp = serializedObject.FindProperty("longPressTime");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Space();

        // Button设置
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Button设置", EditorStyles.boldLabel);
        
        EditorGUILayout.PropertyField(_buttonIdProp, new GUIContent("按钮ID", "按钮ID，用于与UIBasePanel中的方法关联"));
        
        if (_buttonIdProp.longValue <= 0)
        {
            EditorGUILayout.HelpBox("按钮ID必须大于0", MessageType.Error);
        }
        
        EditorGUILayout.PropertyField(_indexProp, new GUIContent("索引参数", "传入点击方法的参数值，-1表示不传递"));
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // 长按设置
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("长按设置", EditorStyles.boldLabel);
        
        EditorGUILayout.PropertyField(_enableLongPressProp, new GUIContent("启用长按模式"));
        
        if (_enableLongPressProp.boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_longPressTimeProp, new GUIContent("长按触发时间(秒)"));
            EditorGUILayout.PropertyField(_longPressUpEventIdProp, new GUIContent("长按抬起事件ID"));
            
            if (_longPressUpEventIdProp.longValue <= 0)
            {
                EditorGUILayout.HelpBox("长按模式下，长按抬起事件ID应大于0", MessageType.Warning);
            }
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // 组件状态
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("组件状态", EditorStyles.boldLabel);
        
        UIBindButton uiBindButton = (UIBindButton)target;
        UIButton uiButton = uiBindButton.GetComponent<UIButton>();
        
        if (uiButton == null)
        {
            EditorGUILayout.HelpBox("缺少 UIButton 组件，将在运行时自动添加", MessageType.Info);
        }
        else
        {
            GUI.color = Color.green;
            EditorGUILayout.HelpBox("UIButton 组件已配置（悬停缩放在 UIButton 中设置）", MessageType.Info);
            GUI.color = Color.white;
        }
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
}

