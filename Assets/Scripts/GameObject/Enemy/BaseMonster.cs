using UnityEngine;
using UnityEngine.Events;

//��ʵ��baseZombies
public class BaseMonster : BaseHumanoid<ZombiesAtk,ZombiesMove>, ILife, IInit
{
    //���ű������¼������ͼ���ע���ע��   
    //�����¼�
    [HideInInspector]
    public UnityEvent<bool> AtkEvent;
    //�ƶ��¼�
    [HideInInspector]
    public UnityEvent<bool> MoveEvent;
    //�����¼�
    [HideInInspector]
    public UnityEvent ResetEvent;

    //�����¼�
    [HideInInspector]
    public UnityEvent HurtEvent;

    public ZombiesAnimation animiComponent;

    public void Init<T>(T info) where T : InfoData
    {
        MonsterInfo monsterInfo = info as MonsterInfo;
        health = monsterInfo.health;
        def = monsterInfo.def;
        maxHealth = monsterInfo.health;
        atkComponent.atk = monsterInfo.atk;
        atkComponent.atkRange = monsterInfo.atkRange;
        moveComponet.moveSpeed = monsterInfo.moveSpeed;
        moveComponet.runSpeed = monsterInfo.runSpeed;
        moveComponet.roundSpeed = monsterInfo.roundSpeed;
        moveComponet.stopDistance = monsterInfo.stopDistance;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        Player.Instance.InitEvent.AddListener(moveComponet.CorrectRotation);

        //�����¼�
        DeadEvent.AddListener(animiComponent.DeadAnimation);
        DeadEvent.AddListener(moveComponet.ZombiesDead);
        DeadEvent.AddListener(atkComponent.ZombiesDead);

        //�����¼�
        AtkEvent.AddListener(animiComponent.AtkState);

        //�ƶ��¼�
        MoveEvent.AddListener(moveComponet.RunState);

        //�����¼�
        ResetEvent.AddListener(StateReset);

        //���˻����¼�
        HurtEvent.AddListener(moveComponet.AgentPassive);
        HurtEvent.AddListener(animiComponent.IsHurt);
    }
    protected override void OnDisable()
    {
        //���������������¼�������ȡ����
        base.OnDisable();
        AtkEvent.RemoveAllListeners();
        MoveEvent.RemoveAllListeners();
        ResetEvent.RemoveAllListeners();
        HurtEvent.RemoveAllListeners();
        Player.Instance.InitEvent.RemoveListener(moveComponet.CorrectRotation);
    }
    //public 
    public virtual void Dead()
    {
        controller.enabled = false;
    }

    public virtual void HealthDecrease(int damage)
    {
        //���ݲ�ͬ���˺��в�ͬ�ķ�Ӧ,������������ʵ��
        

        health = Mathf.Clamp(health - damage + damage * def/100 , 0, maxHealth);
        if (health == 0)
        {
            //��������Ч��
            controller.enabled = false;
            //���������¼�
            DeadEvent?.Invoke();
            //������������֮�µ��߼�������ִ��
            return;
        }
        if (damage > maxHealth / 20)
        {
            HurtEvent?.Invoke();
        }
    }

    public virtual void HealthIncrease(int value)
    {
        health = Mathf.Clamp(health + value,0,maxHealth);
    }

    public virtual void StateReset()
    {
        controller.enabled = true;
        atkComponent.enabled = true;
        moveComponet.enabled = true;
        atkComponent.StateReset();
        moveComponet.StateReset();
        animiComponent.StateReset();

        PushBackSelf();
    }
    protected virtual void PushBackSelf()
    {
        PoolMgr.Instance.PushObject<GameObject, MonsterPoolData>(this.gameObject);
    }

    
}
//������е���Ϊ
public interface IMonster
{
    
}