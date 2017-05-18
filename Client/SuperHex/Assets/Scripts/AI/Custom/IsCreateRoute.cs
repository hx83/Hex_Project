using UnityEngine;
using System.Collections;
using Behavior3CSharp.Core;
using Behavior3CSharp;

namespace AI
{
    /// <summary>
    /// 是否已经创建好一条路线
    /// </summary>
    public class IsCreateRoute : Condition
    {
        private PlayerAIExt ai;
        public IsCreateRoute(B3Settings settings)
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
            if (ai.IsCreateRoute)
                return B3Status.SUCCESS;
            else
                return B3Status.FAILURE;
        }
    }

}
