using UnityEngine;
using System.Collections;
using Behavior3CSharp.Core;
using Behavior3CSharp;

namespace AI
{
    public class RandomDir : Action
    {
        private PlayerAIExt ai;
        public RandomDir(B3Settings settings)
            : base(settings)
        {
            
        }
        protected override void OnOpen(Tick tick)
        {
            ai = tick.Target as PlayerAIExt;
        }
        protected override B3Status OnTick(Tick tick)
        {
            ai.RandomDir();
            return B3Status.SUCCESS;
        }
    }

}
