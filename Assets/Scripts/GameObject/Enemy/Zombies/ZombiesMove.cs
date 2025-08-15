using System;
using System.Collections.Generic;
using PathFind;
using UnityEngine;


public class ZombiesMove : MoveBody
{
    //Ѱ·���
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
    //�ڸ���ע���ע�����ȴ�Ŀ���ʼ����ɺ����
    public void CorrectRotation()
    {
        gameObject.transform.rotation = Quaternion.LookRotation(target.position - gameObject.transform.position);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target = Player.Instance.transform;
        lastTargetPostion = target.position;
        //��ʼ���������
        NavAgentInit();
        //ֹͣ�ƶ��͹������ȴ�����˻�𶯻�����MoveState����
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
        //����ת��
        TurnAround();
        //�����ƶ��͹�����Ϊ
        if (Vector3.Distance(transform.position, Player.Instance.transform.position) < stopDistance)
        {
            monster.AtkEvent?.Invoke(true);     //��ʼս��
            monster.MoveEvent?.Invoke(false);   //ֹͣ�ƶ�
        }
        else
        {
            monster.AtkEvent?.Invoke(false);    //ֹͣս��
            if (CanMove)
            {
                monster.MoveEvent?.Invoke(true);//��ʼ�ƶ�
            }
        }
    }
    /// <summary>
    /// ��ʼ���������
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
                    Debug.Log("�����յ�");
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
    //˻�𶯻��¼����ã�˻��֮ǰ�������ڼ䲻����ʼ�ƶ�
    public void MoveState(int value)
    {
        CanMove = Convert.ToBoolean(value);
    }
    //ɥʬת��
    private void TurnAround()
    {
        if(!CanMove) return;//������ʱ����ת��
        if ((currentNode - gameObject.transform.position) == Vector3.zero) return;
        Quaternion q ;
        q = Quaternion.LookRotation(currentNode - gameObject.transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation,q,roundSpeed * Time.deltaTime);
    }
    //�����¼�����ʱ����Ѱ·���
    public void AgentPassive()
    {
        isDead = true;
    }
    //���˶����¼�����
    public void AgentActive()
    {
        isDead= false;
        target = Player.Instance.transform;
    }

    //ɥʬ������ֹͣ�ƶ��߼�
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
