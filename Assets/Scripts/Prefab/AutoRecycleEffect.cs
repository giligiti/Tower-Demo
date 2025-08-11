using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ��Ч����
/// </summary>
public enum ParticleType
{
    once,           //ֻ����һ��
    duration,       //����ָ��ʱ��
    never,          //һֱ������ʧ
}
/// <summary>
/// �Զ�������Ч����,������ÿ����Ҫ���յ���Ч��
/// </summary>
public class AutoRecycleEffect : MonoBehaviour
{
    

    private ParticleType nowParticleType = ParticleType.once;

    private ParticleSystem[] particleSystems;
    private ParticleSystem mainParticleSystem;  //����Ч
    private string particleName;                //��Ч��
    private bool loop = false;                  //�Ƿ�ѭ��
    private float delayTime;                    //��Ҫ��Ч������ʱ��
    private float releaseDelayTime = 0.1f;      //����ʱ�������ӳٻ��յ�ʱ��

    private UnityEvent startEvent = new UnityEvent();

    /// <summary>
    /// ��Ч��ʼ������ʼ����ŻῪʼ����
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
            //��ʼ����ɺ�ſ�ʼ����
            startEvent.Invoke();
        }
    }
    private void Awake()
    {
        //����Լ��Լ����������ϵ���������ϵͳ
        mainParticleSystem = GetComponent<ParticleSystem>();
        particleSystems = GetComponentsInChildren<ParticleSystem>(false);
    }
    private void OnEnable()
    {
        mainParticleSystem.Stop();//һ��ʼ����ͣ����
        RestartonEnable();        //������
        startEvent.AddListener(AfterInit);//����¼��ȴ���ʼ�����
    }
    private void OnDisable()
    {
        startEvent.RemoveAllListeners();
    }
    private void AfterInit()
    {
        float time;
        mainParticleSystem.Play();//�����ʼ����
        if (nowParticleType == ParticleType.once)
        {
            time = CalculateTotalDuration();
        }
        else
        {
            time = delayTime;
        }

        StartCoroutine(PushParticleBack(time));//����Э��
    }
    IEnumerator PushParticleBack(float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        //yield return new WaitForSeconds(releaseDelayTime);

        PoolMgr.Instance.PushObject<EffectsPoolData>(particleName, this.gameObject);
    }

    // ������������ϵͳ��������ʱ��
    private float CalculateTotalDuration()
    {
        float totalDuration = 0;
        // ������������ϵͳ��������ʱ��
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

    #region �������

    private void RestartonEnable()
    {
        if (mainParticleSystem == null)
        {
            Debug.Log("����δ�ҵ�����ϵͳ");
            return;
        }

        RestartParticleSystem(mainParticleSystem);
        if (particleSystems == null || particleSystems.Length == 0)
            return;
        foreach (ParticleSystem ps in particleSystems)
        {
            //ֹͣ�����������
            RestartParticleSystem(ps);
        }
    }

    private void RestartParticleSystem(ParticleSystem ps)
    {
        
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        ps.Simulate(0,false,false);
        //�����������
        //if (ps.useAutoRandomSeed)
        //{
        //    ps.randomSeed = (uint)Random.Range(1,int.MaxValue);
        //}
    }

    #endregion
}
