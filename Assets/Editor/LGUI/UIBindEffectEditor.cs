using UnityEngine;
using UnityEditor;

/// <summary>
/// UIBindEffect 自定义编辑器
/// 隐藏 BindId，只显示 BindName 和特效设置
/// </summary>
[CustomEditor(typeof(UIBindEffect))]
public class UIBindEffectEditor : Editor
{
    private SerializedProperty _bindNameProp;
    private SerializedProperty _effectsProp;
    private SerializedProperty _autoCollectProp;

    private void OnEnable()
    {
        _bindNameProp = serializedObject.FindProperty("_bindName");
        _effectsProp = serializedObject.FindProperty("m_Effects");
        _autoCollectProp = serializedObject.FindProperty("m_AutoCollect");
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
            EditorGUILayout.PropertyField(_bindNameProp, new GUIContent("绑定变量名称", 
                "在生成的代码中使用的变量名称，用于控制特效"));
            
            if (string.IsNullOrEmpty(_bindNameProp.stringValue))
            {
                EditorGUILayout.HelpBox("请输入绑定变量名称！", MessageType.Warning);
            }
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // 特效设置
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("特效设置", EditorStyles.boldLabel);
        
        EditorGUILayout.PropertyField(_autoCollectProp, new GUIContent("自动收集子节点特效"));
        
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(_effectsProp, new GUIContent("特效列表"), true);
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // 快捷操作
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("快捷操作", EditorStyles.boldLabel);
        
        if (GUILayout.Button("自动收集子节点特效"))
        {
            UIBindEffect effect = target as UIBindEffect;
            if (effect != null)
            {
                // 调用上下文菜单方法
                var method = effect.GetType().GetMethod("EditorAutoCollect", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (method != null)
                {
                    method.Invoke(effect, null);
                    EditorUtility.SetDirty(effect);
                }
            }
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // 使用说明
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("使用说明", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "播放特效：bindData.myEffect.Play(\"effectName\")\n" +
            "停止特效：bindData.myEffect.Stop(\"effectName\")\n" +
            "停止所有：bindData.myEffect.StopAll()", MessageType.Info);
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
}

