using System.Collections;
using System.IO;
using UnityEngine;

public class ABStreamingPathLoader : BinaryLoader
{
    public ABStreamingPathLoader(string url) : base(url)
    {

    }

    public override object GetContent()
    {
        if (_www.assetBundle.name == "")
            _www.assetBundle.name = System.IO.Path.GetFileNameWithoutExtension(Path);
        return _www.assetBundle;
    }
}
