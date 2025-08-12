using UnityEngine;

public class TurretBaseAttack : Attacked, IGetRange
{
    [SerializeField] private ScriptableObject bulletFactory;        //子弹工厂
    [SerializeField] private ScriptableObject effFactory;           //开火特效工厂

    [SerializeField] private int atkSpeed;                          //攻击速度                  //暂时序列化特性
    private TurretType turretType;                                  //攻击类型

    [HideInInspector] public float Range => atkRange;               //接口提供用于空间检测

    /// <summary>
    /// 初始化方法
    /// </summary>
    /// <param name="atk"></param>
    /// <param name="range"></param>
    /// <param name="atkSpeed"></param>
    public void AtkInit(int atk, int range, int atkSpeed)
    {
        this.atk = atk;
        this.atkRange = range;
        this.atkSpeed = atkSpeed;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartFire()
    {

    }
    

}
