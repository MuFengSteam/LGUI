using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// LGUI组件的GameObject菜单项
/// 在 GameObject/LGUI/ 下添加各种LGUI组件
/// </summary>
public static class LGUIMenuItems
{
    // 基础优先级，设置在 GameObject/UI (2000+) 之前
    private const int BASE_PRIORITY = -20;

    #region Canvas & Panel

    [MenuItem("GameObject/LGUI/UICanvas", false, BASE_PRIORITY)]
    public static void CreateUICanvas(MenuCommand menuCommand)
    {
        GameObject go = new GameObject("UICanvas");
        
        // 添加Canvas相关组件
        Canvas canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        CanvasScaler scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        
        go.AddComponent<GraphicRaycaster>();
        go.AddComponent<UICanvas>();
        
        PlaceUIElement(go, menuCommand);
    }

    [MenuItem("GameObject/LGUI/UIPanel", false, BASE_PRIORITY + 1)]
    public static void CreateUIPanel(MenuCommand menuCommand)
    {
        GameObject go = CreateUIObject("UIPanel", menuCommand);
        go.AddComponent<UIPanel>();
        
        // 设置默认大小
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(400, 300);
    }

    #endregion

    #region Basic Components

    [MenuItem("GameObject/LGUI/UIImage", false, BASE_PRIORITY + 10)]
    public static void CreateUIImage(MenuCommand menuCommand)
    {
        GameObject go = CreateUIObject("UIImage", menuCommand);
        UIImage image = go.AddComponent<UIImage>();
        image.raycastTarget = false;
        
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(100, 100);
    }

    [MenuItem("GameObject/LGUI/UIRawImage", false, BASE_PRIORITY + 11)]
    public static void CreateUIRawImage(MenuCommand menuCommand)
    {
        GameObject go = CreateUIObject("UIRawImage", menuCommand);
        UIRawImage rawImage = go.AddComponent<UIRawImage>();
        rawImage.raycastTarget = false;
        
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(100, 100);
    }

    [MenuItem("GameObject/LGUI/UIText", false, BASE_PRIORITY + 12)]
    public static void CreateUIText(MenuCommand menuCommand)
    {
        GameObject go = CreateUIObject("UIText", menuCommand);
        UIText text = go.AddComponent<UIText>();
        text.text = "New Text";
        text.fontSize = 24;
        text.color = Color.white;
        text.raycastTarget = false;
        text.alignment = TextAlignmentOptions.Center;
        
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(200, 50);
    }

    #endregion

    #region Interactive Components

    [MenuItem("GameObject/LGUI/UIButton", false, BASE_PRIORITY + 20)]
    public static void CreateUIButton(MenuCommand menuCommand)
    {
        GameObject go = CreateUIObject("UIButton", menuCommand);
        
        // 添加Image作为背景
        UIImage image = go.AddComponent<UIImage>();
        image.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        image.raycastTarget = true;
        
        // 添加Button
        UIButton button = go.AddComponent<UIButton>();
        button.targetGraphic = image;
        
        // 添加文本子物体
        GameObject textGo = new GameObject("Text");
        textGo.transform.SetParent(go.transform, false);
        RectTransform textRect = textGo.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        UIText text = textGo.AddComponent<UIText>();
        text.text = "Button";
        text.fontSize = 24;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Center;
        text.raycastTarget = false;
        
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(160, 40);
    }

    [MenuItem("GameObject/LGUI/UIToggle", false, BASE_PRIORITY + 21)]
    public static void CreateUIToggle(MenuCommand menuCommand)
    {
        GameObject go = CreateUIObject("UIToggle", menuCommand);
        
        // 背景
        GameObject background = new GameObject("Background");
        background.transform.SetParent(go.transform, false);
        RectTransform bgRect = background.AddComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0, 0.5f);
        bgRect.anchorMax = new Vector2(0, 0.5f);
        bgRect.pivot = new Vector2(0, 0.5f);
        bgRect.anchoredPosition = Vector2.zero;
        bgRect.sizeDelta = new Vector2(20, 20);
        Image bgImage = background.AddComponent<Image>();
        bgImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        
        // 勾选标记
        GameObject checkmark = new GameObject("Checkmark");
        checkmark.transform.SetParent(background.transform, false);
        RectTransform checkRect = checkmark.AddComponent<RectTransform>();
        checkRect.anchorMin = Vector2.zero;
        checkRect.anchorMax = Vector2.one;
        checkRect.offsetMin = new Vector2(2, 2);
        checkRect.offsetMax = new Vector2(-2, -2);
        Image checkImage = checkmark.AddComponent<Image>();
        checkImage.color = new Color(0.2f, 0.8f, 0.2f, 1f);
        
