using UnityEngine;

public class MoveBody : MonoBehaviour
{
    public float moveSpeed;         //移动速度
    public float runSpeed;          //奔跑速度
    public float roundSpeed;        //转向速度
    public float stopDistance;      //停止距离
    public virtual void StateReset()
    {

    }
}
