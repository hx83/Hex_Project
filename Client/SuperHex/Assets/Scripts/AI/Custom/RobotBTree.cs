using UnityEngine;
using System.Collections;
using Behavior3CSharp.Core;
using AI;
public class RobotBTree : BehaviorTree 
{
    public PlayerAIExt ai;

    public float TurnMaxDis;
    public float ForwardMaxDis;
	public RobotBTree(object target = null):base(target)
    {
        this.ai = target as PlayerAIExt;

    }
}
