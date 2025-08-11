using UnityEngine;

/// <summary>
/// 子弹工厂实例挂载在枪上，当人物拿到枪械的时候，开火应该调用上面的开火方法，开火方法使用工厂实例
/// </summary>
[CreateAssetMenu(fileName = "BulletFactory", menuName = "Factory/BulletFactory")]
public class BulletFactory : ScriptableObject
{
    //暂时
    public GameObject Create(int gunID, Vector3 position , Vector3 direction)
    {
        GunInfo info = GameDataMgr.Instance.gunInfos[gunID];
        GameObject obj = PoolMgr.Instance.GetObject<BulletPoolData>(info.bulletName);

        return ObjectInit(obj,position, direction, info);
    }
    private GameObject ObjectInit(GameObject obj,Vector3 position, Vector3 direction, GunInfo info)
    {
        BulletProject bullet = obj.GetComponent<BulletProject>();
        obj.transform.position = position;
        bullet.ShootItSelf(info.gunAtk, direction, info.bulletName, info.bulletSpeed); 
        return obj;
    }

}
