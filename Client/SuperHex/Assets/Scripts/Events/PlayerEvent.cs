using UnityEngine;
using System.Collections;

public class PlayerEvent : BaseEvent
{

    public static string DIE = "die";
    public static string RELIFE = "relife";
    public static string SORT_SCORE = "sortScore";

    public PlayerEvent(string type, object eventObj = null):base(type,eventObj)
    {
        
    }
}
