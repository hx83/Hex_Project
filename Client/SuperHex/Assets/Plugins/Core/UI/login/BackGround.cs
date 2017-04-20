using UnityEngine;
using System.Collections;
using DG.Tweening;

public class BackGround : MonoBehaviour {

    private static BackGround _instance;

    public static BackGround Instance;

    public static bool startp = true;

//    public static void show () {
//        if (_instance == null) return;
//        _instance.gameObject.SetActive(true);
//    }

//    public static void hide () {
//        if (_instance == null) return;
//        _instance.gameObject.SetActive(false);
//    }

    public static void goNear () {
        _instance.leavesBack.goIn();
    }

    public static void goAway () {
        _instance.leavesBack.goOut();
    }

    public LeavesBackGround leavesBack;

    private float _timer;
    private float _lastTime = 0.3f;

    void Awake () {
        if (_instance == null) {
            _instance = this;
        }
        //DontDestroyOnLoad(gameObject);
    }

	// Use this for initialization
	void Start () {
//        BackGround.goNear();
	}
	
	// Update is called once per frame
	void Update () {
	    //if (Time.time - _timer > _lastTime) {
	    //    gameObject.SetActive(false);
	    //}
	}

    public static void updateBackGround () {
        if (_instance == null) return;
        _instance.gameObject.SetActive(true);
        _instance._timer = Time.time;
    }
}
