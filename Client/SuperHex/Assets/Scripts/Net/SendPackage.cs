public class SendPackage
{
    private int _cmdID;
    private byte[] _body;

    public SendPackage(int cmdID, byte[] body = null)
    {
        _cmdID = cmdID;
        _body = body;
    }

    public int CmdID
    {
        get
        {
            return _cmdID;
        }
    }

    public byte[] Body
    {
        get
        {
            return _body;
        }
    }
}
