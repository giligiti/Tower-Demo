using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


//����ع���Ա
//����Ѷ���������أ��Լ��Ӷ�Ӧ�Ķ������ȡ��
//Y�Ƕ���ع���Ա����ľ����������ͣ���MonsterPoolData  �����ȡ�����ߴ���Monster����(BaseMonster)��Y����MonsterPoolData
//���ö������Ҫָ�������ֶ���ء���ͬ�Ķ���ص�ʵ����������ͬ
//��ִ��������Ϸ��ͣ������ʱ����Ҫ����ֹͣ��������ķ���
public class PoolMgr : BaseManager<PoolMgr> 
{
    //��������壬����������
    private GameObject Pool;
    private PoolMgr()
    {
        Pool = new GameObject("PoolMgr");
    }
    private Coroutine clearCheckCoroutine;

    private int clearNum = 5;            //ÿ��������Ҫ����Ķ���س�Ա����
    private int clearFrequency = 10;     //ÿ��������Ҫ����Ƶ�ʵ�ֵ
    private int clearThresholds = 10;    //������ֵ��Ƶ�ʣ������ڸ�ֵ��ᱻ����
    private int clearDelayTime = 300;    //����ض��������ʱ�䣬��λ���룻
    /// <summary>
    /// ����ض��������ʱ��
    /// </summary>
    public int ClearDelayTime { get { return clearDelayTime; }set { clearDelayTime = Mathf.Max(value, 5); } }
    //���������
    private Dictionary<string, PoolData> PoolDic = new Dictionary<string, PoolData>();
    #region ȡ���������

    /// <summary>
    /// ȡ������ķ�������
    /// </summary>
    /// <param name="key">����ص�����</param>
    /// <returns></returns>
    public GameObject GetObject<Y>(string key, string path = null) where Y : PoolData, new()
    {
        return GetObj<Y>(key, path);
    }
    //����ȡ���ķ���
    private GameObject GetObj<Y>(string key, string path = null) where Y : PoolData, new()
    {
        FirstCheck();

        GameObject obj = null;
        //��������ڼ�
        if (!PoolDic.ContainsKey(key))
        {
            Y pool = new Y();
            pool.InitPoolData(key, Pool);
            PoolDic.Add(key, pool);
        }

        obj = PoolDic[key].GetValue(path);
        obj.name = key;
        return obj;
    }

    /// <summary>
    /// ������ʱ��ǰ���ض������
    /// </summary>
    /// <param name="key">������</param>
    /// <param name="objNum">��Ҫ���ɵ�����</param>
    /// <param name="path">��Դ·��</param>
    public void AdvanceInstante<Y>(string key, int objNum, string path = null) where Y : PoolData, new()
    {
        AdvanceInit<Y>(key, objNum, path);
    }
    /// <summary>
    /// ������ʱ��ǰ���ض������ķ��ͷ�����TΪ�������
    /// </summary>
    /// <param name="objNum">��Ҫ���ɵ�����</param>
    /// <param name="path">��Դ·��</param>
    public void AdvanceInstante<T,Y>(int objNum, string path = null) where Y : PoolData, new()
    {
        string key = typeof(T).Name;
        AdvanceInit<Y>(key, objNum, path);
    }
    //��������ع��������ȡ��������ֱ�ӵ��ö�����ڲ���ʵ�����������ķ���
    private void AdvanceInit<Y>(string key, int objNum, string path = null) where Y : PoolData, new()
    {
        FirstCheck();
        //��������ڼ�
        if (!PoolDic.ContainsKey(key))
        {
            Y pool = new Y();
            //ʵ������Ӧ�����Ķ����Զ����д洢
            pool.InitPoolData(key, Pool, objNum, path);
        }
        else
        {
            PoolDic[key].InitPoolData(key, Pool, objNum, path);
        }
    }

    #endregion

    #region �洢�������

    //��
    public void PushObject<T,Y>(GameObject obj) where Y : PoolData, new()
    {
        FirstCheck();
        string key = typeof(T).Name;
        //��������ڼ�
        if (!PoolDic.ContainsKey(key))
        {
            Y pool = new Y();               //�����¶����
            pool.InitPoolData(key, Pool);   //��ʼ���ö����
            PoolDic.Add(key, pool);         //�Ѷ����������
        }

        PoolDic[key].SetValue(obj);
    }
    public void PushObject<Y>(string name, GameObject obj) where Y : PoolData, new()
    {
        FirstCheck();
        //��������ڼ�
        if (!PoolDic.ContainsKey(name))
        {
            Y pool = new Y();               //�����¶����
            pool.InitPoolData(name, Pool);   //��ʼ���ö����
            PoolDic.Add(name, pool);         //�Ѷ����������
        }

        PoolDic[name].SetValue(obj);

    }

    #endregion

    #region �����������

    //��ջ���صĲ���
    public void ClearPool()
    {
        PoolDic.Clear();
        PoolDic = null;
        Pool = null;
    }

    //���ڼ����������
    IEnumerator IEWaitForDelyTime()
    {
        yield return new WaitForSecondsRealtime(clearDelayTime);
        MonoMgr.Instance.StartCoroutine(IEClearPool());
        clearCheckCoroutine = MonoMgr.Instance.StartCoroutine(IEWaitForDelyTime());//������������ʵ��ÿ�ζ��ڼ��
    }
    IEnumerator IEClearPool()
    {
        Debug.Log("��ʼ����");
        //���ҳ���Ҫɾ���Ķ��������һ��ɾ��
        List<string> keyList = new List<string>();  
        foreach (var pool in PoolDic)
        {
            //Ƶ�ʵ�����ֵ��������������С���������Ķ���ؽ��м�¼����
            if (pool.Value.GetSetCount < clearThresholds && pool.Value.poolList.Count > pool.Value.Least)
            {
                keyList.Add(pool.Key);
            }
            else
            {
                //ÿ��ִ�ж�������ͻ����Ƶ��;
                pool.Value.GetSetCount -= clearFrequency; 
            }
            yield return null;
        }
        //ȷ��Ҫ����ĲŻ�������ѭ��
        foreach (var key in keyList)
        {
            Debug.Log(key);
            //�����г�Ա����������ֵ�����ٺ�ֱ��ȫ������
            int i = PoolDic[key].poolList.Count <= clearNum? PoolDic[key].Least: PoolDic[key].poolList.Count - clearNum;

            //whileѭ������ȷ���˼�ʹȷ��Ҫɾ������غ���;�������µĴ�������ȡ������Ҳ����������
            //ɾ�����ȡ���߼��ɶ���ع������ű���get��push�������ϻᴴ���¼�
            //����ػ�����ֱ���������ٵ���Сֵ
            while (PoolDic[key].poolList.Count > i )
            {
                GameObject obj = PoolDic[key].GetValue();
                obj.SetActive(false);
                Object.Destroy(obj);
                yield return null;
            }

        }
    }
    /// <summary>
    /// ֹͣ����������
    /// </summary>
    public void StopClearCheck()
    {
        if (clearCheckCoroutine != null)    
        MonoMgr.Instance.StopCoroutine(clearCheckCoroutine);
    }
    /// <summary>
    /// ��������������
    /// Ĭ�ϵ�һ�δ�������ص�ʱ��ʼ�ſ�ʼ���ڼ��
    /// </summary>
    public void StartClearCheck()
    {
        clearCheckCoroutine = MonoMgr.Instance.StartCoroutine(IEWaitForDelyTime());
    }

    #endregion

    private void FirstCheck()
    {
        if (clearCheckCoroutine == null)
            StartClearCheck();
    }


}
