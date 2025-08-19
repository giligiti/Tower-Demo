using UnityEngine;
using UnityEngine.UI;

public class OptionsPanel : BasePanel
{
    public Button btnClose;
    public Toggle musicOpen;
    public Toggle soundOpen;
    public Slider musicValue;
    public Slider soundValue;

    private const float currValue = 0.1f;
    private float lastValue = -currValue;

    protected override void Init()
    {
        MusicData data = SoundMgr.Instance.musicData;
        //同步数据
        musicOpen.isOn = data.musicMute;
        soundOpen.isOn = data.soundMute;
        musicValue.value = data.musicValue;
        soundValue.value = data.soundValue;


        btnClose.onClick.AddListener(() =>
        {
            //关闭设置面板时，存储数据
            SoundMgr.Instance.SavaMusicData();
            SoundMgr.Instance.PlayUISound(E_SoundType.ui_Button);

            UIManager.Instance.HidePanel<OptionsPanel>();
        });

        musicOpen.onValueChanged.AddListener((bool isopen) =>
        {
            SoundMgr.Instance.MusicMute(!isopen);
        });
        soundOpen.onValueChanged.AddListener((bool isopen) =>
        {
            SoundMgr.Instance.SoundMute(!isopen);
        });
        musicValue.onValueChanged.AddListener((float value) =>
        {
            SoundMgr.Instance.SetMusicVolume(value);
            if ((value - lastValue) >= currValue)
            {
                //播放对应音量bgm
                Debug.Log("播放对应音量bgm: " + value);
                SoundMgr.Instance.PlayUISound(E_SoundType.ui_PointButton);
            }
        });
        soundValue.onValueChanged.AddListener((float value) =>
        {
            SoundMgr.Instance.SetSoundVolume(value);
            if (Mathf.Abs(value - lastValue) >= currValue)
            {
                //播放对应音量音效
                Debug.Log("播放对应音量音效: " + value);
                SoundMgr.Instance.PlayUISound(E_SoundType.ui_PointButton);
            }
        });
    }
    
}
