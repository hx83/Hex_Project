using UnityEngine;

public class ColorUtil
{
    //透明
    public static Color TRANSPARENT = new Color(1, 1, 1, 0);
    /// <summary>
    /// 16进制颜色转成Color
    /// </summary>
    /// <param name="hex">like "66ccff"</param>
    /// <returns></returns>
    public static Color HexToColor(string hex)
    {
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        return new Color32(r, g, b, 255);
    }
}
