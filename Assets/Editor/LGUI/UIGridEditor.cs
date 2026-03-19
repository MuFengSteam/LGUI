using UnityEngine;
using UnityEditor;

/// <summary>
/// UIGrid的自定义编辑器
/// 提供网格布局的可视化配置界面
/// </summary>
[CustomEditor(typeof(UIGrid))]
public class UIGridEditor : Editor
{
    // 布局设置
    private SerializedProperty _startCornerProp;
    private SerializedProperty _startAxisProp;
    private SerializedProperty _cellSizeProp;
    private SerializedProperty _spacingProp;
    private SerializedProperty _constraintProp;
    private SerializedProperty _constraintCountProp;
    
    // 对齐设置
    private SerializedProperty _childAlignmentProp;
    private SerializedProperty _paddingProp;
    
    private bool _propertiesInitialized = false;

    private void OnEnable()
    {
        RefreshSerializedProperties();
    }

    /// <summary>
    /// 刷新序列化属性引用
    /// </summary>
    private void RefreshSerializedProperties()
    {
        if (serializedObject == null || serializedObject.targetObject == null)
        {
            _propertiesInitialized = false;
            return;
        }

        try
        {
            // 使用正确的序列化字段名（m_前缀）
            _startCornerProp = serializedObject.FindProperty("m_StartCorner");
            _startAxisProp = serializedObject.FindProperty("m_StartAxis");
            _cellSizeProp = serializedObject.FindProperty("m_CellSize");
            _spacingProp = serializedObject.FindProperty("m_Spacing");
            _constraintProp = serializedObject.FindProperty("m_Constraint");
            _constraintCountProp = serializedObject.FindProperty("m_ConstraintCount");
            _childAlignmentProp = serializedObject.FindProperty("m_ChildAlignment");
            _paddingProp = serializedObject.FindProperty("m_Padding");
            
            _propertiesInitialized = true;
        }
        catch (System.Exception)
        {
            _propertiesInitialized = false;
        }
    }

    public override void OnInspectorGUI()
    {
        // 确保序列化对象有效
        if (serializedObject == null || serializedObject.targetObject == null)
        {
            EditorGUILayout.HelpBox("目标对象无效", MessageType.Warning);
            return;
        }

        // 如果属性未初始化，重新初始化
        if (!_propertiesInitialized)
        {
            RefreshSerializedProperties();
        }

        serializedObject.Update();

        UIGrid grid = target as UIGrid;
        if (grid == null)
        {
            EditorGUILayout.HelpBox("无法获取UIGrid组件", MessageType.Error);
            return;
        }

        EditorGUILayout.Space();

        // 布局设置
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("布局设置", EditorStyles.boldLabel);

        SafePropertyField(_startCornerProp, "起始角落");
        SafePropertyField(_startAxisProp, "排列方向");
        SafePropertyField(_cellSizeProp, "单元格大小");
        SafePropertyField(_spacingProp, "间距");

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // 约束设置
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("约束设置", EditorStyles.boldLabel);

        SafePropertyField(_constraintProp, "约束模式");
        
        // 只有在非Flexible模式下才显示约束数量
        if (_constraintProp != null && _constraintProp.enumValueIndex != 0)
        {
            SafePropertyField(_constraintCountProp, "约束数量");
            
            if (_constraintCountProp != null && _constraintCountProp.intValue < 1)
            {
                _constraintCountProp.intValue = 1;
            }
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // 对齐设置
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("对齐设置", EditorStyles.boldLabel);

        SafePropertyField(_childAlignmentProp, "子元素对齐");
        SafePropertyField(_paddingProp, "内边距");

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // 状态信息
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("状态信息", EditorStyles.boldLabel);

        Transform transform = grid.transform;
        int childCount = transform.childCount;
        int activeChildCount = 0;

        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.gameObject.activeSelf)
            {
                activeChildCount++;
            }
        }

        EditorGUILayout.HelpBox($"子对象：{childCount} 个（{activeChildCount} 个激活）", MessageType.Info);

        if (GUILayout.Button("立即刷新布局"))
        {
            grid.LayoutGrid();
            EditorUtility.SetDirty(target);
        }

        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }

    /// <summary>
    /// 安全地绘制属性字段（带空值检查）
    /// </summary>
    private void SafePropertyField(SerializedProperty property, string label)
    {
        if (property != null)
        {
            EditorGUILayout.PropertyField(property, new GUIContent(label));
        }
    }
}
