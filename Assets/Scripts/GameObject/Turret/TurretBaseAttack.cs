using System.Collections.Generic;
using Octree;
using UnityEngine;
using UnityEngine.Events;

public class TurretBaseAttack : Attacked
{
    [SerializeField] private BulletFactory bulletFactory;        //子弹工厂
    [SerializeField] private EffectFactory effFactory;           //开火特效工厂
    public Transform fireObj;
    [SerializeField] private int atkSpeed;                          //攻击速度（发/秒）                 //暂时序列化特性
    private TurretType turretType;                                  //攻击类型
    private List<GameObject> firePostion;                           //开火点
    private GameObject targetObj;                                   //目标
    private float aimAccuracy = 0.1f;                               //瞄准精度

    private float FireTime => 1f / atkSpeed * 2;                    //表示攻击间隔                       //暂时修改

    private float nowTime;                                          //记录发射时间
    private SoundPlayer soundPlayer;                                //发声脚本
    private bool isPlayingFireSound = false;                        //标志位决定是否开始播放声音


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
        nowTime = Time.time;
        if (!TryGetComponent(out soundPlayer)) soundPlayer = gameObject.AddComponent<SoundPlayer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 外界调用，开火
    /// </summary>
    /// <param name="target"></param>
    /// 在update中被调用
    public void StartFire(OctreeMono target)               //要修改
    {
        //targetObj = target;
        float angle = Vector3.Angle(fireObj.forward, target.bounds.center - fireObj.position);
        if (angle < aimAccuracy)
        {
            if (Time.time - nowTime > FireTime)
            {
                Fire();
                nowTime = Time.time;
            }
        }
    }
    private void Fire()
    {
        Debug.Log("开火");

        GameObject bullet = bulletFactory.Create(fireObj.position, Quaternion.LookRotation(fireObj.forward));

        bullet.GetComponent<IIgnore>().ToIgnore(this.gameObject);

        effFactory.Create(fireObj.position, Quaternion.LookRotation(fireObj.forward));

        //FireLight();//枪口火光
        //声音
        FireSoundPlay();
    }
    //临时，需要修改，根据炮塔是长时播放类型还是实例化播放类型来决定，现在暂时是长时播放
    private void FireSoundPlay()
    {
        if (isPlayingFireSound) return;
        soundPlayer.PlaySound(E_SoundClip.gun_GatlingFireSound);
        isPlayingFireSound = true;
    }
    //主脚本调用
    public void FireSoundStop()
    {
        soundPlayer.StopSound();
        isPlayingFireSound = false;
    }
    private void FireLight()
    {
        // firePointfire.SetActive(true);
        // float a = Random.Range(0.9f, 1.2f);
        // firePointfire.transform.localScale *= a;
    }
    public void StopFire() { }

    void OnDisable()
    {
        
    }
}
