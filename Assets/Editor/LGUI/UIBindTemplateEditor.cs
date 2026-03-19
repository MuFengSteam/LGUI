using UnityEngine;
using UnityEditor;

/// <summary>
/// UIBindTemplate 自定义编辑器
/// UIBindTemplate 不需要绑定变量名称，它是给 UIBindList 赋值的模板
/// 通过 UIBindList 的绑定变量去获取
/// </summary>
[CustomEditor(typeof(UIBindTemplate))]
public class UIBindTemplateEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Space();

        // 模板说明
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("模板组件", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "UIBindTemplate 是列表项模板组件\n" +
            "• 不需要设置绑定名称\n" +
            "• 通过 UIBindList 的 itemTemplate 属性引用\n" +
            "• 运行时由 UIBindList 克隆和管理", MessageType.Info);
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // 模板信息
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("模板内容", EditorStyles.boldLabel);
        
        UIBindTemplate template = target as UIBindTemplate;
        if (template != null)
        {
            // 统计子组件
            UIBase[] childComponents = template.GetComponentsInChildren<UIBase>(true);
            int bindCount = 0;
            foreach (var comp in childComponents)
            {
                if (comp != template && comp.HasValidBindName)
                {
                    bindCount++;
                }
            }
            
            EditorGUILayout.LabelField($"子绑定组件数量: {bindCount}");
            
            // 显示子组件列表
            if (bindCount > 0)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("子组件列表:", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                foreach (var comp in childComponents)
                {
                    if (comp != template && comp.HasValidBindName)
                    {
                        EditorGUILayout.LabelField($"• {comp.bindName} ({comp.ComponentTypeName})");
                    }
                }
                EditorGUI.indentLevel--;
            }
            else
            {
                EditorGUILayout.HelpBox("模板内没有绑定组件", MessageType.Warning);
            }
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // 使用说明
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("使用方式", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "在回调中访问模板内组件：\n" +
            "bindData.myList.SetList(dataList, (index, data, item) => {\n" +
            "    item.SetText(\"title\", data.title);\n" +
            "    item.SetImage(\"icon\", data.iconId);\n" +
            "});", MessageType.None);
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
}

