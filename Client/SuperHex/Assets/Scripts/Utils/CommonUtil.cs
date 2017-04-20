using UnityEngine;
using System;
using System.Collections.Generic;

/**
 * Author: chan
 * 常用的一些函数
 **/
public class CommonUtil
{
    /**
 *  0   白
 *  1	绿
    2	蓝
    3	蓝+1
    4	紫
    5	紫+1
    6	金
    7	金+1
    8	橙
    9	橙+1
   10   红

 * **/
    public static Color[,] equipQualityInName = new Color[,] { 
                                {new Color(1,1,1) ,                                             new Color(0,0,0)},
                                {new Color(80.0f/255.0f, 239.0f/255.0f, 47.0f/255.0f),          new Color(0,0,0)},
                                {new Color(29.0f/255.0f, 167.0f/255.0f, 246.0f/255.0f),         new Color(0,0,0)},
                                {new Color(29.0f/255.0f, 167.0f/255.0f, 246.0f/255.0f),         new Color(0,0,0)},
                                {new Color(219.0f/255.0f, 0.0f/255.0f, 218.0f/255.0f),          new Color(0,0,0)},
                                {new Color(219.0f/255.0f, 0.0f/255.0f, 218.0f/255.0f),          new Color(0,0,0)},
                                {new Color(255.0f/255.0f, 248.0f/255.0f, 70.0f/255.0f),         new Color(0,0,0)},
                                {new Color(255.0f/255.0f, 248.0f/255.0f, 70.0f/255.0f),         new Color(0,0,0)},
                                {new Color(248.0f/255.0f, 111.0f/255.0f, 0.0f/255.0f),          new Color(0,0,0)},
                                {new Color(248.0f/255.0f, 111.0f/255.0f, 0.0f/255.0f),          new Color(0,0,0)},
                                {new Color(255.0f/255.0f, 25.0f/255.0f, 0.0f/255.0f),           new Color(0,0,0)}
    };


    public static Color getQualityLevelColor(int quality)
    {
        return equipQualityInName[quality, 0];
    }



    public static int GetChildCount(Transform transform, bool onlyActiveSelf = true)
    {
        int cnt = 0;

        foreach (Transform child in transform)
        {
            if (onlyActiveSelf)
            {
                if (child.gameObject.activeSelf)
                {
                    cnt++;
                }
            }
            else
            {
                cnt++;
            }
        }
        return cnt;
    }

    public static List<Transform> GetChildList(Transform transform, bool onlyActiveSelf = true)
    {
        List<Transform> temp = new List<Transform>();

        foreach (Transform child in transform)
        {
            if (onlyActiveSelf)
            {
                if (child.gameObject.activeSelf)
                {
                    temp.Add(child);
                }
            }
            else
            {
                temp.Add(child);
            }
        }
        return temp;
    }


    /// <summary>
    /// 清除所有子级
    /// </summary>
    public static void RemoveAllChildren(GameObject go, bool isDestroy = false)
    {
        if (go == null)
        {
            return;
        }
        for (int i = go.transform.childCount - 1; i >= 0; i--)
        {
            GameObject child = go.transform.GetChild(i).gameObject;
            if (isDestroy)
            {
                GameObject.Destroy(child);
            }
            else
            {
                child.SetActive(false);
            }
        }
    }

    public static T GetComponent<T>(Transform transform, string name = "") where T : Component
    {
        Transform trans = transform.Find(name);

        if (trans)
        {
            GameObject obj = trans.gameObject;
            return obj.GetComponent<T>();
        }
        return null;
    }

    public static List<T> GetComponentsInChildren<T>(Transform transform, List<T> list = null) where T : Component
    {
        if (list == null)
        {
            list = new List<T>();
        }
        T component = transform.GetComponent<T>();

        if (component)
        {
            list.Add(component);
        }
        foreach (Transform child in transform)
        {
            GetComponentsInChildren<T>(child, list);
        }

        return list;
    }

    /// <summary>
    /// 设置物体的层，递归设置子物体
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="layerName"></param>
    public static void RecursiveAndSetLayer(GameObject obj, string layerName)
    {
        int toLayer = LayerMask.NameToLayer(layerName);
        obj.layer = toLayer;

        foreach (Transform child in obj.transform)
        {
            RecursiveAndSetLayer(child.gameObject, layerName);
        }
    }

