using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

/// <summary>
/// UIBindImage的自定义编辑器
/// 提供图片绑定的可视化配置界面
/// </summary>
[CustomEditor(typeof(UIBindImage))]
public class UIBindImageEditor : Editor
{
    private SerializedProperty _bindNameProp;
    private SerializedProperty _defaultSpriteProp;
    private SerializedProperty _preserveAspectProp;

    private readonly Color warningColor = new Color(1f, 0.9f, 0.6f);
    private readonly Color successColor = new Color(0.8f, 1f, 0.8f);
    
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
            // 使用正确的序列化字段名（_前缀，继承自UIBase）
            _bindNameProp = serializedObject.FindProperty("_bindName");
            _defaultSpriteProp = serializedObject.FindProperty("defaultSprite");
            _preserveAspectProp = serializedObject.FindProperty("preserveAspect");
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

        UIBindImage bindImage = target as UIBindImage;
        if (bindImage == null)
        {
            EditorGUILayout.HelpBox("无法获取UIBindImage组件", MessageType.Error);
            return;
        }

        EditorGUILayout.Space();

        // 绑定名称区域
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        {
            EditorGUILayout.LabelField("绑定设置", EditorStyles.boldLabel);

            // 绑定名称输入框
            if (_bindNameProp != null)
            {
                EditorGUI.BeginChangeCheck();
                string newBindName = EditorGUILayout.TextField(new GUIContent("绑定变量名称", 
                    "在生成的代码中使用的变量名称，用于访问此图片组件"), _bindNameProp.stringValue);
                if (EditorGUI.EndChangeCheck())
                {
                    _bindNameProp.stringValue = newBindName;
                }

                // 显示警告或提示
                if (string.IsNullOrEmpty(_bindNameProp.stringValue))
                {
                    EditorGUILayout.HelpBox("请输入绑定变量名称！", MessageType.Warning);
                }
                else if (_bindNameProp.stringValue.Length > 0 && !char.IsLetter(_bindNameProp.stringValue[0]))
                {
                    EditorGUILayout.HelpBox("变量名称必须以字母开头！", MessageType.Error);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("无法找到bindName属性", MessageType.Error);
            }
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // 图片设置区域
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        {
            EditorGUILayout.LabelField("图片设置", EditorStyles.boldLabel);

            // 默认图片设置
            if (_defaultSpriteProp != null)
            {
                EditorGUILayout.PropertyField(_defaultSpriteProp, new GUIContent("默认图片", 
                    "组件初始化时显示的默认图片"));
            }

            // 保持宽高比设置
            if (_preserveAspectProp != null)
            {
                EditorGUILayout.PropertyField(_preserveAspectProp, new GUIContent("保持宽高比", 
                    "设置图片是否保持原始宽高比"));
            }
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // 组件状态区域
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        {
            EditorGUILayout.LabelField("组件状态", EditorStyles.boldLabel);

            // 显示当前检测到的组件类型
            Image imageComponent = bindImage.GetComponent<Image>();

            // 显示组件状态
            if (imageComponent != null)
            {
                GUI.backgroundColor = successColor;
                EditorGUILayout.HelpBox("已检测到 Image 组件", MessageType.Info);

                // 显示图片信息
                if (imageComponent.sprite != null)
                {
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    EditorGUILayout.LabelField("当前图片信息：", EditorStyles.boldLabel);
                    EditorGUILayout.LabelField($"名称：{imageComponent.sprite.name}");
                    EditorGUILayout.LabelField($"尺寸：{imageComponent.sprite.rect.size}");
                    EditorGUILayout.EndVertical();
                }
            }
            else
            {
                GUI.backgroundColor = warningColor;
                EditorGUILayout.HelpBox("未检测到 Image 组件！将自动添加", MessageType.Warning);
            }
            GUI.backgroundColor = Color.white;
        }
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
}
