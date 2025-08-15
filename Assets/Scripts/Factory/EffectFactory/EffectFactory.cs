using UnityEngine;

[CreateAssetMenu(fileName = "EffectFactory", menuName = "Factory/EffectFactory")]
public class EffectFactory : ScriptableObject, IFactory
{
    public int id = 0;
    /// <summary>
    /// �õ�����
    /// </summary>
    /// <param name="position"></param>
    /// <param name="direction">��Ч�ķ���</param>
    /// <param name="effID">���ñ��е�id</param>
    /// <returns></returns>
    public GameObject Create(Vector3 position, Quaternion direction)                    
    {
        EffInfo info = GameDataMgr.Instance.effDatas[id];
        GameObject obj = PoolMgr.Instance.GetObject<EffectsPoolData>(info.name, info.file_path);
        //�õ��Զ����սű�
        IInit autoRecycle = obj.GetComponent<AutoRecycleEffect>();

        //���û��սű��еķ���ȷ�����ͣ������Ǹ��Ŷ��󶯻��ǲ������ͻ���
        obj.transform.position = position;
        obj.transform.rotation = direction;
        autoRecycle.Init<EffInfo>(info);

        return obj;
    }
}
