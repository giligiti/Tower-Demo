using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
//�����ڴ˴�������������ط������¼�
public class ScenesLoadMgr : BaseManager<ScenesLoadMgr>
{
    private ScenesLoadMgr()
    {

    }
    //�ṩ���ⲿ���г�������
    public void LoadScene(string name, UnityAction fun = null)
    {
        //�������������¼����Ĵ�������
        EventCenter.Instance.TriggerEventListener(name);
        //���س�����Ҫ����¼�����
        EventCenter.Instance.ClearEventCenter();

        SceneManager.LoadScene(name);
        
        //��������ķ���
        fun?.Invoke();
    }
    //�����첽���ط���
    public void LoadSceneAsync(string name, UnityAction fun)
    {
        MonoMgr.Instance.StartCoroutine(AsyncFun(name,fun));
    }
    private IEnumerator AsyncFun(string name, UnityAction fun)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(name);
        yield return ao;
        fun?.Invoke();
        //Ҳ��������������
        //AsyncOperation ao = SceneManager.LoadSceneAsync(name);
        ////��ʾ�첽����δ��ɾͻ�������ѭ�����������yield return ao һ��Ч��
        //while (!ao.isDone)
        //{
        //    //���н��������µȲ���
        //    yield return ao.progress;
        //}
    }
}
