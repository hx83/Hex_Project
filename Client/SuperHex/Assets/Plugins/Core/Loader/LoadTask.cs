using System.Collections.Generic;

public class LoadTask
{
    public List<string> dependencies;
    public string abName;
    public System.Action<object> resCallback;
    public string resPath;
    public bool isFinishUnload;
    public LoadTask()
    {
        dependencies = new List<string>();
    }

    public bool IsDependenciesLoadComplete()
    {
        return dependencies.Count == 0;
    }

    public void Dispose()
    {
        dependencies.Clear();
        dependencies = null;
        resCallback = null;
    }
}
