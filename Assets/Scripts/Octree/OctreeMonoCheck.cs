using System;
using System.Collections.Generic;
using UnityEngine;

namespace Octree
{
    public class OctreeMonoCheck : OctreeMono
    {
        [HideInInspector] public Bounds checkBounds;                          //最近邻检测包围盒//初筛无大碍
        public float range;                                 //检测距离

        HashSet<OctreeNode> octSet = new HashSet<OctreeNode>();     //存储周围的的八叉树节点

        HashSet<GameObject> TargetObjects = new HashSet<GameObject>();      //得到对象位置信息,避免一直new，声明成字段
        /// <summary>
        /// 得到攻击范围
        /// </summary>
        /// <param name="range"></param>
        public void InitRange()
        {
            if (TryGetComponent<IGetRange>(out IGetRange AtkRange))
                this.range = AtkRange.Range / 10;                           //修改
            //创建检测范围包围盒
            checkBounds = new Bounds(bounds.center, new Vector3(range, range, range));
        }
        protected override void Start()
        {
            base.Start();
            //得到检测范围
            InitRange();
        }
        protected override void Update()
        {
            base.Update();
            //更新检测范围包围盒的位置并更新节点表
            CheckBoundsPosition();
            //维护节点表
            CheckSurroundNodeList();

        }

        /// <summary>
        /// 更新检测范围Bounds位置
        /// </summary>
        private void CheckBoundsPosition()
        {
            //当位置发生变动
            if (transform.position != lastPosition)
            {
                //更新包围盒位置
                checkBounds.center = bounds.center;
                //更新周围节点
                UpdateSurroundNode();
            }
        }
        #region 空间划分检测
        /// <summary>
        /// 提供一个最近的物体
        /// </summary>
        /// <param name="action">筛选条件</param>
        /// <param name="priority">自定义优先级排序</param>
        /// priority需要接收一个参数，是自身到目标的距离，返回的是自定义的优先级，默认小顶堆（越小越优先）
        /// <returns></returns>
        public HashSet<GameObject> ProvideTargets()
        {
            //避免残留
            TargetObjects.Clear();
            
            //遍历所有节点，把节点的物体全部加入到集合中
            foreach (var obj in octSet)
            {
                //如果节点未存储物体则跳过
                if (obj.isEmpty) continue;

                //把节点存储的物体都添加到hashset中
                foreach (var octMono in obj.objects)
                {
                    TargetObjects.Add(octMono.octMonoObj.gameObject);
                }
            }
            
            return TargetObjects;                                                                    //修改
        }

        /// <summary>
        /// 向八叉树查询周围节点，更新周围节点表
        /// </summary>
        /// <returns></returns>
        public void UpdateSurroundNode()
        {
            if (octreeObject.octree == null) return;
            octSet.Clear();
            octreeObject.octree.RoomSearch(checkBounds, octSet);
        }

        /// <summary>
        /// 检查存储的节点是否变成非叶子节点，并进行维护
        /// </summary>
        private void CheckSurroundNodeList()
        {
            foreach (var obj in octSet)
            {
                if (obj.isLeaf == false)
                {
                    UpdateSurroundNode();
                    break;
                }
            }
        }

        #endregion
        protected override void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(checkBounds.center, checkBounds.size);      //绘制检测范围
        }
    }
}
