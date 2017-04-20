using System;
/// <summary>
/// ACE
/// 全局事件
/// </summary>
public static class GlobalEventDispatcher
{
    public static void AddEventListener(string type, Action<BaseEvent> listener)
    {
        _dispatcher.AddEventListener(type, listener);
    }

    public static void DispatchEvent(string type, object eventObj = null)
    {
        _dispatcher.DispatchEvent(type, eventObj);
    }

    public static bool HasEventListener(string type)
    {
        return _dispatcher.HasEventListener(type);
    }

    public static void RemoveAllEventListender()
    {
        _dispatcher.RemoveAllEventListender();
    }

    public static void RemoveEventListener(string type, Action<BaseEvent> listener)
    {
        _dispatcher.RemoveEventListener(type, listener);
    }

    private static EventDispatcher _dispatcher = new EventDispatcher();
}
