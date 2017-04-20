using UnityEngine;
using System.Collections;
/// <summary>
/// 挂上脚本默认不响应鼠标事件
/// </summary>
public class UIFocus : MonoBehaviour ,ICanvasRaycastFilter
{

    public bool IsFocus = false;

    public bool IsRaycastLocationValid(Vector2 sp,Camera eventCamera)
    {
        return IsFocus;
    }
}
