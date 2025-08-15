using System.Collections.Generic;
using Octree;
using UnityEngine;

public class TurretBase : MonoBehaviour, IInit, IGetRange
{
    public Transform yawObject;                                     //������ת̨
    public Transform pitchObject;                                   //�ڿ���ת
    private float pitchIdentity;                                    //�ڿ�Ĭ�ϽǶ�

    public TurretBaseAttack atkBehaviour;                           //�����ű�����inspector��������
    public OctreeMonoCheck roomCheck;                               //������صİ˲����ռ���ű�
    [HideInInspector] public float Range => attackedRange;          //�ӿ��ṩ���ڿռ���
    private GameObject targetObj;                                   //Ŀ��
    private OctreeMono targetMono;                                  //���ڻ�ȡĿ��İ�Χ��
    //Ϊ�˵��Է����������л�
    [SerializeField] private int attackedRange;                     //������Χ
    
    [SerializeField] private int VerticalSpeed;                     //�����ٶ�
    [SerializeField] private float verticalAngleUp;                 //����
    [SerializeField] private float verticalAngleDown;               //����
    [SerializeField] private int horizontalSpeed;                   //ת���ٶ�
    [SerializeField] private int horizontalAngle;                   //����ת��Ƕ�
    private Quaternion Xturn;                                       //Ŀ�����ת��
    private Quaternion Yturn;                                       //Ŀ������ת��

    
    private IDeath targetDeath;                                     //Ŀ��������¼��ӿڣ���LinkedTarget�����и��£�TargetChange������ʹ��

    public LayerMask layer;                                         //��Ҫ����Ŀ��Ĳ㼶����inspector��������


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
            Debug.Log("��ǰ��Ŀ�꣬����Ѱ������");

