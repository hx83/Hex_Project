using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
/// <summary>
/// 版本管理
/// </summary>
public class VersionManager
{
    public static string localVersion;
    public static string remoteVersion;
    private static string cacheVersion;
    private static Dictionary<string, FileVerInfo> _localFileDic;
    private static Dictionary<string, FileVerInfo> _cacheFileDic;

    /// <summary>
    /// 检查原版本号是否是旧的
    /// </summary>
    /// <param name="originalVer">原始版本号</param>
    /// <param name="comparedVer">作比较的版本号</param>
    /// <returns></returns>
    public static bool CheckVersionIsOld(string originalVer, string comparedVer)
    {
        string[] localVerNumArr = originalVer.Split('.');
        string[] cacheVerNumArr = comparedVer.Split('.');
        uint localVerNum = uint.Parse(localVerNumArr[0]) * 1000000 + uint.Parse(localVerNumArr[1]) * 1000 + uint.Parse(localVerNumArr[2]);
        uint cacheVerNum = uint.Parse(cacheVerNumArr[0]) * 1000000 + uint.Parse(cacheVerNumArr[1]) * 1000 + uint.Parse(cacheVerNumArr[2]);
        return localVerNum < cacheVerNum;
    }
    /// <summary>
    /// 检查是否需要强更
    /// </summary>
    /// <param name="originalVer"></param>
    /// <param name="comparedVer"></param>
    /// <returns></returns>
    public static bool CheckNeedForceUpdate(string originalVer, string comparedVer)
    {
        string[] localVerNumArr = originalVer.Split('.');
        string[] cacheVerNumArr = comparedVer.Split('.');
        uint localVerNum = uint.Parse(localVerNumArr[0]) * 1000000 + uint.Parse(localVerNumArr[1]) * 1000;
        uint cacheVerNum = uint.Parse(cacheVerNumArr[0]) * 1000000 + uint.Parse(cacheVerNumArr[1]) * 1000;
        return localVerNum < cacheVerNum;
    }

    /// <summary>
    /// 解析本地版本文件
    /// </summary>
    /// <param name="bytes"></param>
    public static void ParseLocalVersionFile(byte[] bytes)
    {
        byte[] rawBytes = GZipFileUtil.Uncompress(bytes);
        string dataStr = Encoding.UTF8.GetString(rawBytes);
        _localFileDic = new Dictionary<string, FileVerInfo>();
        _cacheFileDic = new Dictionary<string, FileVerInfo>();
        string[] splitStr = new string[] { "\n" };
        string[] data = dataStr.Split(splitStr, StringSplitOptions.None);
        char[] fileVerSplit = new char[] {':', '|'};
        foreach(string fileVerStr in data)
        {
            string[] fileVer = fileVerStr.Split(fileVerSplit);
            if (fileVer.Length < 3)
                continue;
            FileVerInfo info = new FileVerInfo();
            info.relativePath = fileVer[0];
            info.absolutePath = Path.Combine(AssetPathUtil.StreamingAssetPath, info.relativePath);
            info.md5Code = fileVer[1];
            info.size = uint.Parse(fileVer[2]);
            _localFileDic.Add(info.relativePath, info);
        }

    }
    /// <summary>
    /// 解析缓存版本文件
    /// </summary>
    /// <param name="bytes"></param>
    public static void ParseCacheVersionFile(byte[] bytes)
    {
        string dataStr = Encoding.UTF8.GetString(bytes);
        string[] splitStr = new string[] { "\n" };
        string[] data = dataStr.Split(splitStr, StringSplitOptions.None);
        //第一段为缓存版本号
        cacheVersion = data[0];
        if(!CheckCacheVersionIsOld())
        {
            char[] fileVerSplit = new char[] { ':', '|' };
            int fileVerCount = data.Length;
            for (int i = 1; i < fileVerCount; i++)
            {
                string[] fileVer = data[i].Split(fileVerSplit);
                if (fileVer.Length < 3)
                    continue;
                FileVerInfo info = new FileVerInfo();
                info.relativePath = fileVer[0];
                info.absolutePath = AssetPathUtil.PersistentDataPath + info.relativePath;
                info.md5Code = fileVer[1];
                info.size = uint.Parse(fileVer[2]);
                _cacheFileDic.Add(info.relativePath, info);
                Debug.Log(info.relativePath + "in the cache");
                if (_localFileDic.ContainsKey(info.relativePath))
                {
                    _localFileDic[info.relativePath] = info;
                }
                else
                    _localFileDic.Add(info.relativePath, info);
            }
        }
    }

