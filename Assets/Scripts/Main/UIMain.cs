using UnityEngine;

public class UIMain : MonoBehaviour
{
    //主canvas
    [HideInInspector] public static Canvas canvas;
    //加载Canvas
    private Canvas loadCanvas;
    [SerializeField] Camera UIcamera;               //inspector窗口中引用

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //游戏一开始加载开始界面
        UIManager.Instance.ShowPanel<StartPanel>();
        UIPrepareWork();
    }
    private void UIPrepareWork()
    {
        //完成UI准备工作
        GameObject obj = UIManager.Instance.Canvas;
        DontDestroyOnLoad(obj);
        
        Canvas ca = obj.GetComponent<Canvas>();
        ca.renderMode = RenderMode.ScreenSpaceCamera;
        ca.worldCamera = UIcamera;
    }
    //场景加载的时候
    public void LoadSceneHappen()
    {
        //loadc.LoadProgressFinsh()
        GameObject ca = Instantiate(Resources.Load<GameObject>("UI/SceneLoadCanvas"));
        loadCanvas = ca.GetComponent<Canvas>();
        loadCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        loadCanvas.worldCamera = UIcamera;
    }
    private void LoadSceneOver()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
