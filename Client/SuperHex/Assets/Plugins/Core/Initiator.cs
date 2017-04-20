using System;
using System.Reflection;
using UnityEngine;
using System.Xml;
using System.Collections;
using System.IO;
using System.Collections.Generic;
/// <summary>
/// 启动程序
/// </summary>
public class Initiator : MonoBehaviour
{
    [HideInInspector]
    public bool closeHotUpdate = false;//是否关闭热更新
    [HideInInspector]
    public bool isDebug;
    [HideInInspector]
    public string serverIP;
    [HideInInspector]
    public string serverPort;
    [HideInInspector]
    public string cdnUrl;
    [HideInInspector]
    public string homeUrl;
    [HideInInspector]
    public string surveyUrl;
    //是否开启麦克风
    [HideInInspector]
    public bool isOpenMicrophone = true;
    //---------------------------------------公告相关
    private int noticeType;
    private string noticeTitle;
    private string noticeContent;
    public int NOTICETYPE { get { return noticeType; } }
    public string NOTICETITLE { get { return noticeTitle; } }
    public string NOTICECONTENT { get { return noticeContent; } }
    private NoticeUI m_NOTICEUI;
    public NoticeUI NOTICEUI { get { return m_NOTICEUI; } }
    //---------------------------------------
    //

    private readonly string _dllName = "Assembly-CSharp";
    private readonly string _versionFileName = "version.bytes";
    private SceneLoadingPanel _loadingView;

    private bool _isForceUpdate;
    private string _channel;
    private int _cdnRetryTimes;

    public void Awake()
    {
        _instance = this;
        Debuger.EnableLog = false;
        Application.targetFrameRate = 50;
        isOpenMicrophone = true;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        //保留主程序
        GameObject.DontDestroyOnLoad(gameObject);
        VersionManager.localVersion = Application.version;
        //Bugly、友盟等对接
        SDKManager.SetupBugly();
        //
        LayerManager.Setup();
        AssetPathUtil.Setup();
        LoaderManager.Setup();
        AlertUI.Setup();
        PopManager.Setup();
        /**
        _loadingView = GameObject.Find("UI_Loading").AddComponent<SceneLoadingPanel>();
        m_NOTICEUI = LayerManager.topUILayer.Find("NoticeUI").gameObject.AddComponent<NoticeUI>();
        m_NOTICEUI.Online();
        _loadingView.SetTitle("正在检查游戏版本...");
        */
    }

    //IEnumerator Start()
    void Start()
    {
        /**
        //从本地读取游戏配置文件 
        string mainXml = AssetPathUtil.StreamingAssetPath + "/Main" + AssetPathUtil.XML_SUFFIX;
        WWW www = new WWW(mainXml);
        yield return www;

        XmlDocument configDoc = new XmlDocument();
        configDoc.LoadXml(www.text);

        XmlNode node = configDoc.SelectSingleNode("root").SelectSingleNode("content");
        XmlElement elementXml = node as XmlElement;
        isDebug = elementXml.GetAttribute("isDebug") == "1";
        serverIP = elementXml.GetAttribute("ip");
        serverPort = elementXml.GetAttribute("port");
        closeHotUpdate = elementXml.GetAttribute("closeHotUpdate") == "1";
        cdnUrl = elementXml.GetAttribute("cdnURL");
        homeUrl = elementXml.GetAttribute("homeURL");
        surveyUrl = elementXml.GetAttribute("surveyURL");
        _channel = elementXml.GetAttribute("channel");
        SDKManager.SetupUmeng(_channel);

        Debug.Log("server:" + serverIP + ":" + serverPort + " cdn:" + cdnUrl);
        _loadingView.SetPercent(0.25f);
        if (isDebug)
        {
            Debug.Log("closeHotUpdate start game");
            StartGame();
        }
        else if (closeHotUpdate)
        {
            //读取资源服务器版本文件
            StartCoroutine(DownloadConfigFromCDN());
        }
        else
        {
            Debug.Log("load local version file: " + AssetPathUtil.GetStreamAssetPath("version.bytes"));
            //读取本地版本文件
            LoaderManager.Load(AssetPathUtil.GetStreamAssetPath("version.bytes"), EnumResouceType.BINARY, OnLocalVersionLoaded);
        }
         */



        StartGame();
    }

    private void OnLocalVersionLoaded(object content)
    {
        Debug.Log("local version success");
        VersionManager.ParseLocalVersionFile(content as byte[]);
        string cacheVerUrl = AssetPathUtil.PersistentDataPath + AssetPathUtil.PlatformString + "/" + _versionFileName;
        if (File.Exists(cacheVerUrl))
        {
            Debug.Log("load cache ");
            //cacheVerUrl = AssetPathUtil.PersistentDataPath + AssetPathUtil.PlatformString + "/" + _versionFileName;
            //读取本地缓存版本文件
            LoaderManager.Load(cacheVerUrl, EnumResouceType.CACHE_BINARY, OnCacheVersionLoaded);
        }
        else
        {
            //读取资源服务器版本文件
            StartCoroutine(DownloadConfigFromCDN());
        }
    }

