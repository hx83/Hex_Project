using UnityEngine;
using System.Collections;
using DG.Tweening;
public class MapGrid : MonoBehaviour {

    private int _xIndex;
    private int _yIndex;
    private bool _selected;

    private SpriteRenderer item;
    private SpriteRenderer mainSp;

    private Color color;
    private Color orColor;
    private Player _owner;
    private Player _prevOwner;
    /// <summary>
    /// 格子状态，空的，已被占用，被预占用
    /// </summary>
    private GridState _state;
	void Start () 
    {
        this.State = GridState.EMPTY;

        orColor = new Color(79 / 255f, 79 / 255f, 79 / 255f);
        item = this.transform.FindChild("item").GetComponent<SpriteRenderer>();
        mainSp = this.gameObject.GetComponent<SpriteRenderer>();
        //mainSp.sortingLayerName = LayerUtil.SORTING_LAYER_MAP_GRID;
	}
    //void Update()
    //{
    //    if(this._owner != null)
    //        Debug.Log(item.transform.localScale);
    //}
	public Player Owner
    {
        set
        {
            if(this.Owner == value)
            {
                return;
            }
            //
            if(value == null)
            {
                _owner = value;
                this.Reset();
            }
            else
            {
                if(value != _owner)
                {
                    if(_owner != null)
                        _owner.RemoveGrid(this);
                }
                _owner = value;
                this._state = GridState.OWN;
                this.Fill(value.Color);
            }
        }
        get
        {
            return _owner;
        }
    }

    public Player PrevOwner
    {
        set
        {
            _prevOwner = value;
            if (value == null)
            {
                this.Reset();
            }
            else
            {
                if(this.PrevOwner == this.Owner)
                {
                    return;
                }
                this.State = GridState.PREV_OWN;
                this.PrevFill(value.Color);
            }
        }
        get
        {
            return _prevOwner;
        }
    }

    public int xIndex
    {
        set
        {
            _xIndex = value;
        }
        get
        {
            return _xIndex;
        }
    }
    public int yIndex
    {
        set
        {
            _yIndex = value;
        }
        get
        {
            return _yIndex;
        }
    }
    /// <summary>
    /// 格子当前状态
    /// </summary>
    public GridState State
    {
        get
        {
            return this._state;
        }
        set
        {
            this._state = value;
        }
    }
    //public bool Selected
    //{
    //    set
    //    {
    //        _selected = value;
    //        if(value == false)
    //        {
    //            mainSp.color = Color.white;
    //        }
            
    //    }
    //    get
    //    {
    //        return _selected;
    //    }
    //}
    public void PrevFill(Color color)
    {
        //float h, s, v;
        //Color.RGBToHSV(color, out h,out s,out v);
        //mainSp.color = Color.HSVToRGB(h, s/2f, v);
        mainSp.color = new Color(color.r, color.g, color.b, 0.3f);

        
    }

    private bool isFill;

    public void Fill(Color c)
    {
        mainSp.color = orColor;
        this.color = c;
        item.color = color;

        if(isFill == false)
        {
            isFill = true;
            Tween t = item.transform.DOScale(1, 0.3f);
            t.OnComplete(FillComplete);
        }        
    }


    private void FillComplete()
    {
        mainSp.color = color;
        item.transform.localScale = Vector3.zero;
        isFill = false;
    }

    private void Reset()
    {
        if(this.PrevOwner == null && this.Owner == null)
        {
            mainSp.color = orColor;
            item.transform.localScale = Vector3.zero;
            this.State = GridState.EMPTY;
        }
        else
        {
            if(this.PrevOwner != null && this.Owner == null)
            {
                this.State = GridState.PREV_OWN;
                this.PrevFill(this.PrevOwner.Color);
            }
            else
            {
                this._state = GridState.OWN;
                this.Fill(this.Owner.Color);
            }
        }
    }

}


public class GridConst
{
    public static float Radius = 50 / 100f;
    public static float Height = Mathf.Sqrt(Radius * Radius - Radius / 2 * Radius / 2);

    public static float BorderWidth = 100 / 100f;
}

public enum GridState
{
    EMPTY,
    OWN,
    PREV_OWN,
}