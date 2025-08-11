using System.Collections.Generic;
using UnityEngine;

namespace Octree
{
    public class OctreeNode 
    {
        /// <summary>
        /// ���ڴ洢������//��ΪҶ�ӽڵ�ʱ�Ż�洢
        /// </summary>
        public HashSet<OctreeObject> objects;
        /// <summary>
        /// �˸��ӽڵ�İ�Χ��,�ڱ��ڵ㴴��֮ʱ���Ѿ�������
        /// </summary>
        private Bounds[] childrenBounds = new Bounds[8];                        
        /// <summary>
        /// �洢��ǰ�ڵ��8���ӽڵ㣨���ڵ�δ�ָ��Ϊ null��
        /// </summary>
        public OctreeNode[] children;                                                  

        static int nextId;                                              //����id��ȡ��ͨ��ͬһnextid����ȡid����֤��Ψһ
        public readonly int id;                                         //id

        public Bounds bounds;                                           //���嵱ǰ�ڵ����� 3D �ռ䷶Χ
        public bool isLeaf => children == null;                         //�Ƿ���Ҷ�ӽڵ㣨�Ƿ񱻷ָ���ˣ�
        public bool isEmpty => objects == null || objects.Count == 0;   //�Ƿ��ǿ�Ҷ�ӽڵ�

        float minNodeSize;

        /// <summary>
        /// �����ڵ�ռ�ָ�Ϊ 8 ����������
        /// </summary>
        /// <param name="bounds">���ڵ�Ŀռ�</param>
        /// <param name="minNodeSize"></param>
        public OctreeNode(Bounds bounds, float minNodeSize)
        {
            this.bounds = bounds;
            this.minNodeSize = minNodeSize;
            Vector3 newSize = bounds.size * 0.5f;                       //��������ĳߴ�
            Vector3 centerOffset = bounds.size * 0.25f;                 //�����������ĵ���Ը��ڵ��ƫ��
            Vector3 parentCeter = bounds.center;

            for (int i = 0; i < 8; i++)
            {
                Vector3 childCenter = parentCeter;
                childCenter.x += centerOffset.x * ((i & 1) == 0 ? -1 : 1);
                childCenter.y += centerOffset.y * ((i & 2) == 0 ? -1 : 1);
                childCenter.z += centerOffset.z * ((i & 4) == 0 ? -1 : 1);
                childrenBounds[i] = new Bounds(childCenter, newSize);
            }


        }

        //�ݹ�����߼�
        public void Divide(OctreeObject otObject)
        {
            //����ڵ�ĳߴ�С����С�ߴ��ֱ�Ӵ洢
            if (bounds.size.x <= minNodeSize)
            {
                AddObject(otObject);
                return;
            }
            //����
            //���� children Ϊ null ʱ����������Ϊ 8 ���ӽڵ�����
            children ??= new OctreeNode[8];

            bool intersectedChild = false;
            //�����Ӱ�Χ�У��鿴�ĸ���Χ�к������Χ���ཻ
            for (int i = 0; i < 8; i++)
            {
                //�����˸��ڵ㣬�����ǵİ�Χ��
                children[i] ??= new OctreeNode(childrenBounds[i], minNodeSize);
                //����ཻ�͵ݹ����Divideֱ������������
                if (otObject.Intersects(childrenBounds[i]))
                {
                    children[i].Divide(otObject);
                    intersectedChild = true;
                }
            }
            //�����Ϊ��˵������������Ӱ�Χ�ж����ཻ
            if (!intersectedChild)
            {
                AddObject(otObject);
            }

        }
        private void AddObject(OctreeObject otObject)
        {
            objects ??= new HashSet<OctreeObject>();
            objects.Add(otObject);
            otObject.BelongNode(this);                  //�趨OctreeObject�������ڵ�
        }

        public void RemoveObject(OctreeObject otObject)
        {
            objects.Remove(otObject);
        }
        /// <summary>
        /// ���ڵ�Ͳ���һ�Ŀռ��Ƿ��ཻ�����ݹ��ҵ����кͿռ��ཻ�Ŀռ��е�����
        /// </summary>
        /// <param name="otObject">��Ҫ���Ŀռ�</param>
        /// <param name="objectSet"></param>
        public void RoomCheck(Bounds otObject, HashSet<OctreeNode> objectSet)
        {
            //�Ƿ��ཻ
            if (!bounds.Intersects(otObject)) return;
            //�Ƿ���Ҷ�ӽڵ�
            if (isLeaf)
            {
                objectSet.Add(this);
                return;
            }

            //����Ҷ�ӽڵ㣬�����ݹ�
            foreach (OctreeNode child in children)
            {
                child.RoomCheck(otObject, objectSet);
            }
        }

        /// <summary>
        /// //////////////////////////����
        /// </summary>
        
        //ʹ����ʵ������Ľڵ㼰��·���ڵ�Żᱻ����
        private bool CheckDraw()
        {
            bool needDraw = false;
            if (isLeaf && isEmpty)
                needDraw = false;
            if (isLeaf && !isEmpty)
                return true;
            if (!isLeaf)
            {
                foreach (OctreeNode node in children)
                {
                    if (node.DrawNode())
                        needDraw = true;
                }
            }
            return needDraw;
        }
        public bool DrawNode()
        {
            //�ɸ��ڵ㴥���ͻ�ݹ�����ӽڵ��DrawNode
            //��Ҷ�ӽڵ����ʵ������ʱ�ͻ����Ȼ�󷵻�true�����ڵ�õ�true���ֻ���ƣ�����������Դ����
            bool draw = CheckDraw();
            if (!draw)
                return false;

            return draw;
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
            //��������ʵ��ռ�ݵİ˲������񣨺�ɫ����
            return draw;                                                                            //��ʱ����
            if (!isLeaf || isEmpty) return draw;

            foreach (var item in objects)
            {
                if (item.Intersects(bounds))
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireCube(bounds.center,bounds.size);
                }
            }

            return draw;
        }

    }
}

