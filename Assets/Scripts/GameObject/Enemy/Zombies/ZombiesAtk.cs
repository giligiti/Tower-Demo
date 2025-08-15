using UnityEngine;

//近战攻击丧尸脚本
public class ZombiesAtk : Attacked
{
    public GameObject atkAear;              //攻击检测绑定的物体
    private CapsuleCollider atkCollider;    //攻击检测触发器
    private float atkOffsetTime = 0.5f;        //攻击间隔时间
    private float nowTime = 0;

    public BaseMonster monster;


    private void Awake()
    {
        atkCollider = atkAear.AddComponent<CapsuleCollider>();
        atkCollider.isTrigger = true;
        atkCollider.transform.parent = atkAear.transform;
        atkCollider.radius = atkCollider.height = 0.2f;
        atkCollider.excludeLayers = ~(1<<LayerMask.NameToLayer("CanAtk"));
        atkCollider.includeLayers = 1 << LayerMask.GetMask("CanAtk");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //使用攻击间隔来防止反复触发
    private void OnTriggerEnter(Collider other)
    {
        if (Time.time - nowTime < atkOffsetTime)
            return;
        nowTime = Time.time;
        //调用扣血方法
        other.GetComponent<ILife>().HealthDecrease(atk);
    }

    public void ZombiesDead()
    {
        atkCollider.enabled = false;
    }

}
