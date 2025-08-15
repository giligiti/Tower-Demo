using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Octree
{
    public class OctreeObject 
    {
        public Bounds bounds {  get; private set; }

        public Octree octree;

        public List<OctreeNode> parents = new List<OctreeNode>();               //存储物体所属的叶子节点

        public OctreeMono octMonoObj;                                           //表示这个OctreeObject真正关联的物体//用于八叉树查询

        public UnityEvent OctreeComplete = new UnityEvent();                           

        public OctreeObject(Bounds obj, OctreeMono mono)
        {
            ChangeBounds(obj);
            this.octMonoObj = mono;
        }

        /// <summary>
        /// 得到所属的八叉树了
        /// </summary>
        public void OctreeSystemGet(Octree octree)
        {
            this.octree = octree;
            OctreeComplete?.Invoke();
        }

        public void ChangeBounds(Bounds obj) => this.bounds = obj;

        /// <summary>
        /// 检测boundsToCheck和该OctreeObject物体的包围盒是否相交
        /// </summary>
        /// <param name="boundsToCheck"></param>
        /// <returns></returns>
        public bool Intersects(Bounds boundsToCheck) => bounds.Intersects(boundsToCheck);

        /// <summary>
        /// 设定物体所属的叶子节点
        /// </summary>
        /// <param name="node"></param>
        public void BelongNode(OctreeNode node) => parents.Add(node);

        //被OctreeMono调用的方法

        /// <summary>
        /// 更新八叉树
        /// </summary>
        public void OctreeUpdate()
        {
            //断开旧关联
            BreakParent(false);
            
            octree.ReDivide(this);
        }

        /// <summary>
        /// 关联的物体销毁或者失活的时候
        /// </summary>
        /// <param name="isbreakAll">是否不再加入八叉树</param>
        public void BreakParent(bool isbreakAll)
        {
            //断开旧关联
            if (parents.Count > 0)
            {
                foreach (OctreeNode node in parents)
                {
                    if (isbreakAll) node.RemoveObject(this);
                    if (!Intersects(node.bounds)) node.RemoveObject(this);
                }
                parents.Clear();
            }
            octree.RemoveOtObject(this);

        }

    }
}

