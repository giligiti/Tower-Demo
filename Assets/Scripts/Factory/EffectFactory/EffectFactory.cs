using UnityEngine;

[CreateAssetMenu(fileName = "EffectFactory", menuName = "Factory/EffectFactory")]
public class EffectFactory : ScriptableObject, IFactory
{
    public int id = 0;
    /// <summary>
    /// 得到对象
    /// </summary>
    /// <param name="position"></param>
    /// <param name="direction">特效的方向</param>
    /// <param name="effID">配置表中的id</param>
    /// <returns></returns>
    public GameObject Create(Vector3 position, Quaternion direction)                    
    {
        EffInfo info = GameDataMgr.Instance.effDatas[id];
        GameObject obj = PoolMgr.Instance.GetObject<EffectsPoolData>(info.name, info.file_path);
        //得到自动回收脚本
        IInit autoRecycle = obj.GetComponent<AutoRecycleEffect>();

        //调用回收脚本中的方法确定类型，究竟是跟着对象动还是播放完后就回收
        obj.transform.position = position;
        obj.transform.rotation = direction;
        autoRecycle.Init<EffInfo>(info);

        return obj;
    }
}
