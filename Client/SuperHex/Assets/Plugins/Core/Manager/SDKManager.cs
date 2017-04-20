using UnityEngine;
using System.Collections;
using Umeng;

public class SDKManager 
{

	public static void SetupBugly()
    {
        //初始化Bugly
        BuglyAgent.ConfigDebugMode(true);
#if UNITY_IPHONE || UNITY_IOS
        BuglyAgent.InitWithAppId("900059347");
#elif UNITY_ANDROID
        BuglyAgent.InitWithAppId("900059341");
#endif
        BuglyAgent.EnableExceptionHandler();
        //
        //end Bugly
    }

    public static void SetupUmeng(string channel)
    {
        if (string.IsNullOrEmpty(channel))
            channel = "android";
        //
        //
        //友盟SDK
        GA.StartWithAppKeyAndChannelId("583cfb1382b63576b4001053", channel);
        //调试时开启日志 发布时设置为false
        GA.SetLogEnabled(false);
        //初始化上传工具
        GameAnalysisMgr.Instance.SetUp();
        //
        //end 友盟SDK
        Debug.Log("init channel: " + channel);
    }
}
