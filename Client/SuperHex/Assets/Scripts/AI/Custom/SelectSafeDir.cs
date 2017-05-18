using UnityEngine;
using System.Collections;
using Behavior3CSharp.Core;
using Behavior3CSharp;

namespace AI
{
    public class SelectSafeDir : Action
    {
        private PlayerAIExt ai;
        public SelectSafeDir(B3Settings settings)
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
            
            int count = 0;
            Vector3 v = new Vector3(1, 0, 0);
            v = Quaternion.AngleAxis(Random.Range(0, 360), new Vector3(0, 0, 1)) * v;
            Vector3 f = v * 10 + this.ai.Player.transform.localPosition;

            while(MapManager.GetGridByPos(f) == null && count < 100)
            {
                v = Quaternion.AngleAxis(Random.Range(0, 360), new Vector3(0, 0, 1)) * v;
                f = v * 10 + this.ai.Player.transform.localPosition;
                count++;
            }
            this.ai.Player.MoveLerp(f,1f);

            return B3Status.RUNNING;
        }
    }

}
