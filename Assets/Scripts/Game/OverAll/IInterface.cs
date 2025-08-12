using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 工厂接口
/// </summary>
public interface IFactory
{
    GameObject Create(Vector3 position, Quaternion quaternion);
}
/// <summary>
/// 初始化接口，每个需要工厂进行初始化的产品的主要脚本都需要实现这个接口
/// </summary>
public interface IInit
{
    /// <summary>
    /// 先使用父类装载，具体实现阶段在类型转换成具体子类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="info"></param>
    public void Init<T>(T info) where T : InfoData;
}

/// <summary>
/// 用于得到某个值，目前只用于OctreeMono脚本得到攻击范围
/// </summary>
public interface IGetRange
{
    float Range { get; }
}

/// <summary>
/// 给外界提供死亡事件接口
/// </summary>
public interface IDeath
{
    /// <summary>
    /// 订阅死亡事件
    /// </summary>
    /// <param name="action"></param>
    void SubscribeDeathEvent(UnityAction action);
    /// <summary>
    /// 注销死亡事件
    /// </summary>
    /// <param name="action"></param>
    void UnsubscribeDeathEvent(UnityAction action);

}
