using System.Collections;
using UnityEngine;

//�������з������������ͨ�ýű���������Ԥ������
//Ҫ��inspector�������������Rigibody����Ч
//�ⲿ����ķ�����Ҫ�ǹ�һ�����ֵ
[RequireComponent (typeof(Rigidbody))]
public class BulletProject : MonoBehaviour, IInit
{
    [SerializeField] private EffectFactory factory;     //��Ч����
    private float speed = 50;                           //�ٶ�
    private string bulletName;                          //�ӵ��ľ�������
    public int BulletHitEffID = 0;                      //������Ч��id//��Ӧ���Ƕ�����
    public float offset = 0.2f;                         //��Ч��������ײ���ڲ�Ƕ������
    [HideInInspector]
    public int atk;
    public Rigidbody rb;
    private Transform tf;
    private bool haveAtk = false;                       //�Ƿ��Ѿ����һ�ι�����
    private Coroutine resetCoroutine;                   //����Э��
    private float pushTime = 3;                         //����ʱ��


    private void Awake()
    {
        rb.useGravity = false;
        tf = transform;

        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }
    private void OnEnable()
    {
    }
    public void Init<T>(T info) where T : InfoData
    {
        GunInfo gunInfo = info as GunInfo;
        atk = gunInfo.gunAtk;
        bulletName = gunInfo.bulletName;
        speed = gunInfo.bulletSpeed;
    }
    //����������ͨ���������ã�ʵ���ӵ������������������ʼ����ִ�в�����
    public void ShootItSelf(int atk, Vector3 direction, string bulletName, float bulletSpeed = 0)
    {
        this.atk = atk;
        this.bulletName = bulletName;

        // �淶����������
        direction = direction.normalized;

        // �����ӵ���ת��ʹ�ӵ�ǰ��(forward)���������һ��
        tf.rotation = Quaternion.LookRotation(direction);

        if (bulletSpeed != 0)
            this.speed = bulletSpeed;

        // ���ԣ����ӵ���ǰλ�û������ߣ���ʾ�������
        Debug.DrawRay(tf.position, direction * 10f, Color.red, 2f);

        // �����ӵ���ǰ��(forward)ʩ����
        rb.AddForce(direction * speed, ForceMode.VelocityChange);

        resetCoroutine = StartCoroutine(PushBack()); // ������Զ������ӵ�
    }
    private void OnCollisionEnter(Collision collision)
    {
        IHumanoidBehaviour obj = collision.transform.GetComponentInParent<IHumanoidBehaviour>();
        if (!haveAtk && obj != null )
        {
            obj.HealthDecrease(atk);
            haveAtk = true;
            Debug.Log(1);
        }
        Debug.Log(collision.gameObject.name);
        ContactPoint point = collision.GetContact(0);//��ȡ��һ����ײ��

        //�򹤳������ӵ�������Ч����
        GameObject eft = factory.Create(point.point - point.normal * offset, point.normal);

        ReclearSelf();
    }

    IEnumerator PushBack()
    {
        yield return new WaitForSeconds(pushTime);
        ReclearSelf();
    }

    #region ״̬����

    private void ReclearSelf()
    {
        //��û���Э�̵���
        if (gameObject.activeSelf)
        {
            ResetBullet();
            //�����Լ�
            PoolMgr.Instance.PushObject<BulletPoolData>(bulletName, this.gameObject);
        }
    }
    private void ResetBullet()
    {
        StopCoroutine(resetCoroutine);
        haveAtk = false;
        atk = 0;
        gameObject.transform.position = Vector3.zero;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        gameObject.transform.rotation = Quaternion.identity;
    }

    

    #endregion
}
