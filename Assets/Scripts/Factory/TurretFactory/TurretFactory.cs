using UnityEngine;


public class TurretFactory : ScriptableObject, IFactory
{
    [SerializeField] private int turretID;
    //以后要修改，应该传入网格信息，和面朝方向,
    public GameObject Create(Vector3 position, Quaternion quaternion)
    {
        TurretInfo info = GameDataMgr.Instance.turretInfos[turretID];
        //实例化
        GameObject obj = PoolMgr.Instance.GetObject<TurretPoolData>(info.name,info.prefabPath);
        IInit b = obj.GetComponent<IInit>();
        //初始化
        
        obj.transform.position = position;
        obj.transform.rotation = quaternion;
        b.Init(info);

        return obj;

    }



}
