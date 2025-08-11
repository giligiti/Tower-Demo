using System;
using System.Reflection;
using UnityEngine;
//����Ҫ��ʾʵ��˽�й��캯��
public class BaseManager<T>  where T :class  
{
    private static T instance;
    private static readonly object lockObject = new object();
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        Type t = typeof(T);
                        ConstructorInfo info = t.GetConstructor(BindingFlags.Instance|BindingFlags.NonPublic,null,Type.EmptyTypes,null);
                        if (info != null)
                            instance = info.Invoke(null) as T;
                        else throw new ArgumentException("���󣬵�������δʵ��˽�й��캯��",nameof(info));
                    }
                }
            }
            return instance;
        }
    }
}
