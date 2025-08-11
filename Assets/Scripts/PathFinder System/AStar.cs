using System.Collections.Generic;
using ToolSpace;
using UnityEngine;

namespace PathFind
{
    /// <summary>
    /// A*寻路算法
    /// </summary>
    public class AStar
    {

        private const int XCost = 14;
        private const int VCost = 10;

        public PathNode StartNode { get; private set; }
        public PathNode EndNode { get; private set; }

        public int width = 8;
        public int height = 8;
        public int gridSize = 2;

        //创建寻路网格系统
        MainGrid<PathNode> mainGrid;

        //开放列表//存储需要检查的点
        //用于去重
        private Dictionary<PathNode, int> openDictionary = new Dictionary<PathNode, int>();
        //用于排序
        private CustomePriorityQueue<PathNode, int> openList = new CustomePriorityQueue<PathNode, int>();

        //关闭列表//存储不再需要检查的点
        private HashSet<PathNode> closedList = new HashSet<PathNode>();

        #region 寻路流程

        //1.得到终点起点
        //2.起点进入“加入关闭列表并得到周围的点”的流程
        //3.对周围的点进行筛选，已经存在在开放列表和关闭列表点的排除，加入开放列表
        //4.
        //
        //
        //
        //
        //
        //
        //
        //
        //
        //

        #endregion

        public AStar(int width, int height, int gridSize)
        {
            GridCreatInit(width, height, gridSize);
        }

        private void GridCreatInit(int width, int height, int gridSize)
        {
            this.width = width;
            this.height = height;
            this.gridSize = gridSize;
            mainGrid = new MainGrid<PathNode>();
            mainGrid.CreatGrid(width, height, gridSize);
        }

        /// <summary>
        /// 提供给外界来获得对应的网格系统，来得到、修改节点的值
        /// </summary>
        /// <returns></returns>
        public MainGrid<PathNode> GetMainGridObj()
        {
            return mainGrid;
        }

        #region 开始寻路方法

        /// <summary>
        /// 开始寻路方法
        /// </summary>
        /// <param name="start">起点</param>
        /// <param name="end">终点</param>
        /// <param name="path">路径</param>
        /// <returns></returns>
        private bool StartPathFind(Stack<PathNode> path)
        {
            //终点判断是否是障碍物
            if (EndNode.isWall || StartNode == EndNode)
                return false;

            GetFinallyPath(StartNode, EndNode);
            //寻路失败

            if (EndNode.fatherNode == null && openDictionary.Count == 0) return false;

            //寻路成功
            //执行路径回溯
            RetracePath(StartNode, EndNode, path);

            return true;
        }

        public bool StartPathFind(Vector3 start, Vector3 end, Stack<PathNode> path)
        {
            //初始化起点终点的节点
            InitStartEndPoint(mainGrid.GetNode(start), mainGrid.GetNode(end));

            return StartPathFind(path);
        }

        public bool StartPathFind(PathNode start, PathNode end, Stack<PathNode> path)
        {
            InitStartEndPoint(start, end);
            return StartPathFind(path);
        }

        #endregion

        /// <summary>
        /// 起点终点初始化
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        private void InitStartEndPoint(PathNode start, PathNode end)
        {
            this.StartNode = start;
            this.EndNode = end;
            //起点和终点的父对象清空
            StartNode.fatherNode = null;
            EndNode.fatherNode = null;

            //起点加入关闭列表
            PushIntoCloseList(StartNode);
        }


        /// <summary>
        /// 寻找终点方法
        /// </summary>
        /// <param name="node"></param>
        private void GetFinallyPath(PathNode node, PathNode targetNode)
        {
            while (true)
            {
                PathNode pathNode = OpenToClose(node);
                if (pathNode == targetNode || pathNode == null)
                    return;
                node = pathNode;
            }

        }

        /// <summary>
        /// 选出周围最优点加入关闭列表
        /// </summary>
        /// <param name="node"></param>
        private PathNode OpenToClose(PathNode node)
        {
            //参数node都是上一步选出的最优点，本身已经在关闭列表中

            List<PathNode> list = GetSurroundNode(node);
            //所有有效节点加入优先队列
            foreach (PathNode node2 in list)
            {
                //如果已经在优先队列中的话，检查G值更新
                if (CheckUpdatGCost(node2, node))
                    continue;
                //不在优先队列中
                SetNodeParent(node2, node);         //设置节点的父节点
                node2.ChangeColor(Color.gray);                                                              //调试：改变网格颜色
                CalculationPathCost(node2, EndNode);//计算总代价
                                                    //list中的是经过筛选的的节点，都是可以加入开放列表的
                PushIntoOpenList(node2);
            }
            PathNode pathNode = DequeueOpenlist();
            PushIntoCloseList(pathNode);
            return pathNode;
        }

        /// <summary>
        /// 把点加入开放列表
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private void PushIntoOpenList(PathNode node)
        {
            openDictionary.Add(node, node.FCost);
            openList.Enqueue(node, node.FCost);
        }

        /// <summary>
        /// 设置点的父节点
        /// </summary>
        /// <param name="node"></param>
        /// <param name="fatherNode"></param>
        private void SetNodeParent(PathNode node, PathNode fatherNode)
        {
            node.fatherNode = fatherNode;
        }

