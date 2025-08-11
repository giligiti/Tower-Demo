using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

public class ABMgr : BaseManager<ABMgr>
{
    private Dictionary<string, AssetBundle> abDic = new Dictionary<string, AssetBundle>();
    private ABMgr()
    {

    }

    private AssetBundle mainBundle;             //�洢����
    private AssetBundleManifest manifest;       //�洢������������ϵ
    //���ļ���ַ�ַ���
    private string streamingPath
    {
        get
        {
            return Application.streamingAssetsPath + "/";
        }
    }
    //������
    private string MainBundleName
    {
        get
        {
#if UNITY_ANDROID
            return "ANDROID";
#elif UNITY_IOS
            return "IOS";
#else
            return "PC";
#endif
        }
    }

    private void LoadBundle(string name)
    {
        if (mainBundle == null)
        {
            mainBundle = AssetBundle.LoadFromFile(streamingPath + MainBundleName);
            manifest = mainBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }
        if (manifest == null||mainBundle ==null)
        {
            Debug.Log("nono");
        }
        string[] bundles = manifest.GetAllDependencies(name);//���ݴ�������ֵõ�����������������
        AssetBundle build;
        //��������������
        for (int i = 0; i < bundles.Length; i++)
        {
            //�ж��Ƿ���ع�
            if (abDic.ContainsKey(bundles[i]))
                continue;
            build = AssetBundle.LoadFromFile(streamingPath + bundles[i]);
            abDic.Add(bundles[i], build);//�洢

        }

        //���ذ�
        if (!abDic.ContainsKey(name))//�ж��Ƿ���ع�
        {
            build = AssetBundle.LoadFromFile(name);
            abDic.Add(name, build);
        }
    }
    #region ͬ������
    //ָ�����ͷ���
    public Object LoadRes(string name, string resName,Type types)
    {
        LoadBundle(name);
        Object obj = abDic[name].LoadAsset(resName , types);
        return obj;
    }
    //���ͷ���
    public T LoadRes<T>(string name, string resName) where T : Object
    {
        LoadBundle(name);
        T obj = abDic[name].LoadAsset<T>(resName);
        return obj;
    }

    #endregion

    #region �첽����
    //�첽���ش���ab��������Դ�������������ص��¼�
    public void LoadResAsync(string name, string resName, Type types, UnityAction<Object> events = null)
    {
        
        MonoMgr.Instance.StartCoroutine(RealLoadRes(name, resName, types, events));
    }
    //�����첽
    public void LoadResAsync<T>(string name, string resName, UnityAction<T> events = null)where T : Object
    {
        MonoMgr.Instance.StartCoroutine(RealLoadRes<T>(name, resName, events));
    }
    //Э��
    private IEnumerator RealLoadRes(string name, string resName, Type type, UnityAction<Object> events = null)
    {
        LoadBundle(name);
        AssetBundleRequest abq = abDic[name].LoadAssetAsync(resName,type);
        yield return abq;
        //���ûص�
        events?.Invoke(abq.asset);
    }
    //����Э��
    private IEnumerator RealLoadRes<T>(string name, string resName, UnityAction<T> events = null) where T : Object
    {
        LoadBundle(name);
        AssetBundleRequest abq = abDic[name].LoadAssetAsync<T>(resName);
        yield return abq;
        //���ûص�
        T b = abq.asset as T;
        events?.Invoke(b);
    }
    #endregion

    #region ��Դ��ж��
    //ж�ص�����
    public void UnloadPack(string name)
    {
        if (!abDic.ContainsKey(name))
            return;
        abDic[name].Unload(false);//ж��
        abDic.Remove(name);//���ֵ����Ƴ�
    }

    //ж�����а�
    public void UnloadAllPack(string name)
    {
        AssetBundle.UnloadAllAssetBundles(false);
        abDic.Clear();
    }

    #endregion



}