    private void OnCacheVersionLoaded(object content)
    {
        Debug.Log("load cache success");
        VersionManager.ParseCacheVersionFile(content as byte[]);
        StartCoroutine(DownloadConfigFromCDN());
    }

    //从CDN上读取解析 IP port hotUpdateURL homeURL
    private IEnumerator DownloadConfigFromCDN()
    {
        Debug.Log("download config from cdn: " + cdnUrl + "Main" + AssetPathUtil.XML_SUFFIX);
        WWW www = new WWW(cdnUrl + "Main" + AssetPathUtil.XML_SUFFIX);
        yield return www;

        if (www.error == null)
        {
            XmlDocument configDoc = new XmlDocument();
            XmlElement elementXml;
            XmlNodeList nodeList;
            configDoc.LoadXml(www.text);
            nodeList = configDoc.SelectSingleNode("root").ChildNodes;
            for (int i = 0; i < nodeList.Count; i++)
            {
                elementXml = (XmlElement)nodeList[i];
                
                if (elementXml.Name == "randomIP")
                {
                    int randomLoginIPIndex = UnityEngine.Random.Range(0, elementXml.ChildNodes.Count);
                    XmlElement node = (XmlElement)elementXml.ChildNodes[randomLoginIPIndex];
                    serverIP = node.GetAttribute("IP");
                    serverPort = node.GetAttribute("port");
                }
                else if (elementXml.Name == "content")
                {
                    VersionManager.remoteVersion = elementXml.GetAttribute("ver");
                    _isForceUpdate = elementXml.GetAttribute("isForceUpdate") == "1";
                    homeUrl = elementXml.GetAttribute("homeURL");
                    if (elementXml.HasAttribute(_channel))
                        homeUrl = elementXml.GetAttribute(_channel);
                    if (elementXml.HasAttribute("microphone"))
                        isOpenMicrophone = elementXml.GetAttribute("microphone") == "1";
                    if (elementXml.HasAttribute(_channel + "_ip"))
                    {
                        serverIP = elementXml.GetAttribute(_channel + "_ip");
                        serverPort = elementXml.GetAttribute(_channel + "_port");
                    }
                }
                else if (elementXml.Name == "notice")
                {
                    noticeType = int.Parse(elementXml.GetAttribute("noticeType"));
                    noticeTitle = elementXml.GetAttribute("noticeTitle");
                    noticeContent = elementXml.GetAttribute("noticeContent");
                    Debug.Log(noticeType + "," + noticeTitle + "," + noticeContent);
                }
            }


            Debug.Log("remote xml:" + serverIP + " " + serverPort + " " + Application.version + " home:" + homeUrl);
            Debug.Log("本地版本:" + VersionManager.localVersion + " 远程版本" + VersionManager.remoteVersion);
            //是否强更
            if (_isForceUpdate || VersionManager.CheckNeedForceUpdate(VersionManager.localVersion, VersionManager.remoteVersion))
            {
                Debug.Log("版本过旧");
                AlertUI.Show("<color='#CC3333'>当前游戏不是最新版本</color>\n\n请点击【确认】按钮下载最新游戏客户端", GotoHomeURL);
            }
            else
            {
                //检查是否需要热更新
                if (!closeHotUpdate && VersionManager.CheckVersionIsOld(VersionManager.localVersion, VersionManager.remoteVersion))
                {
                    //创建配置缓存目录
                    string configPath = AssetPathUtil.PersistentDataPath + AssetPathUtil.configPath;
                    if (!Directory.Exists(configPath))
                        Directory.CreateDirectory(configPath);
                    //创建音效缓存目录
                    string audioPath = AssetPathUtil.PersistentDataPath + AssetPathUtil.audioPath + "/" + AssetPathUtil.PlatformString;
                    if (!Directory.Exists(audioPath))
                        Directory.CreateDirectory(audioPath);
                    //创建AB资源缓存目录
                    string abPath = AssetPathUtil.PersistentDataPath + AssetPathUtil.PlatformString;
                    if (!Directory.Exists(abPath))
                        Directory.CreateDirectory(abPath);
                    //加载资源服务器版本文件
                    LoaderManager.Load(cdnUrl + "asset/" + AssetPathUtil.PlatformString + "/" + _versionFileName, EnumResouceType.BINARY, OnCDNVersionFileLoaded);
                }
                else
                {
                    StartGame();
                }
            }
        }
        else
        {
            if (_cdnRetryTimes == 10)
                Debug.LogError(www.error);
            _cdnRetryTimes++;
            if (_cdnRetryTimes > 10)
            {
                _loadingView.SetTitle("网络无法连接，请检查您的网络设置...");
            }
            Debug.Log("正在尝试重新请求CDN....次数:" + _cdnRetryTimes);
            StartCoroutine(DownloadConfigFromCDN());
        }
    }

