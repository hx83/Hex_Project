using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class CacheBinaryLoader : BaseLoaderElement
{
    protected byte[] _data;
    

    public CacheBinaryLoader(string url):base(url)
    {

    }

    public override float GetProgress()
    {
        if (_data == null)
            return 0;
        return 1;
    }

    public override IEnumerator Load()
    {
        using (FileStream stream = File.OpenRead(Path))
        {
            _data = new byte[stream.Length];
            stream.Read(_data, 0, (int)stream.Length);
        }

        if (_data == null)
        {
            if (onLoadError != null)
                onLoadError.Invoke(this);
        }
        else
        {
            Debug.Log("cache:=>" + Path + " success");
            onLoadComplete.Invoke(this);
        }
        yield break;
    }

    public override void Unload()
    {
        onLoadComplete = null;
        onLoadError = null;
        completeCallback = null;
        errorCallback = null;
        _data = null;
    }

    public override object GetContent()
    {
        return _data; 
    }
}
