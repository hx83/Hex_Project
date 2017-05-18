using UnityEngine;
using System.Collections;
using Behavior3CSharp.Core;
using Behavior3CSharp;

namespace AI
{
    /// <summary>
    ///  是否满足可以创建路线的条件--已经走出自己范围边缘
    /// </summary>
    public class IsCanCreateRoute : Condition
    {
        private PlayerAIExt ai;
        public IsCanCreateRoute(B3Settings settings)
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
            
            if (ai.IsCanCreateRoute)
                return B3Status.SUCCESS;
            else
                return B3Status.FAILURE;
        }
    }

}
