using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ScenLoadCanvas : MonoBehaviour
{
    Sprite[] backGrounds;
    Image bk;
    Slider loadLine;
    //加载条默认播放时长
    public float DoAnimiTime = 3f;
    public float DoOutFadeTime = 0.5f;
    private Tween tweenAnimi;

    void Awake()
    {
        bk = GetComponentInChildren<Image>();
        loadLine = GetComponentInChildren<Slider>();
        backGrounds = Resources.LoadAll<Sprite>("Image/LoadBk/");
        //随机播放加载的背景
        int i = Random.Range(0, backGrounds.Length);
        bk.sprite = backGrounds[i];
        //激活默认开始加载
        LoadProgress();
        //监听场景加载
        ScenesLoadMgr.Instance.BackEvent.AddListener(LoadProgressFinsh);
    }
    void OnEnable()
    {
    }
    //未收到加载结束前，假装加载
    private void LoadProgress()
    {
        //快速加载到60%
        tweenAnimi = DOTween.To(() => loadLine.value, x => loadLine.value = x, 0.6f, 3f).OnComplete(() =>
        {
            //慢慢加载到80%
            tweenAnimi = DOTween.To(() => loadLine.value, x => loadLine.value = x, 0.8f, 6f);
        });
    }
    //加载结束后，加载完剩下的进度条
    public void LoadProgressFinsh()
    {
        //不管加载到什么时候立马加载完
        if (tweenAnimi != null) DOTween.Kill(tweenAnimi);
        //取消监听
        ScenesLoadMgr.Instance.FrontEvent.RemoveListener(LoadProgressFinsh);
        //加载剩下进度条，加载完成后
        DOTween.To(() => loadLine.value, x => loadLine.value = x, 1, 0.5f).OnComplete(()=>Destroy(this.gameObject));
        
    }
    void OnDisable()
    {
    }
}
