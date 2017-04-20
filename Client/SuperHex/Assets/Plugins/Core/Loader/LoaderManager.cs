using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class LoaderManager
{
    private static Dictionary<string, BaseLoaderElement> _loadingDic;
    public static void Setup()
    {
        _loadingDic = new Dictionary<string, BaseLoaderElement>();
    }

    public static void Load(string path, EnumResouceType type, System.Action<object> completeCallback, System.Action<string> errorCallback = null)
    {
        BaseLoaderElement loader;
        if (_loadingDic.ContainsKey(path))
        {
            loader = _loadingDic[path];
            loader.completeCallback += completeCallback;
            if (errorCallback != null)
            {
                loader.errorCallback += errorCallback;
            }
        }
        else
        {
            loader = CreateLoader(type, path);
            loader.onLoadComplete = OnLoadComplete;
            loader.onLoadError = OnLoadError;

            loader.completeCallback += completeCallback;
            loader.errorCallback += errorCallback;
            _loadingDic.Add(path, loader);
            Initiator.instance.StartCoroutine(loader.Load());
        }
    }

    private static void OnLoadComplete(BaseLoaderElement loader)
    {
        _loadingDic.Remove(loader.Path);
        loader.completeCallback.Invoke(loader.GetContent());
        loader.Unload();
        
    }

    private static void OnLoadError(BaseLoaderElement loader)
    {
        Debug.Log("resource load error: " + loader.Path);
        _loadingDic.Remove(loader.Path);
        if(loader.errorCallback != null)
            loader.errorCallback.Invoke(loader.Path);
        loader.Unload();
        
    }

    public static BaseLoaderElement CreateLoader(EnumResouceType type, string path)
    {
        BaseLoaderElement loader;
        switch (type)
        {
            case EnumResouceType.CACHE_BINARY:
                loader = new CacheBinaryLoader(path);
                break;
            case EnumResouceType.BINARY:
                loader = new BinaryLoader(path);
                break;
            case EnumResouceType.REMOTE_BINARY:
                loader = new RemoteBinaryLoader(path);
                break;
            case EnumResouceType.ASSETBUNDLE:
                loader = new ABLoader(path);
                break;
            default:
                loader = new BinaryLoader(path);
                break;
        }
        return loader;
    }

    public static void Unload(string path, System.Action<object> completeCallback, System.Action<string> errorCallback = null)
    {
        if (path == null)
            return;
        if (_loadingDic.ContainsKey(path))
        {
            BaseLoaderElement loader = _loadingDic[path];
            loader.completeCallback -= completeCallback;
            loader.errorCallback -= errorCallback;
            if (loader.completeCallback == null)
            {
                _loadingDic.Remove(path);
                loader.Unload();
                //TestCollider.Instance.StopCoroutine("Load");
            }
        }
    }

    public static void UnloadAll()
    {
        foreach(string key in _loadingDic.Keys)
        {
            _loadingDic[key].Unload();
        }
        _loadingDic.Clear();
    }
}
