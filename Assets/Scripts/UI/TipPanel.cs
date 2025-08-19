using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TipPanel : BasePanel
{
    [Header("文本内容组件")]
    public TextMeshProUGUI textContent;
    [Header("确认提示组件")]
    public Button ButtonConfirm;
    [Header("关闭提示组件")]
    public Button ButtonClose;
    private UnityAction lastEvent;
    protected override void Init()
    {
        ButtonConfirm.onClick.AddListener(() =>
        {
            OnConfirmExit();
            UIManager.Instance.HidePanel<TipPanel>();
        });
    }

    public void ChangeInfo(string text)
    {
        textContent.text = text;
    }
    //传入想要按下提示面板后要执行的内容
    public void EventInfo(UnityAction y)
    {
        y = lastEvent;
    }
    private void OnConfirmExit()
    {
        lastEvent?.Invoke();
    }
}
