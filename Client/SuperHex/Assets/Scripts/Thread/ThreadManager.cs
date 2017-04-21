using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ThreadManager
{
    private static Thread _thread;
    private static uint _index;
    private static Queue<ISyncExcute> _queue;
    private static bool _isStart;
    public static void Start()
    {
        _queue = new Queue<ISyncExcute>();
        if(_thread == null)
        {
            _thread = new Thread(new ThreadStart(Excute));
            _thread.IsBackground = true;
        }
        _isStart = true;
        _thread.Start();
    }

    public static void Stop()
    {
        _isStart = false;
        if(_thread != null && _thread.IsAlive)
        {
            _thread.Abort();
        }
    }

    public static void AddItem(ISyncExcute syncItem)
    {
        _queue.Enqueue(syncItem);
    }

    public static void Clear()
    {
        _queue.Clear();
    }

    private static void Excute()
    {
        while(_isStart)
        {
            try
            {
                if (_queue.Count > 0)
                {
                    ISyncExcute syncItem = _queue.Dequeue();
                    syncItem.SyncExcute();
                }
            }
            catch (Exception e)
            {
                Debug.LogError("thread error " + e.Message);
            }
        }
    }
}
