using UnityEngine;
using UnityEngine.Events;

public abstract class BaseHumanoid<T, Y> : MonoBehaviour, IDeath where T : Attacked where Y : MoveBody
{
    public int health;
    public int maxHealth;
    public int def;
    public T atkComponent;   //inspector窗口引用
    public Y moveComponet;   //inspector窗口引用
    public GameObject bodyRigRoot;      //身体主体子节点，用来绑定characterController

    [HideInInspector]
    public CharacterController controller;

    public UnityEvent DeadEvent;    //角色死亡事件，方便集中处理人物模块内的脚本的注册和注销

    //初始化在工厂完成

    public virtual void Awake()
    {
        //controller组件
        controller = bodyRigRoot.GetComponent<CharacterController>();

    }
    protected virtual void OnEnable()
    {
        DeadEvent.AddListener(controllerLose);
    }

    protected virtual void OnDisable()
    {
        //移除所有事件监听
        DeadEvent.RemoveAllListeners();
    }
    private void controllerLose()
    {
        controller.enabled = false;
    }

    #region 接口实现内容:IDeath
    /// <summary>
    /// 提供给外界订阅物体死亡事件
    /// </summary>
    public void SubscribeDeathEvent(UnityAction action)
    {
        DeadEvent.AddListener(action);
    }
    public void UnsubscribeDeathEvent(UnityAction action)
    {
        DeadEvent.RemoveListener(action);
    }
    #endregion  

}
//类人生物死亡动画相关
public interface IHumanoidAnimation
{
    //死亡动画方法
    public void DeadAnimation();

    //死亡动画回调方法
    public void DeadAnimationComplete();

    public void StateReset();
}
//生命行为相关
public interface ILife
{
    //受伤方法
    public void HealthDecrease(int damage);

    //回血方法
    public void HealthIncrease(int value);

    //死亡方法
    public void Dead();

    //状态重置方法（用于对象池调用/或者复活）
    public void StateReset();
}
