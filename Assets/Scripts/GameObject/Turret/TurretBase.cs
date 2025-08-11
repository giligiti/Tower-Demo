using UnityEngine;

public class TurretBase : MonoBehaviour, IInit, IGetFloat
{
    [SerializeField] private ScriptableObject bulletFactory;    //子弹工厂
    [SerializeField] private ScriptableObject effFactory;       //开火特效工厂

    public Transform yawObject;                                 //炮塔旋转台
    public Transform pitchObject;                               //炮口旋转轴
    private float pitchIdentity;                                //炮口默认角度

    //以下这些都要修改，为了调试方便所以序列化
    [SerializeField] private int atk;                               //攻击力
    [SerializeField] private int attackedRange;                     //攻击范围
    private TurretType turretType;                                  //攻击类型
    [SerializeField] private int VerticalSpeed;                     //俯仰速度
    [SerializeField] private int HorizontalSpeed;                   //转弯速度
    [SerializeField] private float verticalAngleUp;                 //仰角
    [SerializeField] private float verticalAngleDown;               //俯角

    public GameObject targetObj;                                //测试的目标
    public float direction;

    public float Range => attackedRange;

    //初始化方法
    public void Init<T>(T info)where T : InfoData
    {
        TurretInfo Info = info as TurretInfo;
        this.atk = Info.atk;
        this.attackedRange = Info.atkRange;
        this.turretType = Info.type;
        this.VerticalSpeed = Info.verticalSpeed;
        this.verticalAngleDown = Info.verticalAngleDown;
        this.verticalAngleUp = Info.verticalAngleUp;
        this.HorizontalSpeed = Info.roundSpeed;

    }

    private void Awake()
    {
        //得到炮口初始默认角度
        Vector3 forwardAngle = new Vector3(pitchObject.forward.x, 0, pitchObject.forward.z);
        pitchIdentity = Vector3.Angle(forwardAngle, pitchObject.forward);
        TurretInfo info = GameDataMgr.Instance.turretInfos[0];                                      //临时测试
        Init<TurretInfo>(info);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
         
    }

    // Update is called once per frame
    void Update()
    {
        DirectionSplit(targetObj.transform.position);
    }

    #region 炮台旋转


    /// <summary>
    /// 方向分解
    /// </summary>
    /// <param name="direction">目标位置</param>
    public void DirectionSplit(Vector3 targetPosition)
    {
        HorizontalTurn(targetPosition, yawObject);

        VerticalTurn(targetPosition, pitchObject);

    }
    /// <summary>
    /// 炮台横向偏转
    /// </summary>
    private void HorizontalTurn(Vector3 targetPostion, Transform turnobj)
    {
        Vector3 direction = targetPostion - turnobj.position;
        direction.y = 0;
        Quaternion quaternion;
        if (direction == Vector3.zero) quaternion = turnobj.rotation;
        else quaternion = Quaternion.LookRotation(direction, turnobj.up);
        if (turnobj.rotation == quaternion) return ;
        turnobj.rotation = Quaternion.RotateTowards(turnobj.rotation, quaternion, HorizontalSpeed * Time.deltaTime);
        return ;
    }

    /// <summary>
    /// 炮口俯仰偏转
    /// </summary>
    /// <param name="q"></param>
    public void VerticalTurn(Vector3 targetPostion, Transform turnobj)
    {
        Vector3 localTarget = targetPostion - turnobj.position;
        // 投影到xz平面
        Vector3 localTargetXZ = new Vector3(localTarget.x, 0, localTarget.z);
        if (localTargetXZ == Vector3.zero) return;
        // 计算需要旋转的垂直角度（相对于局部X轴）Quaternion.Euler(0 , 90, 0) * localTargetXZ
        float verticalAngle = Vector3.SignedAngle(localTargetXZ, localTarget, Quaternion.Euler(0, 90, 0) * localTargetXZ);
        // 限制角度范围
        verticalAngle = Mathf.Clamp(verticalAngle, -verticalAngleUp, verticalAngleDown);
        direction = Mathf.Clamp(direction, verticalAngleDown, verticalAngleUp);
        //仅绕局部X轴创建旋转四元数
        Quaternion targetRotation = Quaternion.Euler(verticalAngle, 0, 0);
        //匀速旋转
        turnobj.localRotation = Quaternion.RotateTowards(turnobj.localRotation, targetRotation, VerticalSpeed * Time.deltaTime);
        // 绘制局部前方向（红色）和目标方向（绿色），基准水平线（蓝色）
        Debug.DrawRay(turnobj.position, turnobj.forward * 10, Color.red);
        Debug.DrawRay(turnobj.position, localTargetXZ, Color.blue);
        Debug.DrawRay(turnobj.position, targetPostion, Color.green);

    }

    #endregion


}
