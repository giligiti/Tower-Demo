using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonController : MonoBehaviour
{
    Transform tf;
    Animator animator;
    CharacterController controller;
    Transform cameraPosition;
    [Header("平常摄像机")]
    [SerializeField] private CinemachineCamera normalCamera;
    [Header("瞄准摄像机")]
    [SerializeField] private CinemachineCamera aimCamera;
    [Header("放大摄像机")]
    [SerializeField] private CinemachineCamera largCamera;
    private CinemachineCamera nowCamera;                    //表示当前相机，用于镜头变换的时候紧盯原来镜头的目标
    private Vector2 screenPoint;                            //表示当前屏幕中心点

    [Header("右手持枪位置")]
    public Transform rightHandPostion;
    [Header("左手持枪位置")]
    public Transform leftHandPostion;
    [Header("枪")]
    public Transform Gun;
    [Header("枪的位置")]
    public Transform shootPosition;
    private Transform GunPosition;

    //重力相关
    private readonly float Gravity = -9.8f;  //重力
    private float verticalVelicity; //玩家当前垂直方向速度
    
    
    //方向输入
    private Vector2 moveInput;
    private Vector2 AimInput;

    //玩家瞄准状态
    public enum ArmState
    {
        Normal,
        Equip,
        Aim,
    }
    public ArmState armState = ArmState.Normal;

    [Header("瞄准跟随速度")]
    [SerializeField] private float aimTurnSpeed = 50;
    //玩家基本状态(蹲站跳)
    [HideInInspector]
    public enum PlayerState
    {
        Crouch,
        Stand,
        Air,
    }
    public PlayerState nowState = PlayerState.Stand;
    //玩家行走状态
    [HideInInspector]
    public enum LocomotionState
    {
        Idle,
        Walk,
        Run,
        Sprint,
    }
    public LocomotionState locomotionState = LocomotionState.Idle;
    

    //用来记录切换的值
    private float crouchThreshold = 0f;
    private float standThreshold = 1f;
    private float airThreshold = 2.2f;
    //行走奔跑速度                    //这里的值取决于对应动画在混合树中的Y轴的值
    private float crouchSpeed = 2.5f;       
    private float walkSpeed = 1.5f;
    private float runSpeed = 2.6f;
    private float SprintSpeed = 4.6f;
    //跳跃速度
    private float jumpVelocity = 6.5f;                                                //以后要修改，暂定
    #region 状态和动画参数

    //接收玩家状态
    private bool isAim;             //瞄准
    private bool isCrouch;          //蹲下
    private bool isWalk;            //行走
    private bool isRun;             //小跑
    private bool isSprint;          //冲刺
    private bool isJump;            //跳跃
    private bool isEquip = false;   //是否装备
    private bool currentState;      //当前状态

    private int verticalSpeedHash;              //动画参数：VerticalSpeed    表示垂直前方速度
    private int turnSpeedHash;                  //动画参数：TurnSpeed        表述转弯速度
    private int HorizontalSpeedHash;            //动画参数：HorizontalSpeed  表示水平左右速度
    private int blendHash;                      //动画参数：Blend            表示蹲站跳的决定值
    private int jumpSpeedHash;                  //动画参数：JumpSpeed        表示垂直方向速度
    private int aimStateHash;
    private int equipHash;

    #endregion

    private Vector3 playerMovement;

    private static readonly int CACHE_SIZE = 3;
    private Vector3[] CachePool = new Vector3[CACHE_SIZE];  //用来存储三帧的移动速度
    private int OldestIndex = 0;                            //表示CachePool的索引

    private Vector3 averageSpeed;                           //表示三帧的平均速度

    private float fillMulti = 1.5f;                         //表示下落的时候的速度是上升时候的多少倍
    private float nowTime = 0;
    private readonly float jumpColdTime = 0.8f;
    

    private void Start()
    {
        tf = transform;//通过这个字段来访问transfrom可以提高效率
        cameraPosition = Camera.main.transform;
        aimCamera.enabled = false;

        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();

        #region 得到动画参数的哈希值

        verticalSpeedHash = Animator.StringToHash("VerticalSpeed");
        HorizontalSpeedHash = Animator.StringToHash("HorizontalSpeed");
        turnSpeedHash = Animator.StringToHash("TurnSpeed");
        blendHash = Animator.StringToHash("Blend");
        jumpSpeedHash = Animator.StringToHash("JumpSpeed");
        aimStateHash = Animator.StringToHash("isAim");
        equipHash = Animator.StringToHash("isEquip");
        #endregion

        //鼠标
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        screenPoint = new Vector2(Screen.width/2, Screen.height/2);

        GunPosition = Gun;
    }
    /// <summary>
    /// 动画IK调用函数
    /// </summary>
    /// <param name="layerIndex"></param>
    private void OnAnimatorIK(int layerIndex)
    {
        float a;
        if (isEquip || isAim)
        {
            a = 1;
            Gun.gameObject.SetActive(true);
            animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandPostion.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandPostion.rotation);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandPostion.position);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandPostion.rotation);
        }
        else
        {
            a = 0;
            Gun.gameObject.SetActive(false);
        }
        

        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, a);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, a);
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, a);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, a);

        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0.1f);
        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0.1f);
    }
    #region 输入接收

    public void GetMoveInput(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }
    public void GetAimPosition(InputAction.CallbackContext ctx)
    {
        AimInput = ctx.ReadValue<Vector2>();
    }
    public void GetSprintInput(InputAction.CallbackContext ctx)
    {
        isSprint = ctx.ReadValueAsButton();
    }
    public void GetCrouchInput(InputAction.CallbackContext ctx)
    {
        isCrouch = ctx.ReadValueAsButton();
    }
    public void GetAimInput(InputAction.CallbackContext ctx)
    {
        isAim = ctx.ReadValueAsButton();
        //摄像机变换
        largCamera.enabled = isAim;
    }
    public void GetWalkInput(InputAction.CallbackContext ctx)
    {
        isWalk = ctx.ReadValueAsButton();
    }
    public void GetJumpInput(InputAction.CallbackContext ctx)
    {
        if (Time.time - nowTime >= jumpColdTime)
        {
            isJump = true;
            nowTime = Time.time;
        }

    }

    public void GetEquirpInput(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            isEquip = !isEquip;
        }
    }

    #endregion

    private void Update()
    {
        CaculateInputDirection();
        ChangePlayerState();
        SetupAnimator();
        //跳跃相关
        Jump();
        OnGravity();
        CaculaterGravity();
        //摄像机
        //CameraTargetStay();

    }
    
    #region 跳跃相关

    
    private void Jump()
    {
        if (controller.isGrounded && isJump)
        {
            verticalVelicity = jumpVelocity;
            nowState = PlayerState.Air;
            isJump = false;
        }
    }
    //计算重力                                                              //以后要修改，暂定
    private void CaculaterGravity()
    {
        if (controller.isGrounded)
        {
            verticalVelicity = Gravity * Time.deltaTime;
            if (nowState == PlayerState.Air)
                nowState = PlayerState.Stand;
        }
        else
        {
            // 应用下落加速度倍率
            if (verticalVelicity < 0)
            {
                verticalVelicity += Gravity * fillMulti * Time.deltaTime;
            }
            else
            {
                verticalVelicity += Gravity * Time.deltaTime;
            }

            // 限制最大下落速度
            if (verticalVelicity < -20f) // 可调整的最大下落速度
            {
                verticalVelicity = -20f;
            }
        }
    }
    //应用重力
    private void OnGravity()
    {
        Vector3 gravityForce = Vector3.zero;
        if (nowState != PlayerState.Air)
        {
            gravityForce = animator.deltaPosition;
            gravityForce.y = verticalVelicity * Time.deltaTime;
            controller.Move(gravityForce);
            averageSpeed = AverageVel(animator.velocity);
        }
        else
        {
            averageSpeed.y = verticalVelicity;
            Vector3 playerDeltaMovement = averageSpeed * Time.deltaTime;
            controller.Move(playerDeltaMovement);
        }
        

    }

    //计算平均速度
    private Vector3 AverageVel(Vector3 velocity)
    {
        CachePool[OldestIndex] = velocity;
        OldestIndex++;
        OldestIndex %= CACHE_SIZE;
        Vector3  average  = Vector3.zero;
        foreach (var cache in CachePool)
        {
            average += cache;
        }
        return average / CACHE_SIZE;
    }
    #endregion

    //判断玩家状态变换
    private void ChangePlayerState()
    {
        
        //判断是否是蹲跳站
        if (isCrouch)
        {
            nowState = PlayerState.Crouch;
        }
        //判断跳跃
        else if (!controller.isGrounded)
        {
            nowState = PlayerState.Air;
        }
        else
        {
            nowState = PlayerState.Stand;
        }
        //判断站冲跑走
        if (Mathf.Approximately(moveInput.sqrMagnitude,0))
        {
            locomotionState = LocomotionState.Idle;
        }
        else if (isSprint)
        {
            locomotionState = LocomotionState.Sprint;
        }
        else if (isWalk)
        {
            locomotionState = LocomotionState.Walk;
        }
        else
        {
            locomotionState = LocomotionState.Run;
        }

        //判断瞄准状态
        if (isAim && isEquip)
        {
            armState = ArmState.Aim;
            aimCamera.enabled = isAim;
        }
        else if(isEquip)
        {
            armState = ArmState.Equip;
            aimCamera.enabled = false;
        }      
        else
        {
            armState = ArmState.Normal;
            aimCamera.enabled = false;
        }

    }

    //计算相对于摄像机视角的输入方向和当前人物方向的变换
    private void CaculateInputDirection()
    {
        //摄像机在地面平面上的投影的单位向量（世界坐标系）
        Vector3 camForward = new Vector3(cameraPosition.forward.x,0,cameraPosition.forward.z).normalized;
        playerMovement = camForward * moveInput.y + cameraPosition.right * moveInput.x;
        playerMovement = tf.InverseTransformVector(playerMovement);
    }
    /// <summary>
    /// 设置摄像机变换时能紧盯原来目标
    /// </summary>
    /// <param name="targetCamera">要转变成的相机</param>
    private void CameraTargetStay()
    {
        CinemachineCamera targetCamera;
        if (aimCamera.enabled)
            targetCamera = aimCamera;
        else if (largCamera.enabled)
            targetCamera = largCamera;
        else
            targetCamera = normalCamera;
        if (nowCamera == targetCamera || targetCamera == null)
            return;
        Ray cameraRay = Camera.main.ScreenPointToRay(screenPoint);
        Physics.Raycast(cameraRay,out RaycastHit hitInfo,999f);
        Vector3 direction = cameraPosition.position - hitInfo.point;
        targetCamera.transform.rotation = Quaternion.LookRotation(direction);
        nowCamera = targetCamera;
    }



    //设置状态机
    private void SetupAnimator()
    {
        //瞄准情况
        if (armState == ArmState.Aim)
        {
            AimRotation();
            Gun.position = shootPosition.position;
            Gun.rotation = shootPosition.rotation;

            animator.SetBool(aimStateHash, true);
            animator.SetBool(equipHash, false);
            animator.SetFloat(HorizontalSpeedHash, moveInput.normalized.x * walkSpeed, 0.1f, Time.deltaTime);
            animator.SetFloat(verticalSpeedHash, moveInput.normalized.y * walkSpeed, 0.1f, Time.deltaTime);
            return;
        }
        else if (armState == ArmState.Equip)
        {
            animator.SetBool(equipHash, true);
            animator.SetBool(aimStateHash, false);
            TurnByCamera();
            
        }
        //这里处理旋转
        else if (armState == ArmState.Normal)
        {
            //Vector3 forw = playerMovement;
            //Quaternion targetRotation = Quaternion.LookRotation(forw);//得到目标四元数
            //tf.rotation = Quaternion.Slerp(tf.rotation,targetRotation,turnScale * Time.deltaTime);//手动加转

            ////获得当前角度和目标角度之间的差值
            //float a = Quaternion.Angle(tf.rotation, targetRotation);
            //animator.SetFloat(turnSpeedHash, a * Mathf.Deg2Rad, 0.1f,Time.deltaTime);
            animator.SetBool(aimStateHash, false);

            TurnByCamera();

        }
        Gun = GunPosition;
        //站立姿态
        if (nowState == PlayerState.Stand)
        {
            animator.SetFloat(blendHash, standThreshold, 0.1f, Time.deltaTime);
            switch (locomotionState)
            {
                case LocomotionState.Idle:
                    animator.SetFloat(verticalSpeedHash, 0, 0.1f, Time.deltaTime);
                    break;
                case LocomotionState.Run:
                    animator.SetFloat(verticalSpeedHash, playerMovement.magnitude * runSpeed, 0.1f, Time.deltaTime);
                    break;
                case LocomotionState.Sprint:
                    animator.SetFloat(verticalSpeedHash, playerMovement.magnitude * SprintSpeed, 0.1f, Time.deltaTime);
                    break;
                case LocomotionState.Walk:
                    animator.SetFloat(verticalSpeedHash, playerMovement.magnitude * walkSpeed, 0.1f, Time.deltaTime);
                    break;
                
            }
        }
        //蹲下姿态
        else if (nowState == PlayerState.Crouch)
        {
            animator.SetFloat(blendHash,crouchThreshold, 0.1f, Time.deltaTime);
            switch (locomotionState)
            {
                case LocomotionState.Idle:
                    animator.SetFloat(verticalSpeedHash, 0, 0.1f, Time.deltaTime);
                    break;
                default:
                    animator.SetFloat(verticalSpeedHash, playerMovement.magnitude * crouchSpeed, 0.1f, Time.deltaTime);
                    break;
            }

        }
        else if (nowState == PlayerState.Air)
        {
            animator.SetFloat(blendHash,airThreshold,0.1f, Time.deltaTime);
            
            animator.SetFloat(jumpSpeedHash, verticalVelicity, 0.1f, Time.deltaTime);
        }
        

    }
    //处理旋转
    private void TurnByCamera()
    {
        float rad = Mathf.Atan2(playerMovement.x, playerMovement.z);
        animator.SetFloat(turnSpeedHash, rad, 0.1f, Time.deltaTime);
        tf.transform.Rotate(0, rad * 200 * Time.deltaTime, 0);
    }
    private void AimRotation()
    {
        //提取出玩家当前的y轴旋转
        float a = tf.rotation.eulerAngles.y;
        Quaternion tfRota = Quaternion.Euler(0, a, 0);
        //提取出相机的y轴旋转
        float camerarota = cameraPosition.eulerAngles.y;
        Quaternion cmRota = Quaternion.Euler(0, camerarota, 0);

        if (cmRota == tfRota)
            return;
        tf.rotation = Quaternion.Slerp(tfRota, cmRota, aimTurnSpeed * Time.deltaTime);
    }


}
