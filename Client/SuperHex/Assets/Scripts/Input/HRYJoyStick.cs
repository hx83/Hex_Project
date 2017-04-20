using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// 模拟摇杆,主要控制器
/// 全部采用 Position 坐标
/// </summary>
public class HRYJoyStick : MonoBehaviour, IDragHandler, IBeginDragHandler, IPointerDownHandler, IPointerUpHandler
{
	public enum TYPE
	{
		/// <summary>
		/// 十字键
		/// </summary>
		Cross,
		/// <summary>
		/// 重定位 + 拖拽
		/// </summary>
		Anchor,
		/// <summary>
		/// 从定位 + 更随
		/// </summary>
		Follow,
	}
	public enum STATE
	{
		pointerUp,
		pointerDown,

		beginDrag,
		drag,
	}
	public string JoyName = "";
	public TYPE type = TYPE.Anchor;
	public STATE state = STATE.pointerUp;
	public Image Joy;
	public Image Stick;
	public float Inner = 30;
	public float Outer = 80;
    private RectTransform _rectTransform;

    #region -> UnityEvent <-
    [System.Serializable] public class OnPointerDownHandler : UnityEvent{}
	[System.Serializable] public class OnPointerUpHandler : UnityEvent{}
	[System.Serializable] public class OnBeginDragHandler : UnityEvent{}
	[System.Serializable] public class OnDragHandler : UnityEvent{}

    /////////////////////////// 当前使用 //////////////////////////////

    public OnPointerDownHandler PointerDown = new OnPointerDownHandler();
	public OnPointerUpHandler PointerUp = new OnPointerUpHandler ();
	public OnBeginDragHandler BeginDrag = new OnBeginDragHandler ();
	public OnDragHandler Drag = new OnDragHandler ();

    /////////////////////////// 辅助侦听器 //////////////////////////////
    public OnPointerDownHandler SubPointerDown = new OnPointerDownHandler ();
	public OnPointerUpHandler SubPointerUp = new OnPointerUpHandler ();
	public OnBeginDragHandler SubBeginDrag = new OnBeginDragHandler ();
	public OnDragHandler SubDrag = new OnDragHandler ();
	/////////////////////////////////////////////////////////
	#endregion
	private float mDistence = 0;
	public float DISTANCE
	{
		get{return mDistence;}
		set{mDistence = value;}
	}
	private Vector3 mDirection = Vector3.forward;
	public Vector3 DIRECTION
	{
		get{return mDirection;}
		set{mDirection = value;}
	}

	public Vector3 ANCHOR = Vector3.zero;
	public Vector3 OFFSET = Vector3.zero;
	public float Percent = 0;
	public Vector3 DRAGAMOUNT = Vector3.zero;
	public Vector3 SPACE = Vector3.zero;
	public Vector3 INPUTAMOUNT = Vector3.zero;

	public float HORIZENTAL
	{
		get
		{
			return DRAGAMOUNT.x;
		}
	}
	public float VERTICAL
	{
		get
		{
			return DRAGAMOUNT.y;
		}
	}
	public Vector3 StartAnchor = Vector3.zero;
	Transform TJoy;
	Transform TStick;

    void Awake()
    {
        _rectTransform = this.transform as RectTransform;
    }

	public void Online()
	{
		Inner = 0;
		Outer = 60;
        //if (Global.isJoystickFollow)
            type = TYPE.Anchor;
        //else
            //type = TYPE.Cross;
	}
	void Start()
	{
        TJoy = transform;
        Joy = TJoy.gameObject.GetComponent<Image>();
        StartAnchor = Joy.rectTransform.position;
        TStick = transform.Find("Stick");
        Stick = TStick.gameObject.GetComponent<Image>();
//		ControllerManager.Instance.Register (this);
	}

	#if UNITY_EDITOR
	void FixedUpdate()
	{
		if(state == STATE.pointerUp)
		{
			float KeyBoardH = Input.GetAxis ("Horizontal");
			float KeyBoardV = Input.GetAxis ("Vertical");
			SimulateDrag(KeyBoardH,KeyBoardV);
		}
	}

	//键盘拖拽摇杆模拟 PC Editor 调试专用
	void SimulateDrag(float x,float y)
	{
		//Debug.Log (name+ OFFSET);
		OFFSET = new Vector3 (x,y);
		DIRECTION = OFFSET.normalized;
		DRAGAMOUNT = DIRECTION;
        if (x != 0 || y != 0)
        {
            this.Drag.Invoke();
        }
        else
            OnPointerUp(null);
    }
#endif

