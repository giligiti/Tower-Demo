using System.Collections;
using UnityEngine;

//这是所有发射类型物体的通用脚本，挂载在预制体上
//要在inspector窗口引用自身的Rigibody，特效
//外部传入的方向需要是归一化后的值
[RequireComponent (typeof(Rigidbody))]
public class BulletProject : MonoBehaviour, IInit
{
    [SerializeField] private EffectFactory factory;     //特效工厂
    private float speed = 50;                           //速度
    private string bulletName;                          //子弹的具体名字
    public int BulletHitEffID = 0;                      //命中特效的id//这应该是定死的
    public float offset = 0.2f;                         //特效创建往碰撞体内部嵌入的深度
    [HideInInspector]
    public int atk;
    public Rigidbody rb;
    private Transform tf;
    private bool haveAtk = false;                       //是否已经造成一次攻击？
    private Coroutine resetCoroutine;                   //回收协程
    private float pushTime = 3;                         //回收时间


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
    //生产请求者通过工厂调用，实现子弹攻击力、攻击方向初始化，执行并发射
    public void ShootItSelf(int atk, Vector3 direction, string bulletName, float bulletSpeed = 0)
    {
        this.atk = atk;
        this.bulletName = bulletName;

        // 规范化方向向量
        direction = direction.normalized;

        // 设置子弹旋转，使子弹前向(forward)与射击方向一致
        tf.rotation = Quaternion.LookRotation(direction);

        if (bulletSpeed != 0)
            this.speed = bulletSpeed;

        // 调试：从子弹当前位置绘制射线，显示射击方向
        Debug.DrawRay(tf.position, direction * 10f, Color.red, 2f);

        // 沿着子弹的前向(forward)施加力
        rb.AddForce(direction * speed, ForceMode.VelocityChange);

        resetCoroutine = StartCoroutine(PushBack()); // 三秒后自动回收子弹
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
        ContactPoint point = collision.GetContact(0);//获取第一个碰撞点

        //向工厂请求子弹命中特效对象
        GameObject eft = factory.Create(point.point - point.normal * offset, point.normal);

        ReclearSelf();
    }

    IEnumerator PushBack()
    {
        yield return new WaitForSeconds(pushTime);
        ReclearSelf();
    }

    #region 状态重置

    private void ReclearSelf()
    {
        //免得回收协程调用
        if (gameObject.activeSelf)
        {
            ResetBullet();
            //回收自己
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
