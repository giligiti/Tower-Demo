using UnityEngine;
using UnityEngine.InputSystem;

//inspector窗口引用：枪的id；开火工厂；子弹工厂；开火点
//还要配置输入系统
public class FireController : MonoBehaviour
{
    private Camera cinemachine;

    public Vector3 dir = new Vector3(1,0,0);

    public int gunID = 0;
    [Header("枪口特效工厂")]
    public EffectFactory fireEff;
    [Header("子弹工厂")]
    public BulletFactory bulletFactory; //子弹工厂
    public Transform firePoint;         //开火点
    private Vector2 startPoint;         //屏幕中心点
    private Ray rayX;                   //屏幕射线
    private Vector3 direction;          //缓存子弹的发射方向

    public bool isFire = false;         //是否按下发射键
    public float fireOffset = 0.05f;    //发射间隔
    private float nowTime = 0;          //记录时间
    private void Awake()
    {
        fireOffset = GameDataMgr.Instance.gunInfos[gunID].shootSpeed/60;         //暂时修改 调整射速用
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
    #region 接收开火输入

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
            //计算子弹方向
            rayX = cinemachine.ScreenPointToRay(startPoint);
            Physics.Raycast(rayX, out RaycastHit hitInfo, 100f);

            Vector3 targetPoint = hitInfo.collider != null? hitInfo.point : rayX.GetPoint(100);
            direction = (targetPoint - firePoint.position).normalized;

            GameObject obj = fireEff.Create(firePoint.position,firePoint.transform.forward,0);      //开火特效生成    //要修改
            obj.transform.SetParent(firePoint);
            bulletFactory.Create(gunID, firePoint.transform.position, direction);       //子弹生成
        }
    }
}