        // 标签
        GameObject label = new GameObject("Label");
        label.transform.SetParent(go.transform, false);
        RectTransform labelRect = label.AddComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0, 0);
        labelRect.anchorMax = new Vector2(1, 1);
        labelRect.offsetMin = new Vector2(25, 0);
        labelRect.offsetMax = Vector2.zero;
        UIText labelText = label.AddComponent<UIText>();
        labelText.text = "Toggle";
        labelText.fontSize = 18;
        labelText.color = Color.white;
        labelText.alignment = TextAlignmentOptions.Left;
        labelText.raycastTarget = false;
        
        // Toggle组件
        UIToggle toggle = go.AddComponent<UIToggle>();
        toggle.targetGraphic = bgImage;
        toggle.graphic = checkImage;
        toggle.isOn = true;
        
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(160, 20);
    }

    [MenuItem("GameObject/LGUI/UISlider", false, BASE_PRIORITY + 22)]
    public static void CreateUISlider(MenuCommand menuCommand)
    {
        GameObject go = CreateUIObject("UISlider", menuCommand);
        
        // 背景
        GameObject background = new GameObject("Background");
        background.transform.SetParent(go.transform, false);
        RectTransform bgRect = background.AddComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0, 0.25f);
        bgRect.anchorMax = new Vector2(1, 0.75f);
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        Image bgImage = background.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        
        // Fill区域
        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(go.transform, false);
        RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = new Vector2(0, 0.25f);
        fillAreaRect.anchorMax = new Vector2(1, 0.75f);
        fillAreaRect.offsetMin = new Vector2(5, 0);
        fillAreaRect.offsetMax = new Vector2(-5, 0);
        
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        RectTransform fillRect = fill.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = new Vector2(0.5f, 1);
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = new Color(0.3f, 0.6f, 1f, 1f);
        
        // Handle区域
        GameObject handleArea = new GameObject("Handle Slide Area");
        handleArea.transform.SetParent(go.transform, false);
        RectTransform handleAreaRect = handleArea.AddComponent<RectTransform>();
        handleAreaRect.anchorMin = new Vector2(0, 0);
        handleAreaRect.anchorMax = new Vector2(1, 1);
        handleAreaRect.offsetMin = new Vector2(10, 0);
        handleAreaRect.offsetMax = new Vector2(-10, 0);
        
        GameObject handle = new GameObject("Handle");
        handle.transform.SetParent(handleArea.transform, false);
        RectTransform handleRect = handle.AddComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(20, 0);
        Image handleImage = handle.AddComponent<Image>();
        handleImage.color = Color.white;
        
        // Slider组件
        UISlider slider = go.AddComponent<UISlider>();
        slider.fillRect = fillRect;
        slider.handleRect = handleRect;
        slider.targetGraphic = handleImage;
        slider.value = 0.5f;
        
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(160, 20);
    }

    [MenuItem("GameObject/LGUI/UIScrollbar", false, BASE_PRIORITY + 23)]
    public static void CreateUIScrollbar(MenuCommand menuCommand)
    {
        GameObject go = CreateUIObject("UIScrollbar", menuCommand);
        
        Image bgImage = go.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        
        // Sliding Area
        GameObject slidingArea = new GameObject("Sliding Area");
        slidingArea.transform.SetParent(go.transform, false);
        RectTransform slidingRect = slidingArea.AddComponent<RectTransform>();
        slidingRect.anchorMin = Vector2.zero;
        slidingRect.anchorMax = Vector2.one;
        slidingRect.offsetMin = new Vector2(10, 10);
        slidingRect.offsetMax = new Vector2(-10, -10);
        
        // Handle
        GameObject handle = new GameObject("Handle");
        handle.transform.SetParent(slidingArea.transform, false);
        RectTransform handleRect = handle.AddComponent<RectTransform>();
        handleRect.anchorMin = Vector2.zero;
        handleRect.anchorMax = new Vector2(0.2f, 1);
        handleRect.offsetMin = new Vector2(-10, -10);
        handleRect.offsetMax = new Vector2(10, 10);
        Image handleImage = handle.AddComponent<Image>();
        handleImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        
        UIScrollbar scrollbar = go.AddComponent<UIScrollbar>();
        scrollbar.handleRect = handleRect;
        scrollbar.targetGraphic = handleImage;
        
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(160, 20);
    }

    [MenuItem("GameObject/LGUI/UIDropdown", false, BASE_PRIORITY + 24)]
    public static void CreateUIDropdown(MenuCommand menuCommand)
    {
        GameObject go = CreateUIObject("UIDropdown", menuCommand);
        
        // 背景
        Image bgImage = go.AddComponent<Image>();
        bgImage.color = new Color(0.25f, 0.25f, 0.25f, 1f);
        
        // Label
        GameObject label = new GameObject("Label");
        label.transform.SetParent(go.transform, false);
        RectTransform labelRect = label.AddComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = new Vector2(10, 0);
        labelRect.offsetMax = new Vector2(-25, 0);
        UIText labelText = label.AddComponent<UIText>();
        labelText.text = "Option A";
        labelText.fontSize = 18;
        labelText.color = Color.white;
        labelText.alignment = TextAlignmentOptions.Left;
        labelText.raycastTarget = false;
        
        // Arrow
        GameObject arrow = new GameObject("Arrow");
        arrow.transform.SetParent(go.transform, false);
        RectTransform arrowRect = arrow.AddComponent<RectTransform>();
        arrowRect.anchorMin = new Vector2(1, 0.5f);
        arrowRect.anchorMax = new Vector2(1, 0.5f);
        arrowRect.pivot = new Vector2(1, 0.5f);
        arrowRect.anchoredPosition = new Vector2(-5, 0);
        arrowRect.sizeDelta = new Vector2(15, 15);
        Image arrowImage = arrow.AddComponent<Image>();
        arrowImage.color = Color.white;
        
        // Template
        GameObject template = CreateDropdownTemplate(go);
        template.SetActive(false);
        
        // Dropdown组件
        UIDropdown dropdown = go.AddComponent<UIDropdown>();
        dropdown.targetGraphic = bgImage;
        dropdown.captionText = labelText;
        dropdown.template = template.GetComponent<RectTransform>();
        
        // 添加默认选项
        dropdown.options.Add(new TMP_Dropdown.OptionData("Option A"));
        dropdown.options.Add(new TMP_Dropdown.OptionData("Option B"));
        dropdown.options.Add(new TMP_Dropdown.OptionData("Option C"));
        
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(160, 30);
    }

    [MenuItem("GameObject/LGUI/UIInputField", false, BASE_PRIORITY + 25)]
    public static void CreateUIInputField(MenuCommand menuCommand)
    {
        GameObject go = CreateUIObject("UIInputField", menuCommand);
        
        // 背景
        Image bgImage = go.AddComponent<Image>();
        bgImage.color = new Color(0.15f, 0.15f, 0.15f, 1f);
        
        // Text Area
        GameObject textArea = new GameObject("Text Area");
        textArea.transform.SetParent(go.transform, false);
        RectTransform textAreaRect = textArea.AddComponent<RectTransform>();
        textAreaRect.anchorMin = Vector2.zero;
        textAreaRect.anchorMax = Vector2.one;
        textAreaRect.offsetMin = new Vector2(10, 5);
        textAreaRect.offsetMax = new Vector2(-10, -5);
        textArea.AddComponent<RectMask2D>();
        
        // Placeholder
        GameObject placeholder = new GameObject("Placeholder");
        placeholder.transform.SetParent(textArea.transform, false);
        RectTransform placeholderRect = placeholder.AddComponent<RectTransform>();
        placeholderRect.anchorMin = Vector2.zero;
        placeholderRect.anchorMax = Vector2.one;
        placeholderRect.offsetMin = Vector2.zero;
        placeholderRect.offsetMax = Vector2.zero;
        TextMeshProUGUI placeholderText = placeholder.AddComponent<TextMeshProUGUI>();
        placeholderText.text = "Enter text...";
        placeholderText.fontSize = 18;
        placeholderText.fontStyle = FontStyles.Italic;
        placeholderText.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        placeholderText.alignment = TextAlignmentOptions.Left;
        placeholderText.raycastTarget = false;
        
        // Text
        GameObject text = new GameObject("Text");
        text.transform.SetParent(textArea.transform, false);
        RectTransform textRect = text.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        TextMeshProUGUI inputText = text.AddComponent<TextMeshProUGUI>();
        inputText.text = "";
        inputText.fontSize = 18;
        inputText.color = Color.white;
        inputText.alignment = TextAlignmentOptions.Left;
        inputText.raycastTarget = false;
        
        // InputField组件
        UIInputField inputField = go.AddComponent<UIInputField>();
        inputField.textViewport = textAreaRect;
        inputField.textComponent = inputText;
        inputField.placeholder = placeholderText;
        inputField.targetGraphic = bgImage;
        
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(200, 30);
    }

    #endregion

    #region Layout Components

    [MenuItem("GameObject/LGUI/UIScrollView", false, BASE_PRIORITY + 30)]
    public static void CreateUIScrollView(MenuCommand menuCommand)
    {
        GameObject go = CreateUIObject("UIScrollView", menuCommand);
        
        // 背景
        Image bgImage = go.AddComponent<Image>();
        bgImage.color = new Color(0.1f, 0.1f, 0.1f, 0.5f);
        go.AddComponent<RectMask2D>();
        
        // Viewport
        GameObject viewport = new GameObject("Viewport");
        viewport.transform.SetParent(go.transform, false);
        RectTransform viewportRect = viewport.AddComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.offsetMin = Vector2.zero;
        viewportRect.offsetMax = Vector2.zero;
        viewportRect.pivot = new Vector2(0, 1);
        viewport.AddComponent<Image>().color = Color.clear;
        viewport.AddComponent<Mask>().showMaskGraphic = false;
        
        // Content
        GameObject content = new GameObject("Content");
        content.transform.SetParent(viewport.transform, false);
        RectTransform contentRect = content.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 1);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.pivot = new Vector2(0, 1);
        contentRect.anchoredPosition = Vector2.zero;
        contentRect.sizeDelta = new Vector2(0, 300);
        
        // ScrollRect
        UIScrollView scrollView = go.AddComponent<UIScrollView>();
        scrollView.viewport = viewportRect;
        scrollView.content = contentRect;
        scrollView.horizontal = false;
        scrollView.vertical = true;
        scrollView.movementType = UnityEngine.UI.ScrollRect.MovementType.Elastic;
        
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(300, 200);
    }

    [MenuItem("GameObject/LGUI/UIGrid", false, BASE_PRIORITY + 31)]
    public static void CreateUIGrid(MenuCommand menuCommand)
    {
        GameObject go = CreateUIObject("UIGrid", menuCommand);
        go.AddComponent<UIGrid>();
        
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(400, 300);
    }

    [MenuItem("GameObject/LGUI/UILayout (Vertical)", false, BASE_PRIORITY + 32)]
    public static void CreateUILayoutVertical(MenuCommand menuCommand)
    {
        GameObject go = CreateUIObject("UILayout", menuCommand);
        UILayout layout = go.AddComponent<UILayout>();
        layout.layoutType = UILayout.LayoutType.Vertical;
        
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(200, 300);
    }

    [MenuItem("GameObject/LGUI/UILayout (Horizontal)", false, BASE_PRIORITY + 33)]
    public static void CreateUILayoutHorizontal(MenuCommand menuCommand)
    {
        GameObject go = CreateUIObject("UILayout", menuCommand);
        UILayout layout = go.AddComponent<UILayout>();
        layout.layoutType = UILayout.LayoutType.Horizontal;
        
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(400, 100);
    }

    #endregion

    #region Helper Methods

    private static GameObject CreateUIObject(string name, MenuCommand menuCommand)
    {
        GameObject go = new GameObject(name);
        go.AddComponent<RectTransform>();
        PlaceUIElement(go, menuCommand);
        return go;
    }

    private static void PlaceUIElement(GameObject go, MenuCommand menuCommand)
    {
        // 设置父物体
        GameObject parent = menuCommand.context as GameObject;
        if (parent != null)
        {
            go.transform.SetParent(parent.transform, false);
        }
        else
        {
            // 检查是否在Prefab编辑模式
            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null)
            {
                // 在Prefab编辑模式，添加到Prefab根节点
                GameObject prefabRoot = prefabStage.prefabContentsRoot;
                go.transform.SetParent(prefabRoot.transform, false);
            }
            else
            {
                // 尝试找到场景中的Canvas
                Canvas canvas = Object.FindObjectOfType<Canvas>();
                if (canvas != null)
                {
                    go.transform.SetParent(canvas.transform, false);
                }
            }
        }

        // 确保有RectTransform
        RectTransform rect = go.GetComponent<RectTransform>();
        if (rect == null)
        {
            rect = go.AddComponent<RectTransform>();
        }

        // 居中
        rect.anchoredPosition = Vector2.zero;

        // 注册Undo
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeGameObject = go;
    }

    private static GameObject CreateDropdownTemplate(GameObject parent)
    {
        GameObject template = new GameObject("Template");
        template.transform.SetParent(parent.transform, false);
        RectTransform templateRect = template.AddComponent<RectTransform>();
        templateRect.anchorMin = new Vector2(0, 0);
        templateRect.anchorMax = new Vector2(1, 0);
        templateRect.pivot = new Vector2(0.5f, 1);
        templateRect.anchoredPosition = new Vector2(0, 0);
        templateRect.sizeDelta = new Vector2(0, 150);
        
        Image templateBg = template.AddComponent<Image>();
        templateBg.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        template.AddComponent<UnityEngine.UI.ScrollRect>();
        
        // Viewport
        GameObject viewport = new GameObject("Viewport");
        viewport.transform.SetParent(template.transform, false);
        RectTransform viewportRect = viewport.AddComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.offsetMin = Vector2.zero;
        viewportRect.offsetMax = Vector2.zero;
        viewport.AddComponent<Image>().color = Color.clear;
        viewport.AddComponent<Mask>().showMaskGraphic = false;
        
        // Content
        GameObject content = new GameObject("Content");
        content.transform.SetParent(viewport.transform, false);
        RectTransform contentRect = content.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 1);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.pivot = new Vector2(0.5f, 1);
        contentRect.anchoredPosition = Vector2.zero;
        contentRect.sizeDelta = new Vector2(0, 28);
        
        // Item
        GameObject item = new GameObject("Item");
        item.transform.SetParent(content.transform, false);
        RectTransform itemRect = item.AddComponent<RectTransform>();
        itemRect.anchorMin = new Vector2(0, 0.5f);
        itemRect.anchorMax = new Vector2(1, 0.5f);
        itemRect.sizeDelta = new Vector2(0, 28);
        
        Toggle itemToggle = item.AddComponent<Toggle>();
        
        // Item Background
        GameObject itemBg = new GameObject("Item Background");
        itemBg.transform.SetParent(item.transform, false);
        RectTransform itemBgRect = itemBg.AddComponent<RectTransform>();
        itemBgRect.anchorMin = Vector2.zero;
        itemBgRect.anchorMax = Vector2.one;
        itemBgRect.offsetMin = Vector2.zero;
        itemBgRect.offsetMax = Vector2.zero;
        Image itemBgImage = itemBg.AddComponent<Image>();
        itemBgImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        
        // Item Checkmark
        GameObject checkmark = new GameObject("Item Checkmark");
        checkmark.transform.SetParent(item.transform, false);
        RectTransform checkmarkRect = checkmark.AddComponent<RectTransform>();
        checkmarkRect.anchorMin = new Vector2(0, 0.5f);
        checkmarkRect.anchorMax = new Vector2(0, 0.5f);
        checkmarkRect.pivot = new Vector2(0, 0.5f);
        checkmarkRect.anchoredPosition = new Vector2(5, 0);
        checkmarkRect.sizeDelta = new Vector2(15, 15);
        Image checkmarkImage = checkmark.AddComponent<Image>();
        checkmarkImage.color = new Color(0.2f, 0.8f, 0.2f, 1f);
        
        // Item Label
        GameObject itemLabel = new GameObject("Item Label");
        itemLabel.transform.SetParent(item.transform, false);
        RectTransform itemLabelRect = itemLabel.AddComponent<RectTransform>();
        itemLabelRect.anchorMin = Vector2.zero;
        itemLabelRect.anchorMax = Vector2.one;
        itemLabelRect.offsetMin = new Vector2(25, 0);
        itemLabelRect.offsetMax = new Vector2(-5, 0);
        TextMeshProUGUI itemLabelText = itemLabel.AddComponent<TextMeshProUGUI>();
        itemLabelText.text = "Option";
        itemLabelText.fontSize = 16;
        itemLabelText.color = Color.white;
        itemLabelText.alignment = TextAlignmentOptions.Left;
        itemLabelText.raycastTarget = false;
        
        itemToggle.targetGraphic = itemBgImage;
        itemToggle.graphic = checkmarkImage;
        itemToggle.isOn = true;
        
        // 设置ScrollRect
        UnityEngine.UI.ScrollRect scrollRect = template.GetComponent<UnityEngine.UI.ScrollRect>();
        scrollRect.viewport = viewportRect;
        scrollRect.content = contentRect;
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.movementType = UnityEngine.UI.ScrollRect.MovementType.Clamped;
        
        return template;
    }

    #endregion
}

