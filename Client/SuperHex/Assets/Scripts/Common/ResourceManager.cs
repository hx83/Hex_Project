using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// ACE
/// 资源管理类
/// </summary>
public class ResourceManager
{
    ///Resource下相对路径，AssetBundle名字
    private static Dictionary<string, string> _abAssetPathDic;
    private static Dictionary<string, object> _assetDic;
    private static Dictionary<string, AssetBundle> _abDic;
    private static AssetBundleManifest _manifest;
    private static Dictionary<string, uint> _loadingABCountDic;
    private static List<LoadTask> _loadedTask;
    private static Dictionary<System.Action<object>, LoadTask> _taskDic;
    private static List<string> _unloadABList;
    private static System.Action _onABFileLoaded;
    public static void LoadABFileInfo(System.Action OnComplete)
    {
        _onABFileLoaded = OnComplete;
        _abAssetPathDic = new Dictionary<string, string>();
        _assetDic = new Dictionary<string, object>();
        _abDic = new Dictionary<string, AssetBundle>();
        _loadingABCountDic = new Dictionary<string, uint>();
        _loadedTask = new List<LoadTask>();
        _taskDic = new Dictionary<System.Action<object>, LoadTask>();
        _unloadABList = new List<string>();

        //加载resouce资源路径与ab路径对照表
        string relativeUrl = AssetPathUtil.PlatformString + "/abfile.bytes";
        GetBinaryResource(relativeUrl, (object content) => {
            byte[] data = GZipFileUtil.Uncompress(content as byte[]);
            string dataStr = Encoding.UTF8.GetString(data);
            string[] fileInfos = dataStr.Split(new string[] { "\n" }, StringSplitOptions.None);
            foreach (string fileInfo in fileInfos)
            {
                string[] abResArr = fileInfo.Split(':');
                if (abResArr.Length > 1)
                {
                    _abAssetPathDic.Add(abResArr[0], abResArr[1]);
                }
            }
            OnComplete.Invoke();
        });
        
    }

    

    private static void StartLoad(LoadTask task)
    {
        string abName = task.abName;
        //先加载依赖AB
        string[] dependenciesAB = _manifest.GetAllDependencies(abName);
        foreach (string dpdAB in dependenciesAB)
        {
            if (_abDic.ContainsKey(dpdAB))
            {
                continue;
            }
            task.dependencies.Add(dpdAB);
            if (_loadingABCountDic.ContainsKey(dpdAB))
            {
                _loadingABCountDic[dpdAB]++;
                continue;
            }
            _loadingABCountDic.Add(dpdAB, 1);
            string relativePath = AssetPathUtil.PlatformString + "/" + dpdAB;
            string dpdUrl;
            if(VersionManager.HasCache(relativePath))
            {
                dpdUrl = VersionManager.GetPath(relativePath);
            }
            else
            {
                dpdUrl = AssetPathUtil.GetAssetBundlePath(dpdAB);
            }
            LoaderManager.Load(dpdUrl, EnumResouceType.ASSETBUNDLE, OnABLoadComplete);
        }


        if (task.IsDependenciesLoadComplete())
        {
            if (!_loadingABCountDic.ContainsKey(abName))
            {
                _loadingABCountDic.Add(abName, 1);
                string relativePath = AssetPathUtil.PlatformString + "/" + abName;
                string abUrl;
                if (VersionManager.HasCache(relativePath))
                {
                    abUrl = VersionManager.GetPath(relativePath);
                }
                else
                {
                    abUrl = AssetPathUtil.GetAssetBundlePath(abName);
                }
                LoaderManager.Load(abUrl, EnumResouceType.ASSETBUNDLE, OnABLoadComplete);
            }
            else
                _loadingABCountDic[task.abName]++;
        }
    }


