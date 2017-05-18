using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    /// <summary>
    /// 专门处理AI的一些行为
    /// </summary>
    public class PlayerAIExt
    {
        private Player player;

        //AI状态
        private bool isRandomDir;

        public int pointIndex = 1;

        private int currentIndex;

        private List<Vector3> pointList = new List<Vector3>();
        private bool _isCreateRoute;
        private bool _isCanCreateRoute = true;
        /// <summary>
        /// 移动距离，提高AI的警惕性会缩小移动范围以保证安全
        /// </summary>
        private float moveDis = 7.0f;
        public PlayerAIExt(Player p)
        {
            this.player = p;
        }
        public Player Player
        {
            get
            {
                return player;
            }
        }



        
        public bool IsCreateRoute
        {
            get
            {
                return _isCreateRoute;
            }
            set
            {
                _isCreateRoute = false;
            }
        }
        public bool IsCanCreateRoute
        {
            get
            {
                return Player.CheckGridAction.IsBegin;
            }
        }
        /// <summary>
        /// 创建一条路径
        /// </summary>
        public bool CreateRoute()
        {
            pointList.Clear();

            pointIndex = 1;
            currentIndex = 0;

            pointList.Add(Player.transform.localPosition);
            //
            Vector3 axis = new Vector3(0,0,1);

            int dir = Random.Range(0,2) == 0 ? 1 : -1;
            
            Vector3 ov = this.player.Direction * moveDis;

            Vector3 v1 = ov;
            Vector3 v3 = Quaternion.AngleAxis(90 * dir, axis) * v1;
            Vector3 v2 = v1 + v3;

            pointList.Add(Player.transform.localPosition + v1);
            pointList.Add(Player.transform.localPosition + v2);
            pointList.Add(Player.transform.localPosition + v3);


            GameObject go = Resources.Load<GameObject>(AssetPathUtil.GetPlayerPath(0));
            for (int i = 1; i < pointList.Count; i++)
            {
                //GameObject o = GameObject.Instantiate<GameObject>(go);
                //o.transform.localPosition = pointList[i];
                //o.transform.SetParent(LayerManager.mapLayer, false);
            }

            isRandomDir = false;

            foreach (var item in pointList)
            {
                if(MapManager.GetGridByPos(item) == null || MapManager.IsNearSideByPos(item))
                {
                    
                    return false;
                }
            }



            _isCreateRoute = true;
            return true;
        }

        public void MoveToPoint()
        {
            if (pointIndex >= pointList.Count)
            {
                return;
            }

            Vector3 point = pointList[pointIndex];

            Vector3 v = point - Player.transform.localPosition;
            Player.MoveLerp(v);
            //
            if (Vector3.Distance(point, Player.transform.localPosition) < 2)
            {
                pointIndex++;
            }
        }

        public void RandomDir()
        {
            if (isRandomDir)
                return;

            isRandomDir = true;

            Vector3 v = new Vector3(1, 0, 0);
            v = Quaternion.AngleAxis(Random.Range(0, 360), new Vector3(0, 0, 1)) * v;
            Player.MoveLerp(v);
        }
    }
}
