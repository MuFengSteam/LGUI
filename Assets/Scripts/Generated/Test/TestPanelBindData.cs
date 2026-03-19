// ============================================
// 此文件由LGUI自动生成，请勿手动修改
// ============================================
using UnityEngine;
using System;

[Serializable]
public class TestPanelBindData
{
    private TestPanel _panel;

    public void Initialize(TestPanel panel)
    {
        _panel = panel;
    }

    // UIBindText: UIText
    private string _mytext;
    public string mytext
    {
        get => _mytext;
        set
        {
            _mytext = value;
            _panel?.GetTextByBindName("mytext")?.SetText(value);
        }
    }

}
