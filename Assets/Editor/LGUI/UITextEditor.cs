using UnityEngine;
using UnityEditor;

/// <summary>
/// UIText组件的自定义编辑器
/// 显示翻译相关信息和打字机效果设置
/// </summary>
[CustomEditor(typeof(UIText))]
[CanEditMultipleObjects]
public class UITextEditor : TMPro.EditorUtilities.TMP_EditorPanelUI
{
    // 翻译属性
    private SerializedProperty _enableTranslationProp;
    private SerializedProperty _translationIdProp;
    private SerializedProperty _originalTextProp;
    private SerializedProperty _nodePathProp;

    // 打字机效果属性
    private SerializedProperty _enableTypeWriterProp;
    private SerializedProperty _typeWriterSpeedProp;
    private SerializedProperty _typeWriterCompleteDelayProp;

    private bool _propertiesInitialized = false;

    protected override void OnEnable()
    {
        base.OnEnable();
        RefreshSerializedProperties();
    }

    private void RefreshSerializedProperties()
    {
        _propertiesInitialized = false;

        if (serializedObject == null) return;

        try
        {
            // 翻译属性
            _enableTranslationProp = serializedObject.FindProperty("_enableTranslation");
            _translationIdProp = serializedObject.FindProperty("_translationId");
            _originalTextProp = serializedObject.FindProperty("_originalText");
            _nodePathProp = serializedObject.FindProperty("_nodePath");

            // 打字机效果属性
            _enableTypeWriterProp = serializedObject.FindProperty("_enableTypeWriter");
            _typeWriterSpeedProp = serializedObject.FindProperty("_typeWriterSpeed");
            _typeWriterCompleteDelayProp = serializedObject.FindProperty("_typeWriterCompleteDelay");

            _propertiesInitialized = (_enableTranslationProp != null && _enableTypeWriterProp != null);
        }
        catch
        {
            _propertiesInitialized = false;
        }
    }

    public override void OnInspectorGUI()
    {
        // 绘制翻译信息区域
        DrawTranslationSection();

        EditorGUILayout.Space(5);

        // 绘制打字机效果区域
        DrawTypeWriterSection();

        EditorGUILayout.Space(5);

        // 绘制原有的TextMeshPro编辑器
        base.OnInspectorGUI();
    }

    private void DrawTranslationSection()
    {
        if (serializedObject == null) return;

        // 检查属性是否有效
        if (!_propertiesInitialized)
        {
            RefreshSerializedProperties();
        }

        if (!_propertiesInitialized)
        {
            EditorGUILayout.HelpBox("无法加载翻译属性", MessageType.Warning);
            return;
        }

        serializedObject.Update();

        EditorGUILayout.BeginVertical("box");
        // EditorGUILayout.LabelField("翻译设置", EditorStyles.boldLabel);

        UIText uiText = target as UIText;
        if (uiText == null) 
        {
            EditorGUILayout.EndVertical();
            return;
        }

        // 显示"翻译？"勾选框
        EditorGUILayout.PropertyField(_enableTranslationProp, new GUIContent("启用翻译？", "是否启用翻译功能"));

        // 只有勾选"翻译？"后才显示翻译相关内容
        if (_enableTranslationProp.boolValue)
        {
            EditorGUILayout.Space(3);

            // 显示翻译ID（只读）
            EditorGUI.BeginDisabledGroup(true);
            if (_translationIdProp != null)
            {
                EditorGUILayout.PropertyField(_translationIdProp, new GUIContent("翻译ID"));
            }
            EditorGUI.EndDisabledGroup();

            // 显示状态
            if (uiText.HasValidTranslationId)
            {
                EditorGUILayout.HelpBox($"已分配翻译ID: {uiText.TranslationId}", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("未分配翻译ID\n使用 Tools → 翻译工具 → Prefab翻译导出工具 进行导出", MessageType.None);
            }

            // 显示原始文本
            EditorGUI.BeginDisabledGroup(true);
            if (_originalTextProp != null)
            {
                EditorGUILayout.PropertyField(_originalTextProp, new GUIContent("原始文本"));
            }
            EditorGUI.EndDisabledGroup();

            // 显示节点路径
            EditorGUI.BeginDisabledGroup(true);
            if (_nodePathProp != null)
            {
                EditorGUILayout.PropertyField(_nodePathProp, new GUIContent("节点路径"));
            }
            EditorGUI.EndDisabledGroup();

            // 操作按钮
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("更新原始文本/路径"))
            {
                foreach (var t in targets)
                {
                    UIText text = t as UIText;
                    if (text != null)
                    {
                        text.UpdateOriginalText();
                        text.GenerateNodePath();
                        EditorUtility.SetDirty(text);
                    }
                }
            }

            if (uiText.HasValidTranslationId)
            {
                GUI.backgroundColor = new Color(1f, 0.5f, 0.5f);
                if (GUILayout.Button("清除翻译"))
                {
                    if (EditorUtility.DisplayDialog("确认清除", "确定要清除翻译ID吗？\n\n这将需要重新导出翻译。", "确定", "取消"))
                    {
                        foreach (var t in targets)
                        {
                            UIText text = t as UIText;
                            if (text != null)
                            {
                                text.ClearTranslation();
                                EditorUtility.SetDirty(text);
                            }
                        }
                    }
                }
                GUI.backgroundColor = Color.white;
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawTypeWriterSection()
    {
        if (serializedObject == null) return;

        // 检查属性是否有效
        if (!_propertiesInitialized)
        {
            RefreshSerializedProperties();
        }

        if (_enableTypeWriterProp == null)
        {
            return;
        }

        serializedObject.Update();

        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("打字机效果", EditorStyles.boldLabel);

        // 显示"启用打字机效果"勾选框
        EditorGUILayout.PropertyField(_enableTypeWriterProp, new GUIContent("启用打字机效果", "勾选后，调用SetText时会使用打字机效果逐字显示"));

        // 只有勾选"启用打字机效果"后才显示其他设置
        if (_enableTypeWriterProp.boolValue)
        {
            EditorGUILayout.Space(3);

            // 显示打字机速度
            if (_typeWriterSpeedProp != null)
            {
                EditorGUILayout.PropertyField(_typeWriterSpeedProp, new GUIContent("打字速度", "每秒显示的字符数（1-200）"));
            }

            // 显示完成延迟
            if (_typeWriterCompleteDelayProp != null)
            {
                EditorGUILayout.PropertyField(_typeWriterCompleteDelayProp, new GUIContent("完成回调延迟", "打字完成后触发回调的延迟时间（秒）"));
            }

            // 显示使用提示
            EditorGUILayout.Space(3);
            EditorGUILayout.HelpBox(
                "使用说明：\n" +
                "• 启用后，调用 SetText() 会自动使用打字机效果\n" +
                "• 调用 SetTextImmediate() 可立即显示，跳过打字机效果\n" +
                "• 代码中可通过 text.typeWriterSpeed 调整速度",
                MessageType.Info);
        }

        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
}
