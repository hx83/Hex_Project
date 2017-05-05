using UnityEngine;
using System.Collections;
using System;

public class GlobalBehavior : MonoBehaviour 
{
    private static System.Action _nextFrameCall;
	// Use this for initialization
	public static void Setup()
    {
        GameObject obj = new GameObject();
        obj.name = "globalBehavior";
        GameObject.DontDestroyOnLoad(obj);

        obj.AddComponent<GlobalBehavior>();
    }
	
	// Update is called once per frame
	void Update () 
    {
        if (_nextFrameCall != null)
        {
            //_nextFrameCall.Invoke();
            Delegate[] list = _nextFrameCall.GetInvocationList();
            if (list != null)
            {
                int count = list.Length;
                for (int i = 0; i < count; i++)
                {
                    System.Action function = (System.Action)list[i];
                    _nextFrameCall -= function;
                    function.Invoke();
                }
                //_nextFrameCall = null;
            }
        }
	}


    public static void RunInNextFrame(System.Action function)
    {
        if (function != null)
            _nextFrameCall += function;
    }
}