    private static void OnABLoadComplete(object content)
    {
        AssetBundle ab = (AssetBundle)content;
        _abDic.Add(ab.name, ab);
        _loadingABCountDic.Remove(ab.name);
        foreach (LoadTask task in _taskDic.Values)
        {
            if (task.IsDependenciesLoadComplete() && ab.name == task.abName)
            {
                _loadedTask.Add(task);
            }
            else if (task.dependencies.Contains(ab.name))
            {
                task.dependencies.Remove(ab.name);
                if (task.IsDependenciesLoadComplete())
                {
                    if (!_loadingABCountDic.ContainsKey(task.abName))
                    {
                        _loadingABCountDic.Add(task.abName, 1);
                        string relativePath = AssetPathUtil.PlatformString + "/" + task.abName;
                        string abUrl;
                        if (VersionManager.HasCache(relativePath))
                        {
                            abUrl = VersionManager.GetPath(relativePath);
                        }
                        else
                        {
                            abUrl = AssetPathUtil.GetAssetBundlePath(task.abName);
                        }
                        LoaderManager.Load(abUrl, EnumResouceType.ASSETBUNDLE, OnABLoadComplete);
                    }
                    else
                        _loadingABCountDic[task.abName]++;
                }
            }
        }
        foreach (LoadTask task in _loadedTask)
        {
            _taskDic.Remove(task.resCallback);
            object obj;// = ab.LoadAsset(StringUtil.GetFileName(task.resPath));
            if (!_assetDic.ContainsKey(task.resPath))
            {
                obj = ab.LoadAsset(Path.GetFileNameWithoutExtension(task.resPath));
                _assetDic.Add(task.resPath, obj);
                if(task.isFinishUnload)
                {
                    string[] names = ab.GetAllAssetNames();
                    foreach(string assName in names)
                    {
                        string resPath = FileUtil.GetRelativeResourcePath(assName);
                        if (_assetDic.ContainsKey(resPath))
                            continue;
                        _assetDic.Add(resPath, ab.LoadAsset(Path.GetFileNameWithoutExtension(resPath)));
                    }
                    _abDic.Remove(ab.name);
                    ab.Unload(false);
                }
            }
            else
                obj = _assetDic[task.resPath];
            task.resCallback.Invoke(obj);
            task.Dispose();
            obj = null;
        }
        _loadedTask.Clear();
        ab = null;
    }

    public static void GetBinaryResource(string relativePath, System.Action<object> OnGetComplete)
    {
        string url;
        EnumResouceType type;
        if (VersionManager.HasCache(relativePath))
        {
            url = VersionManager.GetPath(relativePath);
            type = EnumResouceType.CACHE_BINARY;
        }
        else
        {
            url = AssetPathUtil.StreamingAssetPath + "/" + relativePath;
            type = EnumResouceType.BINARY;
        }
        LoaderManager.Load(url, type, OnGetComplete);
    }

    /// <summary>
    /// 获取资源
    /// 异步方法
    /// </summary>
    /// <param name="resPath">Resources下的相对目录</param>
    /// <param name="OnGetComplete">返回值为资源</param>
    /// <param name="isFinishUnload">下载完AB后是否马上卸载AB的内存镜像（不销毁从中加载的资源）</param>
    public static void GetResource(string resPath, System.Action<object> OnGetComplete, bool isFinishUnload = false)
    {
        if (!Global.IsUseAB)
        {
            object res = Resources.Load(resPath);
            if (res == null)
            {
                Debug.LogWarning(resPath + " not exist!");
                return;
            }
            OnGetComplete.Invoke(res);
            return;
        }
        resPath = resPath.ToLower();
        if (!_abAssetPathDic.ContainsKey(resPath))
        {
            Debug.LogWarning(resPath + " not exist!");
            return;
        }
        if (_taskDic.ContainsKey(OnGetComplete))
            return;
        if (_assetDic.ContainsKey(resPath))
        {
            OnGetComplete.Invoke(_assetDic[resPath]);
            return;
        }
        string abName = _abAssetPathDic[resPath];
        if (_abDic.ContainsKey(abName))
        {
            AssetBundle ab = _abDic[abName];
            object obj = ab.LoadAsset(Path.GetFileNameWithoutExtension(resPath));
            _assetDic.Add(resPath, obj);
            OnGetComplete.Invoke(obj);
        }
        else
        {
            LoadTask task = new LoadTask();
            task.resPath = resPath;
            task.abName = abName;
            task.resCallback = OnGetComplete;
            task.isFinishUnload = isFinishUnload;
            _taskDic.Add(OnGetComplete, task);
            StartLoad(task);
        }
    }
    /// <summary>
    /// 直接获取已加载的资源
    /// 同步方法
    /// 不安全，未加载会返回空
    /// </summary>
    /// <param name="resPath">Resources下的相对目录</param>
    /// <returns></returns>
    public static object GetResource(string resPath)
    {
        if (!Global.IsUseAB)
        {
            return Resources.Load(resPath);
        }
        resPath = resPath.ToLower();
        if (!_abAssetPathDic.ContainsKey(resPath))
        {
            Debug.LogWarning(resPath + " not exist!");
            return null;
        }
        if (_assetDic.ContainsKey(resPath))
        {
            return _assetDic[resPath];
        }
        string abName = _abAssetPathDic[resPath];
        if (_abDic.ContainsKey(abName))
        {
            AssetBundle ab = _abDic[abName];
            object obj = ab.LoadAsset(Path.GetFileNameWithoutExtension(resPath));
            _assetDic.Add(resPath, obj);
            return obj;
        }
        return null;
    }

