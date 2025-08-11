using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour, IHumanoidBehaviour
{
    private static Player instance;
    public static Player Instance =>instance;

    public UnityEvent InitEvent;
    public void Awake()
    {
        instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitEvent?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HealthDecrease(int damage)
    {

    }

    public void HealthIncrease(int value)
    {

    }

    public void Dead()
    {

    }

    public void StateReset()
    {

    }
}
