using UnityEngine;

/// <summary>
/// �ӵ�����ʵ��������ǹ�ϣ��������õ�ǹе��ʱ�򣬿���Ӧ�õ�������Ŀ��𷽷������𷽷�ʹ�ù���ʵ��
/// </summary>
[CreateAssetMenu(fileName = "BulletFactory", menuName = "Factory/BulletFactory")]
public class BulletFactory : ScriptableObject
{
    //��ʱ
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
