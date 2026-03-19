# LGUI
## 作者

- 作者：木枫LL
- GitHub地址：[MuFengSteam](https://github.com/MuFengSteam/LGUI)
- 小红书：[木枫LL](https://www.xiaohongshu.com/user/profile/67c5dc1a000000000a03e5d0)

## 概述

`LGUI` 是一套面向 Unity UGUI 的轻量级绑定框架。核心目标是把面板节点上的绑定组件收集起来，自动生成 `Panel` 与 `BindData` 代码，让业务层可以通过 `bindData.xxx = value` 或 `GetXxxByBindName()` 的方式直接操作 UI。

当前目录下主要包含三部分能力：

- `LGUIRoot`：挂在面板根节点上，负责收集绑定组件并生成代码。
- `UIBasePanel`：面板基类，负责缓存绑定组件、绑定按钮事件、提供按名字访问组件的 API。
- `UIBind*` / `UI*` 组件：提供文本、图片、贴图、按钮、列表、显隐、透明度、旋转、引用等能力。

## 目录结构

```text
Assets/Scripts/LGUI/
├── LGUIRoot.cs
├── UIBase.cs
├── UIBasePanel.cs
├── UIBind*.cs
├── UIReference*.cs
└── LGUIBaseComponent/
    ├── UIButton.cs
    ├── UIImage.cs
    ├── UIRawImage.cs
    ├── UIText.cs
    └── ...
```

## 快速开始

### 1. 创建面板根节点

1. 在场景中创建一个 UI 根节点，通常是 `Canvas`。
2. 给根节点挂上 `LGUIRoot`。
3. `LGUIRoot` 会自动确保节点上存在 `Canvas`、`CanvasScaler`、`GraphicRaycaster`。
4. `uiScriptName` 和 `bindDataClassName` 会根据对象名自动生成。

命名规则：

- 如果节点名本身以 `Panel` 结尾，则 `uiScriptName` 直接使用该名称。
- 否则会自动补成 `xxxPanel`。
- `bindDataClassName` 默认生成为 `xxxPanelBindData`。

### 2. 在子节点上添加绑定组件

把需要被代码驱动的节点挂上对应的 `UIBind*` 组件，并填写 `bindName` 或 `buttonId`。

常用绑定组件如下：

| 组件 | 绑定类型 | 作用 |
| --- | --- | --- |
| `UIBindText` | `string` | 绑定文本，底层使用 `UIText` |
| `UIBindImage` | `int` | 绑定图片入口，底层使用 `UIImage` |
| `UIBindTexture` | `string` | 绑定 `RawImage` / `Texture` |
| `UIBindButton` | `UIBindButton` | 绑定按钮与点击事件 |
| `UIBindToggle` | `bool` | 绑定开关 |
| `UIBindSlider` | `float` | 绑定滑动值 |
| `UIBindBoolToActive` | `bool` | 控制节点显示与隐藏 |
| `UIBindAlpha` | `float` | 控制透明度 |
| `UIBindRotation` | `float` | 控制旋转 |
| `UIBindProgress` | `float` | 控制进度值 |
| `UIBindPosition` | `Vector2` | 控制位置 |
| `UIBindColor` | `Color` | 控制颜色 |
| `UIBindList` | `UIBindList` | 列表容器与项生成 |
| `UIBindInputField` | `UIBindInputField` | 输入框操作 |
| `UIBindEffect` | `UIBindEffect` | 特效播放控制 |
| `UIReference` | `GameObject` | 直接暴露节点引用 |
| `UIReferenceComponent` | 组件类型名 | 暴露指定组件引用 |

### 3. 生成代码

在编辑器里选中挂有 `LGUIRoot` 的节点后执行生成。`LGUIRoot.GenerateCode()` 会把生成结果输出到：

```text
Assets/Scripts/Generated/{目录名}/
```

生成规则：

- `{PanelName}.cs`：只有首次不存在时才生成，后续不会覆盖。
- `{PanelName}BindData.cs`：每次生成都会更新。
- 输出目录名会根据 `Panel` 名称取前缀生成。

## 推荐工作流

1. 创建 UI 预制体或场景节点。
2. 根节点挂 `LGUIRoot`。
3. 子节点挂 `UIBind*` 组件并填写 `bindName`。
4. 生成 `Panel` 与 `BindData` 代码。
5. 在业务脚本中继承 `UIBasePanel`，通过 `bindData` 或 `GetXxxByBindName()` 更新界面。

## 面板脚本示例

```csharp
public partial class LoginPanel : UIBasePanel
{
    protected override void OnShow()
    {
        base.OnShow();

        bindData.playerName = "Lei";
        bindData.volume = 0.8f;
        bindData.isLoading = false;

        GetImageByBindName("icon")?.SetSpriteByPath("UI/Icons/login");
        GetTextureByBindName("avatar")?.SetTextureByPath("Texture/Role/avatar_01");
    }

    public void BtnLogin()
    {
        UnityEngine.Debug.Log("点击登录");
    }
}
```

## `UIBasePanel` 提供的能力

`UIBasePanel` 在初始化时会缓存当前面板下的绑定组件，并提供按名字访问的方法。

常用访问器：

- `GetTextByBindName(string bindName)`
- `GetImageByBindName(string bindName)`
- `GetTextureByBindName(string bindName)`
- `GetBoolToActiveByBindName(string bindName)`
- `GetAllBoolToActiveByBindName(string bindName)`
- `GetToggleByBindName(string bindName)`
- `GetSliderByBindName(string bindName)`
- `GetListByBindName(string bindName)`
- `GetButtonByBindName(string bindName)`
- `GetAlphaByBindName(string bindName)`
- `GetRotationByBindName(string bindName)`
- `GetReferenceByBindName(string bindName)`
- `GetReferenceComponentByBindName(string bindName)`
- `GetEffectByBindName(string bindName)`
- `GetInputFieldByBindName(string bindName)`
- `GetProgressByBindName(string bindName)`

生命周期入口：

- `OnAwake()`
- `OnShow()`
- `OnClose()`

## 按钮绑定规则

`UIBindButton` 通过 `buttonId` 和方法名约定与 `UIBasePanel` 自动关联。

推荐做法：

- 给按钮填写 `bindName`，例如 `BtnLogin`
- 给按钮填写唯一的 `buttonId`
- 生成代码后，在面板类里实现同名方法

基础点击示例：

```csharp
public void BtnLogin()
{
    UnityEngine.Debug.Log("登录");
}
```

带索引参数的按钮：

```csharp
public void BtnItem(int index)
{
    UnityEngine.Debug.Log($"点击项: {index}");
}
```

长按按钮：

- `enableLongPress = true`
- 按下阶段会走按下回调
- 抬起阶段会走 `xxxUp()` 或 `xxxUp(int index)` 方法

示例：

```csharp
public void BtnAttack()
{
    UnityEngine.Debug.Log("开始长按");
}

public void BtnAttackUp()
{
    UnityEngine.Debug.Log("结束长按");
}
```

## 文本绑定

`UIBindText` 底层依赖 `UIText`，推荐统一使用 `UIText`，不要继续使用原生 `UnityEngine.UI.Text`。

示例：

```csharp
bindData.playerName = "Player";

GetTextByBindName("playerName")?.SetText("Ready");
GetTextByBindName("playerName")?.SetAlpha(0.5f);
```

`UIText` 额外支持：

- `SetTextImmediate()`
- 打字机效果 `StartTypeWriter()`
- 透明度控制 `SetAlpha()`
- 可选翻译开关与翻译 ID

说明：

- 如果工程中存在对应的翻译配置类型，`UIText` 会尝试读取翻译内容。
- 如果没有可用的翻译配置，则会回退到原始文本。

## 图片与贴图加载

当前 `LGUI` 目录内的图片、贴图加载逻辑已经统一改为 `Resources.Load` 路径方案。

### `UIImage`

推荐直接使用路径加载：

```csharp
```csharp
bindData.imageId = xx; 这里是直接从Image表读取imageId

或者 GetImageByBindName("icon")?.SetSpriteByPath("UI/Icons/item_sword");
```

路径规则：

- 可以传 `Resources/` 开头的路径。
- 也可以直接传 `Resources` 下的相对路径。
- 传入时可以带扩展名，也可以不带。
- 实际加载时会转成 `Resources.Load<Sprite>()` 使用的相对路径。

关于 `SetSpriteById(int imageConfigId)`：

- 该入口仍然保留在接口上。
- 但当前仓库内没有图片配置表到资源路径的完整映射逻辑。
- 如果项目侧需要继续使用 `int` 映射，请在项目自己的资源映射层补齐，不要直接修改导出的配置数据。

### `UIBindTexture`

`UIBindTexture` 用于 `RawImage` / `Texture` 资源，默认会把简化路径补成：

```text
Resources/Texture/{path}.png
```

示例：

```csharp
bindData.avatar = "Role/avatar_01";

GetTextureByBindName("avatar")?.SetTextureByPath("Texture/Role/avatar_01");
```

也可以直接设置 `Texture`：

```csharp
GetTextureByBindName("avatar")?.SetTexture(myTexture);
```

## 显隐、透明度、旋转、进度

```csharp
bindData.isLoading = true;
bindData.fadeMask = 0.5f;
bindData.arrowAngle = 180f;
bindData.hpProgress = 0.75f;

GetAlphaByBindName("fadeMask")?.FadeIn(0.2f);
GetRotationByBindName("arrow")?.RotateToZ(90f, 0.3f);
GetProgressByBindName("hpProgress")?.SetProgress(35, 100);
```

## 列表

`UIBindList` 支持完整生成模式和循环列表模式，常用入口如下：

- `SetList<T>(List<T> dataList, Action<int, T, UIBindTemplate> bindItemFunc)`
- `SetList(int count, Action<int, UIBindTemplate> bindItemFunc)`
- `RefreshItem(int index)`
- `RefreshAll()`
- `AddItem(Action<UIBindTemplate> bindItemFunc = null)`

示例：

```csharp
var list = GetListByBindName("itemList");
list?.SetList(items, (index, item, template) =>
{
    template.SetText("name", item.Name);
    template.SetImagePath("icon", item.IconPath);
});
```

## 引用类型

### `UIReference`

适合直接暴露节点对象：

```csharp
bindData.rootNode.SetActive(false);
GetReferenceByBindName("rootNode")?.SetPosition(new Vector2(100, 50));
```

### `UIReferenceComponent`

适合暴露特定组件：

```csharp
var button = bindData.confirmButton.GetComponent<UnityEngine.UI.Button>();
var image = bindData.iconImage.GetConfiguredComponent();
```

## 输入框、开关、滑动条、特效

常见调用方式：

```csharp
GetInputFieldByBindName("inputName")?.SetText("Hello");
GetToggleByBindName("musicToggle")?.SetValue(true);
GetSliderByBindName("volumeSlider")?.SetValue(0.6f);
GetEffectByBindName("upgradeFx")?.Play("upgrade");
```

## 使用建议

- 面板根节点统一挂 `LGUIRoot`。
- 文本统一使用 `UIText`，不要混用旧 `Text`。
- `bindName` 尽量与生成后的字段名保持一致，避免后期重构成本。
- 图片和贴图优先使用 `Resources` 相对路径。
- 业务逻辑尽量写在生成出的面板脚本中，不要修改自动生成的 `BindData` 文件。

## 说明

- 本文档只描述当前仓库里 `LGUI` 目录的实际实现。
- 某些外部系统如完整的语言管理、图片配置映射、面板总管理器，如果项目中另外提供，可以在不改导出配置数据的前提下接入。
