using UnityEngine;
using UnityEngine.Events;

public class TurretTest : MonoBehaviour, IDeath
{
    public bool EventInvoke = false;
    public UnityEvent death;
    public void SubscribeDeathEvent(UnityAction action)
    {
        death.AddListener(action);
    }

    public void UnsubscribeDeathEvent(UnityAction action)
    {
        death.RemoveListener(action);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (EventInvoke)
        {
            death?.Invoke();
            Debug.Log("目标死亡");
            EventInvoke = false;
        }
    }
    void OnDisable()
    {
        
    }
}
