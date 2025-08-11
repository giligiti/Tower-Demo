using System;
using System.Reflection;
using UnityEngine;

public class SingleTon<T> where T : class
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
                        Type type = typeof(T);
                        ConstructorInfo constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic,null,Type.EmptyTypes,null);
                        if (constructor != null)
                            instance = constructor.Invoke(null) as T;
                        else
                            throw new ArgumentException("���󣬵�������δʵ��˽�й��캯��", nameof(constructor));
                    }
                }
            }
            return instance;
        }
    }
}
