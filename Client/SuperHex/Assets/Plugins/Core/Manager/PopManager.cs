using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
/// <summary>
/// 浮动的弹框提示
/// </summary>
public class PopManager
{
    private static Transform _parent;
    private static List<GameObject> _showingList;
    private static List<GameObject> _cacheList;
    private static Vector2 _hidePos;
    public static void Setup()
    {
        _parent = LayerManager.topUILayer;
        _showingList = new List<GameObject>();
        _cacheList = new List<GameObject>();
        _hidePos = new Vector2(-2000, -2000);
    }
    
	public static void ShowSimpleItem(string str,POP_TYPE type = POP_TYPE.normal,float time = 1.5f)
    {
        GameObject item;
        int cacheCount = _cacheList.Count;
        if (cacheCount > 0)
        {
            item = _cacheList[cacheCount - 1];
            _cacheList.RemoveAt(cacheCount - 1);
        }
        else
        {
            item = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Utils/PopItem"));
            item.transform.SetParent(_parent, false);
        }
        _showingList.Add(item);

        Color color = Color.white;
        switch(type)
        {
            case POP_TYPE.normal:
                color = Color.white;
                break;
            case POP_TYPE.warning:
                color = new Color(212 / 255f, 0f, 92 / 255f, 1f);
                break;
        }

        Text txt = item.transform.FindChild("Text").GetComponent<Text>();
        txt.text = str;
        txt.color = color;

        RectTransform rectTr = item.GetComponent<RectTransform>();
        rectTr.anchoredPosition = Vector2.zero;

        Tweener tweener = rectTr.DOAnchorPosY(100, time);
        tweener.OnComplete(HideItem);
    }

    private static void HideItem()
    {
        GameObject finishItem = _showingList[0];
        _showingList.RemoveAt(0);
        _cacheList.Add(finishItem);
        finishItem.transform.localPosition = _hidePos;
    }

}

public enum POP_TYPE
{
    normal = 0,
    warning = 1,
}