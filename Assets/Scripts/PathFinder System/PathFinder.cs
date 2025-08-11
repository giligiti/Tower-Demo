using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace PathFind
{
    public class PathFinder : MonoBehaviour
    {
        public enum E_GridState
        {
            start,
            end,
            wall,
            normal,
        }
        [HideInInspector]
        public PathNode starNode;
        [HideInInspector]
        public PathNode endNode;
        public bool Point = false;
        [HideInInspector]
        Stack<PathNode> path = new Stack<PathNode>();

        AStar pathFinder;
        MainGrid<PathNode> grid;

        public E_GridState state = E_GridState.wall;

        [Header("网格精度")]
        [Range(1, 10)]
        public int _GSize{get{ return _gSize; } set{ _gSize = value; Init(); }}
        int _gSize = 2;

        [Header("设置终点")]
        public Vector2Int EndGrid;

        private void Init()
        {
            pathFinder = new AStar(24, 24, _gSize);
            grid = pathFinder.GetMainGridObj();
        }

        void Awake()
        {
            
        }
        private void Start()
        {
            Init();
            CheckStateByBounds();
        }
        private void Update()
        {
            OnMc();
        }
        private void OnMc()
        {
            // if (Input.GetMouseButtonDown(0) && Point)
            // {
            //     Ray x = Camera.main.ScreenPointToRay(Input.mousePosition);
            //     Physics.Raycast(x, out RaycastHit hitInfo, 200f);
            //     UnityEngine.Debug.DrawLine(Camera.main.transform.position, hitInfo.point, Color.red, 3f);
            //     PathNode node = grid.GetNode(hitInfo.point);
            //     if (node != null)
            //         ChangeState(node);
            // }
        }

        private void CheckStateByBounds()
        {
            
        }

        public void ChangeState(PathNode node)
        {
            switch (state)
            {
                case E_GridState.start:
                    node.ChangeColor(Color.yellow);
                    starNode = node;
                    break;
                case E_GridState.end:
                    node.ChangeColor(Color.red);
                    endNode = node;
                    break;
                case E_GridState.wall:
                    node.BecomeWall();
                    break;
                case E_GridState.normal:
                    node.ChangeColor(Color.white);
                    node.isWall = false;
                    node.ChangeValue(node.Value + 1);
                    break;
            }
        }
        //开始寻路
        [ContextMenu("Add Score")]
        public void StartPathFinder()
        {
            Stopwatch sw = Stopwatch.StartNew();                                                                        //调试：计时
            sw.Start();                                                                                                 //调试：计时
            bool pathSuccess = pathFinder.StartPathFind(starNode, endNode, path);
            sw.Stop();                                                                                                  //调试：计时
            float time = sw.ElapsedMilliseconds;                                                                        //调试：计时
            if (pathSuccess)
            {
                UnityEngine.Debug.Log("寻路成功，耗时：" + time + "毫秒");
            }
            else
            {
                UnityEngine.Debug.Log("寻路失败，耗时：" + time + "毫秒");
            }
            StartCoroutine(PathCallBack(path));
        }
        //重置格子状态
        public void ResetGridState()
        {
            path = new Stack<PathNode>();
            pathFinder.ResetPathFinder();
        }

        /// <summary>
        /// 路径回溯显示
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        IEnumerator PathCallBack(Stack<PathNode> path)
        {
            foreach (var node in path)
            {
                node.ChangeColor(Color.green);
                yield return new WaitForSeconds(0.3f);
            }

        }
    }
}

