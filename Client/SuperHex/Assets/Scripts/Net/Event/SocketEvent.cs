using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SocketEvent:ISyncExcute
{
    public const string CONNECT = "Connect";
    public const string CONNECT_FAILED = "ConnectFaild";
    public const string DISCONNECT = "Disconnect";
    public int cmdID;
    private byte[] _bytes;
    private Action<object> _callback;
    private Type _type;
    private object _instance;
    public SocketEvent()
    {
    }

    public void Reset<T>(Action<object> callback)
    {
        _type = typeof(T);
        _callback = callback;
    }

    public void Reset(Action<object> callback)
    {
        _type = null;
        _callback = callback;
    }

    public void Reset(Action<object> callback, Type type)
    {
        _type = type;
        _callback = callback;
    }

    public void SetByteArr(ByteArray byteArr)
    {
        if(byteArr != null)
            _bytes = byteArr.Bytes;
    }

    public bool HasBody()
    {
        if (_bytes == null)
            return false;
        if (_bytes.Length == 0)
            return false;
        return true;
    }

    public bool HasType
    {
        get
        {
            return _type != null;
        }
    }

    public void Clear()
    {
        _bytes = null;
    }

    public SocketEvent Clone()
    {
        SocketEvent evt = new SocketEvent();
        evt.Reset(_callback, _type);
        return evt;
    }

    public void SyncExcute()
    {
        if (!HasType || !HasBody())
        {
            MainEntry.RunInNextFrame(OnExcuteOver);
            return;
        }
        MemoryStream stream = new MemoryStream();
        stream.Write(_bytes, 0, _bytes.Length);
        stream.Position = 0;
        _instance = RuntimeTypeModel.Default.Deserialize(stream, null, _type);
        stream.Dispose();
        MainEntry.RunInNextFrame(OnExcuteOver);
    }

    private void OnExcuteOver()
    {
        //Debug.Log("cmd id:" + cmdID + "excute over time:" + DateTime.Now.Millisecond);
        //if (HasType && HasBody())
        //{
        //    MemoryStream stream = new MemoryStream();
        //    stream.Write(_bytes, 0, _bytes.Length);
        //    stream.Position = 0;
        //    _instance = RuntimeTypeModel.Default.Deserialize(stream, null, _type);
        //    stream.Dispose();
        //}
        _callback.Invoke(_instance);
        Clear();
    }

    public static SocketEvent Get()
    {
        if (_cache.Count > 0)
            return _cache.Dequeue();
        return new SocketEvent();
    }

    public static void Cache(SocketEvent socketEvent)
    {
        socketEvent.Clear();
        _cache.Enqueue(socketEvent);
    }

    private static Queue<SocketEvent> _cache = new Queue<SocketEvent>();
}
