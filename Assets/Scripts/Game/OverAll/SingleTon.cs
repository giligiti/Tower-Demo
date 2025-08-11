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
                            throw new ArgumentException("错误，单例子类未实现私有构造函数", nameof(constructor));
                    }
                }
            }
            return instance;
        }
    }
}
