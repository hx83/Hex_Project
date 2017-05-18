using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 负责检查玩家碰到哪些格子
/// </summary>
public class CheckGrid 
{
    private List<MapGrid> allGridList = new List<MapGrid>();
    /// <summary>
    /// 经过每个格子的时间
    /// </summary>
    private Dictionary<MapGrid, float> timeDict;

    private Player player;

    private int minXIndex = 99999999;
    private int minYIndex = 99999999;
    private int maxXIndex = 0;
    private int maxYIndex = 0;

    private DrawLine Line;

    private MapGrid prevGrid;

    private bool isBegin;

    public MapGrid StartGrid;
    public CheckGrid(Player p)
    {
        this.player = p;

        Line = new DrawLine(player);

        timeDict = new Dictionary<MapGrid, float>();

        isBegin = false;
    }

    public bool IsBegin
    {
        get
        {
            return isBegin;
        }
    }

    public void Update()
    {

        Vector3 pos = player.transform.position;
        //Debug.Log(pos);

        int xindex = (int)Mathf.Floor(pos.x / (1.5f * GridConst.Radius));
        int yindex = (int)Mathf.Floor(pos.y / (GridConst.Height * 2f));



        MapGrid cGrid = MapManager.GetGridByPos(pos);
        //cGrid.Test();
        //return;
        if (cGrid != null)
        {
            if (cGrid.Owner != this.player)
            {
                if (cGrid.State == GridState.PREV_OWN)
                {
                    if (cGrid.PrevOwner == this.player)
                    {
                        float t = this.timeDict[cGrid];
                        if (Time.realtimeSinceStartup - t > 1)
                        {
                            //撞到自己了
                            PlayerManager.HitPlayer(this.player, this.player);
                        }

                    }
                    else
                    {
                        PlayerManager.HitPlayer(this.player, cGrid.PrevOwner);
                    }
                }
                else
                {
                    if (this.timeDict.ContainsKey(cGrid) == false)
                        this.timeDict.Add(cGrid, Time.realtimeSinceStartup);
                    //
                    if (!isBegin)
                    {
                        StartGrid = cGrid;
                        StartGrid.Test();
                        isBegin = true;
                        Line.CanDraw = true;
                    }
                }
            }
            else
            {
                if (isBegin)
                {
                    isBegin = false;
                    this.FillGrid();
                }
            }
        }

        //
        //
        MapGrid mc = cGrid;
        if (mc.Owner != this.player)
        {
            mc.PrevOwner = this.player;

            if (this.timeDict.ContainsKey(mc) == false)
            {
                this.timeDict.Add(mc, Time.realtimeSinceStartup);
            }
        }

        if (mc.xIndex < this.minXIndex)
        {
            this.minXIndex = mc.xIndex;
        }
        if (mc.xIndex > this.maxXIndex)
        {
            this.maxXIndex = mc.xIndex;
        }
        if (mc.yIndex < this.minYIndex)
        {
            this.minYIndex = mc.yIndex;
        }
        if (mc.yIndex > this.maxYIndex)
        {
            this.maxYIndex = mc.yIndex;
        }
        
       
        //
        /**
        List<MapGrid> arr = MapManager.FindAround(xindex, yindex);
        foreach (var mc in arr)
        {
            float ox = pos.x - mc.transform.position.x;
            float oy = pos.y - mc.transform.position.y;
            //trace(mc.index ,ox,oy);
            if (ox * ox + oy * oy < GridConst.Radius * GridConst.Radius)
            {
                //mc.Selected = true;
                if (mc.Owner != this.player)
                {
                    Debug.Log("AAA");
                    mc.PrevOwner = this.player;

                    if (this.timeDict.ContainsKey(mc) == false)
                    {
                        this.timeDict.Add(mc, Time.realtimeSinceStartup);
                    }
                }

                if (mc.xIndex < this.minXIndex)
                {
                    this.minXIndex = mc.xIndex;
                }
                if (mc.xIndex > this.maxXIndex)
                {
                    this.maxXIndex = mc.xIndex;
                }
                if (mc.yIndex < this.minYIndex)
                {
                    this.minYIndex = mc.yIndex;
                }
                if (mc.yIndex > this.maxYIndex)
                {
                    this.maxYIndex = mc.yIndex;
                }
            }
        }
         * */





        
        //
        //
        Line.Update();
    }

