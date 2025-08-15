using UnityEngine;

/// <summary>
/// 子弹工厂实例挂载在枪上，当人物拿到枪械的时候，开火应该调用上面的开火方法，开火方法使用工厂实例
/// </summary>
[CreateAssetMenu(fileName = "BulletFactory", menuName = "Factory/BulletFactory")]
public class BulletFactory : ScriptableObject, IFactory
{
    public int gunID;
    public GameObject Create(Vector3 position, Quaternion quaternion)
    {
        GunInfo info = GameDataMgr.Instance.gunInfos[gunID];

        GameObject obj = PoolMgr.Instance.GetObject<BulletPoolData>(info.bulletName);

        IInit b = obj.GetComponent<BulletProject>();

        obj.transform.position = position;
        obj.transform.rotation = quaternion;

        b.Init(info);

        return obj;
    }
}
