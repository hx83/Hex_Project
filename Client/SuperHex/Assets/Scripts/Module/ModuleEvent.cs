using System;

/**
 * anthor J
 * 
 */
public class ModuleEvent : BaseEvent
{
    public const string TEAM_PUSH = "TEAM_PUSH";
    public const string SHOW = "show";
    public const string HIDE = "hide";
    public const string SELECT = "SELECT";
    //--------------------//

    public string moduleType;
    public ModuleEvent(string type, string moduleType, object data = null)
        : base(type, data)
    {
        this.moduleType = moduleType;
    }
}