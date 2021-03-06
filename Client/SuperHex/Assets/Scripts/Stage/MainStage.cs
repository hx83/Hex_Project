﻿using UnityEngine;
using System.Collections;

public class MainStage : EventDispatcher
{
    private HeroPlayer Hero;
    private BattleUI battleUI;
    private bool isStart;

	public void Init()
    {
        LayerManager.SetMapLayer(GameObject.Find("Map").transform);
        LayerManager.SetUILayer(GameObject.Find("UICanvas").transform);

        MapManager.CreateMap(100, 80, LayerManager.mapLayer);

        ModuleManager.Show(ModuleType.MAIN_PANEL);

        CameraManager.MoveTo(new Vector3(34,34));
    }
    public void Start()
    {
        Vector3 v = new Vector3(1, 0, 0);
        Vector3 v2 = Quaternion.AngleAxis(45, new Vector3(0, 0, 1))*v;

        if (isStart == false)
        {
            Hero = PlayerManager.CreateHero();
            Hero.BornPoint = MapManager.GetRandomBornPoint();
            Hero.AddEventListener(PlayerEvent.DIE, OnHeroDie);
            Hero.AddEventListener(PlayerEvent.RELIFE, OnHeroRelife);

            MapManager.AddPlayer(Hero);
            CameraManager.Follow(Hero.transform);

            battleUI = new BattleUI();
            battleUI.AddEventListener(BaseEvent.COMPLETE, OnBattleUIComplete);
            battleUI.Init();

            isStart = true;
        }
        else
        {
            Hero.Relife();
        }
	}

    private void OnBattleUIComplete(BaseEvent e)
    {
        Hero.JoyStick = battleUI.JoyStick;
    }
	
    public void Update()
    {
	
	}


    private void OnHeroDie(BaseEvent evt)
    {
        ModuleManager.Show(ModuleType.MAIN_PANEL);
        battleUI.Hide();
    }

    private void OnHeroRelife(BaseEvent evt)
    {
        battleUI.Show();
    }
}
