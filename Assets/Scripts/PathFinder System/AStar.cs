using System.Collections.Generic;
using ToolSpace;
using UnityEngine;

namespace PathFind
{
    /// <summary>
    /// A*Ѱ·�㷨
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

        //����Ѱ·����ϵͳ
        MainGrid<PathNode> mainGrid;

        //�����б�//�洢��Ҫ���ĵ�
        //����ȥ��
        private Dictionary<PathNode, int> openDictionary = new Dictionary<PathNode, int>();
        //��������
        private CustomePriorityQueue<PathNode, int> openList = new CustomePriorityQueue<PathNode, int>();

        //�ر��б�//�洢������Ҫ���ĵ�
        private HashSet<PathNode> closedList = new HashSet<PathNode>();

        #region Ѱ·����

        //1.�õ��յ����
        //2.�����롰����ر��б��õ���Χ�ĵ㡱������
        //3.����Χ�ĵ����ɸѡ���Ѿ������ڿ����б�͹ر��б����ų������뿪���б�
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
        /// �ṩ���������ö�Ӧ������ϵͳ�����õ����޸Ľڵ��ֵ
        /// </summary>
        /// <returns></returns>
        public MainGrid<PathNode> GetMainGridObj()
        {
            return mainGrid;
        }

        #region ��ʼѰ·����

        /// <summary>
        /// ��ʼѰ·����
        /// </summary>
        /// <param name="start">���</param>
        /// <param name="end">�յ�</param>
        /// <param name="path">·��</param>
        /// <returns></returns>
        private bool StartPathFind(Stack<PathNode> path)
        {
            //�յ��ж��Ƿ����ϰ���
            if (EndNode.isWall || StartNode == EndNode)
                return false;

            GetFinallyPath(StartNode, EndNode);
            //Ѱ·ʧ��

            if (EndNode.fatherNode == null && openDictionary.Count == 0) return false;

            //Ѱ·�ɹ�
            //ִ��·������
            RetracePath(StartNode, EndNode, path);

            return true;
        }

