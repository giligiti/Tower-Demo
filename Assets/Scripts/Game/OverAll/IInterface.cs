using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// �����ӿ�
/// </summary>
public interface IFactory
{
    GameObject Create(Vector3 position, Quaternion quaternion);
}
/// <summary>
/// ��ʼ���ӿڣ�ÿ����Ҫ�������г�ʼ���Ĳ�Ʒ����Ҫ�ű�����Ҫʵ������ӿ�
/// </summary>
public interface IInit
{
    /// <summary>
    /// ��ʹ�ø���װ�أ�����ʵ�ֽ׶�������ת���ɾ�������
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="info"></param>
    public void Init<T>(T info) where T : InfoData;
}

/// <summary>
/// ���ڵõ�ĳ��ֵ��Ŀǰֻ����OctreeMono�ű��õ�������Χ
/// </summary>
public interface IGetRange
{
    float Range { get; }
}

/// <summary>
/// ������ṩ�����¼��ӿ�
/// </summary>
public interface IDeath
{
    /// <summary>
    /// ���������¼�
    /// </summary>
    /// <param name="action"></param>
    void SubscribeDeathEvent(UnityAction action);
    /// <summary>
    /// ע�������¼�
    /// </summary>
    /// <param name="action"></param>
    void UnsubscribeDeathEvent(UnityAction action);

}
