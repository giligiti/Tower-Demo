using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class ButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,IPointerDownHandler,IPointerUpHandler
{
    //按下声音
    public E_SoundType buttonSoundType = E_SoundType.ui_Button;
    //指向声音
    public E_SoundType buttonPointSoundType = E_SoundType.ui_PointButton;

    #region  缓动效果

    public Ease point = Ease.OutBack;
    public Ease exit = Ease.InOutCirc;
    public Ease down = Ease.OutExpo;
    public Ease up = Ease.OutBack;

    #endregion

    //是否悬停、按下
    private bool isHovering = false;
    private bool isPressing = false;


    [Tooltip("鼠标悬停放大倍数")]
    private float hoverScale = 1.1f;
    [Tooltip("鼠标按下缩小倍数")]
    private float pressScale = 0.9f;
    [Tooltip("表示动画播放时间")]
    public float duration = 0.2f;
    //表示原始缩放
    private Vector3 originScale;
    //表示当前播放的动画
    private Tween currentTween;

    Button button;


    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            
        });
        originScale = transform.localScale;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        //播放声音
        SoundMgr.Instance.PlayUISound(buttonPointSoundType);
        Debug.Log(gameObject.name + " " + eventData.pointerId);
        //放大button
        isHovering = true;
        if (!isPressing) PlayTweenAnimation(hoverScale, point);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        if (!isPressing) PlayTweenAnimation(1, exit);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        isPressing = true;
        SoundMgr.Instance.PlayUISound(buttonSoundType);
        PlayTweenAnimation(pressScale, down);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressing = false;
        float scale = isHovering ? hoverScale : 1;
        PlayTweenAnimation(scale, up);
    }
    private void PlayTweenAnimation(float targetScale, Ease easeType)
    {
        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill();
        }
        
        currentTween = transform.DOScale(targetScale * originScale, duration).SetEase(easeType).SetUpdate(true);

    }

    void OnDisable()
    {
        button.onClick.RemoveAllListeners();
        transform.localScale = originScale;
        currentTween = null;
    }

    
}