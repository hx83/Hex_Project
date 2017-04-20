using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIUtil 
{
    public static int REFER_WIDTH = 750;
    public static int REFER_HEIGHT = 1334;
    public static int PIXELS_PER_UNIT = 100;
    public static int SCREEN_WIDTH;
    public static int SCREEN_HEIGHT;

    public static Vector2 GetUIMousePoint()
    {
        Vector3 vect = Input.mousePosition;
        return new Vector2(SCREEN_WIDTH*vect.x/Screen.width,SCREEN_HEIGHT*vect.y/Screen.height);
    }
    public static void Setup()
    {
        RectTransform trans = LayerManager.topUILayer as RectTransform;
        SCREEN_WIDTH = (int)trans.rect.width;
        SCREEN_HEIGHT = (int)trans.rect.height;
        
    }
    /// <summary>
    /// 获取一个UI的宽高
    /// </summary>
    /// <param name="trans"></param>
    /// <returns></returns>
    public static Vector2 GetSize(RectTransform trans)
    {
        return trans.sizeDelta;
    }

    /// <summary>
    /// 设置UI的宽高
    /// </summary>
    /// <param name="trans"></param>
    public static void SetSize(RectTransform rectTrans, float w, float h)
    {
        rectTrans.sizeDelta = new Vector2(w, h);
    }

    /// <summary>
    /// 给面板添加通用色底
    /// </summary>
    /// <param name="parent"></param>
    public static GameObject AddPanelBottomBg(Transform parent, float alpha = 1)
    {
        GameObject obj = new GameObject();
        Image image = obj.AddComponent<Image>();
        image.color = new Color(37 / 255f, 48 / 255f, 65 / 255f, alpha);
        SetSize(image.rectTransform, UIUtil.SCREEN_WIDTH, UIUtil.SCREEN_HEIGHT);
        obj.transform.SetParent(parent, false);
        obj.transform.SetSiblingIndex(0);
        return obj;
    }

    public static void SetAnchoredPosition(RectTransform rectTransf, float x, float y)
    {
        Vector2 pos = rectTransf.anchoredPosition;
        pos.Set(x, y);
        rectTransf.anchoredPosition = pos;
    }

    public static void SetAnchoredPosition(Transform transform, float x, float y)
    {
        RectTransform rectTransf = transform as RectTransform;
        Vector2 pos = rectTransf.anchoredPosition;
        pos.Set(x, y);
        rectTransf.anchoredPosition = pos;
    }
}
