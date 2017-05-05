public class Global
{
    //开发模式
    public static bool IsDebug = true;
    //是否打印socket log;
    public static bool IsTraceSocketLog = true;
    //是否使用AssetBundle
#if UNITY_ANDROID || UNITY_IOS
    public static bool IsUseAB = false;
#else
    public static bool IsUseAB = false;
#endif

    public static readonly int china = 156;
}
