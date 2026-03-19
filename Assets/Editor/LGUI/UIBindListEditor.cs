using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections.Generic;

[CustomEditor(typeof(UIBindList))]
public class UIBindListEditor : Editor
{
    // Serialized properties
    private SerializedProperty bindNameProp;
    private SerializedProperty modeProp;
    private SerializedProperty itemTemplateProp;
    private SerializedProperty contentTransformProp;
    private SerializedProperty scrollRectProp;
    private SerializedProperty visibleItemsProp;
    private SerializedProperty itemSpacingProp;
    private SerializedProperty verticalProp;
    private SerializedProperty clearOnStartProp;

    // Foldout states
    private bool showBindingSettings = true;
    private bool showListSettings = true;
    private bool showLoopingSettings = true;
    private bool showActionSettings = true;
    private bool showComponentStatus = true;

    private void OnEnable()
    {
        // Initialize serialized properties
        // 注意：UIBase中的字段名是 _bindName（带下划线）
        bindNameProp = serializedObject.FindProperty("_bindName");
        modeProp = serializedObject.FindProperty("mode");
        itemTemplateProp = serializedObject.FindProperty("itemTemplate");
        contentTransformProp = serializedObject.FindProperty("contentTransform");
        scrollRectProp = serializedObject.FindProperty("scrollRect");
        visibleItemsProp = serializedObject.FindProperty("visibleItems");
        itemSpacingProp = serializedObject.FindProperty("itemSpacing");
        verticalProp = serializedObject.FindProperty("vertical");
        clearOnStartProp = serializedObject.FindProperty("clearOnStart");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        UIBindList bindList = target as UIBindList;
        
        // Title and description
        EditorGUILayout.Space();
        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
        titleStyle.fontSize = 14;
        EditorGUILayout.LabelField("UI Bind List", titleStyle);
        EditorGUILayout.LabelField("绑定数据列表组件，支持完全生成和循环列表模式", EditorStyles.miniLabel);
        EditorGUILayout.Space();

        // Binding Settings
        DrawBindingSettings();

        // List Settings
        DrawListSettings(bindList);

        // Looping List Settings
        DrawLoopingSettings(bindList);

        // Action Settings
        DrawActionSettings();

        // Component Status
        DrawComponentStatus(bindList);

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawBindingSettings()
    {
        showBindingSettings = EditorGUILayout.Foldout(showBindingSettings, "绑定设置", true, EditorStyles.foldoutHeader);
        
        if (showBindingSettings)
        {
            EditorGUI.indentLevel++;
            
            // Binding name field
            EditorGUILayout.PropertyField(bindNameProp, new GUIContent("绑定名称"));
            
            // Binding name validation
            string bindName = bindNameProp.stringValue;
            if (string.IsNullOrEmpty(bindName))
            {
                EditorGUILayout.HelpBox("绑定名称不能为空", MessageType.Error);
            }
            else if (!IsValidVariableName(bindName))
            {
                EditorGUILayout.HelpBox("绑定名称必须以字母开头，只能包含字母、数字和下划线", MessageType.Error);
            }
            
            EditorGUI.indentLevel--;
        }
    }

    private void DrawListSettings(UIBindList bindList)
    {
        showListSettings = EditorGUILayout.Foldout(showListSettings, "列表设置", true, EditorStyles.foldoutHeader);
        
        if (showListSettings)
        {
            EditorGUI.indentLevel++;
            
            // List mode
            EditorGUILayout.PropertyField(modeProp, new GUIContent("列表模式"));
            
            EditorGUILayout.Space();
            
            // Item template field
            EditorGUILayout.PropertyField(itemTemplateProp, new GUIContent("项目模板"));
            
            if (bindList.itemTemplate == null)
            {
                EditorGUILayout.HelpBox("必须指定一个项目模板", MessageType.Error);
            }
            
            // Content container field
            EditorGUILayout.PropertyField(contentTransformProp, new GUIContent("内容容器"));
            
            if (bindList.contentTransform == null)
            {
                EditorGUILayout.HelpBox("必须指定一个内容容器RectTransform", MessageType.Warning);
            }
            
            // Scroll rect field
            EditorGUILayout.PropertyField(scrollRectProp, new GUIContent("滚动视图"));
            
            UIBindList.ListMode listMode = (UIBindList.ListMode)modeProp.enumValueIndex;
            if (listMode == UIBindList.ListMode.LoopingList && bindList.scrollRect == null)
            {
                EditorGUILayout.HelpBox("循环列表模式必须指定ScrollRect组件", MessageType.Error);
            }
            
            EditorGUI.indentLevel--;
        }
    }

    private void DrawLoopingSettings(UIBindList bindList)
    {
        UIBindList.ListMode listMode = (UIBindList.ListMode)modeProp.enumValueIndex;
        string header = listMode == UIBindList.ListMode.LoopingList ? "循环列表设置" : "布局设置";
        
        showLoopingSettings = EditorGUILayout.Foldout(showLoopingSettings, header, true, EditorStyles.foldoutHeader);
        
        if (showLoopingSettings)
        {
            EditorGUI.indentLevel++;
            
            if (listMode == UIBindList.ListMode.LoopingList)
            {
                // Visible items
                EditorGUILayout.PropertyField(visibleItemsProp, new GUIContent("可见项目数"));
                
                // Direction toggle
                EditorGUILayout.PropertyField(verticalProp, new GUIContent("垂直方向"));
            }
            
            // Item spacing field
            EditorGUILayout.PropertyField(itemSpacingProp, new GUIContent("项目间距"));
            
            EditorGUI.indentLevel--;
        }
    }

    private void DrawActionSettings()
    {
        showActionSettings = EditorGUILayout.Foldout(showActionSettings, "行为设置", true, EditorStyles.foldoutHeader);
        
        if (showActionSettings)
        {
            EditorGUI.indentLevel++;
            
            // Clear on start toggle
            EditorGUILayout.PropertyField(clearOnStartProp, new GUIContent("启动时清空"));
            
            EditorGUI.indentLevel--;
        }
    }

    private void DrawComponentStatus(UIBindList bindList)
    {
        showComponentStatus = EditorGUILayout.Foldout(showComponentStatus, "组件状态", true, EditorStyles.foldoutHeader);
        
        if (showComponentStatus)
        {
            EditorGUI.indentLevel++;
            
            // Check RectTransform
            RectTransform rectTransform = bindList.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                EditorGUILayout.HelpBox("RectTransform: 已找到", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("RectTransform: 缺失 (必需)", MessageType.Error);
            }
            
            // Check item template
            if (bindList.itemTemplate != null)
            {
                string status = "项目模板: " + bindList.itemTemplate.name;
                
                // Check if template has RectTransform
                RectTransform templateRect = bindList.itemTemplate.GetComponent<RectTransform>();
                if (templateRect != null)
                {
                    status += $" (大小: {templateRect.rect.width} x {templateRect.rect.height})";
                }
                else
                {
                    status += " (缺少RectTransform)";
                }
                
                EditorGUILayout.HelpBox(status, MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("项目模板: 未设置", MessageType.Warning);
            }
            
            // Check content container
            if (bindList.contentTransform != null)
            {
                EditorGUILayout.HelpBox("内容容器: " + bindList.contentTransform.name, MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("内容容器: 未设置", MessageType.Warning);
            }
            
            // Check scroll rect and its structure
            if (bindList.scrollRect != null)
            {
                EditorGUILayout.HelpBox("ScrollRect: " + bindList.scrollRect.name, MessageType.Info);
                
                // Check ScrollView structure for common issues
                bool hasIssues = false;
                List<string> issues = new List<string>();
                
                // Check if viewport has RectMask2D
                if (bindList.scrollRect.viewport != null)
                {
                    var rectMask = bindList.scrollRect.viewport.GetComponent<RectMask2D>();
                    var mask = bindList.scrollRect.viewport.GetComponent<Mask>();
                    if (rectMask == null && mask == null)
                    {
                        hasIssues = true;
                        issues.Add("Viewport 缺少裁剪组件 (RectMask2D 或 Mask)");
                    }
                    
                    var viewportImage = bindList.scrollRect.viewport.GetComponent<Image>();
                    if (viewportImage == null && mask != null)
                    {
                        hasIssues = true;
                        issues.Add("Viewport 使用 Mask 但缺少 Image 组件");
                    }
                }
                else
                {
                    hasIssues = true;
                    issues.Add("ScrollRect 未设置 Viewport");
                }
                
                // Check if ScrollView has Image for raycast
                var scrollImage = bindList.scrollRect.GetComponent<Image>();
                if (scrollImage == null)
                {
                    hasIssues = true;
                    issues.Add("ScrollView 缺少 Image 组件（无法拖拽）");
                }
                else if (!scrollImage.raycastTarget)
                {
                    hasIssues = true;
                    issues.Add("ScrollView 的 Image raycastTarget 为 false（无法拖拽）");
                }
                
                if (hasIssues)
                {
                    string issueText = "ScrollView 结构问题:\n• " + string.Join("\n• ", issues);
                    EditorGUILayout.HelpBox(issueText, MessageType.Warning);
                    
                    if (GUILayout.Button("修复 ScrollView 结构"))
                    {
                        FixScrollViewStructure(bindList);
                    }
                }
            }
            else
            {
                string message = "ScrollRect: 未设置";
                
                if (bindList.mode == UIBindList.ListMode.LoopingList)
                {
                    message += " (循环列表模式下必需)";
                    EditorGUILayout.HelpBox(message, MessageType.Error);
                }
                else
                {
                    EditorGUILayout.HelpBox(message, MessageType.Info);
                }
            }
            
            // Item count information
            EditorGUILayout.LabelField("活动项目数量: " + bindList.GetItemCount());
            
            EditorGUILayout.Space();
            
            // Button to clear items
            if (GUILayout.Button("清空列表项"))
            {
                bindList.ClearItems();
            }
            
            EditorGUI.indentLevel--;
        }
    }

    /// <summary>
    /// 修复 ScrollView 结构问题
    /// </summary>
    private void FixScrollViewStructure(UIBindList bindList)
    {
        if (bindList.scrollRect == null)
            return;

        Undo.RecordObject(bindList.scrollRect.gameObject, "Fix ScrollView Structure");

        // 1. 确保 ScrollView 有 Image 组件用于拖拽
        var scrollImage = bindList.scrollRect.GetComponent<Image>();
        if (scrollImage == null)
        {
            scrollImage = bindList.scrollRect.gameObject.AddComponent<Image>();
        }
        scrollImage.color = new Color(1, 1, 1, 1); // 不透明（alpha=1）
        scrollImage.raycastTarget = true;

        // 2. 移除 ScrollView 上错误放置的 Mask 组件
        var scrollMask = bindList.scrollRect.GetComponent<Mask>();
        if (scrollMask != null)
        {
            Undo.DestroyObjectImmediate(scrollMask);
        }

        // 3. 修复 Viewport
        if (bindList.scrollRect.viewport != null)
        {
            Undo.RecordObject(bindList.scrollRect.viewport.gameObject, "Fix Viewport");

            // 确保 Viewport 有 Image
            var viewportImage = bindList.scrollRect.viewport.GetComponent<Image>();
            if (viewportImage == null)
            {
                viewportImage = bindList.scrollRect.viewport.gameObject.AddComponent<Image>();
                viewportImage.color = new Color(1, 1, 1, 0); // 透明
            }
            viewportImage.raycastTarget = true;

            // 确保有裁剪组件（优先使用 RectMask2D）
            var rectMask = bindList.scrollRect.viewport.GetComponent<RectMask2D>();
            var mask = bindList.scrollRect.viewport.GetComponent<Mask>();
            if (rectMask == null && mask == null)
            {
                bindList.scrollRect.viewport.gameObject.AddComponent<RectMask2D>();
            }
        }

        EditorUtility.SetDirty(bindList.scrollRect.gameObject);
        if (bindList.scrollRect.viewport != null)
        {
            EditorUtility.SetDirty(bindList.scrollRect.viewport.gameObject);
        }

        Debug.Log("[UIBindList] ScrollView 结构已修复");
    }

    private bool IsValidVariableName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return false;
                
        if (!char.IsLetter(name[0]) && name[0] != '_')
            return false;
                
        for (int i = 1; i < name.Length; i++)
        {
            if (!char.IsLetterOrDigit(name[i]) && name[i] != '_')
                return false;
        }
            
        return true;
    }
} 