    public void Clear()
    {
        minXIndex = 99999999;
        minYIndex = 99999999;
        maxXIndex = 0;
        maxYIndex = 0;

        this.timeDict.Clear();
        Line.Clear();

        StartGrid = null;
    }

    public void Reset()
    {
        Line.Clear();
        foreach (var item in this.timeDict)
        {
            item.Key.PrevOwner = null;
        }
        this.timeDict.Clear();
        Clear();
    }




    private void FillGrid()
    {
        allGridList.Clear();
        //float t = Time.realtimeSinceStartup;

        for (int i = minXIndex - 1; i <= this.maxXIndex + 1; i++)
        {
            for (int j = this.minYIndex - 1; j <= this.maxYIndex + 1; j++)
            {
                MapGrid temp = MapManager.GetGrid(i, j);
                if (temp != null)
                {
                    allGridList.Add(temp);
                }
            }
        }
        //
        foreach (var mc in allGridList)
        {
            if (this.IsInArea(mc))
            {
                //mc.Selected = true;
                mc.Owner = this.player;
                this.player.AddMineGrid(mc);
            }
        }
        //Debug.Log((Time.realtimeSinceStartup - t) * 1000);
        this.player.IsCreateRoute = false;
        this.Clear();
    }

    private bool IsInArea(MapGrid mc)
    {
        if (mc == null)
            return false;

        if (mc.PrevOwner == this.player || mc.Owner == this.player)
        {
            return true;
        }

        if (this.IsMinYOK(mc) && this.IsMaxYOK(mc) && this.IsMinXOK(mc) && this.IsMaxXOK(mc))
        {
            return true;
        }

        else
        {
            return false;
        }
    }

    private bool IsMinYOK(MapGrid mc)
    {

        bool mark = false;//用来记录是否连续边界，是就忽略
        uint count = 0;

        for (int i = this.minYIndex; i < mc.yIndex; i++)
        {
            MapGrid temp = MapManager.GetGrid(mc.xIndex, i);
            if (temp != null)
            {
                if (temp.PrevOwner == this.player || temp.Owner == this.player)
                {
                    if (mark == false)
                    {
                        mark = true;
                        count++;
                    }
                }
                else
                {
                    mark = false;
                }
            }
        }

        if (count > 1) count += (count - 1);
        return count % 2 != 0;
    }
    private bool IsMaxYOK(MapGrid mc)
    {
        bool mark = false;//用来记录是否连续边界，是就忽略
        uint count = 0;

        for (int i = mc.yIndex; i <= this.maxYIndex; i++)
        {
            MapGrid temp = MapManager.GetGrid(mc.xIndex, i);
            if (temp != null)
            {
                if (temp.PrevOwner == this.player || temp.Owner == this.player)
                {
                    if (mark == false)
                    {
                        mark = true;
                        count++;
                    }
                }
                else
                {
                    mark = false;
                }
            }
        }

        if (count > 1) count += (count - 1);
        return count % 2 != 0;
    }



    private bool IsMinXOK(MapGrid mc)
    {
        bool mark = false;//用来记录是否连续边界，是就忽略
        uint count = 0;

        for (int i = this.minXIndex; i < mc.xIndex; i++)
        {
            MapGrid temp = MapManager.GetGrid(i, mc.yIndex);
            if (temp != null)
            {
                if (temp.PrevOwner == this.player || temp.Owner == this.player)
                {
                    if (mark == false)
                    {
                        mark = true;
                        count++;
                    }
                }
                else
                {
                    mark = false;
                }
            }
        }

        if (count > 1) count += (count - 1);
        return count % 2 != 0;
    }
    private bool IsMaxXOK(MapGrid mc)
    {
        bool mark = false;//用来记录是否连续边界，是就忽略
        uint count = 0;

        for (int i = mc.xIndex; i <= this.maxXIndex; i++)
        {
            MapGrid temp = MapManager.GetGrid(i, mc.yIndex);
            if (temp != null)
            {
                if (temp.PrevOwner == this.player || temp.Owner == this.player)
                {
                    if (mark == false)
                    {
                        mark = true;
                        count++;
                    }
                }
                else
                {
                    mark = false;
                }
            }
        }

        if (count > 1) count += (count - 1);
        return count % 2 != 0;
    }
}
