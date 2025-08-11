using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ZombiesAtk))]
[RequireComponent(typeof(ZombiesMove))]
[RequireComponent(typeof(ZombiesAnimation))]
public class Brute : BaseMonster
{
    public override void HealthDecrease(int damage)
    {
        //先执行父类检测方法
        base.HealthDecrease(damage);
        //实现受击击退

    }
    protected override void PushBackSelf()
    {
        PoolMgr.Instance.PushObject<Brute, MonsterPoolData>(this.gameObject);
    }

}