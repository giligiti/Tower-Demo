using System.Collections.Generic;
using Octree;
using UnityEngine;

public class TurretBase : MonoBehaviour, IInit
{
    public Transform yawObject;                                     //������ת̨
    public Transform pitchObject;                                   //�ڿ���ת
    private float pitchIdentity;                                    //�ڿ�Ĭ�ϽǶ�

    public TurretBaseAttack atkBehaviour;                           //�����ű�����inspector��������
    //Ϊ�˵��Է����������л�
    [SerializeField] private int attackedRange;                     //������Χ
    
    [SerializeField] private int VerticalSpeed;                     //�����ٶ�
    [SerializeField] private float verticalAngleUp;                 //����
    [SerializeField] private float verticalAngleDown;               //����
    [SerializeField] private int horizontalSpeed;                   //ת���ٶ�
    [SerializeField] private int horizontalAngle;                   //����ת��Ƕ�

    public GameObject targetObj;                                    //���Ե�Ŀ��
    private IDeath targetDeath;                                     //Ŀ��������¼��ӿڣ���LinkedTarget�����и��£�TargetChange������ʹ��

    public OctreeMonoCheck roomCheck;                               //������صİ˲����ռ���ű�

    public LayerMask layer;                                         //��Ҫ����Ŀ��Ĳ㼶����inspector��������

    private bool isTargetChange = true;                             //��ǰĿ���Ƿ����ı� 

    //��ʼ������
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
        //��ù����ű�

        //�õ��ڿڳ�ʼĬ�ϽǶ�
        Vector3 forwardAngle = new Vector3(pitchObject.forward.x, 0, pitchObject.forward.z);
        pitchIdentity = Vector3.Angle(forwardAngle, pitchObject.forward);

        TurretInfo info = GameDataMgr.Instance.turretInfos[0];                                      //��ʱ����
        Init<TurretInfo>(info);                                                                     //��ʱ����
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

    #region Ѱ�й�������
    private void BehaviourFlow()
    {
        //��ǰ��Ŀ�꣬����Ѱ������
        if (targetObj == null)
        {
            FindTargetFlow();
        }
        //��ǰ��Ŀ�꣬���빥������
        else
        {
            //                                                                                      ���ԣ���������Ŀ��ĺ���
            Debug.DrawLine(transform.position, targetObj.transform.position, Color.red);
            AtkFlow();
        }
    }

    /// <summary>
    /// Ѱ������
    /// </summary>
    private void FindTargetFlow()
    {
        //���Ի�ȡĿ��
        if (!TryGetTargetObj(out GameObject obj)) return;
        //�ı�Ŀ��
        TargetChange(obj);
    }

    #region ��ȡĿ��
    /// <summary>
    /// ���Ի�ȡĿ��
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    private bool TryGetTargetObj(out GameObject obj)
    {
        HashSet<GameObject> objects = roomCheck.ProvideTargets();
        //ɸѡ
        obj = TargetChoose(objects);
        //���û�еõ�Ŀ��
        if (obj == null)
        {
            obj = null;
            return false;
        }

        return true;
    }

    /// <summary>
    /// ɸѡĿ��
    /// </summary>
    /// <param name="objects">��Χ������ļ���</param>
    private GameObject TargetChoose(HashSet<GameObject> objects)
    {
        //����Ԥѡ�����Ԫ��
        (GameObject, float) resverPair = (null, float.MaxValue);
        //������������
        foreach (GameObject obj in objects)
        {
            //���ݲ㼶Ҫ��ɸѡ
            int layerNum = 1 << obj.layer;
            if ((layer & layerNum) != 0)
            {
                float distance = Vector3.Distance(transform.position, obj.transform.position);
                //ȥ��δ���﹥����Χ����
                if (distance > attackedRange) continue;
                //ȥ�������޷�����λ�õ�����
                //if ()                                                                            //
                //ѡ�����Ҫ��ģ����������
                if (distance < resverPair.Item2) resverPair = (obj, distance);
            }
        }
        return resverPair.Item1;
    }


    /// <summary>
    /// �ı�Ŀ��
    /// </summary>
    /// <param name="obj">�µ�Ŀ��</param>
    /// ��ֻ����ΪĿ���뿪������Χ�������Ŀ��
    private void TargetChange(GameObject obj)
    {
        //���ֻ����ΪĿ���뿪�˹�����Χ
        if (obj == null)
        {
            TargetDead();
            //����ע������������¼�
            targetDeath.UnsubscribeDeathEvent(TargetDead);
            return;
        }
        //Ŀ�������������¼�����Ҫע����Ŀ�������Ի�ע�������¼�

        //ѡ����Ŀ��
        targetObj = obj;
        //������Ŀ��������¼�
        LinkedTarget(obj);
    }

    private void LinkedTarget(GameObject obj)
    {
        targetDeath = obj.GetComponent<IDeath>();
        //����Ŀ��������¼�
        targetDeath.SubscribeDeathEvent(TargetDead);
    }

    /// <summary>
    /// Ŀ������
    /// </summary>
    private void TargetDead()
    {
        //�ÿ�Ŀ�꣬�Զ���update�н���Ѱ·�߼�
        targetObj = null;
    }

    #endregion

    #region ����
    /// <summary>
    /// ��������
    /// </summary>
    private void AtkFlow()
    {
        //����ת��Ŀ��
        DirectionSplit(targetObj.transform.position);
        //��ʼ����
        //if (pitchObject.)                                                             //

    }

    #endregion

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
    /// ��������:���ƫת�Ƕ��Ƿ񳬹�����
    /// </summary>
    /// <param name="targetPostion">Ŀ��λ��</param>
    /// <param name="Xturnobj">����ƫת����</param>
    /// <param name="Yturnobj">����ƫת����</param>
    /// <returns></returns>
    private bool TurretAngleCalculation(Vector3 targetPostion, Transform Xturnobj, Transform Yturnobj)
    {
        Vector3 Xdirection = (targetPostion - Xturnobj.position).normalized;
        Xdirection.y = 0;
        if (Vector3.Angle(Xturnobj.transform.forward, Xdirection) > horizontalAngle / 2) return false;

        return false;                                                                               //

    }
    /// <summary>
    /// ��̨����ƫת
    /// </summary>
    private void HorizontalTurn(Vector3 targetPostion, Transform turnobj)
    {
        Vector3 direction = (targetPostion - turnobj.position).normalized;
        direction.y = 0;
        Quaternion quaternion;
        if (direction == Vector3.zero) quaternion = turnobj.rotation;
        else quaternion = Quaternion.LookRotation(direction, turnobj.up);
        if (turnobj.rotation == quaternion) return;
        turnobj.rotation = Quaternion.RotateTowards(turnobj.rotation, quaternion, horizontalSpeed * Time.deltaTime);
        return;
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
        //���ƾֲ�X�ᴴ����ת��Ԫ��
        Quaternion targetRotation = Quaternion.Euler(verticalAngle, 0, 0);
        //������ת
        turnobj.localRotation = Quaternion.RotateTowards(turnobj.localRotation, targetRotation, VerticalSpeed * Time.deltaTime);


        // ���ƾֲ�ǰ���򣨺�ɫ����Ŀ�귽����ɫ������׼ˮƽ�ߣ���ɫ��                                                       ����
        Debug.DrawRay(turnobj.position, turnobj.forward * 10, Color.red);
        Debug.DrawRay(turnobj.position, localTargetXZ, Color.blue);
        Debug.DrawRay(turnobj.position, targetPostion, Color.green);

    }

    #endregion

    

    #endregion

}
