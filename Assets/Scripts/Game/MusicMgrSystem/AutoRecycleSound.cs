using UnityEngine;

public class AutoRecycleSound : MonoBehaviour
{
    public AudioSource ad;
    private const float offset = 0.1f;
    private float currTime = 0;
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        //设置为3D音效组件
        ad.spatialBlend = 1;
        //保证音频不循环
        ad.loop = false;
    }
    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        
    }
    void Update()
    {
        if (!ad.isPlaying)
        {
            currTime += Time.deltaTime;
            if (currTime >= offset) { currTime = 0; RecycleSound(); }
        }
    }
    //依赖注入，得到Audiosource组件
    public void GetAudio(AudioSource ad)
    {
        this.ad = ad;
    }

    /// <summary>
    /// 扰动声音，使其有起伏
    /// </summary>
    /// 在管理器加载初始化后调用
    public void DisturbanceVoice()
    {
        //让声音有所变化
        //因为每次重新加载都会同步，所以不需要回归状态
        ad.pitch = Random.Range(ad.pitch - 0.1f, ad.pitch + 0.1f);
        ad.volume = Random.Range(ad.pitch - 0.1f, ad.pitch + 0.1f);
    }

    public void RecycleSound()
    {
        PoolMgr.Instance.PushObject<SoundPoolData>("soundObj", this.gameObject);
    }
}