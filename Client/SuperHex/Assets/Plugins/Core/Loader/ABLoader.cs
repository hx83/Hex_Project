using System.Collections;
using System.IO;
using UnityEngine;

public class ABLoader:BaseLoaderElement
{
    protected AssetBundleCreateRequest _request;


    public ABLoader(string url):base(url)
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
        _request = AssetBundle.LoadFromFileAsync(_path);
        yield return _request;
        if(onLoadComplete == null)
        {
            _request.assetBundle.Unload(true);
            _request = null;
        }
        else
            onLoadComplete.Invoke(this);
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
