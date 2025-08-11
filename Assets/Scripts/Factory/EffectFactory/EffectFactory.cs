using UnityEngine;

//��ʱ��������Ҫ�޸�
[CreateAssetMenu(fileName = "EffectFactory", menuName = "Factory/EffectFactory")]
public class EffectFactory : ScriptableObject
{
    public int id = 0;
    /// <summary>
    /// �õ�����
    /// </summary>
    /// <param name="position"></param>
    /// <param name="direction">��Ч�ķ���</param>
    /// <param name="effID">���ñ��е�id</param>
    /// <returns></returns>
    public GameObject Create(Vector3 position, Vector3 direction, int effID = -1)                    
    {
        id = effID < 0? id: effID;
        EffInfo info = GameDataMgr.Instance.effDatas[id];
        GameObject obj = PoolMgr.Instance.GetObject<EffectsPoolData>(info.name, info.file_path);
        //�õ��Զ����սű�
        AutoRecycleEffect autoRecycle = obj.GetComponent<AutoRecycleEffect>();

        return ObjectInit(obj,position,direction,autoRecycle,info);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="position"></param>
    /// <param name="direction">������������������</param>
    /// <param name="autoRecycle"></param>
    /// <param name="info"></param>
    /// <returns></returns>
    public GameObject ObjectInit(GameObject obj,Vector3 position, Vector3 direction, AutoRecycleEffect autoRecycle , EffInfo info)
    {
        
        //���û��սű��еķ���ȷ�����ͣ������Ǹ��Ŷ��󶯻��ǲ������ͻ���
        obj.transform.position = position;
        obj.transform.rotation = Quaternion.LookRotation(direction);

        autoRecycle.Init(info.name, info.delayTime, info.particleType);

        return obj;
    }
}
