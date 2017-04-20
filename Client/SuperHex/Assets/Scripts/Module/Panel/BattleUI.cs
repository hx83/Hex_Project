using UnityEngine;
using System.Collections;

public class BattleUI : EventDispatcher
{
    private Transform _mainUI;
    private HRYJoyStick _joyStick;
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
        
        //UpdateOperateArea();
        DispatchEvent(BaseEvent.COMPLETE);
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
}
