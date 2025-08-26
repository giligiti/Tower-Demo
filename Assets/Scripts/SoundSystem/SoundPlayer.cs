using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    public AudioSource audioSource;
    //public 
    private SoundMgr soundMgr = SoundMgr.Instance;

    void Awake()
    {
        //注册到声音管理器中
        soundMgr.RegisterAudioSource(audioSource);
        //播放器设置为3d播放
        audioSource.spatialBlend = 1;
    }


    void OnDestroy()
    {
        //在管理器中注销自己
        soundMgr.CleanAudioSource(audioSource);
    }
}