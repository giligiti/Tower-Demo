using System.Collections.Generic;
using UnityEngine;

namespace Octree
{
    public class Octree 
    {
        public OctreeNode root;         //根节点
        public Bounds bounds;           //表示最大的包围盒
        private HashSet<OctreeObject> objects;                              //存储需要重新分配的节点//单例的数据
        private HashSet<OctreeObject> allOtObject;                          //存储八叉树中所有的物体

        public List<OctreeObject> actuallyLeaf;

        //创建根节点和最大包围盒
        public Octree(List<GameObject> worldSpace, float minNodeSize)
        {
            CalculateBounds(worldSpace);
            CreateTree(minNodeSize);
            objects = GameDataMgr.Instance.octreeMonos;
            //把更新方法注册到mono模块中
            MonoMgr.Instance.AddEventListener(Redivide, E_LifeFunction.update);

        }

        //创建根节点
        private void CreateTree(float minNodeSize) => root = new OctreeNode(bounds, minNodeSize);
        //计算根节点包围盒
        private void CalculateBounds(List<GameObject> worldSpace)
        {
            //使包围盒容纳所有的物体
            foreach (var child in worldSpace)
            {
                bounds.Encapsulate(child.GetComponent<Collider>().bounds);
            }
            worldSpace = null;
            //使包围盒成为正方形
            Vector3 size = bounds.size;
            Vector3 minSize = Vector3.one * Mathf.Max(size.x, size.y, size.z) * 0.5f;
            bounds.SetMinMax(bounds.center -  minSize, bounds.center + minSize);
            bounds.center = new Vector3(0,minSize.x - 5,0);
        }

        #region 八叉树插入或移除OctreeObject

        /// <summary>
        /// 新节点加入八叉树前需要做的准备
        /// </summary>
        /// <param name="otObject"></param>
        private void NewNodeInsertInit(OctreeObject otObject)
        {
            allOtObject ??= new HashSet<OctreeObject>();
            otObject.OctreeSystemGet(this);
            allOtObject.Add(otObject);
        }

        /// <summary>
        /// 节点加入待插入列表
        /// </summary>
        /// <param name="object"></param>
        public void ReDivide(OctreeObject otObject)
        {
            NewNodeInsertInit(otObject);
            objects.Add(otObject);
        }

        /// <summary>
        /// 节点插入方法
        /// </summary>
        public void Redivide()
        {
            if (objects == null || objects.Count == 0) return;
            foreach (var obj in objects)
            {
                NewNodeInsertInit(obj);
                root.Divide(obj);
            }
            objects.Clear();
        }
        /// <summary>
        /// 节点移除
        /// </summary>
        /// <param name="otObject"></param>
        public void RemoveOtObject(OctreeObject otObject)
        {
            allOtObject.Remove(otObject);
        }
        #endregion

        #region 空间划分检测相关

        public void RoomSearch(Bounds otObj, HashSet<OctreeNode> objectSet)
        {
            root.RoomCheck(otObj, objectSet);
        }


        #endregion


    }
}