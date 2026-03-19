// ============================================
// 此文件由LGUI自动生成
// 可以在此文件中添加业务逻辑
// ============================================
using UnityEngine;

public partial class TestPanel : UIBasePanel
{
    private TestPanelBindData _bindData;

    public TestPanelBindData bindData
    {
        get
        {
            if (_bindData == null)
            {
                _bindData = new TestPanelBindData();
                _bindData.Initialize(this);
            }
            return _bindData;
        }
    }

    protected override void OnAwake()
    {
        base.OnAwake();
        // 在这里初始化面板
        bindData.mytext = "123";
        // 事件监听示例（取消注释并修改EventId和回调函数）:
        // EventManager.AddListenerMessage<int>(EventId.None, OnEventCallback);
    }

    private void OnDestroy()
    {
        // 在这里清理资源
        
        // 移除事件监听示例（与OnAwake中的监听对应）:
        // EventManager.RemoveListenerMessage<int>(EventId.None, OnEventCallback);
    }

}
