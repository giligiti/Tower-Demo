using UnityEngine;

//暂时，工厂需要修改
[CreateAssetMenu(fileName = "EffectFactory", menuName = "Factory/EffectFactory")]
public class EffectFactory : ScriptableObject
{
    public int id = 0;
    /// <summary>
    /// 得到对象
    /// </summary>
    /// <param name="position"></param>
    /// <param name="direction">特效的方向</param>
    /// <param name="effID">配置表中的id</param>
    /// <returns></returns>
    public GameObject Create(Vector3 position, Vector3 direction, int effID = -1)                    
    {
        id = effID < 0? id: effID;
        EffInfo info = GameDataMgr.Instance.effDatas[id];
        GameObject obj = PoolMgr.Instance.GetObject<EffectsPoolData>(info.name, info.file_path);
        //得到自动回收脚本
        AutoRecycleEffect autoRecycle = obj.GetComponent<AutoRecycleEffect>();

        return ObjectInit(obj,position,direction,autoRecycle,info);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="position"></param>
    /// <param name="direction">方向必须是世界坐标的</param>
    /// <param name="autoRecycle"></param>
    /// <param name="info"></param>
    /// <returns></returns>
    public GameObject ObjectInit(GameObject obj,Vector3 position, Vector3 direction, AutoRecycleEffect autoRecycle , EffInfo info)
    {
        
        //调用回收脚本中的方法确定类型，究竟是跟着对象动还是播放完后就回收
        obj.transform.position = position;
        obj.transform.rotation = Quaternion.LookRotation(direction);

        autoRecycle.Init(info.name, info.delayTime, info.particleType);

        return obj;
    }
}
