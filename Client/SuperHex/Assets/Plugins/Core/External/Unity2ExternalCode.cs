using UnityEngine;
using System.Collections;

/// <summary>
/// unity主动调外部的方法
/// </summary>
public class Unity2ExternalCode
{

    public static void GetVersion()
    {
#if UNITY_EDITOR
        
#elif UNITY_ANDROID
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        jo.Call("GetVersion");
#elif UNITY_IPHONE
#endif
    }
}
