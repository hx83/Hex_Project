public class RemoteBinaryLoader:BinaryLoader
{
    public RemoteBinaryLoader(string url):base(url)
    {

    }

    public override object GetContent()
    {
        return _www;
    }
}
