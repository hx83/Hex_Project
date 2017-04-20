using UnityEngine;
using System.Collections;
using System.IO;

public class AssetPathUtil
{
    //.assetBundle
    public static string abSuffix = ".assetBundle";

    public static string XML_SUFFIX = ".xml";

    public static string BYTES_SUFFIX = ".bytes";

    public static string TXT_SUFFIX = ".txt";

    public static string configPath = "config/";

    public static string audioPath = "Audio/GeneratedSoundBanks";

    private static string _prefabPath = "Prefabs/";

    private static string _modelPath = "Prefabs/Model/";
    //地图素材
    private static string _mapPath = "Prefabs/Map/";
    private static string _playerPath = "Prefabs/Player/";

    private static string _effectPath = _modelPath + "Effect/";

    private static string _uiPath = "Prefabs/UI/";

    private static string _xmlPath = "Xml/";

    private static string _streamingAsset;

    private static string _persistentAsset;

    private static string _abStreamAsset;

    public static void Setup()
    {
        //初始化本地URL
#if UNITY_ANDROID && !UNITY_EDITOR
        _streamingAsset = Application.streamingAssetsPath;
        //Android 比较特别
#else
        _streamingAsset = "file://" + Application.streamingAssetsPath;
        //此url 在 windows 及 WP IOS  可以使用   
#endif
        _abStreamAsset = Application.streamingAssetsPath + "/" + PlatformString + "/";
        _persistentAsset = Application.persistentDataPath + "/rescache/";
    }
    public static string StreamingAssetPath
    {
        get
        {
            return _streamingAsset;
        }
    }
    /// <summary>
    /// 缓存加载路径
    /// </summary>
    public static string PersistentDataPath
    {
        get
        {
            return _persistentAsset;
        }
    }

    public static string GetConfig(string name)
    {
        return StreamingAssetPath + "/config/" + name;
    }

    public static string GetPrefab(string name)
    {
        return _prefabPath + name;
    }
    public static string GetXML(string name)
    {
        return _xmlPath + name;
    }

    public static string GetModel(string name)
    {
        return _modelPath + name;
    }
	
    /// <summary>
    /// 获取面板资源路径
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string GetUI(string name)
    {
        return _uiPath + name;
    }
		
    public static string GetEffectPath(string name)
    {
        return _effectPath + name;
    }
    /// <summary>
    /// 获取地图素材
    /// </summary>
    /// <param name="mapID"></param>
    /// <returns></returns>
    public static string GetMapPath(uint mapID)
    {
        return _mapPath + mapID;
    }
    /// <summary>
    /// 获取角色素材
    /// </summary>
    /// <param name="skinID"></param>
    /// <returns></returns>
    public static string GetPlayerPath(uint skinID)
    {
        return _playerPath + skinID;
    }

    /// <summary>
    /// 获取AssetBundle路径
    /// </summary>
    /// <param name="abName"></param>
    /// <returns></returns>
    public static string GetAssetBundlePath(string abName)
    {
        return _abStreamAsset + abName;
    }

    public static string GetStreamAssetPath(string name)
    {
        return _streamingAsset + "/" + PlatformString + "/" + name;
    }

    public static string PlatformString
    {
        get
        {
#if UNITY_STANDALONE_WIN
                return "windows";
#elif UNITY_IPHONE
                return "iOS";
#elif UNITY_ANDROID
                return "android";
#else
                return "unknown";
#endif
        }
    }
}
