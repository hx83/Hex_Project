using System;
using System.Collections.Generic;
using System.Text;
/**
 * anthor J
 * 
 */
public class StringUtil
{
    public const string CHARACTERS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890`,.;'[]/~!@#$%^&*()_+-=";

    public static string RenewZero(string str, uint len)
    {
        while(str.Length < len)
        {
            str = "0" + str;
        }
        return str;
    }

    public static string WatchFormat(uint time, bool showHour = true)
    {
        string h = RenewZero(Math.Floor(time / 3600f).ToString(), 2);
        string m;
        string s = RenewZero((time % 60f).ToString(), 2);

        if (showHour)
        {
            m = RenewZero((Math.Floor(time / 60f) % 60f).ToString(), 2);
            return h + ":" + m + ":" + s;
        }
        else
        {
            m = RenewZero(Math.Floor(time / 60f).ToString(), 2);
            return m + ":" + s;
        }
    }
    public static string TimeFormatAutoHideHour(uint time)
    {
        double h = Math.Floor(time / 3600f);
        double m = Math.Floor(time / 60f) % 60f;
        uint s = time - System.Convert.ToUInt32(h * 3600) - System.Convert.ToUInt32(m * 60);
        if (h == 0)
            return m + "分钟" + s + "秒";
        else
            return h + "小时" + m + "分钟";

    }
    public static string TimeFormat(uint time)
    {
        return Math.Floor(time / 3600f) + "小时" + Math.Floor(time / 60f) % 60f + "分钟";
    }

    public static string CheckStrLen(string str, int maxChars)
    {
        byte[] byteStr = Encoding.Default.GetBytes(str);
        if (maxChars > 0 && byteStr.Length > maxChars)
        {
            str = str.Substring(0, str.Length - 1);
            while (Encoding.Default.GetBytes(str).Length > maxChars)
                str = str.Substring(0, str.Length - 1);
        }
        return str;
    }

    //public static string GetName()
    //{
    //    Dictionary<string, NameConfig> nameDic = NameConfig.Get();
    //    List<string> nameList = new List<string>(nameDic.Keys);
    //    string randName = nameList[UnityEngine.Random.Range(0, nameList.Count)];
    //    return GetConfigName(nameDic[randName]);
    //}

    //public static string GetConfigName(NameConfig nameConfig)
    //{
    //    if(nameConfig == null)
    //        return "Pop" + UnityEngine.Random.Range(0, 1000);
    //    string[] key1 = nameConfig.NameKey1.Split(new char[] { '|' });
    //    string[] key2 = nameConfig.NameKey2.Split(new char[] { '|' });

    //    string name1 = LanguageManager.GetLanguage(key1[UnityEngine.Random.Range(0, key1.Length)]);
    //    string name2 = LanguageManager.GetLanguage(key2[UnityEngine.Random.Range(0, key2.Length)]);

    //    if(name1.Length + name2.Length > 14)
    //    {
    //        return name1;
    //    }
    //    return name1 + name2;
    //}

    public static string NumToKString(uint num)
    {
        string numStr = num.ToString();
        if (num >= 1000)
        {
            num = num / 100;
            if(num % 10 == 0)
                numStr = (num / 10f) + ".0K";
            else
                numStr = (num / 10f) + "K";
        }
        return numStr;
    }
}