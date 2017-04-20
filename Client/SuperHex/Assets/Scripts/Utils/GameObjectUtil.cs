using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// GameObject辅助类
/// </summary>
static public class GameObjectUtil
{
    public static Vector2 HIDE_POS = new Vector2(-2000, -2000);
    public static void AddChild(Transform parent, Transform child)
    {
        if (child != null && parent != null)
        {
            child.SetParent(parent.transform, false);
            //child.parent = parent.transform;
            child.localPosition = Vector3.zero;
            //child.localRotation = Quaternion.identity;
            child.localScale = Vector3.one;
            child.gameObject.layer = parent.gameObject.layer;
        }
    }

    static public GameObject AddChild(this GameObject parent, GameObject prefab)
    {
        GameObject go = GameObject.Instantiate(prefab) as GameObject;
        if (go != null && parent != null)
        {
            Transform t = go.transform;
            t.SetParent(parent.transform, false);
            //t.parent = parent.transform;
            t.localPosition = Vector3.zero;
            //t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
            go.layer = parent.layer;
        }
        return go;
    }

    public static GameObject CloneGameObject(GameObject model, Transform parent)
    {
        GameObject item = GameObject.Instantiate(model) as GameObject;
        item.SetActive(true);
        item.transform.SetParent(parent, false);
        item.transform.localEulerAngles = Vector3.zero;
        item.transform.localScale = Vector3.one;
        return item;
    }

   public static void SetScale(Transform transf, float x = 1, float y = 1, float z = 1)
    {
        Vector3 localScale = transf.localScale;
        localScale.Set(x, y , z);
        transf.localScale = localScale;
    }
}
