using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof(NavMeshAgent))]
public class ZombiesMove : MoveBody
{
    public NavMeshAgent agent;
    private Transform target;
    public BaseMonster monster;
    public ZombiesAnimation anim;
    private bool CanMove = false;
    private bool isDead = false;

    private void OnEnable()
    {
        
    }
    //在父类注册和注销，等待目标初始化完成后调用
    public void CorrectRotation()
    {
        target = Player.Instance.transform;
        gameObject.transform.rotation = Quaternion.LookRotation(target.position - gameObject.transform.position);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //初始化导航组件
        NavAgentInit();
        
        //停止移动和攻击，等待怪物嘶吼动画调用MoveState函数
        monster.MoveEvent?.Invoke(false);
        monster.AtkEvent?.Invoke(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null || isDead )
            return;
        target = Player.Instance.transform;
        agent.SetDestination(target.position);
        //控制转向
        TurnAround();
        //控制移动和攻击行为
        if (Vector3.Distance(this.transform.position, target.position) < stopDistance)
        {
            monster.AtkEvent?.Invoke(true);     //开始战斗
            monster.MoveEvent?.Invoke(false);   //停止移动
        }
        else
        {
            monster.AtkEvent?.Invoke(false);    //停止战斗
            if (CanMove)
            {
                monster.MoveEvent?.Invoke(true);//开始移动
            }
        }
    }
    private void NavAgentInit()
    {
        agent.stoppingDistance = 1;
        agent.speed = moveSpeed * 0.35f;
        agent.acceleration = moveSpeed;
        agent.updateRotation = false;
    }
    public void RunState(bool value)
    {
        agent.isStopped = !value;
    }
    //嘶吼动画事件调用：嘶吼之前、攻击期间不允许开始移动
    public void MoveState(int value)
    {
        CanMove = Convert.ToBoolean(value);
    }
    //丧尸转向
    private void TurnAround()
    {
        if(!CanMove) return;//攻击的时候不能转向
        Quaternion q = Quaternion.LookRotation(target.position - gameObject.transform.position );
        transform.rotation = Quaternion.Slerp(transform.rotation,q,roundSpeed * Time.deltaTime);
    }
    //受伤事件，临时禁用寻路组件
    public void AgentPassive()
    {
        agent.enabled = false;
        isDead = true;
    }
    //受伤动画事件调用
    public void AgentActive()
    {
        agent.enabled = true;
        isDead= false;
        agent.Warp(transform.position);
        target = Player.Instance.transform;
        agent.SetDestination(target.position);
    }

    //丧尸死亡，停止移动逻辑
    public void ZombiesDead()
    {
        isDead = true;
        agent.enabled = false;
    }

    public override void StateReset()
    {
        base.StateReset();
        target = null;
        agent.enabled = true;
    }
}
