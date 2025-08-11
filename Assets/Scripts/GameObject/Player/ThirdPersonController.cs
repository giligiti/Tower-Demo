using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonController : MonoBehaviour
{
    Transform tf;
    Animator animator;
    CharacterController controller;
    Transform cameraPosition;
    [Header("ƽ�������")]
    [SerializeField] private CinemachineCamera normalCamera;
    [Header("��׼�����")]
    [SerializeField] private CinemachineCamera aimCamera;
    [Header("�Ŵ������")]
    [SerializeField] private CinemachineCamera largCamera;
    private CinemachineCamera nowCamera;                    //��ʾ��ǰ��������ھ�ͷ�任��ʱ�����ԭ����ͷ��Ŀ��
    private Vector2 screenPoint;                            //��ʾ��ǰ��Ļ���ĵ�

    [Header("���ֳ�ǹλ��")]
    public Transform rightHandPostion;
    [Header("���ֳ�ǹλ��")]
    public Transform leftHandPostion;
    [Header("ǹ")]
    public Transform Gun;
    [Header("ǹ��λ��")]
    public Transform shootPosition;
    private Transform GunPosition;

    //�������
    private readonly float Gravity = -9.8f;  //����
    private float verticalVelicity; //��ҵ�ǰ��ֱ�����ٶ�
    
    
    //��������
    private Vector2 moveInput;
    private Vector2 AimInput;

    //�����׼״̬
    public enum ArmState
    {
        Normal,
        Equip,
        Aim,
    }
    public ArmState armState = ArmState.Normal;

    [Header("��׼�����ٶ�")]
    [SerializeField] private float aimTurnSpeed = 50;
    //��һ���״̬(��վ��)
    [HideInInspector]
    public enum PlayerState
    {
        Crouch,
        Stand,
        Air,
    }
    public PlayerState nowState = PlayerState.Stand;
    //�������״̬
    [HideInInspector]
    public enum LocomotionState
    {
        Idle,
        Walk,
        Run,
        Sprint,
    }
    public LocomotionState locomotionState = LocomotionState.Idle;
    

    //������¼�л���ֵ
    private float crouchThreshold = 0f;
    private float standThreshold = 1f;
    private float airThreshold = 2.2f;
    //���߱����ٶ�                    //�����ֵȡ���ڶ�Ӧ�����ڻ�����е�Y���ֵ
    private float crouchSpeed = 2.5f;       
    private float walkSpeed = 1.5f;
    private float runSpeed = 2.6f;
    private float SprintSpeed = 4.6f;
    //��Ծ�ٶ�
    private float jumpVelocity = 6.5f;                                                //�Ժ�Ҫ�޸ģ��ݶ�
    #region ״̬�Ͷ�������

    //�������״̬
    private bool isAim;             //��׼
    private bool isCrouch;          //����
    private bool isWalk;            //����
    private bool isRun;             //С��
    private bool isSprint;          //���
    private bool isJump;            //��Ծ
    private bool isEquip = false;   //�Ƿ�װ��
    private bool currentState;      //��ǰ״̬

    private int verticalSpeedHash;              //����������VerticalSpeed    ��ʾ��ֱǰ���ٶ�
    private int turnSpeedHash;                  //����������TurnSpeed        ����ת���ٶ�
    private int HorizontalSpeedHash;            //����������HorizontalSpeed  ��ʾˮƽ�����ٶ�
    private int blendHash;                      //����������Blend            ��ʾ��վ���ľ���ֵ
    private int jumpSpeedHash;                  //����������JumpSpeed        ��ʾ��ֱ�����ٶ�
    private int aimStateHash;
    private int equipHash;

    #endregion

    private Vector3 playerMovement;

    private static readonly int CACHE_SIZE = 3;
    private Vector3[] CachePool = new Vector3[CACHE_SIZE];  //�����洢��֡���ƶ��ٶ�
    private int OldestIndex = 0;                            //��ʾCachePool������

    private Vector3 averageSpeed;                           //��ʾ��֡��ƽ���ٶ�

    private float fillMulti = 1.5f;                         //��ʾ�����ʱ����ٶ�������ʱ��Ķ��ٱ�
    private float nowTime = 0;
    private readonly float jumpColdTime = 0.8f;
    

    private void Start()
    {
        tf = transform;//ͨ������ֶ�������transfrom�������Ч��
        cameraPosition = Camera.main.transform;
        aimCamera.enabled = false;

        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();

        #region �õ����������Ĺ�ϣֵ

        verticalSpeedHash = Animator.StringToHash("VerticalSpeed");
        HorizontalSpeedHash = Animator.StringToHash("HorizontalSpeed");
        turnSpeedHash = Animator.StringToHash("TurnSpeed");
        blendHash = Animator.StringToHash("Blend");
        jumpSpeedHash = Animator.StringToHash("JumpSpeed");
        aimStateHash = Animator.StringToHash("isAim");
        equipHash = Animator.StringToHash("isEquip");
        #endregion

        //���
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        screenPoint = new Vector2(Screen.width/2, Screen.height/2);

        GunPosition = Gun;
    }
    /// <summary>
    /// ����IK���ú���
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
    #region �������

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
        //������任
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
        //��Ծ���
        Jump();
        OnGravity();
        CaculaterGravity();
        //�����
        //CameraTargetStay();

    }
    
    #region ��Ծ���

    
    private void Jump()
    {
        if (controller.isGrounded && isJump)
        {
            verticalVelicity = jumpVelocity;
            nowState = PlayerState.Air;
            isJump = false;
        }
    }
    //��������                                                              //�Ժ�Ҫ�޸ģ��ݶ�
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
            // Ӧ��������ٶȱ���
            if (verticalVelicity < 0)
            {
                verticalVelicity += Gravity * fillMulti * Time.deltaTime;
            }
            else
            {
                verticalVelicity += Gravity * Time.deltaTime;
            }

            // ������������ٶ�
            if (verticalVelicity < -20f) // �ɵ�������������ٶ�
            {
                verticalVelicity = -20f;
            }
        }
    }
    //Ӧ������
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

    //����ƽ���ٶ�
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

    //�ж����״̬�任
    private void ChangePlayerState()
    {
        
        //�ж��Ƿ��Ƕ���վ
        if (isCrouch)
        {
            nowState = PlayerState.Crouch;
        }
        //�ж���Ծ
        else if (!controller.isGrounded)
        {
            nowState = PlayerState.Air;
        }
        else
        {
            nowState = PlayerState.Stand;
        }
        //�ж�վ������
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

        //�ж���׼״̬
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

    //���������������ӽǵ����뷽��͵�ǰ���﷽��ı任
    private void CaculateInputDirection()
    {
        //������ڵ���ƽ���ϵ�ͶӰ�ĵ�λ��������������ϵ��
        Vector3 camForward = new Vector3(cameraPosition.forward.x,0,cameraPosition.forward.z).normalized;
        playerMovement = camForward * moveInput.y + cameraPosition.right * moveInput.x;
        playerMovement = tf.InverseTransformVector(playerMovement);
    }
    /// <summary>
    /// ����������任ʱ�ܽ���ԭ��Ŀ��
    /// </summary>
    /// <param name="targetCamera">Ҫת��ɵ����</param>
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



    //����״̬��
    private void SetupAnimator()
    {
        //��׼���
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
        //���ﴦ����ת
        else if (armState == ArmState.Normal)
        {
            //Vector3 forw = playerMovement;
            //Quaternion targetRotation = Quaternion.LookRotation(forw);//�õ�Ŀ����Ԫ��
            //tf.rotation = Quaternion.Slerp(tf.rotation,targetRotation,turnScale * Time.deltaTime);//�ֶ���ת

            ////��õ�ǰ�ǶȺ�Ŀ��Ƕ�֮��Ĳ�ֵ
            //float a = Quaternion.Angle(tf.rotation, targetRotation);
            //animator.SetFloat(turnSpeedHash, a * Mathf.Deg2Rad, 0.1f,Time.deltaTime);
            animator.SetBool(aimStateHash, false);

            TurnByCamera();

        }
        Gun = GunPosition;
        //վ����̬
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
        //������̬
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
    //������ת
    private void TurnByCamera()
    {
        float rad = Mathf.Atan2(playerMovement.x, playerMovement.z);
        animator.SetFloat(turnSpeedHash, rad, 0.1f, Time.deltaTime);
        tf.transform.Rotate(0, rad * 200 * Time.deltaTime, 0);
    }
    private void AimRotation()
    {
        //��ȡ����ҵ�ǰ��y����ת
        float a = tf.rotation.eulerAngles.y;
        Quaternion tfRota = Quaternion.Euler(0, a, 0);
        //��ȡ�������y����ת
        float camerarota = cameraPosition.eulerAngles.y;
        Quaternion cmRota = Quaternion.Euler(0, camerarota, 0);

        if (cmRota == tfRota)
            return;
        tf.rotation = Quaternion.Slerp(tfRota, cmRota, aimTurnSpeed * Time.deltaTime);
    }


}
