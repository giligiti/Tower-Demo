using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundMgr : MonoBehaviour
{
    private static SoundMgr instance;
    public static SoundMgr Instance => instance;
    //音乐播放器
    private AudioSource audioSource;
    //音效播放器
    private AudioSource soundSource;
    //存储在挂载在物体上的音效播放器
    private HashSet<AudioSource> setObjAudio = new HashSet<AudioSource>();
    //存储加载出来的音乐:背景音乐，UI音效等
    private Dictionary<E_SoundType, AudioClip> dicAudio = new Dictionary<E_SoundType, AudioClip>();
    //存储加载出来的音乐的路径
    List<SoundInfo> soundDatas = new List<SoundInfo>();

    public MusicData musicData;

    void Awake()
    {
        instance = this;
        audioSource = this.gameObject.AddComponent<AudioSource>();//BGM
        soundSource = this.gameObject.AddComponent<AudioSource>();//音效
        setObjAudio.Add(soundSource);
        DontDestroyOnLoad(this);

        musicData = JsonMgr.Instance.LoadData<MusicData>("MusicData");
        soundDatas = JsonMgr.Instance.LoadData<List<SoundInfo>>("SoundInfo");

        //通过存储的音效数据进行初始化
        SetSoundVolume(musicData.soundValue);
        SetMusicVolume(musicData.musicValue);
        SoundMute(musicData.soundMute);
        MusicMute(musicData.musicMute);
        //背景音乐开始播放
        BGMChange(E_SoundType.bgm_Normal);
        audioSource.loop = true;
    }
    #region SoundPlayer相关
    /// <summary>
    /// 固定3D音效组件注册到管理器中的方法
    /// </summary>
    /// <param name="audio"></param>
    public void RegisterAudioSource(AudioSource audio)
    {
        //同步当前全局音效状态
        audio.volume = musicData.soundValue;
        audio.mute = !musicData.soundMute;
        setObjAudio.Add(audio);//保存到字典中，用于全局静音时使用

    }
    public void CleanAudioSource(AudioSource audio)
    {
        setObjAudio.Remove(audio);
    }
    /// <summary>
    /// 检查清除，防止空引用，防止物体未正常在ondestory中注销
    /// </summary>
    private void CleanUpADSet()
    {
        List<AudioSource> audioSources = new List<AudioSource>();
        foreach (var ad in setObjAudio)
        {
            if (ad == null || ad.gameObject == null)
            {
                audioSources.Add(ad);
            }
        }
        foreach (var ad in audioSources)
        {
            setObjAudio.Remove(ad);
        }
    }

    #endregion

    #region 加载音乐
    /// <summary>
    /// 加载音乐，防止重复加载
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private AudioClip GetAudio(E_SoundType type)
    {
        //加载对应音效
        AudioClip clip = null;
        if (dicAudio.ContainsKey(type)) clip = dicAudio[type];
        else
        {
            foreach (var data in soundDatas)
            {
                if (data.soundType == type)
                {
                    //AB包加载
                    //clip = ABMgr.Instance.LoadRes<AudioClip>(data.abName, data.path);
                    //Resources加载
                    clip = Resources.Load<AudioClip>(data.path);
                }
            }
        }
        return clip;
    }

    #endregion

    #region 播放声音
    /// <summary>
    /// 播放BGM
    /// </summary>
    /// <param name="type"></param>
    public void BGMChange(E_SoundType type)
    {
        audioSource.Stop();
        AudioClip clip = GetAudio(type);
        audioSource.clip = clip;
        audioSource.volume = musicData.musicValue;
        audioSource.mute = musicData.musicMute;
        audioSource.Play();
    }

    /// <summary>
    /// 全局播放（UI）
    /// </summary>
    /// <param name="type"></param>
    public void PlayUISound(E_SoundType type)
    {
        soundSource.PlayOneShot(GetAudio(type));
    }

    /// <summary>
    ///  固定3D物体的音频资源获取
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public AudioClip GetObjSound(E_SoundType type)
    {
        return GetAudio(type);
    }

    /// <summary>
    /// 物体音效播放
    /// </summary>
    /// <param name="type"></param>
    /// <param name="position"></param>
    public void PlaySoundObj(E_SoundType type, Vector3 position)
    {
        GameObject obj = PoolMgr.Instance.GetObject<SoundPoolData>("soundObj");
        //设置物体
        obj.transform.position = position;

        //进行初始化：音效同步，声音起伏;音效组件的初始化在自身挂载的脚本中,设置为不循环、3d音效播放
        AutoRecycleSound atSound = obj.GetComponent<AutoRecycleSound>();
        atSound.ad.volume = musicData.soundValue;
        atSound.ad.mute = musicData.soundMute;

        //声音起伏
        atSound.DisturbanceVoice();

        //加载对应音效
        AudioClip clip = GetAudio(type);
        //播放
        atSound.ad.PlayOneShot(clip);
    }



    #endregion

    #region 管理全局声音相关

    //调整所有的音效的音量
    public void SetSoundVolume(float volume)
    {
        CleanUpADSet();
        musicData.soundValue = volume;
        foreach (var ad in setObjAudio)
        {
            ad.volume = volume;
        }
    }
    //调整BGM的音量
    public void SetMusicVolume(float volume)
    {
        musicData.musicValue = volume;
        audioSource.volume = volume;
    }

    /// <summary>
    /// 音效静音方法
    /// </summary>
    /// <param name="mute">是否静音</param>
    public void SoundMute(bool mute)
    {
        CleanUpADSet();
        musicData.soundMute = mute;
        foreach (var ad in setObjAudio)
        {
            ad.mute = mute;
        }
    }

    /// <summary>
    /// 背景音乐静音方法
    /// </summary>
    /// <param name="mute"></param>
    public void MusicMute(bool mute)
    {
        musicData.musicMute = mute;
        audioSource.mute = mute;
    }

    #endregion

    /// <summary>
    /// 存储音效数据
    /// </summary>
    public void SavaMusicData()
    {
        JsonMgr.Instance.SaveData(musicData, "MusicData");
    }

}
/// <summary>
/// 表示具体音效资源
/// </summary>
public enum E_SoundType
{
    //BGM
    bgm_Normal,//平常bgm
    bmg_Atk,//战斗bgm

    //UI

    ui_Button,//按键声音
    ui_PointButton,//鼠标点击物体声音

    //3d音效相关

    //邮箱冲锋枪
    gun_SubmachineGunFireSound,//子弹开火声音
    gun_SubmachineGunHitSound,//子弹击中声音
    //加特林
    gun_GatlingFireSound,//开火

}