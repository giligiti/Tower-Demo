using UnityEngine;

public class TurretBase : MonoBehaviour, IInit, IGetFloat
{
    [SerializeField] private ScriptableObject bulletFactory;    //�ӵ�����
    [SerializeField] private ScriptableObject effFactory;       //������Ч����

    public Transform yawObject;                                 //������ת̨
    public Transform pitchObject;                               //�ڿ���ת��
    private float pitchIdentity;                                //�ڿ�Ĭ�ϽǶ�

    //������Щ��Ҫ�޸ģ�Ϊ�˵��Է����������л�
    [SerializeField] private int atk;                               //������
    [SerializeField] private int attackedRange;                     //������Χ
    private TurretType turretType;                                  //��������
    [SerializeField] private int VerticalSpeed;                     //�����ٶ�
    [SerializeField] private int HorizontalSpeed;                   //ת���ٶ�
    [SerializeField] private float verticalAngleUp;                 //����
    [SerializeField] private float verticalAngleDown;               //����

    public GameObject targetObj;                                //���Ե�Ŀ��
    public float direction;

    public float Range => attackedRange;

    //��ʼ������
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
        //�õ��ڿڳ�ʼĬ�ϽǶ�
        Vector3 forwardAngle = new Vector3(pitchObject.forward.x, 0, pitchObject.forward.z);
        pitchIdentity = Vector3.Angle(forwardAngle, pitchObject.forward);
        TurretInfo info = GameDataMgr.Instance.turretInfos[0];                                      //��ʱ����
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

    #region ��̨��ת


    /// <summary>
    /// ����ֽ�
    /// </summary>
    /// <param name="direction">Ŀ��λ��</param>
    public void DirectionSplit(Vector3 targetPosition)
    {
        HorizontalTurn(targetPosition, yawObject);

        VerticalTurn(targetPosition, pitchObject);

    }
    /// <summary>
    /// ��̨����ƫת
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
    /// �ڿڸ���ƫת
    /// </summary>
    /// <param name="q"></param>
    public void VerticalTurn(Vector3 targetPostion, Transform turnobj)
    {
        Vector3 localTarget = targetPostion - turnobj.position;
        // ͶӰ��xzƽ��
        Vector3 localTargetXZ = new Vector3(localTarget.x, 0, localTarget.z);
        if (localTargetXZ == Vector3.zero) return;
        // ������Ҫ��ת�Ĵ�ֱ�Ƕȣ�����ھֲ�X�ᣩQuaternion.Euler(0 , 90, 0) * localTargetXZ
        float verticalAngle = Vector3.SignedAngle(localTargetXZ, localTarget, Quaternion.Euler(0, 90, 0) * localTargetXZ);
        // ���ƽǶȷ�Χ
        verticalAngle = Mathf.Clamp(verticalAngle, -verticalAngleUp, verticalAngleDown);
        direction = Mathf.Clamp(direction, verticalAngleDown, verticalAngleUp);
        //���ƾֲ�X�ᴴ����ת��Ԫ��
        Quaternion targetRotation = Quaternion.Euler(verticalAngle, 0, 0);
        //������ת
        turnobj.localRotation = Quaternion.RotateTowards(turnobj.localRotation, targetRotation, VerticalSpeed * Time.deltaTime);
        // ���ƾֲ�ǰ���򣨺�ɫ����Ŀ�귽����ɫ������׼ˮƽ�ߣ���ɫ��
        Debug.DrawRay(turnobj.position, turnobj.forward * 10, Color.red);
        Debug.DrawRay(turnobj.position, localTargetXZ, Color.blue);
        Debug.DrawRay(turnobj.position, targetPostion, Color.green);

    }

    #endregion


}
