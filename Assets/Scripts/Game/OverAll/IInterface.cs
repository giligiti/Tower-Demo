using UnityEngine;

//�����ӿ�
public interface IFactory
{
    GameObject Create(Vector3 position, Quaternion quaternion);
}
//��ʼ���ӿڣ�ÿ����Ҫ��ʼ���Ĳ�Ʒ����Ҫ�ű�����Ҫʵ������ӿ�
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
public interface IGetFloat
{
    float Range{ get; }
}