	#region -> 事件 <-
	public void OnPointerDown(PointerEventData eventData)
	{
		
		Begin (eventData.position);
		this.state = STATE.pointerDown;
		this.PointerDown.Invoke ();
		this.SubPointerDown.Invoke ();
    }
	public void OnPointerUp(PointerEventData eventData)
	{
		this.state = STATE.pointerUp;
		this.PointerUp.Invoke ();
		this.SubPointerUp.Invoke ();
		End ();
        
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		this.state = STATE.beginDrag;

	}
	public void OnDrag(PointerEventData eventData)
	{
		this.state = STATE.drag;
        //Debug.Log(name + " OnDrag" + DRAGAMOUNT);
        this.Drag.Invoke ();
		this.SubDrag.Invoke ();
		Move (eventData.position);

	}
	#endregion

	/// <summary>
	/// 定位 模拟摇杆
	/// </summary>
	public void Begin(Vector2 _ScreenPoint)
	{
		
		CheckOffsetInfo (GetRectlocalposition(_ScreenPoint));
		switch(type)
		{
		case TYPE.Cross:
			Stick.rectTransform.localPosition = GetStickOffset (OFFSET);
//			UIStick.Show ();
			break;

			case TYPE.Anchor:
//			UIStick.Show ();
			Joy.rectTransform.localPosition = GetJoyAnchor (OFFSET);
			break;
			case TYPE.Follow:
//			UIStick.Show ();
			Joy.rectTransform.localPosition = GetJoyAnchor (OFFSET);
			break;
		}
	}
	public void Move(Vector3 _ScreenPoint)	
	{
		CheckOffsetInfo (GetRectlocalposition(_ScreenPoint));
		switch(type)
		{
			case TYPE.Cross:
			Stick.rectTransform.localPosition = GetStickOffset (OFFSET);
			break;

			case TYPE.Anchor:
			Stick.rectTransform.localPosition = GetStickOffset (OFFSET);

			break;
		case TYPE.Follow:

			Joy.rectTransform.localPosition = GetFollowSpace ();
			Stick.rectTransform.localPosition = GetStickOffset (OFFSET);


			break;
		}
	}
	public void End()
	{
        if (this.Stick != null && this.Stick.rectTransform != null)
        {
			this.Stick.rectTransform.localPosition = Vector3.zero;
        }
		this.DISTANCE = 0;
		this.DRAGAMOUNT = Vector3.zero;
		this.SPACE = Vector3.zero;
		switch(type)
		{
			case TYPE.Cross:
//			UIStick.Hide ();
			break;

			case TYPE.Anchor:
                Joy.rectTransform.position = StartAnchor;
//			UIStick.Hide ();
                break;

			case TYPE.Follow:
//			UIStick.Hide ();
			break;
		}
	}

	#region -> 坐标判断 <-
	/// <summary>
	/// 计算 ScreenPoint and Joy 的偏移数据
	/// </summary>
	private void CheckOffsetInfo(Vector3 _Rectlocalposition)
	{
		
		ANCHOR = _Rectlocalposition;
		OFFSET = ANCHOR - Joy.rectTransform.localPosition;

		DISTANCE = OFFSET.magnitude;
		DIRECTION = OFFSET.normalized;
	}
	/// <summary>
	/// 计算圈的数据
	/// 返回 Stick 合适的偏移坐标
	/// </summary>
	Vector3 GetStickOffset(Vector3 _offset)
	{
		if (DISTANCE > Inner || DRAGAMOUNT.magnitude > 0) 
		{
			if (DISTANCE > Outer) 
			{
				DRAGAMOUNT = DIRECTION * Outer;
			} 
			else 
			{
				DRAGAMOUNT = _offset;
			}
		}
		Percent = DRAGAMOUNT.magnitude / Outer;
		return DRAGAMOUNT;
	}
	/// <summary>
	/// 返回 Joy 的偏移量
	/// </summary>
	Vector2 GetJoyAnchor(Vector3 _offset)
	{
		return ANCHOR;
	}
	/// <summary>
	/// 获取跟随后的摇杆位置
	/// </summary>
	Vector3 GetFollowSpace()
	{
		float space = DISTANCE - Outer;
		if (DISTANCE > Outer) 
		{
			if (space > 0) 
			{
				//超出范围跟随修正
				SPACE = space * DIRECTION;
			}
		} 
		else 
		{
			SPACE = Vector3.zero;
		}
		return Joy.rectTransform.localPosition + SPACE;
	}
	//----------------------------------------
	public Vector2 GetRectlocalposition(Vector2 _screenPoint)
	{
		Vector2 Rectlocalposition = Vector2.zero;
        if (LayerManager.uiCanvas == null)
            return Rectlocalposition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle (LayerManager.uiLayer,_screenPoint, LayerManager.uiCanvas.worldCamera, out Rectlocalposition);
		return Rectlocalposition;
	}
	#endregion

	public void Change(TYPE _type)
	{
//		UIStick.Show ();
		type = _type;
		Joy.rectTransform.localPosition = StartAnchor;

	}
    public RectTransform rectTransform
    {
        get
        {
            return _rectTransform;
        }
    }
    public void Reset()
	{
        End ();
	}
}
