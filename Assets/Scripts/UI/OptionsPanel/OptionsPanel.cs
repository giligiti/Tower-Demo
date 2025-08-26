using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class OptionsPanel : BasePanel
{
    [SerializeField] GameObject windonw;
    [SerializeField] Button btnClose;
    [SerializeField] Button btnConfirm;
    [SerializeField] Toggle musicOpen;
    [SerializeField] Toggle soundOpen;
    [SerializeField] Slider musicValue;
    [SerializeField] Slider soundValue;


    private const float currValue = 0.2f;
    private float lastValue = -currValue;
    private Vector3 originScale;

    protected override void Awake()
    {
        base.Awake();
        SyncData();
        windonw.transform.localScale = Vector3.one / 2;
        windonw.transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack).SetUpdate(true);
    }
    protected override void Init()
    {
        originScale = transform.localScale;

        //关闭按钮
        btnClose.onClick.AddListener(() =>
        {
            //缩小并关闭面板
            transform.DOScale(originScale * 0.7f, 0.2f).SetEase(Ease.InSine).OnComplete(() =>
            UIManager.Instance.HidePanel<OptionsPanel>(false));
        });
        //确认按钮
        btnConfirm.onClick.AddListener(() =>
        {
            //确定设置面板时，存储数据
            SoundMgr.Instance.SavaMusicData();
            SoundMgr.Instance.PlayUISound(E_SoundClip.ui_Button);
            //缩小并关闭面板
            transform.DOScale(originScale * 0.7f, 0.2f).SetEase(Ease.InSine).OnComplete(() =>
            UIManager.Instance.HidePanel<OptionsPanel>(false));
        });

        //音乐开关
        musicOpen.onValueChanged.AddListener((bool isopen) =>
        {
            SoundMgr.Instance.MusicMute(!isopen);
        });
        //音效开关
        soundOpen.onValueChanged.AddListener((bool isopen) =>
        {
            SoundMgr.Instance.SoundMute(!isopen);
        });
        //音乐滑动条
        musicValue.onValueChanged.AddListener((float value) =>
        {
            SoundMgr.Instance.SetMusicVolume(value);
            if (Mathf.Abs(value - lastValue) >= currValue)
            {
                //播放对应音量bgm
                Debug.Log("播放对应音量bgm: " + value);
                lastValue = value;
                SoundMgr.Instance.PlayUISound(E_SoundClip.ui_PointButton);
            }
        });
        //音效滑动条
        soundValue.onValueChanged.AddListener((float value) =>
        {
            SoundMgr.Instance.SetSoundVolume(value);
            if (Mathf.Abs(value - lastValue) >= currValue)
            {
                //播放对应音量音效
                Debug.Log("播放对应音量音效: " + value);
                lastValue = value;
                SoundMgr.Instance.PlayUISound(E_SoundClip.ui_PointButton);
            }
        });
    }
    //同步声音数据
    private void SyncData()
    {
        MusicData data = SoundMgr.Instance.MusicData;
        //同步数据
        musicOpen.isOn = !data.musicMute;

        soundOpen.isOn = !data.soundMute;
        musicValue.value = data.musicValue;
        soundValue.value = data.soundValue;
        Debug.Log(musicOpen.isOn + "   " + soundOpen.isOn + "   " + musicValue.value + "   " + soundValue.value);
    }

}
