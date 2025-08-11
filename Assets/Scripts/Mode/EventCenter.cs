using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//����¼�������AddEventListener
//ע���¼������÷�����Ҫ�������һ��obj�������õĺ�����obj���¼������ߣ������ж��¼������ߵ����
//�¼�������TriggerEventListener
//�¼������������¼���Ҫ�����¼������߱��������Ӧ�ű�
public class EventCenter :BaseManager<EventCenter>
{
    private EventCenter()
    {

    }
    //�¼�����
    //��Ҫ�����¼������߱���
    private Dictionary<string, UnityAction<Object>> eventDic = new Dictionary<string, UnityAction<Object>>();
    //����Ҫ�����¼������߱���
    private Dictionary<string,UnityAction> eventNop = new Dictionary<string, UnityAction>();

    #region ��Ҫ�����¼������߱���ļ�����ע������

    //�¼�ע��
    public void AddEventListener(string name, UnityAction<Object> events)
    {
        if (eventDic.ContainsKey(name))
        {
            eventDic[name] += events;
        }
        else
        {
            eventDic.Add(name, events);
        }
    }

    //�¼�����
    public void TriggerEventListener(string name , Object obj = null)
    {
        if (!eventDic.ContainsKey(name))
            return;
        eventDic[name].Invoke(obj);
    }

    #endregion

    #region ����Ҫ�����¼������߱���ļ���ע����������

    //�¼�����������������

    //�¼�ע��
    public void AddEventListener(string name, UnityAction events)
    {
        if (eventDic.ContainsKey(name))
        {
            eventNop[name] += events;
        }
        else
        {
            eventNop.Add(name, events);
        }
    }
    //�¼�����
    public void TriggerEventListener(string name)
    {
        if (!eventNop.ContainsKey(name))
            return;
        eventNop[name].Invoke();
    }

    #endregion
    //���ڼ��س���ʱ����¼����ģ���ֹ�ڴ�й¶
    public void ClearEventCenter()
    {
        eventDic.Clear();
    }

}
