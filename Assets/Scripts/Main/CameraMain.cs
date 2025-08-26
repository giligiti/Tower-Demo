using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// 
/// </summary>
/// 主相机并不固定
/// 只有UI相机才是固定不变带到下一个场景的
/// 其他相机都是每个场景有每个场景的相机
/// 每个场景中需要添加的相机直接添加到该场景的主相机的堆栈就好了
public class CameraMain : MonoBehaviour
{
    // [SerializeField] MonsterFactory bruteFactory;
    // [SerializeField] EffectFactory effectFactory;
    // [SerializeField] BulletFactory bulletFactory;
    // public Vector3 dir;

    public LayerMask layer;
    //相机管理
    private Camera MainCamera;
    public Camera UICamera;
    public List<Camera> othersCamera = new List<Camera>();

    void Awake()
    {
        //找场景中的主相机，并让每次场景加载完成后都寻找一次主相机
        FinAndSetCamera();
        ScenesLoadMgr.Instance.BackEvent.AddListener(FinAndSetCamera);
        DontDestroyOnLoad(UICamera);
        DontDestroyOnLoad(this);

    }
    private void FinAndSetCamera()
    {
        //设置主相机
        MainCamera = Camera.main;
        //排除层级
        MainCamera.cullingMask &= ~layer.value;
        //改变主相机堆栈
        var cameradata = MainCamera.GetUniversalAdditionalCameraData();
        cameradata.cameraStack.Add(UICamera);
        foreach (var camera in othersCamera)
        {
            cameradata.cameraStack.Add(camera);
        }
    }

    void Start()
    {
        //GameObject obj = bruteFactory.Create(Vector3.zero, Quaternion.identity);
        //eff = effectFactory.Create(Vector2.zero, Vector3.forward);


    }

}
