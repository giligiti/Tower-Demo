using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SoundMgr))]
[RequireComponent(typeof(UIMain))]
[RequireComponent(typeof(CameraMain))]
public class SceneMain : MonoBehaviour
{
    public static SceneMain sceneMain;
    private UIMain uiMain;
    private SoundMgr soundMgr;
    void Awake()
    {
        sceneMain = this;
        uiMain = GetComponent<UIMain>();
        soundMgr = GetComponent<SoundMgr>();
        DontDestroyOnLoad(this);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //播放声音
        soundMgr.PlayBGMSound(E_SoundClip.bgm_Normal);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void LoadScene<T>(string sceneName, UnityAction action = null) where T :BasePanel
    {
        //唤出加载面板
        uiMain.LoadSceneHappen();
        action += () => UIManager.Instance.ShowPanel<T>();
        ScenesLoadMgr.Instance.LoadSceneAsync(sceneName, action);
    }
}
