using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PathFind
{
    public class PathNode : MonoBehaviour, IGridNodes,IBecomeWall
    {
        [HideInInspector] public PathNode fatherNode;        //��¼���ڵ�
        [HideInInspector] public int FCost => HCost + GCost + nodevalue;    //�ܴ���
        [HideInInspector] public int HCost;                             //Ԥ�ƴ���
        [HideInInspector] public int GCost;                             //�������
        private int nodevalue;
        [HideInInspector] public int Value { get => nodevalue; private set { text.text = value.ToString(); nodevalue = value; } }
        int IGridNodes.Value { get => nodevalue; }

        //����ֵ
        [HideInInspector] public int x;
        [HideInInspector] public int y;

        public SpriteRenderer Children;                     //ͼƬ���
        public TextMeshPro text;                            //�ı����

        [HideInInspector] public bool isWall = false;       //�Ƿ�Ϊ�ϰ�
                                                            //·����ʾ��ɫ
        private Color wallColor = Color.black;
        private Color normalColor = Color.white;

        private void Awake()
        {
            Children.color = normalColor;
        }


        /// <summary>
        /// �����ʼ��
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void InitGridNodes(int x, int y, int value)
        {
            this.x = x;
            this.y = y;
            this.Value = value;

        }
        public void InitGridScale(int gridSize)
        {
            transform.localScale = new Vector3(gridSize, gridSize, gridSize);      //�������������С
            text.fontSize = gridSize * 7;                                                               //���������С

        }

        //�÷����Ϊ�ϰ���
        public bool BecomeWall()
        {
            isWall = true;
            Children.color = wallColor;
            return isWall;
        }
        //���÷���
        public void ResetNode()
        {
            isWall = false;
            Value = 0;
            GCost = 0;
            HCost = 0;
            Children.color = normalColor;
        }

        public void ChangeColor(Color color)
        {
            Children.color = color;
        }

        public void ChangeValue(int value)
        {
            this.Value = value;
        }

        private bool Equals(PathNode other) => other != null && x == other.x && y == other.y;
        public override bool Equals(object other)
        {
            return Equals(other as PathNode);
        }
        public override int GetHashCode() => x.GetHashCode() ^ y.GetHashCode();

        void IBecomeWall.BecomeWall()
        {
            isWall = true;
            ChangeColor(Color.black);
        }
    }
    /// <summary>
    /// �ڵ�ӿ�
    /// </summary>
    public interface IGridNodes
    {
        public int Value { get; }
        void InitGridNodes(int x, int y, int value);

        void InitGridScale(int gridSize);

        void ChangeValue(int value);

        void ResetNode();
    }

    /// <summary>
    /// ·���ڵ�ӿ�
    /// </summary>
    /// ���ں�A*ϵͳ���ã���Ϊ·���ڵ�������Ѱ·�Ľڵ����Ҫʵ������ӿ�
    public interface IPathNode
    {
        /// <summary>
        /// ��¼���ڵ�
        /// </summary>
        /// <value></value>
        PathNode FatherNode { get; set; }
        //�ܴ���
        int FCost { get; set; }
        //Ԥ�ڴ���
        int HCost { get; set; }
        //ʵ�ʴ���
        int GCost { get; set; }
        //�ڵ���Զ������ֵ
        int Value { get; set; }

        /// <summary>
        /// ��ʼ���ڵ�
        /// </summary>
        void InitPathNode();

        /// <summary>
        /// ��ȡ�ڵ���������ϵ�µ�λ��
        /// </summary>
        /// <returns></returns>
        Transform GetNodeTransform();

        /// <summary>
        /// �õ���Χ�ڵ�
        /// </summary>
        /// <returns></returns>
        List<PathNode> GetSurroundNode();

        /// <summary>
        /// ���ýڵ�
        /// </summary>
        void ResetNode();

    }

    public interface IBecomeWall
    {
        void BecomeWall();
    }
}


