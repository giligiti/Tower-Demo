using System.Collections;
using UnityEngine;

//�������з������������ͨ�ýű���������Ԥ������
//Ҫ��inspector�������������Rigibody����Ч
//�ⲿ����ķ�����Ҫ�ǹ�һ�����ֵ
[RequireComponent (typeof(Rigidbody))]
public class BulletProject : MonoBehaviour, IInit,IIgnore
{
    [SerializeField] private EffectFactory factory;     //��Ч����
    private float speed = 50;                           //�ٶ�
    private string bulletName;                          //�ӵ��ľ�������
    public int BulletHitEffID = 0;                      //������Ч��id//��Ӧ���Ƕ�����
    public float offset = 0.2f;                         //��Ч��������ײ���ڲ�Ƕ������

     public GameObject fireOrigin;     //����Դͷ�����ں����ӵ��ͷ����ߵ���ײ

    [HideInInspector] public int atk;
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
    //����������ͨ���������ã�ʵ���ӵ������������������ʼ����ִ�в�����
    public void Init<T>(T info) where T : InfoData
    {
        GunInfo gunInfo = info as GunInfo;
        atk = gunInfo.gunAtk;
        bulletName = gunInfo.bulletName;
        speed = gunInfo.bulletSpeed;

        // �����ӵ���ǰ��(forward)ʩ����
        rb.AddForce(tf.forward * speed, ForceMode.VelocityChange);

        rb.constraints = RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationY |
                         RigidbodyConstraints.FreezeRotationZ;

        resetCoroutine = StartCoroutine(PushBack()); // ������Զ������ӵ�

    }

    /// <summary>
    /// �õ�������
    /// </summary>
    /// <param name="obj"></param>
    public void ToIgnore(GameObject obj)
    {
        fireOrigin = obj;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Transform ctf = collision.transform;
        //����ͬ�㼶����,ͬ�㼶���岻����ײ
        //���Է���Դͷ
        if (fireOrigin != null && (ctf.gameObject == fireOrigin || ctf.IsChildOf(fireOrigin.transform))) return;

        ILife obj = ctf.GetComponentInParent<ILife>();
        if (!haveAtk && obj != null)
        {
            obj.HealthDecrease(atk);
            haveAtk = true;
            Debug.Log(1);
        }
        ContactPoint point = collision.GetContact(0);//��ȡ��һ����ײ��

        //�򹤳������ӵ�������Ч����
        GameObject eft = factory.Create(point.point - point.normal * offset, Quaternion.LookRotation(point.normal));

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
        fireOrigin = null;
    }




    #endregion
}
