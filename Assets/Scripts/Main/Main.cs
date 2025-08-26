using UnityEngine;

[RequireComponent(typeof(SoundMgr))]
[RequireComponent(typeof(UIMain))]
[RequireComponent(typeof(CameraMain))]
public class Main : MonoBehaviour
{
    public static SceneMain sceneMain;
    private UIMain uiMain;
    private SoundMgr soundMgr;
    public static Main main;
    void Awake()
    {
        main = this;
    }
    void Start()
    {
        
    }
}