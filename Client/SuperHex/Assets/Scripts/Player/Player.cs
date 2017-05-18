using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Behavior3CSharp.Core;

public class Player : EventDispatcher
{
    protected List<MapGrid> mineList;
    private SpriteRenderer spRender;
    private GameObject obj;
    private uint _id;
    private uint _skinID;
    //private uint _score;
    private Color _color;
    protected float _speed = 3f;
    protected Vector3 _direction;
    //
    protected bool _isDie = true;
    //
    private CheckGrid checkGrid;
    //
    protected RobotBTree tree;
    //
    private Vector3 targetV;
    private float lerpTime = 2;

	public Player()
    {
        tree = AIManager.GetTree(this);

        mineList = new List<MapGrid>();

        GameObject go = Resources.Load<GameObject>(AssetPathUtil.GetPlayerPath(0));
        obj = GameObject.Instantiate<GameObject>(go);

        spRender = obj.GetComponent<SpriteRenderer>();
        spRender.sortingLayerName = LayerUtil.SORTING_LAYER_PLAYER;

        Color = new Color(156 / 255F, 255 / 255f, 137 / 255f);

        checkGrid = new CheckGrid(this);

        //_direction = new Vector3(-0.3f, -1, 0).normalized;
    }
    public CheckGrid CheckGridAction
    {
        get
        {
            return checkGrid;
        }
    }

    public Vector3 Direction
    {
        get
        {
            return this._direction;
        }
    }

    public Transform transform
    {
        get
        {
            return obj.transform;
        }
    }
    /// <summary>
    /// 线条颜色
    /// </summary>
    public Color Color
    {
        set
        {
            _color = value;
            spRender.color = Color;
        }
        get
        {
            return _color;
        }
    }
    /// <summary>
    /// 皮肤ID
    /// </summary>
    public uint SkinID
    {
        set
        {
            _skinID = value;
        }
        get
        {
            return _skinID;
        }
    }
    public uint ID
    {
        set
        {
            _id = value;
        }
        get
        {
            return _id;
        }
    }
    /// <summary>
    /// 是否死亡
    /// </summary>
    public bool IsDie
    {
        get
        {
            return _isDie;
        }
    }

    


    public BornPoint BornPoint
    {
        set
        {
            this.transform.localPosition = value.pos;
            foreach (var item in value.grids)
            {
                item.Owner = this;
                this.AddMineGrid(item);
            }

            this._isDie = false;
        }
    }
    /// <summary>
    /// 记录我自己拥有的格子
    /// </summary>
    /// <param name="grid"></param>
    public void AddMineGrid(MapGrid grid)
    {
        if(mineList.IndexOf(grid) == -1)
            mineList.Add(grid);
    }

    public void RemoveGrid(MapGrid grid)
    {
        if (mineList.IndexOf(grid) != -1)
            mineList.Remove(grid);
    }
    public List<MapGrid> MyGrids
    {
        get
        {
            return mineList;
        }
    }
    /// <summary>
    /// 计算分数
    /// </summary>
    public int Score
    {
        get
        {
            return mineList.Count;
        }
    }
    public void Update()
    {
        if(this.IsDie == true)
        {
            return;
        }
        //AI
        tree.Tick();
        //

        _direction = Vector3.Lerp(_direction, targetV, Time.deltaTime / lerpTime).normalized;
        //
        this.transform.localPosition += Time.deltaTime * _speed * _direction;
        if(MapManager.IsOutMap(this))
        {
            this.Die();
            return;
        }

        checkGrid.Update();

    }
    /// <summary>
    /// 往某个方向移动
    /// </summary>
    /// <param name="vector"></param>
    public void Move(Vector3 vector)
    {   
        _direction = vector.normalized;
    }
    /// <summary>
    /// 平滑转到某个方向,主要给机器人用
    /// </summary>
    /// <param name="vector"></param>
    public void MoveLerp(Vector3 vector,float time = 2)
    {
        lerpTime = time;
        targetV = vector;
    }

    private void FillMyGrid(List<MapGrid> list)
    {

    }
    public virtual void Die()
    {
        this._isDie = true;
        foreach (var item in mineList)
        {
            item.Owner = null;
        }
        this.mineList.Clear();
        this.checkGrid.Reset();
        this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, -100);
        _direction = Vector3.zero;
    }
    public virtual void Relife()
    {
        this.BornPoint = MapManager.GetRandomBornPoint();
        this._isDie = false;
    }
    /// <summary>
    /// 玩家是否已经接近边界了
    /// </summary>
    /// <returns></returns>
    public bool IsNearSide
    {
        get
        {
            return MapManager.IsNearSide(this);
        }
    }    


    public bool IsCreateRoute
    {
        set
        {
            tree.ai.IsCreateRoute = false;
        }
    }
}
