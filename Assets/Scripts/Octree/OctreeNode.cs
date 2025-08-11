using System.Collections.Generic;
using UnityEngine;

namespace Octree
{
    public class OctreeNode 
    {
        /// <summary>
        /// 用于存储子物体//作为叶子节点时才会存储
        /// </summary>
        public HashSet<OctreeObject> objects;
        /// <summary>
        /// 八个子节点的包围盒,在本节点创建之时就已经创建了
        /// </summary>
        private Bounds[] childrenBounds = new Bounds[8];                        
        /// <summary>
        /// 存储当前节点的8个子节点（若节点未分割，则为 null）
        /// </summary>
        public OctreeNode[] children;                                                  

        static int nextId;                                              //用于id获取，通过同一nextid来获取id，保证不唯一
        public readonly int id;                                         //id

        public Bounds bounds;                                           //定义当前节点管理的 3D 空间范围
        public bool isLeaf => children == null;                         //是否是叶子节点（是否被分割过了）
        public bool isEmpty => objects == null || objects.Count == 0;   //是否是空叶子节点

        float minNodeSize;

        /// <summary>
        /// 将父节点空间分割为 8 个子立方体
        /// </summary>
        /// <param name="bounds">父节点的空间</param>
        /// <param name="minNodeSize"></param>
        public OctreeNode(Bounds bounds, float minNodeSize)
        {
            this.bounds = bounds;
            this.minNodeSize = minNodeSize;
            Vector3 newSize = bounds.size * 0.5f;                       //子立方体的尺寸
            Vector3 centerOffset = bounds.size * 0.25f;                 //子立方体中心点相对父节点的偏移
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

        //递归插入逻辑
        public void Divide(OctreeObject otObject)
        {
            //如果节点的尺寸小于最小尺寸就直接存储
            if (bounds.size.x <= minNodeSize)
            {
                AddObject(otObject);
                return;
            }
            //否则
            //仅当 children 为 null 时，创建长度为 8 的子节点数组
            children ??= new OctreeNode[8];

            bool intersectedChild = false;
            //遍历子包围盒，查看哪个包围盒和物体包围盒相交
            for (int i = 0; i < 8; i++)
            {
                //创建八个节点，和他们的包围盒
                children[i] ??= new OctreeNode(childrenBounds[i], minNodeSize);
                //如果相交就递归调用Divide直到到达最大深度
                if (otObject.Intersects(childrenBounds[i]))
                {
                    children[i].Divide(otObject);
                    intersectedChild = true;
                }
            }
            //如果不为真说明物体和所有子包围盒都不相交
            if (!intersectedChild)
            {
                AddObject(otObject);
            }

        }
        private void AddObject(OctreeObject otObject)
        {
            objects ??= new HashSet<OctreeObject>();
            objects.Add(otObject);
            otObject.BelongNode(this);                  //设定OctreeObject的所属节点
        }

        public void RemoveObject(OctreeObject otObject)
        {
            objects.Remove(otObject);
        }
        /// <summary>
        /// 检测节点和参数一的空间是否相交，并递归找到所有和空间相交的空间中的物体
        /// </summary>
        /// <param name="otObject">需要检测的空间</param>
        /// <param name="objectSet"></param>
        public void RoomCheck(Bounds otObject, HashSet<OctreeNode> objectSet)
        {
            //是否相交
            if (!bounds.Intersects(otObject)) return;
            //是否是叶子节点
            if (isLeaf)
            {
                objectSet.Add(this);
                return;
            }

            //不是叶子节点，继续递归
            foreach (OctreeNode child in children)
            {
                child.RoomCheck(otObject, objectSet);
            }
        }

        /// <summary>
        /// //////////////////////////调试
        /// </summary>
        
        //使含有实际物体的节点及其路径节点才会被绘制
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
            //由根节点触发就会递归调用子节点的DrawNode
            //当叶子节点存在实际物体时就会绘制然后返回true，父节点得到true就又会绘制，这样往上溯源绘制
            bool draw = CheckDraw();
            if (!draw)
                return false;

            return draw;
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
            //画出物体实际占据的八叉树网格（红色网格）
            return draw;                                                                            //暂时调试
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

