using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class SocketExtend
{

    private const int MAX_PACKAGE_LENGTH = 65535;
    private const int PACKAGE_HEAD_LENGTH = 6;
    private const int COMMAND_HEAD_LENGTH = 2;

    private string _ip;
    private int _port;

    private Socket _clientSocket;
    private List<SendPackage> _sendPackages = new List<SendPackage>();
    private byte[] _receiveBytes = new byte[MAX_PACKAGE_LENGTH];
    private ByteArray _receiveByteArray = new ByteArray();

    private bool _connected;
    private bool _getHeading;
    private ByteArray _sendData = new ByteArray();
    private List<ByteArray> _receiveCallbackDataList = new List<ByteArray>();

    private EventDispatcher _eventDispatcher = new EventDispatcher();
    private Dictionary<int, Dictionary<Action<object>, SocketEvent>> _cmdEventDic = new Dictionary<int, Dictionary<Action<object>, SocketEvent>>();
    public string IP
    {
        set
        {
            _ip = value;
        }
        get
        {
            return _ip;
        }
    }

    public int Port
    {
        set
        {
            _port = value;
        }
        get
        {
            return _port;
        }
    }

    public bool IsConnected
    {
        get
        {
            if (_clientSocket != null && _connected)
            {
                bool inReadState = _clientSocket.Poll(1000, SelectMode.SelectRead);
                bool available = _clientSocket.Available == 0;
                if ((inReadState && available) || !_clientSocket.Connected)
                {
                    Debug.Log("Socket Disconnected....");
                    return false;
                }
                return true;
            }
            return false;
        }
    }

    public void Connect(string ip, int port)
    {
        Close();
        _ip = ip;
        _port = port;

        try
        {
#if UNITY_IPHONE && !UNITY_EDITOR
            IOSSocketClient(_ip, _port);
#else
            IPEndPoint address = new IPEndPoint(IPAddress.Parse(_ip), _port);
            _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _clientSocket.BeginConnect(address, new AsyncCallback(ConnectCallback), _clientSocket);
#endif
        }
        catch (SocketException socketEx)
        {
            if (CanLog())
            {
                Log("Connect.....[" + socketEx.ErrorCode + "]" + socketEx.Message);
            }
            DispatchEvent(SocketEvent.CONNECT_FAILED);
        }
        catch (Exception ex)
        {
            Debug.Log("Connect Exception..." + ex.Message);
            DispatchEvent(SocketEvent.CONNECT_FAILED);
            throw ex;
        }
    }

    private void IOSSocketClient(String serverIp, int serverPorts)
    {
        String newServerIp = "";
        AddressFamily newAddressFamily = AddressFamily.InterNetwork;
        getIPType(serverIp, serverPorts, out newServerIp, out newAddressFamily);
        if (!string.IsNullOrEmpty(newServerIp))
        {
            serverIp = newServerIp;
        }
        _clientSocket = new Socket(newAddressFamily, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint address = new IPEndPoint(IPAddress.Parse(serverIp), serverPorts);
        _clientSocket.BeginConnect(address, new AsyncCallback(ConnectCallback), _clientSocket); //建立异步连接服务 , Connected 进行监听
    }

    private void getIPType(String serverIp, int serverPorts, out String newServerIp, out AddressFamily mIPType)
    {
        mIPType = AddressFamily.InterNetwork;
        newServerIp = serverIp;
        try
        {
            string mIPv6 = GetIPv6(serverIp, serverPorts.ToString());
            if (!string.IsNullOrEmpty(mIPv6))
            {
                string[] m_StrTemp = System.Text.RegularExpressions.Regex.Split(mIPv6, "&&");
                if (m_StrTemp != null && m_StrTemp.Length >= 2)
                {
                    string IPType = m_StrTemp[1];
                    if (IPType == "ipv6")
                    {
                        newServerIp = m_StrTemp[0];
                        mIPType = AddressFamily.InterNetworkV6;
                    }
                }
            }
        }
        catch (Exception error)
        {
            Debug.Log("GetIPv6 error:" + error);
        }
    }

    //处理ios ipv6 兼容
#if UNITY_IPHONE
        [DllImport("__Internal")]
	    private static extern string getIPv6(string mHost, string mPort);  
#endif

    //"192.168.1.1&&ipv4"
    private string GetIPv6(string mHost, string mPort)
    {
#if UNITY_IPHONE && !UNITY_EDITOR
		string mIPv6 = getIPv6(mHost, mPort);
		return mIPv6;
#else
        return mHost + "&&ipv4";
#endif
    }

    public void Close(bool dispatchEvent = false)
    {
        _sendPackages.Clear();
        _receiveByteArray.Clear();
        
        _connected = false;
        _getHeading = true;
        try
        {
            if (_clientSocket != null)
            {
                if (_clientSocket.Connected)
                {
                    _clientSocket.Shutdown(SocketShutdown.Both);
                    _clientSocket.Close();
                }
                _clientSocket = null;
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Close....[" + socketException.ErrorCode + "]" + socketException.Message);
        }
        catch (Exception exception)
        {
            Debug.Log("Socket Close Exception " + exception.Message);
            throw exception;
        }
        if (dispatchEvent)
        {
            Debug.Log("Socket Disconnected.... [" + _ip + ":" + _port.ToString() + "]");
            //MainEntry.RunInNextFrame(Update) ;
            DispatchEvent(SocketEvent.DISCONNECT);
        }
    }

    public void Send(int type)
    {
        SendPackage temp = new SendPackage(type);
        _sendPackages.Add(temp);

        StartSend();
    }

    public void Send(int type, ByteArray data)
    {
        SendPackage temp = new SendPackage(type, data.Bytes);
        _sendPackages.Add(temp);

        StartSend();
    }

    public void Send<T>(int type, T msg) where T : global::ProtoBuf.IExtensible
    {
        MemoryStream stream = new MemoryStream();
        ProtoBuf.Serializer.Serialize<T>(stream, msg);

        byte[] bytes = new byte[stream.Length];
        stream.Position = 0;
        stream.Read(bytes, 0, (int)stream.Length);

        SendPackage temp = new SendPackage(type, bytes);
        _sendPackages.Add(temp);

        StartSend();
    }

    private void StartSend()
    {
        if (_connected)
        {
            if (_sendPackages.Count == 0)
                return;
            if (_clientSocket == null)
                return;
            try
            {
                ByteArray bytes = _sendData;
                bytes.Clear();

                while (_sendPackages.Count > 0)
                {
                    SendPackage sendData = _sendPackages[0];
                    _sendPackages.RemoveAt(0);

                    //ByteArray package = CmdUtil.GetPackage(sendData.CmdID, (uint)TimeManager.ServerTime, sendData.Body);
                    ByteArray package = CmdUtil.GetPackage(sendData.CmdID, 0, sendData.Body);
                    if (CanLog())
                    {
                        //Log(">>>> [" + _ip + ":" + _port + "][CmdID:" + sendData.CmdID + "][Data Length:" + (package.Length - COMMAND_HEAD_LENGTH) + "]");
                    }
                    bytes.WriteBytes(package);
                }

                _clientSocket.BeginSend(bytes.Bytes, 0, bytes.Length, SocketFlags.None, new AsyncCallback(SendCallback), _clientSocket);
                if (CanLog())
                {
                    //Log(">>>> [" + _ip + ":" + _port + "][Flush....][Data Length:" + bytes.Length + "]");
                }
            }
            catch (SocketException socketEx)
            {
                Debug.Log("StartSend...[" + socketEx.ErrorCode + "]" + socketEx.Message);
                if (socketEx.NativeErrorCode != 10035)
                {
                    Close(true);
                }
            }
            catch (Exception ex)
            {
                Debug.Log("Start Send Exception " + ex.Message);
                Close(true);
                throw ex;
            }
        }
    }

    private void StartReceive()
    {
        if (_connected)
        {
            try
            {
                _clientSocket.BeginReceive(_receiveBytes, 0, MAX_PACKAGE_LENGTH, SocketFlags.None, new AsyncCallback(ReceiveCallBack), _clientSocket);
            }
            catch (SocketException socketEx)
            {
                Debug.Log("StartReceive.....[" + socketEx.ErrorCode + "]" + socketEx.Message);
                Close(true);
            }
            catch (Exception ex)
            {
                Debug.Log("Start Receive Exception" + ex.Message);
                Close(true);
                throw ex;
            }
        }
    }

    private void ConnectCallback(IAsyncResult result)
    {
        Socket socket = (Socket)result.AsyncState;

        try
        {
            socket.EndConnect(result);
            _connected = true;

            StartReceive();
            StartSend();
            if (CanLog())
            {
                Log("Socket Connected.....[" + _ip + ":" + _port.ToString() + "]");
            }
            DispatchEvent(SocketEvent.CONNECT);
        }
        catch (SocketException socketEx)
        {
            Debug.Log("ConnectCallback....[" + socketEx.ErrorCode + "]" + socketEx.Message);
            DispatchEvent(SocketEvent.CONNECT_FAILED);
        }
        catch (Exception ex)
        {
            Debug.Log("Connect Callback Exception " + ex.Message);
            DispatchEvent(SocketEvent.CONNECT_FAILED);
            throw ex;
        }
    }

    private void SendCallback(IAsyncResult result)
    {
        if (_connected)
        {
            Socket socket = (Socket)result.AsyncState;
            try
            {
                socket.EndSend(result);
            }
            catch (SocketException socketEx)
            {
                Debug.Log("SendCallBack.....[" + socketEx.ErrorCode + "]" + socketEx.Message);
                Close(true);
            }
            catch (Exception ex)
            {
                Debug.Log("Send CallBack exception " + ex.Message);
                Close(true);
                throw ex;
            }
        }
    }

    private uint _packBodyLen;
    private bool _isCompress;
    private int _cmdID;
    private void ReceiveCallBack(IAsyncResult result)
    {
        if (_connected)
        {
            Socket socket = (Socket)result.AsyncState;
            try
            {
                int available = socket.EndReceive(result);
                if (available < 0)
                {
                    Debug.Log("Socket Receive Available <= 0:" + available);
                    Close(true);
                }
                else if(available == 0)
                {
                    StartReceive();
                }
                else
                {
                    _receiveByteArray.WriteBytes(_receiveBytes, available);
                    while (true)
                    {
                        if (_getHeading)
                        {
                            if (_receiveByteArray.Length >= PACKAGE_HEAD_LENGTH)
                            {
                                uint len1 = _receiveByteArray.ReadUnsignedByte();
                                uint len2 = _receiveByteArray.ReadUnsignedByte();
                                uint len3 = _receiveByteArray.ReadUnsignedByte();
                                _packBodyLen = len1 | len2 << 8 | len3 << 16;
                                _isCompress = _receiveByteArray.ReadUnsignedByte() == 1;
                                _cmdID = (int)_receiveByteArray.ReadUnsignedShort();

                                if (_packBodyLen < COMMAND_HEAD_LENGTH || _packBodyLen > MAX_PACKAGE_LENGTH)
                                {
                                    Debug.LogError("收到垃圾包，CMDID:" + _cmdID + " 包体长度:" + _packBodyLen);
                                    _receiveByteArray.Clear();
                                    break;
                                }

                                if(_packBodyLen == COMMAND_HEAD_LENGTH)
                                {
                                    if (CanLog())
                                    {
                                        Log("<<<< [" + _ip + ":" + _port + "][CmdID:" + _cmdID + "][Data Length:0]");
                                    }
                                    //Debug.Log( "当前收到的协议ID = " + _cmdID.ToString() );
                                    DispatchCmdEvent(_cmdID);
                                }
                                else
                                    _getHeading = false;
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            if (_receiveByteArray.Length >= _packBodyLen - COMMAND_HEAD_LENGTH)
                            {
                                ByteArray bodyByteArr = ByteArray.Get();
                                _receiveByteArray.ReadBytes(bodyByteArr, (int)_packBodyLen - COMMAND_HEAD_LENGTH);
                                if (_isCompress)
                                    bodyByteArr.UnCompress();

                                if (CanLog())
                                {
                                    Log("<<<< [" + _ip + ":" + _port + "][CmdID:" + _cmdID + "][Data Length:" + bodyByteArr.Length + "]");
                                }
                                DispatchCmdEvent(_cmdID, bodyByteArr);
                                _getHeading = true;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    StartReceive();
                }
            }
            catch (SocketException socketEx)
            {
                Debug.LogError("ReceiveCallBack.....[" + socketEx.ErrorCode + "]" + socketEx.Message);
                Close(true);
            }
            catch (Exception ex)
            {
                Debug.LogError("Receive CallBack Exception " + ex.Message);
                Close(true);
                throw ex;
            }
        }
    }

    private const int maxDispatchCmdCount = 50;
    public void Update()
    {
        //int count = _dispatchCmdCacheList.Count;
        //if (count > maxDispatchCmdCount)
        //    count = maxDispatchCmdCount;
        //for (int i = 0; i < count; i++)
        //{
        //    _cmdDispatcher.DispatchEventInstance(_dispatchCmdCacheList[i]);
        //    SocketEvent.Cache(_dispatchCmdCacheList[i]);
        //}
        //_dispatchCmdCacheList.RemoveRange(0, count);
    }

    struct DispatchCmdCache
    {
        public int cmdID;
        public ByteArray data;
    }

    private bool CanLog()
    {
        return Global.IsDebug && Global.IsTraceSocketLog;
    }

    private void Log(string message)
    {
        Debug.Log(message + "...." + DateTime.Now.Millisecond);
    }

    public bool HasCmdListener(int cmdID)
    {
        return _cmdEventDic.ContainsKey(cmdID);
    }

    public void AddCmdListener<T>(int cmdID, Action<object> listener)
    {
        Dictionary<Action<object>, SocketEvent> eventListenrDic;
        _cmdEventDic.TryGetValue(cmdID, out eventListenrDic);
        if (eventListenrDic == null)
        {
            eventListenrDic = new Dictionary<Action<object>, SocketEvent>();
            _cmdEventDic.Add(cmdID, eventListenrDic);
        }
        if (eventListenrDic.ContainsKey(listener))
            return;
        SocketEvent evt =  new SocketEvent();
        evt.Reset<T>(listener);
        eventListenrDic.Add(listener, evt);
    }

    public void AddCmdListener(int cmdID, Action<object> listener)
    {
        Dictionary<Action<object>, SocketEvent> eventListenrDic;
        _cmdEventDic.TryGetValue(cmdID, out eventListenrDic);
        if (eventListenrDic == null)
        {
            eventListenrDic = new Dictionary<Action<object>, SocketEvent>();
            _cmdEventDic.Add(cmdID, eventListenrDic);
        }
        if (eventListenrDic.ContainsKey(listener))
            return;
        SocketEvent evt = new SocketEvent();
        evt.Reset(listener);
        eventListenrDic.Add(listener, evt);
    }

    public void RemoveCmdListener(int cmdID, Action<object> listener)
    {
        Dictionary<Action<object>, SocketEvent> eventListenrDic;
        _cmdEventDic.TryGetValue(cmdID, out eventListenrDic);
        if (eventListenrDic == null)
            return;
        if(eventListenrDic.ContainsKey(listener))
            eventListenrDic.Remove(listener);
    }

    public void DispatchCmdEvent(int cmdID, ByteArray data = null)
    {
        Dictionary<Action<object>, SocketEvent> eventListenrDic;
        _cmdEventDic.TryGetValue(cmdID, out eventListenrDic);
        if (eventListenrDic != null)
        {
            //Debug.Log("cmd id" + cmdID + " time:" + DateTime.Now.Millisecond);
            foreach(SocketEvent evt in eventListenrDic.Values)
            {
                SocketEvent copy = evt.Clone();
                copy.cmdID = cmdID;
                copy.SetByteArr(data);
                copy.SyncExcute();
                //ThreadManager.AddItem(copy);
            }
        }
        if (data != null)
            ByteArray.Cache(data);

    }

    public bool HasEventListener(string type)
    {
        return _eventDispatcher.HasEventListener(type);
    }

    public void AddEventListener(string type, Action<BaseEvent> listener)
    {
        _eventDispatcher.AddEventListener(type, listener);
    }

    public void RemoveEventListener(string type, Action<BaseEvent> listener)
    {
        _eventDispatcher.RemoveEventListener(type, listener);
    }

    public void DispatchEvent(String type)
    {
        GlobalBehavior.RunInNextFrame(() =>
        {
            _eventDispatcher.DispatchEvent(type);
        });
    }

    public void RemoveAllEventListender()
    {
        _eventDispatcher.RemoveAllEventListender();
    }

}
