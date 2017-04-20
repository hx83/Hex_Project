using System.Collections;
using System.IO;
using UnityEngine;

public class ABPersistentPathLoader: BaseLoaderElement
{
    protected AssetBundleCreateRequest _request;

    public ABPersistentPathLoader(string url):base(url)
    {

    }

    public override float GetProgress()
    {
        if (_request == null)
            return 0;
        return _request.progress;
    }

    public override IEnumerator Load()
    {
        byte[] data;
        using (FileStream stream = File.OpenRead(Path))
        {
            data = new byte[stream.Length];
            stream.Read(data, 0, (int)stream.Length);
        }
        if (data == null || data.Length == 0)
        {
            if (onLoadError != null)
                onLoadError.Invoke(this);
            yield break;
        }
        else
        {
            _request = AssetBundle.LoadFromMemoryAsync(data);
            yield return _request;
            if(onLoadComplete == null)
            {
                _request.assetBundle.Unload(true);
                _request = null;
            }
            else
            {
                Debug.Log("cache:=>" + Path + " success");
                onLoadComplete.Invoke(this);
            }
        }
    }

    public override void Unload()
    {
        onLoadComplete = null;
        onLoadError = null;
        completeCallback = null;
        errorCallback = null;
        if(_request != null && _request.isDone)
            _request = null;
    }

    public override object GetContent()
    {
        if (_request == null)
            return null;
        if(_request.assetBundle.name == "")
            _request.assetBundle.name = System.IO.Path.GetFileNameWithoutExtension(Path);
        return _request.assetBundle;
    }
}
