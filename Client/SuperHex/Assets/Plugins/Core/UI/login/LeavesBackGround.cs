using UnityEngine;
using DG.Tweening;

public class LeavesBackGround : LeafObject {


#if UNITY_EDITOR
    private bool isAway = true;
    private float hitTimer = -10;
#endif

    public float waterDistanceLeval = 0.01f;
    public RectTransform waterBackgraound;
    private Vector3 _waterOriginScale;

    private float _lastTime = 0.2f;
    private float _timer = 0;

    void Awake () {

    }

	// Use this for initialization
    new void Start () {
        base.Start();
        _waterOriginScale = waterBackgraound.localScale;
    }
	
	// Update is called once per frame
	void Update () {
#if UNITY_EDITOR
//        if (Input.GetMouseButton(0) && (Time.time - hitTimer > 3)) {
//            hitTimer = Time.time;
//            if (isAway) {
//                goIn();
//                isAway = false;
//            }
//            else {
//                goOut();
//                isAway = true;
//            }
//        }
#endif
	}
//
//    public void refresh () {
//	    _timer = Time.time;
//    }

    public Tweener goIn() {
        BroadcastMessage("goNear");
        waterBackgraound.DOKill(true);
        return waterBackgraound.DOScale((1+waterDistanceLeval) * _waterOriginScale, 1.5f);
    }

    public Tweener goOut() {
        BroadcastMessage("goAway");
        waterBackgraound.DOKill(true);
        return waterBackgraound.DOScale(_waterOriginScale, 1.5f);
    }
}