    /// <summary>
    /// 直接从已加载的AssetBundle获取资源
    /// 同步方法
    /// 不安全，未加载会返回空
    /// </summary>
    /// <param name="abName"></param>
    /// <param name="resPath">Resources下的相对目录</param>
    /// <returns></returns>
    public static object GetResource(string abName, string resPath)
    {
        if (!Global.IsUseAB)
        {
            return Resources.Load(resPath);
        }
        resPath = resPath.ToLower();
        if (_assetDic.ContainsKey(resPath))
        {
            return _assetDic[resPath];
        }
        if (_abDic.ContainsKey(abName))
        {
            object obj = _abDic[abName].LoadAsset(Path.GetFileNameWithoutExtension(resPath));
            _assetDic.Add(resPath, obj);
            return obj;

        }
        return null;
    }
    /// <summary>
    /// 从图集中获取图片
    /// </summary>
    /// <param name="atlasName">图集名</param>
    /// <param name="resPath">Resources下的相对目录</param>
    /// <returns></returns>
    public static Sprite GetSpriteFromAtlas(string atlasName, string resPath)
    {
        resPath = resPath.ToLower();
        string atlasABName = AtlasType.GetAtlasABName(atlasName);
        Sprite sprite = null;
        if (Global.IsUseAB)
        {
            if (_assetDic.ContainsKey(resPath))
            {
                sprite = (Sprite)_assetDic[resPath];
            }
            else if (_abDic.ContainsKey(atlasABName))
            {
                object obj = _abDic[atlasABName].LoadAsset(Path.GetFileNameWithoutExtension(resPath));
                _assetDic.Add(resPath, obj);
                sprite = (Sprite)obj;
            }
        }
        else
        {
            GameObject go = Resources.Load<GameObject>(resPath);
            if (go == null)
                return null;
            Image resImg = go.GetComponent<Image>();
            SpriteRenderer spRender = go.GetComponent<SpriteRenderer>();
            if (resImg != null)
                sprite = resImg.sprite;
            else if (spRender != null)
                sprite = spRender.sprite;
        }
        return sprite;
    }

    /// <summary>
    /// 卸载加载中的资源
    /// </summary>
    /// <param name="resPath">Resources下的相对目录</param>
    /// <param name="OnGetComplete">获取时的回调函数</param>
    public static void Release(string resPath, System.Action<object> OnGetComplete, bool isUnloadAB = false)
    {
        if (!Global.IsUseAB)
            return;
        if (string.IsNullOrEmpty(resPath))
            return;
        resPath = resPath.ToLower();
        if (!_abAssetPathDic.ContainsKey(resPath))
        {
            return;
        }
        string abName = _abAssetPathDic[resPath];
        if (_taskDic.ContainsKey(OnGetComplete))
        {
            LoadTask task = _taskDic[OnGetComplete];
            if(task.dependencies.Count == 0)
                UnloadLoadingAB(abName);
            foreach (string dependency in task.dependencies)
            {
                UnloadLoadingAB(dependency);
            }
            task.Dispose();
            _taskDic.Remove(OnGetComplete);
        }
        else if(isUnloadAB && _abDic.ContainsKey(abName))
        {
            AssetBundle ab = _abDic[abName];
            ab.Unload(true);
            _abDic.Remove(abName);
            _assetDic.Remove(resPath);
        }
    }

    public static void UnloadLoadingAssets()
    {
        if (Global.IsUseAB)
        {
            foreach (LoadTask task in _taskDic.Values)
            {
                if (task.dependencies.Count == 0)
                    UnloadLoadingAB(task.abName);
                else
                {
                    foreach (string dependency in task.dependencies)
                    {
                        UnloadLoadingAB(dependency);
                    }
                }
                task.Dispose();
            }
            _taskDic.Clear();
            _loadingABCountDic.Clear();
        }
    }

