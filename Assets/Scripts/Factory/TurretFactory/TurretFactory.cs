using UnityEngine;


public class TurretFactory : ScriptableObject, IFactory
{
    [SerializeField] private int turretID;
    //�Ժ�Ҫ�޸ģ�Ӧ�ô���������Ϣ�����泯����,
    public GameObject Create(Vector3 position, Quaternion quaternion)
    {
        TurretInfo info = GameDataMgr.Instance.turretInfos[turretID];
        //ʵ����
        GameObject obj = PoolMgr.Instance.GetObject<TurretPoolData>(info.name,info.prefabPath);
        IInit b = obj.GetComponent<IInit>();
        //��ʼ��
        
        obj.transform.position = position;
        obj.transform.rotation = quaternion;
        b.Init(info);

        return obj;

    }



}
