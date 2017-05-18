using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapManager 
{
    private static int N = 100000;
    private static List<MapGrid> allList;
    private static Dictionary<int, MapGrid> dict;
    //public static Transform LineContainer;
    private static Transform Map;

    public static int maxXindex;
    public static int maxYindex;

    private static float MapWidth;
    private static float MapHeight;
    public static void CreateMap(int w, int h, Transform parent)
    {
        Map = parent;

        maxXindex = w - 1;
        maxYindex = h -1;

        if(dict == null)
        {
            dict = new Dictionary<int, MapGrid>();
            allList = new List<MapGrid>();
        }
        dict.Clear();
        allList.Clear();

        GameObject obj = Resources.Load<GameObject>(AssetPathUtil.GetMapPath(0));
        GameObject borderY = Resources.Load<GameObject>(AssetPathUtil.GetMapPath(1001));
        GameObject borderX = Resources.Load<GameObject>(AssetPathUtil.GetMapPath(1000));

        MapWidth = (w - 1) * GridConst.Radius * 1.5f;
        MapHeight = (h - 1) * GridConst.Height * 2;

        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                GameObject go = GameObject.Instantiate<GameObject>(obj);
                //go.AddComponent<SpriteRenderer>().sprite = sp; ;
                go.name = i.ToString();
                go.transform.SetParent(parent, false);

                int xIndex = i;
                int yIndex = j;

                //Debug.Log(xIndex + "_" + yIndex);
                float x = xIndex * GridConst.Radius * 1.5f;
                float y = yIndex * 2 * GridConst.Height - GridConst.Height * (i % 2);

                go.transform.localPosition = new Vector3(x, y, 0);

                MapGrid grid = go.AddComponent<MapGrid>();
                grid.xIndex = xIndex;
                grid.yIndex = yIndex;
                grid.Center = new Vector2(x, y);

                dict.Add(xIndex * N + yIndex, grid);
                allList.Add(grid);
            }            
        }
        //
        float H = (h * GridConst.Height * 2);
        float W= (w * GridConst.Radius * 1.5f);

        GameObject b = GameObject.Instantiate<GameObject>(borderY);
        b.transform.localScale = new Vector3(1, H , 1);
        b.transform.localPosition = new Vector3( -GridConst.Radius*1.5f, H / 2 - GridConst.Height, -1);
        b.transform.SetParent(parent, false);

        b = GameObject.Instantiate<GameObject>(borderY);
        b.transform.localScale = new Vector3(1, H + GridConst.Height*2, 1);
        b.transform.localPosition = new Vector3(W, H / 2 - GridConst.Height, -1);
        b.transform.SetParent(parent, false);

        b = GameObject.Instantiate<GameObject>(borderX);
        b.transform.localScale = new Vector3(W + GridConst.Radius*2, 1, 1);
        b.transform.localPosition = new Vector3(W/2 - GridConst.Radius*1.5f, -GridConst.Height, -1);
        b.transform.SetParent(parent, false);

        b = GameObject.Instantiate<GameObject>(borderX);
        b.transform.localScale = new Vector3(W + GridConst.Radius * 2, 1, 1);
        b.transform.localPosition = new Vector3(W / 2 - GridConst.Radius * 1.5f,H - GridConst.Height, -1);
        b.transform.SetParent(parent, false);
    }
    /// <summary>
    /// 是否在地图外
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public static bool IsOutMap(Player p)
    {
        Vector3 pos = p.transform.localPosition;
        if(pos.x < 0 || pos.x > MapWidth || pos.y < 0 || pos.y > MapHeight)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static void AddPlayer(Player p)
    {
        p.transform.SetParent(Map, false);
    }
    public static MapGrid GetGrid(int x,int y)
    {
        if (dict.ContainsKey(x * N + y))
        {
            return dict[x * N + y];
        }
        else
        {
            return null;
        }
    }

    private static MapGrid GetGridByPixel(float x, float y)
    {
        Vector2 op = new Vector2(x,y);

        int xindex = (int)Mathf.Floor(x / (1.5f * GridConst.Radius));
        int yindex = (int)Mathf.Floor(y / (GridConst.Height * 2f));

        List<MapGrid> list = FindAround(xindex, yindex);
        if (list.Count == 0)
        {
            return null;
        }
        else
        {
            float n = 9999;
            MapGrid grid = null;
            foreach (var item in list)
            {
                float m = Vector2.Distance(item.Center,op);
                if (m < n)
                {
                    n = m;
                    grid = item;
                }
            }
            return grid;
        }
    }

    public static MapGrid GetGridByPos(Vector3 pos)
    {
        return GetGridByPixel(pos.x, pos.y);
    }

    public static List<MapGrid> FindAround(int xindex, int yindex)
    {
        int[] xarr = new int[] { 1, 1, 0, -1, -1, -1, 0, 1 };
        int[] yarr = new int[] { 0, 1, 1, 1, 0, -1, -1, -1 };

        List<MapGrid> list = new List<MapGrid>();

        MapGrid mc = MapManager.GetGrid(xindex, yindex);



        if (mc != null)
        {
            list.Add(mc);

            for (int i = 0; i < xarr.Length; i++)
            {
                int ox = xindex + xarr[i];
                int oy = yindex + yarr[i];

                MapGrid temp = MapManager.GetGrid(ox, oy);
                if (temp != null)
                {
                    list.Add(temp);
                }
            }
        }

        return list;
    }
    /// <summary>
    /// 随机出生点
    /// </summary>
    /// <returns></returns>
    public static BornPoint GetRandomBornPoint()
    {
        int n = allList.Count;

        MapGrid grid = null;
        List<MapGrid> tempList = null;
        while (tempList == null || tempList.Count < 6)
        {
            int m = Random.Range(0, n);
            grid = allList[m];
            tempList = FindConnected6(grid);
        }


        //grid = MapManager.GetGrid(4, 15);
        //tempList = FindConnected6(grid);

        BornPoint bp = new BornPoint();
        tempList.Add(grid);
        bp.grids = tempList;
        bp.pos = grid.transform.localPosition;
        return bp;
    }

    /// <summary>
    /// 找到该格子周围连接的6个
    /// </summary>
    /// <param name="grid"></param>
    /// <returns></returns>
    private static List<MapGrid> FindConnected6(MapGrid grid)
    {
        int dir = grid.xIndex%2 == 0 ? 1 : -1;
        List<MapGrid> list = new List<MapGrid>();

        MapGrid g = MapManager.GetGrid(grid.xIndex, grid.yIndex - 1);
        if (g != null && g.Owner == null)
            list.Add(g);

        g = MapManager.GetGrid(grid.xIndex, grid.yIndex + 1);
        if (g != null && g.Owner == null)
            list.Add(g);

        g = MapManager.GetGrid(grid.xIndex - 1, grid.yIndex);
        if (g != null && g.Owner == null)
            list.Add(g);

        g = MapManager.GetGrid(grid.xIndex - 1, grid.yIndex + 1 * dir);
        if (g != null && g.Owner == null)
            list.Add(g);

        g = MapManager.GetGrid(grid.xIndex + 1, grid.yIndex);
        if (g != null && g.Owner == null)
            list.Add(g);

        g = MapManager.GetGrid(grid.xIndex + 1, grid.yIndex + 1 * dir);
        if (g != null && g.Owner == null)
            list.Add(g);

        return list;
    }

    /// <summary>
    /// 玩家是否接近地图边界
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public static bool IsNearSide(Player p)
    {
        MapGrid grid = GetGridByPos(p.transform.localPosition);

        if (grid == null)
            return true;

        if(grid.xIndex <= 1 || grid.xIndex >= maxXindex - 1 || grid.yIndex <= 1 || grid.yIndex >= maxYindex - 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool IsNearSideByPos(Vector3 pos)
    {
        MapGrid grid = GetGridByPos(pos);

        if (grid == null)
            return true;

        if (grid.xIndex <= 1 || grid.xIndex >= maxXindex - 1 || grid.yIndex <= 1 || grid.yIndex >= maxYindex - 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}


public class BornPoint
{
    public Vector3 pos;
    public List<MapGrid> grids;
}

