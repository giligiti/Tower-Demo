using System;
using System.Collections.Generic;
using UnityEngine;

namespace Octree
{
    public class OctreeObject 
    {
        public Bounds bounds {  get; private set; }

        public Octree octree;

        public List<OctreeNode> parents = new List<OctreeNode>();               //存储物体所属的叶子节点

        public OctreeMono octMonoObj;                                           //表示这个OctreeObject真正关联的物体//用于八叉树查询



        public OctreeObject(Bounds obj, OctreeMono mono)
        {
            ChangeBounds(obj);
            this.octMonoObj = mono;
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
            BreakParent();
            
            octree.ReDivide(this);
        }

        /// <summary>
        /// 关联的物体销毁或者失活的时候
        /// </summary>
        public void BreakParent()
        {
            //断开旧关联
            if (parents.Count > 0)
            {
                foreach (OctreeNode node in parents)
                {
                    node.RemoveObject(this);
                }
                parents.Clear();
            }
            octree.RemoveOtObject(this);

        }

    }
}

