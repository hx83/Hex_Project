using System;
using System.Collections;
using UnityEngine;

public class ABLoaderFromFile : BaseLoaderElement
{
    private AssetBundleCreateRequest _request;
    public ABLoaderFromFile(string url):base(url)
    {

    }
    public override object GetContent()
    {
        if (_request.assetBundle.name == "")
            _request.assetBundle.name = System.IO.Path.GetFileNameWithoutExtension(Path);
        return _request.assetBundle;
    }

    public override float GetProgress()
    {
        return _request.progress;
    }

    public override IEnumerator Load()
    {
        _request = AssetBundle.LoadFromFileAsync(_path);
        yield return _request;
        if (_request == null)
            yield break;
        if (_request.assetBundle == null)
            yield break;
        onLoadComplete.Invoke(this);
    }

    public override void Unload()
    {
        _request = null;
    }
}
