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
    //�ڸ���ע���ע�����ȴ�Ŀ���ʼ����ɺ����
    public void CorrectRotation()
    {
        target = Player.Instance.transform;
        gameObject.transform.rotation = Quaternion.LookRotation(target.position - gameObject.transform.position);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
        target = Player.Instance.transform;
        agent.SetDestination(target.position);
        //����ת��
        TurnAround();
        //�����ƶ��͹�����Ϊ
        if (Vector3.Distance(this.transform.position, target.position) < stopDistance)
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
    //˻�𶯻��¼����ã�˻��֮ǰ�������ڼ䲻����ʼ�ƶ�
    public void MoveState(int value)
    {
        CanMove = Convert.ToBoolean(value);
    }
    //ɥʬת��
    private void TurnAround()
    {
        if(!CanMove) return;//������ʱ����ת��
        Quaternion q = Quaternion.LookRotation(target.position - gameObject.transform.position );
        transform.rotation = Quaternion.Slerp(transform.rotation,q,roundSpeed * Time.deltaTime);
    }
    //�����¼�����ʱ����Ѱ·���
    public void AgentPassive()
    {
        agent.enabled = false;
        isDead = true;
    }
    //���˶����¼�����
    public void AgentActive()
    {
        agent.enabled = true;
        isDead= false;
        agent.Warp(transform.position);
        target = Player.Instance.transform;
        agent.SetDestination(target.position);
    }

    //ɥʬ������ֹͣ�ƶ��߼�
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
