using UnityEngine;

[RequireComponent(typeof(GatlingRotate))]
public class GatlingTurret : TurretBase
{
    private ISpecialFireHandle fireHandle;       //inspector窗口进行赋值
    protected override void Awake()
    {
        base.Awake();
        fireHandle = GetComponent<ISpecialFireHandle>();
        if (fireHandle != null)
        {
            BeforeFireFunction += fireHandle.BeforeAtk;
            StopFireAction += fireHandle.StopAtk;
        }
        else Debug.LogError($"{gameObject.name}上未找到ISpecialFireHandle脚本");

    }
    
}