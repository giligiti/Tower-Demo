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
    //丧尸吼叫动画事件调用
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
        animator.enabled = false;//失活动画组件，启动布娃娃效果
        //死亡动画结束后五秒钟后再执行
        transform.DOLocalMoveY(5f, 5f).SetDelay(5f).OnComplete(() =>
        {
            monster.ResetEvent?.Invoke();//重置状态
        });
        //atkComponent.enabled = false;
        //moveComponet.enabled = false;

        //Dead();
    }

    public void StateReset()
    {
        animator.enabled = true;
        
    }

    //假若死亡不用布娃娃效果才用这个函数
    //死亡动画结束后
    public virtual void DeadAnimationComplete()
    {
        //死亡动画结束后五秒钟后再执行
        transform.DOLocalMoveY(5f, 5f).SetDelay(5f).OnComplete(() =>
        {
            monster.StateReset();//重置状态
        });
    }

    
}