    private uint _downloadTotal = 0;
    private int _totalCount;
    private int _loadedCount;
    private Dictionary<string, FileVerInfo> _loadFileDic;
    private bool _isStartUpdate;
    private List<FileVerInfo> _downloadList;
    private void OnCDNVersionFileLoaded(object content)
    {
        _downloadList = VersionManager.ParseRemoteVersionFile(content as byte[]);
        _loadFileDic = new Dictionary<string, FileVerInfo>();
        _totalCount = _downloadList.Count;
        if (_totalCount == 0)
        {
            VersionManager.localVersion = VersionManager.remoteVersion;
            VersionManager.SaveCacheVersionFile(false);
            StartGame();
        }
        else
        {
            foreach (FileVerInfo info in _downloadList)
            {
                _downloadTotal += info.size;
            }
            uint downloadTotalKB = _downloadTotal / 1024;
            string sizeTips;
            if (downloadTotalKB > 1024)
            {
                sizeTips = (uint)(downloadTotalKB / 1024f) + "MB";
            }
            else
                sizeTips = downloadTotalKB + "KB";
            AlertUI.Show("发现新版本，本次更新将消耗" + sizeTips + "流量。", StartDownloadNewRes);
            _loadingView.SetPercent(0);
        }
    }

    private void StartDownloadNewRes()
    {
        _isStartUpdate = true;
        foreach (FileVerInfo info in _downloadList)
        {
            string url = cdnUrl + "asset/" + info.relativePath;
            _loadFileDic.Add(url, info);
            LoaderManager.Load(url, EnumResouceType.REMOTE_BINARY, OnNewResLoadComplete, OnNewResLoadError);
        }
        _loadingView.SetTitle("更新中...");
        //此处最多到99%，后续的留给检查公告、登陆

    }

    private void OnNewResLoadComplete(object content)
    {
        WWW www = content as WWW;
        FileVerInfo loadedFile = _loadFileDic[www.url];
        string relativePath = loadedFile.relativePath;
        string outputPath = AssetPathUtil.PersistentDataPath + relativePath;
        using (FileStream fileStream = System.IO.File.Create(outputPath))
        {
            fileStream.Write(www.bytes, 0, www.bytes.Length);
            fileStream.Flush();
            fileStream.Close();
        }
        VersionManager.AddCacheFileInfo(loadedFile);
        _loadedCount++;
        _loadingView.SetPercent((float)_loadedCount / _totalCount - 0.01f);
        if (_loadedCount == _totalCount)
        {
            _isStartUpdate = false;
            _loadingView.SetTitle("正在解压资源，不消耗任何流量。");
            VersionManager.localVersion = VersionManager.remoteVersion;
            VersionManager.SaveCacheVersionFile(false);
            StartGame();
        }
    }
    /// <summary>
    /// 加载失败重试
    /// </summary>
    /// <param name="path"></param>
    private void OnNewResLoadError(string path)
    {
        _loadingView.SetTitle("网络中断，正重新尝试加载，请检查您的网络");
        LoaderManager.Load(path, EnumResouceType.REMOTE_BINARY, OnNewResLoadComplete, OnNewResLoadError);
    }

    private void StartGame()
    {
        if (noticeType.Equals(1))
        {
            //---------------------- 维护状态判断 ------------------------------
            Debug.Log("维护状态判断");
            NOTICEUI.Open(NOTICETITLE, NOTICECONTENT);
            _loadingView.SetTitle("服务器正在维护中。。。请稍后再试");
            return;
        }
        _loadingView = null;
#if UNITY_ANDROID// && !UNITY_EDITOR
        
        string dllRelativePath = _dllName + ".bytes";
        string path;
        EnumResouceType type;
        if (VersionManager.HasCache(_dllName + ".bytes"))
        {
            path = VersionManager.GetPath(dllRelativePath);
            type = EnumResouceType.CACHE_BINARY;
            Debug.Log("DLL load from cache");
        }
        else
        {
            path = AssetPathUtil.StreamingAssetPath + "/" + dllRelativePath;
            type = EnumResouceType.BINARY;
            Debug.Log("DLL load from local");
        }
        LoaderManager.Load(path, type, OnDllLoadComplete);
        //Assembly ass = Assembly.Load(_dllName);
        //Type T = ass.GetType("MainEntry");
        //gameObject.AddComponent(T);
#else
        Assembly ass = Assembly.Load(_dllName);
        Type T = ass.GetType("MainEntry");
        gameObject.AddComponent(T);
#endif
    }

    private void OnDllLoadComplete(object content)
    {
        var acDLL = Assembly.Load(content as byte[]);
        if (acDLL.GetType("MainEntry") != null)
        {
            Type T = acDLL.GetType("MainEntry");
            gameObject.AddComponent(T);
        }
    }

    private void GotoHomeURL()
    {
        Application.OpenURL(homeUrl);
    }

    public void OnApplicationQuit()
    {
        if (_isStartUpdate)
            VersionManager.SaveCacheVersionFile(true);
    }

    public string Channel
    {
        get
        {
            return _channel;
        }
    }

    public static Initiator instance
    {
        get
        {
            return _instance;
        }
    }
    private static Initiator _instance;
}
