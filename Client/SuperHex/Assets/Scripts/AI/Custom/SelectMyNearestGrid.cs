using UnityEngine;
using System.Collections;
using Behavior3CSharp.Core;
using Behavior3CSharp;
using System.Collections.Generic;

namespace AI
{
    public class SelectMyNearestGrid : Action
    {
        private PlayerAIExt ai;
        public SelectMyNearestGrid(B3Settings settings)
            : base(settings)
        {

        }
        protected override void OnOpen(Tick tick)
        {
            ai = tick.Target as PlayerAIExt;
        }
        protected override B3Status OnTick(Tick tick)
        {
            float n = 99999;

            MapGrid grid = null;
            List<MapGrid> list = ai.Player.MyGrids;
            foreach (var item in list)
            {
                float m = Vector3.Distance(ai.Player.transform.localPosition, item.Center);
                if (m < n)
                {
                    n = m;
                    grid = item;
                }
            }

            if(grid != null)
            {
                Vector3 v = grid.Center - ai.Player.transform.localPosition;
                ai.Player.MoveLerp(v);
                return B3Status.RUNNING;
            }
            else
            {
                return B3Status.FAILURE;
            }
            
        }

    }

}
