using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MonoMgr :BaseManager<MonoMgr>
{
    private MonoController monocontroller;
    //˽�й��캯��
    private MonoMgr()
    {
        GameObject obj = new GameObject("Mono");
        monocontroller = obj.AddComponent<MonoController>();
    }
    public void AddEventListener(UnityAction listener, E_LifeFunction function = E_LifeFunction.update)
    {
        monocontroller.AddEventListener(listener, function);
    }
    public void RemoveEventListener(UnityAction listener, E_LifeFunction function = E_LifeFunction.update)
    {
        monocontroller.RemoveEventListener(listener,function);
    }
    //ʵ��Э�̵����ú͹ر�
    public Coroutine StartCoroutine(IEnumerator routine)
    {
        return monocontroller.StartCoroutine(routine);
    }
    public void StopCoroutine(IEnumerator routine)
    {
        monocontroller.StopCoroutine(routine);
    }
    //ͨ��Coroutine��ֹͣ
    public void StopCoroutine(Coroutine routine)
    {
        monocontroller.StopCoroutine(routine);
    }
}