            FindTargetFlow();
        }
        //��ǰ��Ŀ�꣬���빥������
        else
        {
            Debug.Log($"��ǰ��Ŀ��:{targetObj.name}���빥������");
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

        //����
        foreach (var objs in objects)
        {
            Debug.DrawLine(transform.position, objs.transform.position, Color.blue);
        }

        //ɸѡ
        obj = TargetChoose(objects);
        //���û�еõ�Ŀ��
        if (obj == null) return false;

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
                targetMono = obj.GetComponent<OctreeMono>();                                      //�޸�
                if (!TargetKeep(targetMono.turePosition, out float distance)) continue;
                //ѡ�����Ҫ��������еģ����������
                if (distance < resverPair.Item2) resverPair = (obj, distance);
            }
        }
        return resverPair.Item1;
    }


    /// <summary>
    /// �ı�Ŀ��
    /// </summary>
    /// <param name="obj">�µ�Ŀ��</param>
    /// �����������Ŀ��λ�ã���Ŀ���뿪������Χ�������Ŀ��
    private void TargetChange(GameObject obj)
    {
        //���ֻ����ΪĿ���뿪�˹�����Χ
        if (obj == null)
        {
            targetObj = null;
            //����ע������������¼�
            targetDeath.UnsubscribeDeathEvent(TargetDead);
            Debug.Log("Ŀ���뿪������Χ");                                                          //����
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
        //�ÿ�Ŀ�꣬�Զ���update�����н���Ѱ·�߼�
        targetObj = null;
        Debug.Log("Ŀ������");                                                              //����
    }

    #endregion

    #region ��������
    /// <summary>
    /// ��������
    /// </summary>
    private void AtkFlow()
    {
        //����Ŀ�꣬���������������ת����ֵ��Xturn��Yturn������������ת�Ĳ�����ȡ���˴�distance����
        if (!TargetKeep(targetMono.turePosition, out float distance))                       //�޸�
        {
            TargetChange(null);
            return;
        }
        //����ת��Ŀ��
        DirectionSplit();
        //��ʼ����
        Attacked();                                                            
    }

    /// <summary>
    /// ���Ŀ���Ƿ��������
    /// </summary>
    /// <returns></returns>
    private bool TargetKeep(Vector3 objPosition, out float distance)
    {
        distance = Vector3.Distance(objPosition, transform.position) * 2;                       //���ԣ�����
        if (distance > attackedRange)
        {
            Debug.Log("����̫Զ");

            return false;
        }

        //���Ŀ��λ��,���Ŀ���뿪������Χ�����TargetChange�����Ŀ��,���¿�ʼѰ·
        if (!TurretAngleCalculation(objPosition, yawObject, pitchObject)) return false;
        return true;
    }

    protected void Attacked()
    {
        atkBehaviour.StartFire(targetMono);                         //Ҫ�޸�
    }

    #endregion

    #region ��̨��ת
    /// <summary>
    /// ����ֱ������ת
    /// </summary>
    /// <param name="direction">Ŀ��λ��</param>
    public void DirectionSplit()
    {
        HorizontalTurn(Xturn, yawObject);

        VerticalTurn(Yturn, pitchObject);
    }
    /// <summary>
    /// ���ƫת�Ƕ��Ƿ񳬹�����
    /// </summary>
    /// <param name="targetPostion">Ŀ��λ��</param>
    /// <param name="Xturnobj">����ƫת����</param>
    /// <param name="Yturnobj">����ƫת����</param>
    /// <returns></returns>
    private bool TurretAngleCalculation(Vector3 targetPostion, Transform Xturnobj, Transform Yturnobj)
    {
        //�жϺ���ƫת
        Vector3 Xdirection = (targetPostion - Xturnobj.position).normalized;
        Xdirection.y = 0;
        //��ֹ�����쳣��ת
        if (Xdirection.sqrMagnitude < 0.001f) return false;
        //��������������
        if (Vector3.Angle(transform.forward, Xdirection) > horizontalAngle / 2)
        {
            Debug.Log("����Խ��");
            return false;
        }
        

        //�ж�����ƫת
        Vector3 localTarget = targetPostion - Yturnobj.position;
        // ͶӰ��xzƽ��
        Vector3 localTargetXZ = new Vector3(localTarget.x, 0, localTarget.z);
        if (localTargetXZ.sqrMagnitude < 0.001f) return false;
        // ������Ҫ��ת�Ĵ�ֱ�Ƕȣ�����ھֲ�X�ᣩQuaternion.Euler(0 , 90, 0) * localTargetXZ
        float verticalAngle = Vector3.SignedAngle(localTargetXZ, localTarget, Quaternion.Euler(0, 90, 0) * localTargetXZ);
        if (-verticalAngleDown < verticalAngle || -verticalAngleUp > verticalAngle)
        {
            Debug.Log("������Խ��");
            return false;
        }

        //���,���������������£���ֵ�洢
        Xturn = Quaternion.LookRotation(Xdirection, Vector3.up);
        Yturn = Quaternion.Euler(verticalAngle, 0, 0);
        return true;                                                                               

    }
    /// <summary>
    /// ��̨����ƫת
    /// </summary>
    private void HorizontalTurn(Quaternion quaternion, Transform turnobj)
    {
        turnobj.rotation = Quaternion.RotateTowards(turnobj.rotation, quaternion, horizontalSpeed * Time.deltaTime);
        return;
    }

    /// <summary>
    /// �ڿڸ���ƫת
    /// </summary>
    /// <param name="q"></param>
    public void VerticalTurn(Quaternion targetRotation, Transform turnobj)
    {
        #region ����
        // Vector3 localTarget = targetPostion - turnobj.position;
        // // ͶӰ��xzƽ��
        // Vector3 localTargetXZ = new Vector3(localTarget.x, 0, localTarget.z);
        // if (localTargetXZ == Vector3.zero) return;
        // // ������Ҫ��ת�Ĵ�ֱ�Ƕȣ�����ھֲ�X�ᣩQuaternion.Euler(0 , 90, 0) * localTargetXZ
        // float verticalAngle = Vector3.SignedAngle(localTargetXZ, localTarget, Quaternion.Euler(0, 90, 0) * localTargetXZ);
        // // ���ƽǶȷ�Χ
        // verticalAngle = Mathf.Clamp(verticalAngle, -verticalAngleUp, verticalAngleDown);
        // //���ƾֲ�X�ᴴ����ת��Ԫ��
        // Quaternion targetRotation = Quaternion.Euler(verticalAngle, 0, 0);
        //������ת
        #endregion

        turnobj.localRotation = Quaternion.RotateTowards(turnobj.localRotation, targetRotation, VerticalSpeed * Time.deltaTime);


        // ���ƾֲ�ǰ���򣨺�ɫ��                                                      ����
        Debug.DrawRay(turnobj.position, turnobj.forward * 10, Color.red);
        //Ŀ�귽����ɫ������׼ˮƽ�ߣ���ɫ�� 
        // Debug.DrawRay(turnobj.position, localTargetXZ, Color.blue);
        // Debug.DrawRay(turnobj.position, targetPostion, Color.green);

    }

    #endregion



    #endregion

}
