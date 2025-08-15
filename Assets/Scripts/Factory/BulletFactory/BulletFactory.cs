using UnityEngine;

/// <summary>
/// �ӵ�����ʵ��������ǹ�ϣ��������õ�ǹе��ʱ�򣬿���Ӧ�õ�������Ŀ��𷽷������𷽷�ʹ�ù���ʵ��
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
