using System;
using System.Collections;
using UnityEngine;

public abstract class BaseLoaderElement:EventDispatcher
{
    public uint cacheTime = 0;
    public System.Action<BaseLoaderElement> onLoadComplete;
    public System.Action<BaseLoaderElement> onLoadError;
    public System.Action<object> completeCallback;
    public System.Action<string> errorCallback;
    protected string _path;

    public BaseLoaderElement(string path)
    {
        _path = path;
    }

    public string Path
    {
        get
        {
            return _path;
        }
    }

    public abstract IEnumerator Load();

    public abstract void Unload();

    public abstract float GetProgress();

    public abstract object GetContent();
}
