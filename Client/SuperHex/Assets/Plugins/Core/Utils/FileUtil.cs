using System.IO;
using UnityEngine;
/// <summary>
/// 文件工具
/// </summary>
public class FileUtil
{
    /// <summary>
    /// 获取Asset在Resource目录下相对路径（全小写）
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetRelativeResourcePath(string path)
    {
        string result = path.Split('.')[0].ToLower();
        result = result.Replace("assets/resources/", "");
        return result;

    }

    public static void DeleteAllFiles(string path)
    {
        string[] directories = Directory.GetDirectories(path);
        int directoryCount = directories.Length;
        for (int i = 0; i< directoryCount; i++)
        {
            DeleteAllFiles(directories[i]);
        }
        string[] files = Directory.GetFiles(path);
        int fileCount = files.Length;
        for (int j = 0; j < fileCount; j++)
        {
            File.Delete(files[j]);
        }
    }

}
