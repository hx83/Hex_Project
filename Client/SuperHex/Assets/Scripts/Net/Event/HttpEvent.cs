using System.IO;

public class HttpEvent:BaseEvent
{
    private MemoryStream _bodyStream;
    public HttpEvent(string type, MemoryStream bodyMStream = null):base(type, null)
    {
        _bodyStream = bodyMStream;
    }

    public MemoryStream BodyStream
    {
        get
        {
            return _bodyStream;
        }
    }

    public bool HasBody()
    {
        if (BodyStream == null)
            return false;
        if (BodyStream.Length == 0)
            return false;
        return true;
    }

    public T GetData<T>()
    {
        BodyStream.Position = 0;
        return ProtoBuf.Serializer.Deserialize<T>(BodyStream);
    }
}
