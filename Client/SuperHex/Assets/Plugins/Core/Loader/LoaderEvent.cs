public class LoaderEvent:BaseEvent
{
    public const string LOAD_COMPLETE = "loadComplete";
    LoaderEvent(string type, object obj = null) : base(type, obj)
    {

    }

}
