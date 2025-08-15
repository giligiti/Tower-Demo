using System.Collections.Generic;
using Octree;
using UnityEngine;

public class TurretBase : MonoBehaviour, IInit, IGetRange
{
    public Transform yawObject;                                     //炮塔旋转台
    public Transform pitchObject;                                   //炮口旋转
    private float pitchIdentity;                                    //炮口默认角度

    public TurretBaseAttack atkBehaviour;                           //攻击脚本，在inspector窗口引用
    public OctreeMonoCheck roomCheck;                               //自身挂载的八叉树空间检测脚本
    [HideInInspector] public float Range => attackedRange;          //接口提供用于空间检测
    private GameObject targetObj;                                   //目标
    private OctreeMono targetMono;                                  //用于获取目标的包围盒
    //为了调试方便所以序列化
    [SerializeField] private int attackedRange;                     //攻击范围
    
    [SerializeField] private int VerticalSpeed;                     //俯仰速度
    [SerializeField] private float verticalAngleUp;                 //仰角
    [SerializeField] private float verticalAngleDown;               //俯角
    [SerializeField] private int horizontalSpeed;                   //转弯速度
    [SerializeField] private int horizontalAngle;                   //横向转弯角度
    private Quaternion Xturn;                                       //目标横向转弯
    private Quaternion Yturn;                                       //目标纵向转弯

    
    private IDeath targetDeath;                                     //目标的死亡事件接口，在LinkedTarget方法中更新，TargetChange方法中使用

    public LayerMask layer;                                         //需要检测的目标的层级，在inspector窗口引用


    //初始化方法
    public void Init<Y>(Y info) where Y : InfoData
    {
        TurretInfo Info = info as TurretInfo;
        atkBehaviour.AtkInit(Info.atk, Info.atkRange, Info.atkSpeed);
        this.attackedRange = Info.atkRange;
        this.VerticalSpeed = Info.verticalSpeed;
        this.verticalAngleDown = Info.verticalAngleDown;
        this.verticalAngleUp = Info.verticalAngleUp;
        this.horizontalSpeed = Info.roundSpeed;
        this.horizontalAngle = Info.horizontalAngle;
    }

    private void Awake()
    {
        //得到炮口初始默认角度
        Vector3 forwardAngle = new Vector3(pitchObject.forward.x, 0, pitchObject.forward.z);
        pitchIdentity = Vector3.Angle(forwardAngle, pitchObject.forward);

        TurretInfo info = GameDataMgr.Instance.turretInfos[0];                                      //临时测试
        Init<TurretInfo>(info);                                                                     //临时测试
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        BehaviourFlow();
    }

    #region 寻敌攻击流程
    private void BehaviourFlow()
    {
        //当前无目标，进入寻敌流程
        if (targetObj == null)
        {
            Debug.Log("当前无目标，进入寻敌流程");

            FindTargetFlow();
        }
        //当前有目标，进入攻击流程
        else
        {
            Debug.Log($"当前有目标:{targetObj.name}进入攻击流程");
            AtkFlow();
        }
    }

    /// <summary>
    /// 寻敌流程
    /// </summary>
    private void FindTargetFlow()
    {
        //尝试获取目标
        if (!TryGetTargetObj(out GameObject obj)) return;
        //改变目标
        TargetChange(obj);
    }

    #region 获取目标
    /// <summary>
    /// 尝试获取目标
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    private bool TryGetTargetObj(out GameObject obj)
    {
        HashSet<GameObject> objects = roomCheck.ProvideTargets();

        //调试
        foreach (var objs in objects)
        {
            Debug.DrawLine(transform.position, objs.transform.position, Color.blue);
        }

        //筛选
        obj = TargetChoose(objects);
        //如果没有得到目标
        if (obj == null) return false;

        return true;
    }

    /// <summary>
    /// 筛选目标
    /// </summary>
    /// <param name="objects">周围的物体的集合</param>
    private GameObject TargetChoose(HashSet<GameObject> objects)
    {
        //声明预选物体的元组
        (GameObject, float) resverPair = (null, float.MaxValue);
        //处理所有物体
        foreach (GameObject obj in objects)
        {
            //根据层级要求筛选
            int layerNum = 1 << obj.layer;
            if ((layer & layerNum) != 0)
            {
                targetMono = obj.GetComponent<OctreeMono>();                                      //修改
                if (!TargetKeep(targetMono.turePosition, out float distance)) continue;
                //选择符合要求的物体中的，距离最近的
                if (distance < resverPair.Item2) resverPair = (obj, distance);
            }
        }
        return resverPair.Item1;
    }


    /// <summary>
    /// 改变目标
    /// </summary>
    /// <param name="obj">新的目标</param>
    /// 炮塔持续检测目标位置，若目标离开攻击范围，则传入空目标
    private void TargetChange(GameObject obj)
    {
        //如果只是因为目标离开了攻击范围
        if (obj == null)
        {
            targetObj = null;
            //主动注销物体的死亡事件
            targetDeath.UnsubscribeDeathEvent(TargetDead);
            Debug.Log("目标离开攻击范围");                                                          //调试
            return;
        }
        //目标死亡后，死亡事件不需要注销，目标死亡自会注销所有事件

        //选中新目标
        targetObj = obj;
        //订阅新目标的死亡事件
        LinkedTarget(obj);
    }

    private void LinkedTarget(GameObject obj)
    {
        targetDeath = obj.GetComponent<IDeath>();
        //订阅目标的死亡事件
        targetDeath.SubscribeDeathEvent(TargetDead);
    }

