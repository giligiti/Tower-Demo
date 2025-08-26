using UnityEngine;
using UnityEngine.UI;

public class PreparePanel : BasePanel
{
    public Button startButton;
    protected override void Init()
    {
        startButton.onClick.AddListener(() =>
        {
            SceneMain.sceneMain.LoadScene<NothingPanel>("Game");
            UIManager.Instance.HidePanel<PreparePanel>();
        });
    }

}