        /// <summary>
        /// 返回优先队列中的优先级最高的点
        /// </summary>
        /// <returns></returns>
        private PathNode DequeueOpenlist()
        {
            while (openList.TryDequeue(out PathNode node, out int priority))
            {
                if (openDictionary.ContainsKey(node))
                {
                    openDictionary.Remove(node);
                    return node;
                }
            }
            openDictionary.Clear();
            return null;
        }

        /// <summary>
        /// 把点加入到关闭列表
        /// </summary>
        /// <param name="node"></param>
        private void PushIntoCloseList(PathNode node)
        {
            closedList.Add(node);
        }

        /// <summary>
        /// 得到目标点周围的有效点
        /// </summary>
        /// <param name="node"></param>
        private List<PathNode> GetSurroundNode(PathNode node)
        {
            //八个点
            PathNode pathNode;

            List<PathNode> nodesList = new List<PathNode>(8);

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    pathNode = mainGrid.GetNode(node.x + i, node.y + j);

                    //如果不符合要求
                    if (!CheckPoint(pathNode))
                        continue;

                    //如果是斜向移动的话检查是否可以通行，避免斜向穿墙
                    if (i != 0 && j != 0)
                    {
                        PathNode pathNode1 = mainGrid.GetNode(pathNode.x + i, pathNode.y);
                        PathNode pathNode2 = mainGrid.GetNode(pathNode.x, pathNode.y + j);
                        PathNode pathNode3 = mainGrid.GetNode(pathNode.x - i, pathNode.y);
                        PathNode pathNode4 = mainGrid.GetNode(pathNode.x, pathNode.y - j);
                        if (!CheckPoint(pathNode1) && !CheckPoint(pathNode2) || !CheckPoint(pathNode3) && !CheckPoint(pathNode4))
                            continue;

                    }
                    if (i == 0 && j == 0)//跳过自己
                        continue;

                    nodesList.Add(pathNode);
                }
            }
            return nodesList;

        }

        /// <summary>
        /// 检查点是否符合计算要求
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private bool CheckPoint(PathNode node)
        {
            //判空/是否障碍
            if (node == null || node.isWall || closedList.Contains(node)) return false;

            return true;
        }

        /// <summary>
        /// 检查是否需要更新G值并完成相关更新
        /// </summary>
        /// <param name="node"></param>
        private bool CheckUpdatGCost(PathNode node, PathNode fathernode)
        {
            if (openDictionary.ContainsKey(node))
            {
                int newCost = CalculationGenuineCost(node, fathernode);
                if (node.GCost > newCost)
                {
                    //更新G值，父对象，加入开放列表
                    node.GCost = newCost;
                    SetNodeParent(node, fathernode);
                    openDictionary[node] = node.FCost;
                    openList.Enqueue(node, node.FCost);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 计算点的总代价
        /// 对角线距离
        /// </summary>
        /// <param name="node">要计算的点</param>
        /// <param name="endNode">目标点</param>
        /// <returns></returns>
        private int CalculationPathCost(PathNode node, PathNode endNode)
        {

            //实际距离
            //同步网格的实际距离
            node.GCost = CalculationGenuineCost(node, node.fatherNode);

            //预估距离
            //同步网格的预估距离
            node.HCost = CalculationHerishCost(node, endNode);

            return node.FCost;

        }

        /// <summary>
        /// 计算实际距离
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private int CalculationGenuineCost(PathNode node, PathNode fatherNode)
        {
            //实际距离/判断是否处于父节点的十字网格位置
            int gdis = node.x == fatherNode.x || node.y == fatherNode.y ? VCost : XCost;
            gdis += fatherNode.GCost;
            return gdis;
        }

        /// <summary>
        /// 判断预估距离
        /// </summary>
        /// <param name="node"></param>
        /// <param name="endNode"></param>
        /// <returns></returns>
        private int CalculationHerishCost(PathNode node, PathNode endNode)
        {
            //预估距离
            int h = Mathf.Abs(node.x - endNode.x);
            int v = Mathf.Abs(node.y - endNode.y);

            //斜向代价
            int diagonalCost = Mathf.Min(h, v) * XCost;
            //垂直代价
            int straightCost = Mathf.Abs(h - v) * VCost;

            node.HCost = diagonalCost + straightCost;

            return diagonalCost + straightCost;
        }

        /// <summary>
        /// 回溯路径，返回正常顺序的路径
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="nodes"></param>
        /// <returns></returns>
        private Stack<PathNode> RetracePath(PathNode start, PathNode end, Stack<PathNode> nodes)
        {
            PathNode currentNode = end;

            while (currentNode != start)
            {
                nodes.Push(currentNode);
                currentNode = currentNode.fatherNode;
            }
            nodes.Push(start);

            return nodes;
        }

        /// <summary>
        /// 重置所有格子的状态
        /// </summary>
        /// <param name="nodes"></param>
        public void ResetPathFinder()
        {
            PathNode[,] nodes = mainGrid.GridNodesArray;
            foreach (var node in nodes)
            {
                node.ResetNode();
            }
            openDictionary.Clear();
            openList.Clear();
            closedList.Clear();
        }

    }

}