        public bool StartPathFind(Vector3 start, Vector3 end, Stack<PathNode> path)
        {
            //��ʼ������յ�Ľڵ�
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
        /// ����յ��ʼ��
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        private void InitStartEndPoint(PathNode start, PathNode end)
        {
            this.StartNode = start;
            this.EndNode = end;
            //�����յ�ĸ��������
            StartNode.fatherNode = null;
            EndNode.fatherNode = null;

            //������ر��б�
            PushIntoCloseList(StartNode);
        }


        /// <summary>
        /// Ѱ���յ㷽��
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
        /// ѡ����Χ���ŵ����ر��б�
        /// </summary>
        /// <param name="node"></param>
        private PathNode OpenToClose(PathNode node)
        {
            //����node������һ��ѡ�������ŵ㣬�����Ѿ��ڹر��б���

            List<PathNode> list = GetSurroundNode(node);
            //������Ч�ڵ�������ȶ���
            foreach (PathNode node2 in list)
            {
                //����Ѿ������ȶ����еĻ������Gֵ����
                if (CheckUpdatGCost(node2, node))
                    continue;
                //�������ȶ�����
                SetNodeParent(node2, node);         //���ýڵ�ĸ��ڵ�
                node2.ChangeColor(Color.gray);                                                              //���ԣ��ı�������ɫ
                CalculationPathCost(node2, EndNode);//�����ܴ���
                                                    //list�е��Ǿ���ɸѡ�ĵĽڵ㣬���ǿ��Լ��뿪���б��
                PushIntoOpenList(node2);
            }
            PathNode pathNode = DequeueOpenlist();
            PushIntoCloseList(pathNode);
            return pathNode;
        }

        /// <summary>
        /// �ѵ���뿪���б�
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private void PushIntoOpenList(PathNode node)
        {
            openDictionary.Add(node, node.FCost);
            openList.Enqueue(node, node.FCost);
        }

        /// <summary>
        /// ���õ�ĸ��ڵ�
        /// </summary>
        /// <param name="node"></param>
        /// <param name="fatherNode"></param>
        private void SetNodeParent(PathNode node, PathNode fatherNode)
        {
            node.fatherNode = fatherNode;
        }

        /// <summary>
        /// �������ȶ����е����ȼ���ߵĵ�
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
        /// �ѵ���뵽�ر��б�
        /// </summary>
        /// <param name="node"></param>
        private void PushIntoCloseList(PathNode node)
        {
            closedList.Add(node);
        }

        /// <summary>
        /// �õ�Ŀ�����Χ����Ч��
        /// </summary>
        /// <param name="node"></param>
        private List<PathNode> GetSurroundNode(PathNode node)
        {
            //�˸���
            PathNode pathNode;

            List<PathNode> nodesList = new List<PathNode>(8);

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    pathNode = mainGrid.GetNode(node.x + i, node.y + j);

                    //���������Ҫ��
                    if (!CheckPoint(pathNode))
                        continue;

                    //�����б���ƶ��Ļ�����Ƿ����ͨ�У�����б��ǽ
                    if (i != 0 && j != 0)
                    {
                        PathNode pathNode1 = mainGrid.GetNode(pathNode.x + i, pathNode.y);
                        PathNode pathNode2 = mainGrid.GetNode(pathNode.x, pathNode.y + j);
                        PathNode pathNode3 = mainGrid.GetNode(pathNode.x - i, pathNode.y);
                        PathNode pathNode4 = mainGrid.GetNode(pathNode.x, pathNode.y - j);
                        if (!CheckPoint(pathNode1) && !CheckPoint(pathNode2) || !CheckPoint(pathNode3) && !CheckPoint(pathNode4))
                            continue;

                    }
                    if (i == 0 && j == 0)//�����Լ�
                        continue;

                    nodesList.Add(pathNode);
                }
            }
            return nodesList;

        }

        /// <summary>
        /// �����Ƿ���ϼ���Ҫ��
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private bool CheckPoint(PathNode node)
        {
            //�п�/�Ƿ��ϰ�
            if (node == null || node.isWall || closedList.Contains(node)) return false;

            return true;
        }

        /// <summary>
        /// ����Ƿ���Ҫ����Gֵ�������ظ���
        /// </summary>
        /// <param name="node"></param>
        private bool CheckUpdatGCost(PathNode node, PathNode fathernode)
        {
            if (openDictionary.ContainsKey(node))
            {
                int newCost = CalculationGenuineCost(node, fathernode);
                if (node.GCost > newCost)
                {
                    //����Gֵ�������󣬼��뿪���б�
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
        /// �������ܴ���
        /// �Խ��߾���
        /// </summary>
        /// <param name="node">Ҫ����ĵ�</param>
        /// <param name="endNode">Ŀ���</param>
        /// <returns></returns>
        private int CalculationPathCost(PathNode node, PathNode endNode)
        {

            //ʵ�ʾ���
            //ͬ�������ʵ�ʾ���
            node.GCost = CalculationGenuineCost(node, node.fatherNode);

            //Ԥ������
            //ͬ�������Ԥ������
            node.HCost = CalculationHerishCost(node, endNode);

            return node.FCost;

        }

        /// <summary>
        /// ����ʵ�ʾ���
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private int CalculationGenuineCost(PathNode node, PathNode fatherNode)
        {
            //ʵ�ʾ���/�ж��Ƿ��ڸ��ڵ��ʮ������λ��
            int gdis = node.x == fatherNode.x || node.y == fatherNode.y ? VCost : XCost;
            gdis += fatherNode.GCost;
            return gdis;
        }

        /// <summary>
        /// �ж�Ԥ������
        /// </summary>
        /// <param name="node"></param>
        /// <param name="endNode"></param>
        /// <returns></returns>
        private int CalculationHerishCost(PathNode node, PathNode endNode)
        {
            //Ԥ������
            int h = Mathf.Abs(node.x - endNode.x);
            int v = Mathf.Abs(node.y - endNode.y);

            //б�����
            int diagonalCost = Mathf.Min(h, v) * XCost;
            //��ֱ����
            int straightCost = Mathf.Abs(h - v) * VCost;

            node.HCost = diagonalCost + straightCost;

            return diagonalCost + straightCost;
        }

        /// <summary>
        /// ����·������������˳���·��
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
        /// �������и��ӵ�״̬
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