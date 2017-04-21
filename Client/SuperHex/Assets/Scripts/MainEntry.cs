using UnityEngine;
using System.Collections;
using System;

public class MainEntry : MonoBehaviour 
{   
    //private bool isStart;

    private MainStage stage;

    private static System.Action _nextFrameCall;
    public static MainEntry Instance;
	void Start () 
    {
        Application.targetFrameRate = 60;
        Instance = this.gameObject.GetComponent<MainEntry>();

        //模块管理初始化
        ModuleManager.Setup();
        //人物
        PlayerManager.Setup();
        //相机
        CameraManager.Setup(GameObject.Find("Main Camera").transform);
        

        stage = new MainStage();
        stage.Init();
	}

    public void GameStart()
    {
        stage.Start();
    }
	
	// Update is called once per frame
	void Update () 
    {
        CameraManager.Update();
        PlayerManager.Update();

        stage.Update();

        if (_nextFrameCall != null)
        {
            //_nextFrameCall.Invoke();
            Delegate[] list = _nextFrameCall.GetInvocationList();
            if (list != null)
            {
                int count = list.Length;
                for (int i = 0; i < count; i++)
                {
                    System.Action function = (System.Action)list[i];
                    _nextFrameCall -= function;
                    function.Invoke();
                }
                //_nextFrameCall = null;
            }
        }
	}
    void FixedUpdate()
    {
        
    }


    public static void RunInNextFrame(System.Action function)
    {
        if (function != null)
            _nextFrameCall += function;
    }
}
