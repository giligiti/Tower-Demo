using UnityEngine;
using UnityEngine.InputSystem;

//inspector�������ã�ǹ��id�����𹤳����ӵ������������
//��Ҫ��������ϵͳ
public class FireController : MonoBehaviour
{
    private Camera cinemachine;

    public Vector3 dir = new Vector3(1,0,0);

    public int gunID = 0;
    [Header("ǹ����Ч����")]
    public EffectFactory fireEff;
    [Header("�ӵ�����")]
    public BulletFactory bulletFactory; //�ӵ�����
    public Transform firePoint;         //�����
    private Vector2 startPoint;         //��Ļ���ĵ�
    private Ray rayX;                   //��Ļ����
    private Vector3 direction;          //�����ӵ��ķ��䷽��

    public bool isFire = false;         //�Ƿ��·����
    public float fireOffset = 0.05f;    //������
    private float nowTime = 0;          //��¼ʱ��
    private void Awake()
    {
        fireOffset = GameDataMgr.Instance.gunInfos[gunID].shootSpeed/60;         //��ʱ�޸� ����������
        Debug.Log(fireOffset);
        cinemachine = Camera.main;   
        startPoint = new Vector2(Screen.width / 2, Screen.height / 2);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        Fire();
    }
    #region ���տ�������

    public void GetFireInput(InputAction.CallbackContext ctx)
    {
        isFire = ctx.ReadValueAsButton();
    }

    #endregion

    private void Fire()
    {
        if (isFire && Time.time - nowTime > fireOffset)
        {
            nowTime = Time.time;
            //�����ӵ�����
            rayX = cinemachine.ScreenPointToRay(startPoint);
            Physics.Raycast(rayX, out RaycastHit hitInfo, 100f);

            Vector3 targetPoint = hitInfo.collider != null? hitInfo.point : rayX.GetPoint(100);
            direction = (targetPoint - firePoint.position).normalized;

            GameObject obj = fireEff.Create(firePoint.position,firePoint.transform.forward,0);      //������Ч����    //Ҫ�޸�
            obj.transform.SetParent(firePoint);
            bulletFactory.Create(gunID, firePoint.transform.position, direction);       //�ӵ�����
        }
    }
}