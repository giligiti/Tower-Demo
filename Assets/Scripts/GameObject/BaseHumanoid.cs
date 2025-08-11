using UnityEngine;
using UnityEngine.Events;

public abstract class BaseHumanoid<T,Y> : MonoBehaviour where T : Attacked where Y : MoveBody
{
    public int health;
    public int maxHealth;
    public int def;
    public T atkComponent;   //inspector��������
    public Y moveComponet;   //inspector��������
    public GameObject bodyRigRoot;      //���������ӽڵ㣬������characterController

    [HideInInspector]
    public CharacterController controller;

    public UnityEvent DeadEvent;    //��ɫ�����¼������㼯�д�������ģ���ڵĽű���ע���ע��
    
    //��ʼ���ڹ������

    public virtual void Awake()
    {
        //controller���
        controller =  bodyRigRoot.GetComponent<CharacterController>();

    }
    protected virtual void OnEnable()
    {
        DeadEvent.AddListener(controllerLose);
    }

    protected virtual void OnDisable()
    {
        //�Ƴ������¼�����
        DeadEvent.RemoveAllListeners();
    }
    private void controllerLose()
    {
        controller.enabled = false;
    }

}
//�������������������
public interface IHumanoidAnimation
{
    //������������
    public void DeadAnimation();

    //���������ص�����
    public void DeadAnimationComplete();

    public void StateReset();
}
//��������������Ϊ���
public interface IHumanoidBehaviour
{
    //���˷���
    public void HealthDecrease(int damage);

    //��Ѫ����
    public void HealthIncrease(int value);

    //��������
    public void Dead();

    //״̬���÷��������ڶ���ص���/���߸��
    public void StateReset();
}