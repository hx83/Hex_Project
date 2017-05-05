using UnityEngine;
using System.Collections;
using System;

public class MainEntry : MonoBehaviour 
{   
    //private bool isStart;

    private MainStage stage;

    
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

        
	}
    void FixedUpdate()
    {
        
    }


    
}
