using UnityEngine;

[CreateAssetMenu(fileName = "MonsterFactory", menuName = "Factory/MonsterFactory")]
public class MonsterFactory : BaseFactory, IFactory
{
    public GameObject Create(Vector3 position, Quaternion quaternion)
    {
        MonsterInfo info = GameDataMgr.Instance.monsterInfos[infoID];
        GameObject obj = PoolMgr.Instance.GetObject<MonsterPoolData>(info.name);
        IInit b = obj.GetComponent<IInit>();

        obj.transform.position = position;
        obj.transform.rotation = quaternion;
        b.Init(info);

        //父类初始化方法
        return obj;
    }

}


