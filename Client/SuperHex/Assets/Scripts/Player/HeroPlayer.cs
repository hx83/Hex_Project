using UnityEngine;
using System.Collections;

public class HeroPlayer : Player
{
    //private MouseJoyStick joystick;
    private HRYJoyStick _joystick;
   	public HeroPlayer():base()
    {
        //this.joystick = this.transform.gameObject.AddComponent<MouseJoyStick>();
        //this.joystick.Drag.AddListener(OnChangeDir);
        
        //this.joystick.Online();
    }

    public HRYJoyStick JoyStick
    {
        set
        {
            if(JoyStick != null)
            {
                JoyStick.Drag.RemoveListener(OnChangeDir);
            }
            _joystick = value;
            _joystick.Drag.AddListener(OnChangeDir);
            _joystick.Online();
        }
        get
        {
            return _joystick;
        }
    }

    private void OnChangeDir()
    {
        this.Move(this.JoyStick.DIRECTION);
    }

    public override void Die()
    {
        base.Die();
        this.DispatchEventInstance(new PlayerEvent(PlayerEvent.DIE));
    }
    public virtual void Relife()
    {
        base.Relife();
        this.DispatchEventInstance(new PlayerEvent(PlayerEvent.RELIFE));
    }
}
