using UnityEngine;
using System.Collections;
using Behavior3CSharp.Core;
using Behavior3CSharp;

namespace AI
{
    public class IsDangerous : Condition
    {
        private PlayerAIExt ai;
        public IsDangerous(B3Settings settings)
            : base(settings)
        {

        }

        protected override void OnOpen(Tick tick)
        {
            ai = tick.Target as PlayerAIExt;
        }
        protected override B3Status OnTick(Tick tick)
        {
            base.OnTick(tick);

            if(this.ai.Player.IsNearSide)
            {
                return B3Status.SUCCESS;
            }
            else
            {
                return B3Status.FAILURE;
            }
        }
    }
}
