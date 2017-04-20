using System.Collections;
using UnityEngine;

public class BinaryLoader : BaseLoaderElement
{
    protected WWW _www;
    public BinaryLoader(string url):base(url)
    {

    }

    public override float GetProgress()
    {
        if (_www == null)
            return 0;
        return _www.progress;
    }

    public override IEnumerator Load()
    {
        _www = new WWW(Path);
        yield return _www;
        if (_www == null)
            yield break;
        if (_www.error != null)
        {
            if (onLoadError != null)
                onLoadError.Invoke(this);
        }
        else
        {
            if (_www.bytes.Length == 0)
            {
                if (onLoadError != null)
                    onLoadError.Invoke(this);
            }
            else
                onLoadComplete.Invoke(this);
        }
    }

    public override object GetContent()
    {
        return _www.bytes;
    }

    public override void Unload()
    {
        onLoadComplete = null;
        onLoadError = null;
        completeCallback = null;
        errorCallback = null;
        _www.Dispose();
        _www = null;
    }
}
