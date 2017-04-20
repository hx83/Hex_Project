using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrawLine
{
    private Player player;
    private int pointCount;
    private LineRenderer lr;
    private List<Vector3> list;
    /// <summary>
    /// 当前是否满足画线状态
    /// </summary>
    private bool _canDraw;
    public DrawLine(Player p)
    {
        this.player = p;
        Material lineMaterial = Resources.Load<Material>("LineMaterial");
        lineMaterial.SetColor("_Color", this.player.Color);
        lr = this.player.transform.gameObject.AddComponent<LineRenderer>();
        lr.material = lineMaterial;
        lr.SetWidth(0.15f, 0.15f);
        list = new List<Vector3>();

        CanDraw = false;
        
    }
    public bool CanDraw
    {
        set
        {
            _canDraw = value;
        }
        get
        {
            return _canDraw;
        }
    }
    

    public void Update()
    {

        if(CanDraw == false)
        {
            return;
        }
        if(lr == null)
        {
            return;
        }
        //
        int count = list.Count;
        if(count == 0)
        {
            list.Add(player.transform.position);
        }

        pointCount++;
        if (pointCount % 5 == 0)
        {
            list.Add(player.transform.position);
            pointCount = 0;
        }

        count = list.Count;
        lr.SetVertexCount(count);
        if (count > 2)
        {
            for (int i = 0; i < count; i++)
			{
                lr.SetPosition(i,list[i]);
			}
        }
        
    }

    public void Clear()
    {
        CanDraw = false;
        lr.SetVertexCount(0);
        list.Clear();
        pointCount = 0;
    }
}
