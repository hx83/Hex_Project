using UnityEngine;
using System.Collections;

public class CameraManager
{
    private static Camera camera;
    private static Transform target;
    private static Vector3 pos;
    public static void Setup(Transform c)
    {
        camera = c.GetComponent<Camera>();
        pos = camera.transform.position;
    }
    public static void Update()
    {
        if(target == null)
        {
            return;
        }

        Vector3 targetPos = target.position;
        targetPos.z = pos.z;
        camera.transform.position = Vector3.Lerp(pos, targetPos, Time.deltaTime * 5);
        pos = camera.transform.position;
    }
    public static void Follow(Transform trans)
    {
        target = trans;
    }

    public static void MoveTo(Vector3 p)
    {
        Vector3 p2 = new Vector3(p.x, p.y, pos.z);
        camera.transform.position = p2;
    }
}
