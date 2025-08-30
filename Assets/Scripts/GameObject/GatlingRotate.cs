using DG.Tweening;
using UnityEngine;

public class GatlingRotate : MonoBehaviour,ISpecialFireHandle
{
    [Range(0,100)]
    public float targetSpeed = 60;
    public float TargetSpeed { get { return targetSpeed; } set { targetSpeed = value; } }
    public float nowSpeed = 0;
    public float detlaTime = 3f;
    public bool isGatlingAlready = false;                   //加特林是否准备完毕
    private bool isTurring = false;
    [SerializeField] Transform gatlingBarrel;               //加特林枪管
    private Tween tweenSpeedChange;

    /// <summary>
    /// 让加特林枪管开始旋转
    /// </summary>
    private void StartRotate()
    {
        if (isTurring) return;
        tweenSpeedChange?.Kill();
        DOTween.Kill(tweenSpeedChange);
        isTurring = true;
        float durationTime = Mathf.Max(detlaTime * Mathf.Abs(targetSpeed - nowSpeed) / targetSpeed, 0.01f);
        tweenSpeedChange = DOTween.To(() => nowSpeed, x => nowSpeed = x, targetSpeed, durationTime).OnComplete
        (
            () => { isGatlingAlready = true; }
        );
    }
    private void StopRotate()
    {
        if (!isGatlingAlready || Mathf.Approximately(nowSpeed, 0)) return;
        isGatlingAlready = false;
        DOTween.Kill(tweenSpeedChange);
        float durationTime = Mathf.Max(detlaTime / 2 * (nowSpeed / targetSpeed), 0.01f);
        tweenSpeedChange = DOTween.To(() => nowSpeed, x => nowSpeed = x, 0, durationTime).OnComplete
        (
            () => { isTurring = false; }
        );
    }
    #region ISpecialFireHandle接口实现
    public bool BeforeAtk() { StartRotate(); return isGatlingAlready; }
    public void StopAtk()=> StopRotate();
    #endregion

    void Update()
    {
        if (isTurring)
        {
            gatlingBarrel.Rotate(Vector3.forward, nowSpeed * Time.deltaTime * 30, Space.Self);
        }
            
    }

    
}