    /// <summary>
    /// 设置物体的层，递归设置子物体
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="layerName"></param>
    public static void RecursiveAndSetLayer(GameObject obj, int layerIndex)
    {
        obj.layer = layerIndex;

        foreach (Transform child in obj.transform)
        {
            RecursiveAndSetLayer(child.gameObject, layerIndex);
        }
    }

    /// <summary>
    /// 设置物体的static，递归设置子物体
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="isStatic"></param>
    public static void SetStatic(GameObject obj, bool isStatic, bool isChangeChildren = true)
    {
        obj.isStatic = isStatic;

        if (isChangeChildren == false)
        {
            return;
        }
        foreach (Transform child in obj.transform)
        {
            SetStatic(child.gameObject, isStatic, isChangeChildren);
        }
    }

    /// <summary>
    /// 碰撞检测（屏幕坐标系）
    /// </summary>
    /// <param name="go"></param>
    /// <param name="screenPos"></param>
    //public static bool HitTestScreenPoint(GameObject go, Vector3 screenPos)
    //{
    //    Camera[] cameraList = new Camera[100];
    //    int cameraCount = Camera.GetAllCameras(cameraList);

    //    for (int i = 0; i < cameraCount; i++)
    //    {
    //        Camera cam = cameraList[i];

    //        if (cam.enabled == false || NGUITools.GetActive(cam.gameObject) == false)
    //        {
    //            continue;
    //        }
    //        Vector3 viewportPoint = cam.ScreenToViewportPoint(screenPos);

    //        if (viewportPoint.x < 0f || viewportPoint.x > 1f || viewportPoint.y < 0f || viewportPoint.y > 1f)
    //        {
    //            continue;
    //        }

    //        Ray ray = cam.ScreenPointToRay(screenPos);
    //        float distance = cam.farClipPlane - cam.nearClipPlane;

    //        foreach (RaycastHit hit in Physics.RaycastAll(ray, distance, cam.cullingMask))
    //        {
    //            if (hit.collider.gameObject == go)
    //            {
    //                return true;
    //            }
    //        }

    //        Plane plane = new Plane(Vector3.back, 0f);

    //        if (plane.Raycast(ray, out distance))
    //        {
    //            foreach (Collider2D collider in Physics2D.OverlapPointAll(ray.GetPoint(distance)))
    //            {
    //                if (collider.gameObject == go)
    //                {
    //                    return true;
    //                }
    //            }
    //        }
    //    }

    //    return false;
    //}

    private static DateTime coolingDateTime;
    static public string GetCoolingTime(uint u)        // 秒......
    {
        if (coolingDateTime == null)
        {
            coolingDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        }
        return coolingDateTime.AddSeconds(u).TimeOfDay.ToString();
    }

    /// <summary>
    /// 返回粒子特效的时间长度 粒子循环则返回-1；
    /// </summary>
    /// <param name="particle"></param>
    /// <returns></returns>
    public static float getParticleSystemLength(Transform particle)
    {
        ParticleSystem[] particleSystems = particle.GetComponentsInChildren<ParticleSystem>();
        float maxDuration = 0f;
        for (int i = 0; i < particleSystems.Length; i++ )
        {
            if(particleSystems[i].enableEmission)
            {
                if(particleSystems[i].loop)
                {
                    return -1f;
                }
                float duration = 0f;
                if(particleSystems[i].emissionRate <= 0)
                {
                    duration = particleSystems[i].startDelay + particleSystems[i].startLifetime;
                }else
                {
                    duration = particleSystems[i].startDelay + Mathf.Max(particleSystems[i].duration, particleSystems[i].startLifetime);
                }
                if(duration > maxDuration)
                {
                    maxDuration = duration;
                }
            }
        }
        return maxDuration;
    }


    public static string simplifyNum(uint num, bool isGold = false)
    {
        string newString;
        int leng = num.ToString().Length;
        if (!isGold)
        {
            if (leng > 8)
            {
                newString = num.ToString().Substring(0, leng - 8) + "亿";
            }
            else if (leng > 6)
            {
                newString = num.ToString().Substring(0, leng - 4) + "万";
            }
            else
            {
                newString = num.ToString();
            }
        }
        else
        {
            if (leng > 6)
            {
                newString = num.ToString().Substring(0, leng - 4) + "万";
            }
            else
            {
                newString = num.ToString();
            }
        }
        return newString;
    }
}