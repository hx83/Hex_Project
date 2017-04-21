using System.IO;

public class ReceivePackage
{
    private int _cmdID;
    private MemoryStream _bodyStream;

    public ReceivePackage(int cmdID, MemoryStream bodyStream = null)
    {
        _cmdID = cmdID;
        _bodyStream = bodyStream;
    }

    public int CmdID
    {
        get
        {
            return _cmdID;
        }
    }

    public MemoryStream BodyStream
    {
        get
        {
            return _bodyStream;
        }
    }
}
