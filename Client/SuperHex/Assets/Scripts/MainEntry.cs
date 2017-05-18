using UnityEngine;
using System.Collections;
using System;
using Behavior3CSharp;
using AI;

public class MainEntry : MonoBehaviour 
{   
    //private bool isStart;

    private MainStage stage;

    
    public static MainEntry Instance;
	void Start () 
    {
        Application.runInBackground = true;
        Application.targetFrameRate = 60;
        Instance = this.gameObject.GetComponent<MainEntry>();

        //模块管理初始化
        ModuleManager.Setup();
        //人物
        PlayerManager.Setup();
        //相机
        CameraManager.Setup(GameObject.Find("Main Camera").transform);
        //


        B3Config.Register("CreateRoute", typeof(CreateRoute));
        B3Config.Register("IsCanCreateRoute", typeof(IsCanCreateRoute));
        B3Config.Register("IsCreateRoute", typeof(IsCreateRoute));
        B3Config.Register("MoveToPoint", typeof(MoveToPoint));
        B3Config.Register("RandomDir", typeof(RandomDir));
        B3Config.Register("SelectMyNearestGrid", typeof(SelectMyNearestGrid));
        B3Config.Register("IsDangerous", typeof(IsDangerous));
        B3Config.Register("SelectSafeDir", typeof(SelectSafeDir));

        AIManager.Setup();
        //

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

        
	}
    void FixedUpdate()
    {
        
    }


    
}
