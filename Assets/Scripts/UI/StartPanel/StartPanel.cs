using UnityEngine;

[RequireComponent(typeof(StartPanelView))]
public class StartPanel : BasePanel
{
    [SerializeField] StartPanelView view;

    protected override void Init()
    {
        view.InitView(UIManager.Instance);
        view.SetBackGround(Resources.Load<Sprite>("Image/StartBK"));
    }
}
