// 作者: 木枫LL
// GitHub: https://github.com/MuFengSteam/
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(LGUIRoot))]
[CanEditMultipleObjects]
public class LGUIRootEditor : Editor
{
    private SerializedProperty _depthProp;
    private SerializedProperty _bindDataClassNameProp;
    private SerializedProperty _uiScriptNameProp;
    private bool _autoNameFoldout = true;
    private bool _bindingsFoldout = false;

    private void OnEnable()
    {
        _depthProp = serializedObject.FindProperty("_depth");
        _bindDataClassNameProp = serializedObject.FindProperty("_bindDataClassName");
        _uiScriptNameProp = serializedObject.FindProperty("_uiScriptName");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        LGUIRoot root = (LGUIRoot)target;

        EditorGUILayout.Space();

        // Widget Settings
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Widget设置", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_depthProp);
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // 绑定组件列表
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        _bindingsFoldout = EditorGUILayout.Foldout(_bindingsFoldout, "绑定组件列表", true, EditorStyles.foldoutHeader);
        if (_bindingsFoldout)
        {
            EditorGUI.indentLevel++;
            var components = root.CollectBindComponents();
            if (components != null && components.Count > 0)
            {
                foreach (var component in components)
                {
                    if (component != null)
                    {
                        string displayName = component.HasValidBindName ? component.bindName : component.name;
                        EditorGUILayout.ObjectField(displayName, component, component.GetType(), true);
                    }
                }
                EditorGUILayout.LabelField($"共 {components.Count} 个绑定组件");
            }
            else
            {
                EditorGUILayout.HelpBox("没有找到绑定组件", MessageType.Info);
            }
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // LGUI Settings
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("LGUI设置", EditorStyles.boldLabel);
        
        // 显示当前预制体名称
        string prefabName = root.gameObject.name;
        EditorGUILayout.LabelField("预制体名称:", prefabName);
        
        // 类名设置区域
        _autoNameFoldout = EditorGUILayout.Foldout(_autoNameFoldout, "类名设置", true, EditorStyles.foldoutHeader);
        if (_autoNameFoldout)
        {
            EditorGUI.indentLevel++;
            
            // 显示自动生成的名称
            bool isEmpty = string.IsNullOrEmpty(_bindDataClassNameProp.stringValue) && 
                           string.IsNullOrEmpty(_uiScriptNameProp.stringValue);
            
            string autoBindDataName = GetAutoBindDataName(prefabName);
            string autoScriptName = GetAutoScriptName(prefabName);
            
            if (isEmpty)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.TextField("自动生成的绑定数据类名:", autoBindDataName);
                EditorGUILayout.TextField("自动生成的UI脚本名:", autoScriptName);
                EditorGUI.EndDisabledGroup();
                
                EditorGUILayout.HelpBox("首次生成代码时将使用自动生成的类名", MessageType.Info);
            }
            else
            {
                // 显示当前使用的类名
                EditorGUILayout.PropertyField(_bindDataClassNameProp);
                EditorGUILayout.PropertyField(_uiScriptNameProp);
                
                EditorGUILayout.HelpBox("已生成代码，后续生成将保留脚本并只更新绑定数据", MessageType.Info);
            }
            
            EditorGUI.indentLevel--;
        }
        
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // Canvas Settings
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Canvas状态", EditorStyles.boldLabel);
        Canvas canvas = root.GetComponent<Canvas>();
        if (canvas != null)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.EnumPopup("渲染模式", canvas.renderMode);
            EditorGUILayout.IntField("排序顺序", canvas.sortingOrder);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.HelpBox("Canvas设置由LGUIRoot自动管理", MessageType.Info);
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // Generate Code Button
        if (GUILayout.Button("生成代码", GUILayout.Height(30)))
        {
            root.GenerateCode();
        }

        serializedObject.ApplyModifiedProperties();
    }

    // 获取自动生成的绑定数据类名
    private string GetAutoBindDataName(string prefabName)
    {
        // 移除(Clone)后缀
        if (prefabName.EndsWith("(Clone)"))
        {
            prefabName = prefabName.Substring(0, prefabName.Length - 7);
        }
        
        // 如果名称以Panel结尾
        if (prefabName.EndsWith("Panel"))
        {
            string baseName = prefabName.Substring(0, prefabName.Length - 5); // 移除"Panel"
            return baseName + "PanelBindData";
        }
        else
        {
            return prefabName + "PanelBindData";
        }
    }
    
    // 获取自动生成的UI脚本名
    private string GetAutoScriptName(string prefabName)
    {
        // 移除(Clone)后缀
        if (prefabName.EndsWith("(Clone)"))
        {
            prefabName = prefabName.Substring(0, prefabName.Length - 7);
        }
        
        // 如果名称以Panel结尾
        if (prefabName.EndsWith("Panel"))
        {
            return prefabName;
        }
        else
        {
            return prefabName + "Panel";
        }
    }
} 