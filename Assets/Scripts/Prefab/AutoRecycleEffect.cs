using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 特效类型
/// </summary>
public enum ParticleType
{
    once,           //只播放一次
    duration,       //播放指定时间
    never,          //一直不会消失
}
/// <summary>
/// 自动回收特效方法,挂载在每个需要回收的特效上
/// </summary>
public class AutoRecycleEffect : MonoBehaviour
{
    

    private ParticleType nowParticleType = ParticleType.once;

    private ParticleSystem[] particleSystems;
    private ParticleSystem mainParticleSystem;  //主特效
    private string particleName;                //特效名
    private bool loop = false;                  //是否循环
    private float delayTime;                    //想要特效持续的时间
    private float releaseDelayTime = 0.1f;      //倒计时结束后延迟回收的时间

    private UnityEvent startEvent = new UnityEvent();

    /// <summary>
    /// 特效初始化，初始化后才会开始播放
    /// </summary>
    /// <param name="delayTime"></param>
    /// <param name="nowtype"></param>
    public void Init(string name, float delayTime = 0, int ty = 0)
    {
        this.particleName = name;
        ParticleType nowtype;
        switch (ty)
        {
            case 0:
                nowtype = ParticleType.once;
                break;
            case 1:
                nowtype = ParticleType.duration;
                break;
            case 2:
                nowtype = ParticleType.never;
                break;
            default:
                nowtype = ParticleType.once;
                break;
        }

        nowtype = delayTime == 0 ? ParticleType.once : nowtype;
        loop = nowtype != ParticleType.once;

        this.delayTime = delayTime;
        this.nowParticleType = nowtype;
        if (nowtype == ParticleType.never)
        {
            mainParticleSystem.Play();
        }
        else
        {
            //初始化完成后才开始播放
            startEvent.Invoke();
        }
    }
    private void Awake()
    {
        //获得自己以及子物体身上的所有粒子系统
        mainParticleSystem = GetComponent<ParticleSystem>();
        particleSystems = GetComponentsInChildren<ParticleSystem>(false);
    }
    private void OnEnable()
    {
        mainParticleSystem.Stop();//一开始先暂停播放
        RestartonEnable();        //先重置
        startEvent.AddListener(AfterInit);//添加事件等待初始化完成
    }
    private void OnDisable()
    {
        startEvent.RemoveAllListeners();
    }
    private void AfterInit()
    {
        float time;
        mainParticleSystem.Play();//激活后开始播放
        if (nowParticleType == ParticleType.once)
        {
            time = CalculateTotalDuration();
        }
        else
        {
            time = delayTime;
        }

        StartCoroutine(PushParticleBack(time));//开启协程
    }
    IEnumerator PushParticleBack(float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        //yield return new WaitForSeconds(releaseDelayTime);

        PoolMgr.Instance.PushObject<EffectsPoolData>(particleName, this.gameObject);
    }

    // 计算所有粒子系统的最大持续时间
    private float CalculateTotalDuration()
    {
        float totalDuration = 0;
        // 计算所有粒子系统的最大持续时间
        if (particleSystems != null && nowParticleType == ParticleType.once)
        {
            foreach (ParticleSystem ps in particleSystems)
            {
                var main = ps.main;
                main.loop = loop;
                float duartion = main.duration + main.startLifetime.constantMax;
                totalDuration = Mathf.Max(totalDuration, duartion);
            }
        }
        return totalDuration;
    }

    #region 重置相关

    private void RestartonEnable()
    {
        if (mainParticleSystem == null)
        {
            Debug.Log("错误，未找到粒子系统");
            return;
        }

        RestartParticleSystem(mainParticleSystem);
        if (particleSystems == null || particleSystems.Length == 0)
            return;
        foreach (ParticleSystem ps in particleSystems)
        {
            //停止并清除子粒子
            RestartParticleSystem(ps);
        }
    }

    private void RestartParticleSystem(ParticleSystem ps)
    {
        
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        ps.Simulate(0,false,false);
        //重置随机种子
        //if (ps.useAutoRandomSeed)
        //{
        //    ps.randomSeed = (uint)Random.Range(1,int.MaxValue);
        //}
    }

    #endregion
}
