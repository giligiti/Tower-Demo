using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class StartPanelView : MonoBehaviour
{
    [SerializeField] private Button btnStart;     //开始
    [SerializeField] private Button btnSetting;   //设置
    [SerializeField] private Button btnAbout;     //关于
    [SerializeField] private Button btnLeave;     //退出
    [SerializeField] private Image backGround;

    public void InitView(UIManager instance)
    {
        //开始游戏
        btnStart.onClick.AddListener(() =>
        {
            SceneMain.sceneMain.LoadScene<PreparePanel>("SelectRole");
            instance.HidePanel<StartPanel>();
        });
        //设置
        btnSetting.onClick.AddListener(() =>
        {
            OptionsPanel panel = instance.ShowPanel<OptionsPanel>();
        });
        //关于
        btnAbout.onClick.AddListener(() =>
        {


        });
        //退出
        btnLeave.onClick.AddListener(() =>
        {
            TipPanel panel = instance.ShowPanel<TipPanel>();
            panel.WideInfo(() => Application.Quit(), "Are you sure you want to quit the game?");
            panel.transform.DOMove(panel.transform.right * 5, 0.4f).SetEase(Ease.OutBack).SetUpdate(true).From();
        });
    }
    public void SetBackGround(Sprite sprite)
    {
        backGround.sprite = sprite;
    }
}
