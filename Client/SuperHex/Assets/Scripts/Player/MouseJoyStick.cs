using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class MouseJoyStick : MonoBehaviour 
{

    [System.Serializable]
    public class OnDragHandler : UnityEvent { }

    public OnDragHandler Drag = new OnDragHandler();
    public OnDragHandler Stop = new OnDragHandler();

    //private GameObject referObj;
    private Vector3 mDirection = Vector3.forward;

    private bool _isOnline;

    private bool _isStop;

    private int count = 0;
    private float time = 0f;

    private Vector3 ov;
	// Use this for initialization
	void Start () 
    {

	}
   

    public Vector3 DIRECTION
    {
        get { return mDirection; }
        set { mDirection = value; }
    }

    //public void SetPlyaer(GameObject obj)
    //{
    //    this.referObj = obj;
    //}

    public void Online()
    {
        _isOnline = true;
        ov = new Vector3(Screen.width/2,Screen.height/2);
    }

    void FixedUpdate()
    {
        if(!_isOnline)
        {
            return;
        }

        if(_isStop)
        {
            this.DIRECTION = Vector3.zero;
            this.Stop.Invoke();
        }
        else
        {
            Vector3 v = Input.mousePosition - ov;
            Vector3 v2 = new Vector3(v.x, v.y, 0);
            v2 = v2.normalized;
            this.DIRECTION = v2;
            this.Drag.Invoke();
        }
        
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _isStop = false;
        }
        
        if(Input.GetKeyDown(KeyCode.Space))
        {
            _isStop = !_isStop;
        }
    }
}
