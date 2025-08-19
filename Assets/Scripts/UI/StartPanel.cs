using UnityEngine;
using UnityEngine.UI;

public class StartPanel : BasePanel
{
    public Button btnStart;     //开始
    public Button btnSetting;   //设置
    public Button btnAbout;     //关于
    public Button btnLeave;     //退出
    UIManager instance = UIManager.Instance;
    protected override void Init()
    {
        //开始游戏
        btnStart.onClick.AddListener(() =>
        {
            instance.HidePanel<StartPanel>();
        });
        //设置
        btnSetting.onClick.AddListener(() =>
        {
            instance.ShowPanel<OptionsPanel>();
            instance.HidePanel<StartPanel>();
        });
        //关于
        btnAbout.onClick.AddListener(() =>
        {
        });
        //退出
        btnLeave.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }
}
