using System;
using UnityEngine;
using usercmd;

/**
 * anthor J
 * 
 */
public class TeamSocketManager
{
    public const int RECONNECT_MAX_TIME = 3;
    public const int RECONNECT_DELAY = 10;

    private static bool isReconnect = false;
    private static int reconnectTime = 0;

    public static bool IsReconnect
    {
        set
        {
            isReconnect = value;
        }
        get
        {
            return isReconnect;
        }
    }

    //----------------------------------------//

    private static SocketExtend instance;

    public static SocketExtend Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new SocketExtend();
                instance.AddEventListener(SocketEvent.DISCONNECT, OnDisconnect);
                instance.AddEventListener(SocketEvent.CONNECT_FAILED, OnConnectFailed);
            }
            return instance;
        }
    }

    public static void Connect(string ip, int port)
    {
        Instance.Connect(ip, port);
    }

    public static void Connect(string address)
    {
        string[] ipPort = address.Split(':');
        Instance.Connect(ipPort[0], int.Parse(ipPort[1]));
    }

    public static void Send<T>(object cmdID, T msg) where T : ProtoBuf.IExtensible
    {
        Instance.Send<T>((int)cmdID, msg);
    }

    public static void Send(object cmdID)
    {
        Instance.Send((int)cmdID);
    }

    public static void Send(object cmdID, ByteArray byteArr)
    {
        Instance.Send((int)cmdID, byteArr);
    }

    public static void Close(bool dispatchEvent = false)
    {
        Instance.Close(dispatchEvent);
    }
    private static void OnDisconnect(BaseEvent e)
    {
        ReconnectToServer(false);
    }

    private static void OnConnectFailed(BaseEvent e)
    {
        ReconnectToServer();
    }

    private static void ReconnectToServer(bool delay = true)
    {
        if (isReconnect)
        {
            if (reconnectTime < RECONNECT_MAX_TIME)
            {
                Debug.Log("重连中...");
                reconnectTime++;
            }
            else
            {
            }
        }
    }

    private static void OnVerify()
    {
    }

    public static void BackToLoginPanel()
    {
        isReconnect = false;
        reconnectTime = 0;

        Close();
    }

    //----------------------------------------
    //cmd Listener
    //----------------------------------------

    public static bool HasCmdListener(object cmdID)
    {
        return Instance.HasCmdListener((int)cmdID);
    }

    public static void AddCmdListener(object cmdID, Action<object> listener)
    {
        Instance.AddCmdListener((int)cmdID, listener);
    }

    public static void AddCmdListener<T>(object cmdID, Action<object> listener)
    {
        Instance.AddCmdListener<T>((int)cmdID, listener);
    }

    public static void RemoveCmdListener(object cmdID, Action<object> listener)
    {
        Instance.RemoveCmdListener((int)cmdID, listener);
    }

    //----------------------------------------
    //event Listener
    //----------------------------------------

    public static bool HasEventListener(string type)
    {
        return Instance.HasEventListener(type);
    }

    public static void AddEventListener(string type, Action<BaseEvent> listener)
    {
        Instance.AddEventListener(type, listener);
    }

    public static void RemoveEventListener(string type, Action<BaseEvent> listener)
    {
        Instance.RemoveEventListener(type, listener);
    }

    internal static void Send<T>(TeamCmd joinUserTeam, Action<GameObject> onJoinTeam)
    {
        throw new NotImplementedException();
    }

    internal static void Send<T>(TeamCmd joinUserTeam, object onJoinTeamBy)
    {
        throw new NotImplementedException();
    }
}