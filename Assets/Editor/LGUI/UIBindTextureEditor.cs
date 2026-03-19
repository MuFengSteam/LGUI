using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

[CustomEditor(typeof(UIBindTexture))]
public class UIBindTextureEditor : Editor
{
    private UIBindTexture _target;
    private SerializedProperty _bindNameProp;

    private readonly Color warningColor = new Color(1f, 0.9f, 0.6f);
    private readonly Color successColor = new Color(0.8f, 1f, 0.8f);

    private void OnEnable()
    {
        _target = (UIBindTexture)target;
        // UIBase 中的字段名是 _bindName
        _bindNameProp = serializedObject.FindProperty("_bindName");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Space();

        // 绑定名称区域
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        {
            EditorGUILayout.LabelField("绑定设置", EditorStyles.boldLabel);

            // 检查属性是否存在
            if (_bindNameProp != null)
            {
                // 绑定名称输入框
                EditorGUI.BeginChangeCheck();
                string newBindName = EditorGUILayout.TextField(new GUIContent("绑定变量名称", 
                    "在生成的代码中使用的变量名称，用于访问此原始图片组件"), _bindNameProp.stringValue);
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
            else
            {
                EditorGUILayout.HelpBox("无法找到 bindName 属性", MessageType.Error);
            }
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // 组件状态区域
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        {
            EditorGUILayout.LabelField("组件状态", EditorStyles.boldLabel);

            // 显示当前检测到的组件类型
            RawImage rawImageComponent = _target.GetComponent<RawImage>();

            // 显示组件状态
            if (rawImageComponent != null)
            {
                GUI.backgroundColor = successColor;
                EditorGUILayout.HelpBox("已检测到 RawImage 组件", MessageType.Info);

                // 显示贴图信息
                if (rawImageComponent.texture != null)
                {
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    EditorGUILayout.LabelField("当前贴图信息：", EditorStyles.boldLabel);
                    EditorGUILayout.LabelField($"名称：{rawImageComponent.texture.name}");
                    EditorGUILayout.LabelField($"尺寸：{rawImageComponent.texture.width}x{rawImageComponent.texture.height}");
                    EditorGUILayout.EndVertical();
                }
                
                // 显示 UV Rect 信息
                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.LabelField("UV矩形：", EditorStyles.boldLabel);
                Rect uvRect = rawImageComponent.uvRect;
                EditorGUILayout.LabelField($"X: {uvRect.x:F2}, Y: {uvRect.y:F2}");
                EditorGUILayout.LabelField($"宽: {uvRect.width:F2}, 高: {uvRect.height:F2}");
                EditorGUILayout.EndVertical();
            }
            else
            {
                GUI.backgroundColor = warningColor;
                EditorGUILayout.HelpBox("未检测到 RawImage 组件！将自动添加", MessageType.Warning);
            }
            GUI.backgroundColor = Color.white;
        }
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
} 