public interface IEventDispatcher
{
    bool HasEventListener(string type);
    void AddEventListener(string type, System.Action<BaseEvent> listener);
    void RemoveAllEventListender();
    void DispatchEvent(string type, object eventObj = null);
    void RemoveEventListener(string type, System.Action<BaseEvent> listener);
}
