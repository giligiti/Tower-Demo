using DG.Tweening;
using UnityEngine;

[RequireComponent (typeof(Animator))]
public class ZombiesAnimation : MonoBehaviour, IHumanoidAnimation
{
    public BaseMonster monster;

    public Animator animator;

    public bool ScreamOver = false;
    

    private void Awake()
    {
        animator.SetBool("isAtkState",true);
    }
    private void Update()
    {

    }
    //ɥʬ��ж����¼�����
    public void StartRun()
    {
        animator.SetBool("isRun", true);
    }
    public void AtkState(bool value)
    {
        animator.SetBool("isAttacked", value);
    }
    public void IsHurt()
    {
        animator.SetTrigger("ifHurt");
    }

    public virtual void DeadAnimation()
    {
        animator.SetBool("isRun", false);
        animator.SetBool("isAttacked", false);
        animator.enabled = false;//ʧ������������������Ч��
        //�������������������Ӻ���ִ��
        transform.DOLocalMoveY(5f, 5f).SetDelay(5f).OnComplete(() =>
        {
            monster.ResetEvent?.Invoke();//����״̬
        });
        //atkComponent.enabled = false;
        //moveComponet.enabled = false;

        //Dead();
    }

    public void StateReset()
    {
        animator.enabled = true;
        
    }

    //�����������ò�����Ч�������������
    //��������������
    public virtual void DeadAnimationComplete()
    {
        //�������������������Ӻ���ִ��
        transform.DOLocalMoveY(5f, 5f).SetDelay(5f).OnComplete(() =>
        {
            monster.StateReset();//����״̬
        });
    }

    
}