    /// <summary>
    /// 解析CDN版本文件获取热更新文件表
    /// </summary>
    /// <param name="bytes"></param>
    public static List<FileVerInfo> ParseRemoteVersionFile(byte[] bytes)
    {
        List<FileVerInfo> downLoadList = new List<FileVerInfo>();
        byte[] rawBytes = GZipFileUtil.Uncompress(bytes);
        string dataStr = Encoding.UTF8.GetString(rawBytes);
        string[] splitStr = new string[] { "\n" };
        string[] data = dataStr.Split(splitStr, StringSplitOptions.None);
        char[] fileVerSplit = new char[] { ':', '|' };
        foreach (string fileVerStr in data)
        {
            string[] fileVer = fileVerStr.Split(fileVerSplit);
            if (fileVer.Length < 3)
                continue;
            FileVerInfo info = new FileVerInfo();
            info.relativePath = fileVer[0];
            info.absolutePath = AssetPathUtil.PersistentDataPath + info.relativePath;
            info.md5Code = fileVer[1];
            info.size = uint.Parse(fileVer[2]);
            
            if (_localFileDic.ContainsKey(info.relativePath))
            {
                FileVerInfo localFileInfo = _localFileDic[info.relativePath];
                if(localFileInfo.md5Code != info.md5Code)
                {
                    downLoadList.Add(info);
                }
            }
            else
            {
                downLoadList.Add(info);
            }
        }
        return downLoadList;
    }

    //生成配置文件
    public static void SaveCacheVersionFile(bool isPause)
    {
        FileStream cacheVerFileStream = new FileStream(AssetPathUtil.PersistentDataPath + AssetPathUtil.PlatformString + "/version.bytes", FileMode.Create);
        StringBuilder verSB = new StringBuilder();
        if (isPause)
            verSB.Append(Application.version).Append("\n");
        else
            verSB.Append(localVersion).Append("\n");
        foreach (FileVerInfo info in _cacheFileDic.Values)
        {
            verSB.Append(info.relativePath).Append(":").Append(info.md5Code).Append("|").Append(info.size).Append("\n");
        }
        byte[] txtBytes = Encoding.UTF8.GetBytes(verSB.ToString());
        cacheVerFileStream.Write(txtBytes, 0, txtBytes.Length);
        cacheVerFileStream.Flush();
        cacheVerFileStream.Close();
        cacheVerFileStream = null;
    }

    //添加本地文件信息
    public static void AddCacheFileInfo(FileVerInfo info)
    {
        if(_localFileDic.ContainsKey(info.relativePath))
        {
            _localFileDic[info.relativePath] = info;
        }
        else
            _localFileDic.Add(info.relativePath, info);
        if (_cacheFileDic.ContainsKey(info.relativePath))
        {
            _cacheFileDic[info.relativePath] = info;
        }
        else
            _cacheFileDic.Add(info.relativePath, info);
    }

    /// <summary>
    /// 获取资源的绝对路径
    /// </summary>
    /// <param name="relativePath"></param>
    /// <returns></returns>
    public static string GetPath(string relativePath)
    {
        if (_localFileDic == null)
            return "";
        if(!_localFileDic.ContainsKey(relativePath))
        {
            Debug.LogWarning("Not Exist " + relativePath);
            return "";
        }
        FileVerInfo info = _localFileDic[relativePath];
        return info.absolutePath;
    }

    public static bool HasCache(string relativePath)
    {
        if (_cacheFileDic == null)
            return false;
        return _cacheFileDic.ContainsKey(relativePath);
    }

    /// <summary>
    /// 校验缓存版本号是否是旧的
    /// </summary>
    /// <returns></returns>
    private static bool CheckCacheVersionIsOld()
    {
        if(!CheckVersionIsOld(localVersion, cacheVersion))
        {
            Debug.Log("母包是最新的，清除缓存");
            //删除本地缓存
            FileUtil.DeleteAllFiles(AssetPathUtil.PersistentDataPath);
            return true;
        }
        //本地版本号为缓存版本号
        localVersion = cacheVersion;
        return false;
    }

}
public class FileVerInfo
{
    public string relativePath;
    public string absolutePath;
    public string md5Code;
    public uint size;
}