using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class BattleUI : EventDispatcher
{
    private Transform _mainUI;
    private HRYJoyStick _joyStick;
    private Text txt;
	public BattleUI()
    {
        
    }
    public void Init()
    {
        ResourceManager.GetResource(AssetPathUtil.GetUI("BattleUI"), OnGetMainUI);
    }

    private void OnGetMainUI(object content)
    {
        _mainUI = GameObject.Instantiate(content as GameObject).transform;
        _mainUI.SetParent(LayerManager.uiLayer, false);

        _joyStick = _mainUI.FindChild("Joystick").gameObject.AddComponent<HRYJoyStick>();

        txt = _mainUI.FindChild("RankUI/RankListItem0/TxtScore").GetComponent<Text>();

        //UpdateOperateArea();
        DispatchEvent(BaseEvent.COMPLETE);

        PlayerManager.AddEventListener(PlayerEvent.SORT_SCORE, OnSortScore);
    }
    public HRYJoyStick JoyStick
    {
        get
        {
            return _joyStick;
        }
    }

    public void Show()
    {
        _mainUI.gameObject.SetActive(true);

    }
    public void Hide()
    {
        _mainUI.gameObject.SetActive(false);
        _joyStick.Reset();
    }
    //
    private void OnSortScore(BaseEvent evt)
    {
        List<Player> list = evt.EventObj as List<Player>;
        for (int i = 0; i < list.Count; i++)
        {
            Player p = list[0];
            txt.text = p.Score.ToString() ;
        }
    }
}