    /// <summary>
    /// 目标死亡
    /// </summary>
    private void TargetDead()
    {
        //置空目标，自动在update函数中进入寻路逻辑
        targetObj = null;
        Debug.Log("目标死亡");                                                              //调试
    }

    #endregion

    #region 攻击流程
    /// <summary>
    /// 攻击流程
    /// </summary>
    private void AtkFlow()
    {
        //检验目标，并计算出炮塔的旋转，赋值给Xturn和Yturn，用于下面旋转的参数获取，此处distance无用
        if (!TargetKeep(targetMono.turePosition, out float distance))                       //修改
        {
            TargetChange(null);
            return;
        }
        //炮塔转向目标
        DirectionSplit();
        //开始攻击
        Attacked();                                                            
    }

    /// <summary>
    /// 检测目标是否符合条件
    /// </summary>
    /// <returns></returns>
    private bool TargetKeep(Vector3 objPosition, out float distance)
    {
        distance = Vector3.Distance(objPosition, transform.position) * 2;                       //调试，距离
        if (distance > attackedRange)
        {
            Debug.Log("距离太远");

            return false;
        }

        //检测目标位置,如果目标离开攻击范围则调用TargetChange传入空目标,重新开始寻路
        if (!TurretAngleCalculation(objPosition, yawObject, pitchObject)) return false;
        return true;
    }

    protected void Attacked()
    {
        atkBehaviour.StartFire(targetMono);                         //要修改
    }

    #endregion

    #region 炮台旋转
    /// <summary>
    /// 方向分别进行旋转
    /// </summary>
    /// <param name="direction">目标位置</param>
    public void DirectionSplit()
    {
        HorizontalTurn(Xturn, yawObject);

        VerticalTurn(Yturn, pitchObject);
    }
    /// <summary>
    /// 检测偏转角度是否超过界限
    /// </summary>
    /// <param name="targetPostion">目标位置</param>
    /// <param name="Xturnobj">横向偏转物体</param>
    /// <param name="Yturnobj">俯仰偏转物体</param>
    /// <returns></returns>
    private bool TurretAngleCalculation(Vector3 targetPostion, Transform Xturnobj, Transform Yturnobj)
    {
        //判断横向偏转
        Vector3 Xdirection = (targetPostion - Xturnobj.position).normalized;
        Xdirection.y = 0;
        //防止出现异常旋转
        if (Xdirection.sqrMagnitude < 0.001f) return false;
        //如果超过横向界限
        if (Vector3.Angle(transform.forward, Xdirection) > horizontalAngle / 2)
        {
            Debug.Log("横向越界");
            return false;
        }
        

        //判断纵向偏转
        Vector3 localTarget = targetPostion - Yturnobj.position;
        // 投影到xz平面
        Vector3 localTargetXZ = new Vector3(localTarget.x, 0, localTarget.z);
        if (localTargetXZ.sqrMagnitude < 0.001f) return false;
        // 计算需要旋转的垂直角度（相对于局部X轴）Quaternion.Euler(0 , 90, 0) * localTargetXZ
        float verticalAngle = Vector3.SignedAngle(localTargetXZ, localTarget, Quaternion.Euler(0, 90, 0) * localTargetXZ);
        if (-verticalAngleDown < verticalAngle || -verticalAngleUp > verticalAngle)
        {
            Debug.Log("纵向向越界");
            return false;
        }

        //最后,条件都满足的情况下，赋值存储
        Xturn = Quaternion.LookRotation(Xdirection, Vector3.up);
        Yturn = Quaternion.Euler(verticalAngle, 0, 0);
        return true;                                                                               

    }
    /// <summary>
    /// 炮台横向偏转
    /// </summary>
    private void HorizontalTurn(Quaternion quaternion, Transform turnobj)
    {
        turnobj.rotation = Quaternion.RotateTowards(turnobj.rotation, quaternion, horizontalSpeed * Time.deltaTime);
        return;
    }

    /// <summary>
    /// 炮口俯仰偏转
    /// </summary>
    /// <param name="q"></param>
    public void VerticalTurn(Quaternion targetRotation, Transform turnobj)
    {
        #region 抛弃
        // Vector3 localTarget = targetPostion - turnobj.position;
        // // 投影到xz平面
        // Vector3 localTargetXZ = new Vector3(localTarget.x, 0, localTarget.z);
        // if (localTargetXZ == Vector3.zero) return;
        // // 计算需要旋转的垂直角度（相对于局部X轴）Quaternion.Euler(0 , 90, 0) * localTargetXZ
        // float verticalAngle = Vector3.SignedAngle(localTargetXZ, localTarget, Quaternion.Euler(0, 90, 0) * localTargetXZ);
        // // 限制角度范围
        // verticalAngle = Mathf.Clamp(verticalAngle, -verticalAngleUp, verticalAngleDown);
        // //仅绕局部X轴创建旋转四元数
        // Quaternion targetRotation = Quaternion.Euler(verticalAngle, 0, 0);
        //匀速旋转
        #endregion

        turnobj.localRotation = Quaternion.RotateTowards(turnobj.localRotation, targetRotation, VerticalSpeed * Time.deltaTime);


        // 绘制局部前方向（红色）                                                      调试
        Debug.DrawRay(turnobj.position, turnobj.forward * 10, Color.red);
        //目标方向（绿色），基准水平线（蓝色） 
        // Debug.DrawRay(turnobj.position, localTargetXZ, Color.blue);
        // Debug.DrawRay(turnobj.position, targetPostion, Color.green);

    }

    #endregion



    #endregion

}