    public static void UnloadUnpersistentAssets()
    {
        if (Global.IsUseAB)
        {
            foreach (LoadTask task in _taskDic.Values)
            {
                if (task.dependencies.Count == 0)
                    UnloadLoadingAB(task.abName);
                foreach (string dependency in task.dependencies)
                {
                    UnloadLoadingAB(dependency);
                }
                task.Dispose();
            }
            _taskDic.Clear();
            _loadingABCountDic.Clear();
            foreach (string abName in _abDic.Keys)
            {
                if (_persistentList !=null && _persistentList.Contains(abName))
                    continue;
                _abDic[abName].Unload(true);
                _unloadABList.Add(abName);
            }
            _assetDic.Clear();
            foreach (string unloadAbName in _unloadABList)
            {
                _abDic.Remove(unloadAbName);
            }
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }
    }

    private static void UnloadLoadingAB(string abName)
    {
        if (_loadingABCountDic.ContainsKey(abName))
        {
            _loadingABCountDic[abName]--;
            if (_loadingABCountDic[abName] == 0)
            {
                _loadingABCountDic.Remove(abName);
                LoaderManager.Unload(AssetPathUtil.GetAssetBundlePath(abName), OnABLoadComplete);
            }
        }
    }


    /// <summary>
    /// 通过资源相对于Resources目录的相对路径获取AB名字
    /// </summary>
    /// <param name="resRelaviePath"></param>
    /// <returns></returns>
    public static string GetAssetBundleName(string resRelaviePath)
    {
        string abName = "";
        try
        {
            abName = _abAssetPathDic[resRelaviePath.ToLower()];
        }
        catch(Exception ex)
        {
            Debug.Log(abName);
        }
        return abName;
    }

    public static void Preload(Action onPreloadComplete)
    {
        _onPreloadComplete = onPreloadComplete;
        _preloadList = new List<string>();
        _preloadList.Add("atlas_common");
        _preloadList.Add("atlas_battlemodel");
        _preloadList.Add("atlas_battle");
        _preloadList.Add("atlas_guide");
        _preloadList.Add("atlas_icon");
        _preloadList.Add("prefabs_ui_alertuiwithbackground");
        _preloadList.Add("prefabs_ui_teaminvitealertui");
        _preloadList.Add("prefabs_ui_teamreconnectalertui"); 
        _preloadList.Add("prefabs_ui_redpoint"); 
         //_preloadList.Add("assets_art_music_mus_battle_loop");

         //游戏生命周期中不销毁的
         _persistentList = new List<string>();
        _persistentList.AddRange(_preloadList);

        //预加载但会销毁的

        _preloadList.Add(AssetPathUtil.PlatformString);
        PreloadNext();
    }

    public static void Preload(List<string> pathList, Action onPreloadComplete)
    {
        _onPreloadComplete = onPreloadComplete;
        _preloadList = pathList;
        PreloadNext();
    }

    private static void PreloadNext()
    {
        if (_preloadList.Count > 0)
        {
            PreloadAssetBundle(_preloadList[0]);
            _preloadList.RemoveAt(0);
        }
        else
        {
            //加载manifest文件
            if(_manifest == null)
            {
                AssetBundle manifestAB = _abDic[AssetPathUtil.PlatformString];
                _manifest = manifestAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                manifestAB.Unload(false);
                _abDic.Remove(AssetPathUtil.PlatformString);
            }
            _onPreloadComplete.Invoke();
        }
    }

    /// <summary>
    /// 开始预加载assetbundle
    /// </summary>
    /// <param name="abName"></param>
    /// <param name="OnLoadComplete">参数为AssetBundle</param>
    private static void PreloadAssetBundle(string abName)
    {
        if (_abDic.ContainsKey(abName))
        {
            PreloadNext();
            return;
        }
        string relativePath = AssetPathUtil.PlatformString + "/" + abName;
        if(VersionManager.HasCache(relativePath))
            LoaderManager.Load(VersionManager.GetPath(relativePath), EnumResouceType.ASSETBUNDLE, OnPreloadOne);
        else
            LoaderManager.Load(AssetPathUtil.GetAssetBundlePath(abName), EnumResouceType.ASSETBUNDLE, OnPreloadOne);
    }

    private static void OnPreloadOne(object content)
    {
        AssetBundle ab = (AssetBundle)content;
        _abDic.Add(ab.name, ab);
        PreloadNext();
    }

    private static List<string> _persistentList;
    private static List<string> _preloadList;
    private static Action _onPreloadComplete;
    private static int _preloadIndex;
}