using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;


public class TipPanel : BasePanel
{
    [Header("提示窗口")]
    [SerializeField] RectTransform window;

    [Header("标题组件")]
    [SerializeField] TextMeshProUGUI TitleContext;

    [Header("文本内容组件")]
    [SerializeField] TextMeshProUGUI textContext;

    [Header("确认提示组件")]
    [SerializeField] Button ButtonConfirm;

    [Header("关闭提示组件")]
    [SerializeField] Button ButtonClose;
    
    private Vector3 originScale;            //原始尺寸

    private UnityAction lastEvent;
    protected override void Init()
    {
        originScale = window.localScale;

        //确认按钮事件
        ButtonConfirm.onClick.AddListener(() =>
        {
            lastEvent?.Invoke();
            window.DOScale(originScale * 0.5f, 0.2f).SetEase(Ease.InSine).OnComplete(() =>
            UIManager.Instance.HidePanel<TipPanel>(false));
        });
        //关闭按钮事件
        ButtonClose.onClick.AddListener(() =>
        {
            //缩小并关闭面板
            window.DOScale(originScale * 0.5f, 0.2f).SetEase(Ease.InSine).OnComplete(()=>
            UIManager.Instance.HidePanel<TipPanel>(false));
            
        });
    }

    /// <summary>
    /// 外界输入
    /// </summary>
    /// <param name="action">传入想要按下提示面板后要执行的内容</param>
    /// <param name="text">想要提示面板显示的文字</param>
    /// <param name="type"></param>
    public void WideInfo(UnityAction action, string text, E_TipPanelType type = E_TipPanelType.tip)
    {
        lastEvent += action;
        textContext.text = text;
        TipPanelChangeType(type);
    }

    private void TipPanelChangeType(E_TipPanelType type)
    {
        switch (type)
        {
            case E_TipPanelType.erro:
                TitleContext.text = "WARNNING";
                break;
            default:
                TitleContext.text = "ATTENTION";
                break;
        }
    }
    private void ChangeTipPanelType()
    {

    }
}

/// <summary>
/// 根据不同的type切换不同的大小
/// </summary>
public enum E_TipPanelType
{
        //错误提示
        erro,
        //退出提示
        exit,
        //小提示
        tip,
        //无标题提示
        nil,
}
