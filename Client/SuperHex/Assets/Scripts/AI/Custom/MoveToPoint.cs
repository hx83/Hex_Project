using UnityEngine;
using System.Collections;
using Behavior3CSharp.Core;
using Behavior3CSharp;

namespace AI
{
    /// <summary>
    /// 移向某个点
    /// </summary>
    public class MoveToPoint : Action
    {
        private int index;
        private PlayerAIExt ai;
        public MoveToPoint(B3Settings settings)
            : base(settings)
        {
            index = int.Parse(this._properties["index"]);
        }
        protected override void OnOpen(Tick tick)
        {
            ai = tick.Target as PlayerAIExt;
        }
        protected override B3Status OnTick(Tick tick)
        {
            base.OnTick(tick);
            if (ai.pointIndex == index)
            {
                ai.MoveToPoint();
                return B3Status.RUNNING;
            }
            else
            {
                return B3Status.FAILURE;
            }
        }
    }

}
