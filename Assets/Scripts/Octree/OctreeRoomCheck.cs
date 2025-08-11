using System;
using System.Collections.Generic;
using ToolSpace;
using UnityEngine;

namespace Octree
{
    public class OctreeRoomCheck : OctreeMono
    {
        [HideInInspector] public Bounds checkBounds;                          //最近邻检测包围盒//初筛无大碍
        public float range;                                 //检测距离

        HashSet<OctreeNode> octSet = new HashSet<OctreeNode>();     //存储所有的八叉树节点

        HashSet<GameObject> TargetObjects = new HashSet<GameObject>();      //得到对象位置信息,避免一直new，声明成字段
        /// <summary>
        /// 得到攻击范围
        /// </summary>
        /// <param name="range"></param>
        public void InitRange()
        {
            if (TryGetComponent<IGetFloat>(out IGetFloat AtkRange))
                this.range = AtkRange.Range;
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
            CheckBoundsPosition();
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
        /// 提供一个最近的目标的位置信息
        /// </summary>
        /// <param name="action">筛选条件</param>
        /// <param name="priority">自定义优先级排序</param>
        /// <returns></returns>
        public GameObject ProvideTargetPosition(Func<GameObject, bool> action = null, Func<float, float> priority = null)
        {
            TargetObjects.Clear();
            //创建优先队列
            CustomePriorityQueue<GameObject, float> priorityQueue = new CustomePriorityQueue<GameObject, float>();
            //遍历所有节点
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
            //处理所有节点
            foreach (GameObject obj in TargetObjects)
            {
                if (action != null)
                {
                    //如果不符合条件则跳过该物体
                    if (!action(obj)) continue;
                }

                float distance = Vector3.Distance(transform.position, obj.transform.position);
                //实现自定义优先级排序
                if (priority != null) distance = priority(distance);

                priorityQueue.Enqueue(obj, distance);
            }
            GameObject nearObj = priorityQueue.Dequeue();
            //调试：绘制自身到目标的红线
            Debug.DrawLine(transform.position, nearObj.transform.position, Color.red);

            return nearObj;                                                                    //修改
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
