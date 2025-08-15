using UnityEngine;
using UnityEngine.Events;

//其实是baseZombies
public class BaseMonster : BaseHumanoid<ZombiesAtk,ZombiesMove>, ILife, IInit
{
    //主脚本负责事件发布和集中注册和注销   
    //攻击事件
    [HideInInspector]
    public UnityEvent<bool> AtkEvent;
    //移动事件
    [HideInInspector]
    public UnityEvent<bool> MoveEvent;
    //重置事件
    [HideInInspector]
    public UnityEvent ResetEvent;

    //击退事件
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

        //死亡事件
        DeadEvent.AddListener(animiComponent.DeadAnimation);
        DeadEvent.AddListener(moveComponet.ZombiesDead);
        DeadEvent.AddListener(atkComponent.ZombiesDead);

        //攻击事件
        AtkEvent.AddListener(animiComponent.AtkState);

        //移动事件
        MoveEvent.AddListener(moveComponet.RunState);

        //重置事件
        ResetEvent.AddListener(StateReset);

        //受伤击退事件
        HurtEvent.AddListener(moveComponet.AgentPassive);
        HurtEvent.AddListener(animiComponent.IsHurt);
    }
    protected override void OnDisable()
    {
        //父类会把所有死亡事件监听都取消掉
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
        //根据不同的伤害有不同的反应,可以在子类中实现
        

        health = Mathf.Clamp(health - damage + damage * def/100 , 0, maxHealth);
        if (health == 0)
        {
            //触发死亡效果
            controller.enabled = false;
            //触发死亡事件
            DeadEvent?.Invoke();
            //若死亡这在这之下的逻辑都不会执行
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
//怪物独有的行为
public interface IMonster
{
    
}