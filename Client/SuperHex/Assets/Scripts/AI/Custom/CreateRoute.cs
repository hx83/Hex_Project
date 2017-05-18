using UnityEngine;
using System.Collections;
using Behavior3CSharp.Core;
using Behavior3CSharp;

namespace AI
{
    /// <summary>
    /// 创建一个行走路线
    /// </summary>
    public class CreateRoute : Action
    {
        private PlayerAIExt ai;
        public CreateRoute(B3Settings settings)
            : base(settings)
        {
            
        }
        protected override void OnOpen(Tick tick)
        {
            ai = tick.Target as PlayerAIExt;
        }
        protected override B3Status OnTick(Tick tick)
        {
            if(this.ai.CreateRoute())
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
