using System;
using UnityEngine;
/// <summary>
/// ACE
/// </summary>
public class AppModuleProxy : IAppModule
{
    private AppModule _module;
    private object _showData;
    private string _name;
    private Type _classType;
    private string _path;
    private int _siblingIndex;
    public AppModuleProxy(string name, Type classType)
    {
        _name = name;
        _classType = classType;
    }

    public void Show(object obj = null)
    {
        _showData = obj;
        if (_module != null)
            _module.Show(obj);
        else
        {
            _path = AssetPathUtil.GetUI(_name);
            ResourceManager.GetResource(_path, OnGetAppModule);
        }
    }

    private void OnGetAppModule(object content)
    {
        GameObject go = GameObject.Instantiate((GameObject)content);
        _module = go.AddComponent(_classType) as AppModule;
        _module.SetSiblingIndex(_siblingIndex);
        _module.Show(_showData);
    }

    public void SetSiblingIndex(int value)
    {
        _siblingIndex = value;
        if (_module != null)
            _module.SetSiblingIndex(_siblingIndex);
    }

    public void Destroy()
    {
        if (_module != null)
            _module.Destroy();
        _module = null;
        ResourceManager.Release(_path, OnGetAppModule);
    }

    public void Hide()
    {
        if (_module != null)
        {
            _module.Hide();
        }
        else
        {
            ResourceManager.Release(_path, OnGetAppModule);
        }
    }

    public string Type
    {
        get { return _name; }
    }
}
