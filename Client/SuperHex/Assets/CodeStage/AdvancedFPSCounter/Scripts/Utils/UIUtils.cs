using UnityEngine;
using System.Collections.Generic;

namespace CodeStage.AdvancedFPSCounter
{
	internal class UIUtils
	{
		internal static void ResetRectTransform(RectTransform rectTransform)
		{
			rectTransform.localRotation = Quaternion.identity;
			rectTransform.localScale = Vector3.one;
			rectTransform.pivot = new Vector2(0.5f, 0.5f);
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;
			rectTransform.anchoredPosition3D = Vector3.zero;
			rectTransform.offsetMin = Vector2.zero;
			rectTransform.offsetMax = Vector2.zero;
		}

       internal static  GameObject AddChild(Transform parent, GameObject prefab, Vector3 pos)
        {
            GameObject go = GameObject.Instantiate(prefab) as GameObject;
#if UNITY_EDITOR
            UnityEditor.Undo.RegisterCreatedObjectUndo(go, "Create Object");
#endif
            if (go != null && parent != null)
            {
                Transform t = go.transform;
                t.SetParent(parent, false);
                t.localPosition = pos;
                t.localRotation = Quaternion.identity;
                t.localScale = Vector3.one;
                go.layer = parent.gameObject.layer;
            }
            return go;
        }

       internal static void GenerateObjCells(ref List<GameObject> listObjs, int iDataNums, ref GameObject itemModel, ref Transform itemParent)
       {
           if (listObjs.Count < iDataNums)
           {
               int _iNeedGenerateNums = iDataNums - listObjs.Count;
               for (int _index = 0; _index < _iNeedGenerateNums; ++_index)
               {
                   GameObject _obj = UIUtils.AddChild(itemParent, itemModel, Vector3.zero);
                   listObjs.Add(_obj);
               }
           }

           for (int _index = 0; _index < listObjs.Count; ++_index)
           {
               listObjs[_index].SetActive(false);
           }
       }
	}
}