using System.Collections.Generic;

public class BaseEvent
{
    public static string DEBUG = "debug";
    public static string COMPLETE = "Complete";
    public static string UPDATE = "Update";
    public static string START = "Start";
    protected string _type;
    protected object _eventObj;

    public BaseEvent(string type, object eventObj = null)
    {
        _type = type;
        _eventObj = eventObj;
    }

    public virtual void Reset(string type, object eventObj = null)
    {
        _type = type;
        _eventObj = eventObj;
    }

    public string Type
    {
        get
        {
            return _type;
        }
    }

    public object EventObj
    {
        get
        {
            return _eventObj;
        }
    }

    public virtual void Clear()
    {
        _eventObj = null;
        _type = "";
    }


    public static BaseEvent GetCache(string type, object eventObj = null)
    {
        if(_pool.Count > 0)
        {
            BaseEvent evt = _pool.Dequeue();
            evt.Reset(type, eventObj);
            return evt;
        }
        return new BaseEvent(type, eventObj);
    }

    public static void Cache(BaseEvent obj)
    {
        obj.Clear();
        if (_pool.Count < _poolMaxSize)
            _pool.Enqueue(obj);
    }

    private static Queue<BaseEvent> _pool = new Queue<BaseEvent>();
    private static int _poolMaxSize = 500;
}
