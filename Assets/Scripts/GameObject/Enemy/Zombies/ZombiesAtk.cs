using UnityEngine;

//��ս����ɥʬ�ű�
public class ZombiesAtk : Attacked
{
    public GameObject atkAear;              //�������󶨵�����
    private CapsuleCollider atkCollider;    //������ⴥ����
    private float atkOffsetTime = 0.5f;        //�������ʱ��
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

    //ʹ�ù����������ֹ��������
    private void OnTriggerEnter(Collider other)
    {
        if (Time.time - nowTime < atkOffsetTime)
            return;
        nowTime = Time.time;
        //���ÿ�Ѫ����
        other.GetComponent<ILife>().HealthDecrease(atk);
    }

    public void ZombiesDead()
    {
        atkCollider.enabled = false;
    }

}
