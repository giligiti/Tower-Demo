using System;
using System.Collections.Generic;
using PathFind;
using UnityEngine;


public class ZombiesMove : MoveBody
{
    //寻路组件
    public PathFinder pathFinder;
    private Stack<PathNode> path = new Stack<PathNode>();

    private Transform target;
    public BaseMonster monster;
    public ZombiesAnimation anim;
    private bool CanMove = false;
    private bool isDead = false;

    private bool isMoving = true;
    private Vector3 currentNode;

    private Vector3 lastTargetPostion;
    private float offsetPosition = 2f;

    private void OnEnable()
    {
        
    }
    //在父类注册和注销，等待目标初始化完成后调用
    public void CorrectRotation()
    {
        gameObject.transform.rotation = Quaternion.LookRotation(target.position - gameObject.transform.position);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target = Player.Instance.transform;
        lastTargetPostion = target.position;
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
        if (Vector3.Distance(target.position,lastTargetPostion) > offsetPosition)
        {
            lastTargetPostion = target.position;
            NavAgentInit();
        }
        PathMove();
        //agent.SetDestination(target.position);
        //控制转向
        TurnAround();
        //控制移动和攻击行为
        if (Vector3.Distance(transform.position, Player.Instance.transform.position) < stopDistance)
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
    /// <summary>
    /// 初始化导航组件
    /// </summary>
    private void NavAgentInit()
    {
        path = pathFinder.StartPathFinder(transform.position,Player.Instance.transform.position);
        //agent.stoppingDistance = 1;
        //agent.speed = moveSpeed * 0.35f;
        //agent.acceleration = moveSpeed;
        //agent.updateRotation = false;
    }
    private void PathMove()
    {
        if (isMoving)
        {
            if (Vector3.Distance(transform.position, currentNode) < 0.1)
            {
                if (path.Count > 0)
                {
                    currentNode = path.Pop().transform.position;
                }
                else
                {
                    isMoving = false;
                    Debug.Log("到达终点");
                    return;
                }
            }
            //(currentNode - transform.position).normalized * moveSpeed * Time.deltaTime
            transform.Translate((currentNode - transform.position).normalized * 2 * Time.deltaTime,Space.World);
        }
    }
    public void RunState(bool value)
    {
        isMoving = value;
        //agent.isStopped = !value;
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
        if ((currentNode - gameObject.transform.position) == Vector3.zero) return;
        Quaternion q ;
        q = Quaternion.LookRotation(currentNode - gameObject.transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation,q,roundSpeed * Time.deltaTime);
    }
    //受伤事件，临时禁用寻路组件
    public void AgentPassive()
    {
        isDead = true;
    }
    //受伤动画事件调用
    public void AgentActive()
    {
        isDead= false;
        target = Player.Instance.transform;
    }

    //丧尸死亡，停止移动逻辑
    public void ZombiesDead()
    {
        isDead = true;
    }

    public override void StateReset()
    {
        base.StateReset();
        target = null;
    }
}
