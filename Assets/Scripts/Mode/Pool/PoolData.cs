using System.Collections.Generic;
using UnityEngine;

public abstract class PoolData
{
    //����ÿһ�����Ӷ������͵ĸ�����
    public GameObject Father;

    protected int least = 5;    
    public virtual int Least { get { return least ; } set { least = value; } }//����������С����ֵ//��д����ʵ�ָı�Ĭ����Сֵ
    private int getSetCount = 0;
    /// <summary>
    /// Ƶ��
    /// </summary>
    public virtual int GetSetCount {  get { return getSetCount; } set { getSetCount = Mathf.Max(value,0); } }

    public Queue<GameObject> poolList;
    //��ʾ�����޲ι��캯��,�Է���һ
    public PoolData() { }
    //�������Ҫ�洢�Ķ�������ȡ�����������ϴ����Ķ���أ��������ø�����
    public void InitPoolData(string name, GameObject poolObj)
    {
        Father = new GameObject(name);
        Father.transform.parent = poolObj.transform;
        poolList = new Queue<GameObject>();
    }
    //��ǰ���ض������//�첽����//��ʹ���ߵ��ã�һ����ع�������ʱ�����
    /// <summary>
    /// һ����ع�������ʱ�����
    /// </summary>
    /// <param name="name">������</param>
    /// <param name="poolObj">����صĸ�����</param>
    /// <param name="objNum">��Ҫ��ǰ���صĶ�������</param>
    /// <param name="path">�����·��</param>
    public void InitPoolData(string name, GameObject poolObj, int objNum, string path = null)
    {
        InitPoolData(name, poolObj);
        for (int i = 0; i < objNum; i++)
        {
            //ʵ������Ӧ�����Ķ��󲢽��д洢
            SetValue(ObjectInstante(Father.name,path));
            getSetCount++;
        }
    }


    //ȡ
    public virtual GameObject GetValue(string path = null)
    {
        getSetCount++;
        GameObject obj = null;
        //�ж���
        if (poolList.Count > 0)
        {
            obj = poolList.Dequeue();
        }
        //û����
        else
        {
            //ʵ����
            obj = ObjectInstante(Father.name, path);
        }
        // //ȡ��������ֱ�۵���ʾ�ڲ㼶�����
        // obj.transform.parent = null;
        obj.SetActive(true);
        return obj;
    }
    //��
    public virtual void SetValue(GameObject obj)//�麯�������������д�����߼���������Ч������ʱ���յ��������
    {
        getSetCount++;
        obj.transform.parent = Father.transform;
        obj.name = Father.name;
        //����
        obj.SetActive(false);
        poolList.Enqueue(obj);
    }
    //ʵ���������������������дʵ����������ʵ��ʵ�����Ķ�����
    protected abstract GameObject ObjectInstante(string key,string path = null);
    ////�첽ʵ�������� 
    //protected GameObject ObjecsInstanteAsync(string key)
    //{

    //}
    
}