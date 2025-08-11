using UnityEngine;

//工厂接口
public interface IFactory
{
    GameObject Create(Vector3 position, Quaternion quaternion);
}
//初始化接口，每个需要初始化的产品的主要脚本都需要实现这个接口
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
public interface IGetFloat
{
    float Range{ get; }
}
