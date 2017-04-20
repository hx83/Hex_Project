using System.Collections.Generic;
using System;
public class ModuleManager
{
    private static Dictionary<string, AppModuleProxy> _moduleDictionary = new Dictionary<string, AppModuleProxy>();
    private static Dictionary<string, Type> _launchClassDic;
    private static List<AppModuleProxy> _showingList;
    public static void Setup()
    {
        _showingList = new List<AppModuleProxy>();

        _moduleDictionary = new Dictionary<string, AppModuleProxy>();
        _launchClassDic = new Dictionary<string, Type>();

        _launchClassDic.Add(ModuleType.MAIN_PANEL, typeof(MainPanel));
        
    }

    public static void HideAll()
    {
        foreach(AppModuleProxy module in _moduleDictionary.Values)
        {
            module.Hide();
        }
        _showingList.Clear();
    }

    public static void DisposeAll()
    {
        foreach (AppModuleProxy module in _moduleDictionary.Values)
        {
            module.Destroy();
        }
        _moduleDictionary.Clear();
        _showingList.Clear();
    }

    public static void Show(string type, object data = null)
    {
        AppModuleProxy appModule;
        if (_moduleDictionary.ContainsKey(type))
        {
            appModule = _moduleDictionary[type];
        }
        else
        {
            appModule = new AppModuleProxy(type, _launchClassDic[type]);        
            _moduleDictionary.Add(type, appModule);
        }
        appModule.SetSiblingIndex(_moduleDictionary.Count - 1);
        appModule.Show(data);
        //if(type != ModuleType.LOGIN_PANEL)
        //{
        //    Hide(ModuleType.LOGIN_PANEL);
        //}
        _showingList.Add(appModule);
        DispathcEvent(ModuleEvent.SHOW, type);
    }

    public static void Hide(string type)
    {
        AppModuleProxy appModule = _moduleDictionary[type];
        appModule.Hide();
        if (_showingList.Contains(appModule))
            _showingList.Remove(appModule);
        //if (_showingList.Count == 0)
        //    Show(ModuleType.LOGIN_PANEL);

        DispathcEvent(ModuleEvent.HIDE, type);
    }

    //----------------------------------------
    //event Listener
    //----------------------------------------

    private static EventDispatcher _eventDispatcher = new EventDispatcher();

    public static bool HasEventListener(string type)
    {
        return _eventDispatcher.HasEventListener(type);
    }

    public static void AddEventListener(string type, Action<BaseEvent> listener)
    {
        _eventDispatcher.AddEventListener(type, listener);
    }

    public static void RemoveEventListener(string type, Action<BaseEvent> listener)
    {
        _eventDispatcher.RemoveEventListener(type, listener);
    }

    public static void DispathcEvent(string type, object eventObj = null)
    {
        _eventDispatcher.DispatchEvent(type, eventObj);
    }
}
