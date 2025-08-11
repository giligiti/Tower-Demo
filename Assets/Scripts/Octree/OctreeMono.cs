using System;
using System.Collections.Generic;
using ToolSpace;
using UnityEngine;
using UnityEngine.Events;


namespace Octree
{
    public class OctreeMono : MonoBehaviour
    {
        public OctreeObject octreeObject;                   //表示本物体的抽象八叉树空间
        public Bounds bounds;
        public Bounds checkBounds;                          //最近邻检测包围盒//初筛无大碍
        public float range;                                 //检测距离

        HashSet<OctreeNode> octSet = new HashSet<OctreeNode>();     //存储所有的八叉树节点

        
        HashSet<GameObject> TargetObjects = new HashSet<GameObject>();      //得到对象位置信息,避免一直new，所以声明成字段

        private Vector3 lastPosition = new Vector3();

        /// <summary>
        /// 得到攻击范围
        /// </summary>
        /// <param name="range"></param>
        public void InitRange()
        {
            if (!TryGetComponent<IGetFloat>(out IGetFloat AtkRange)) throw new Exception("OctreeMono脚本未得到攻击检测范围");
            this.range = AtkRange.Range;
        }

        private void Awake()
        {
            
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            //得到攻击范围
            InitRange();
            //建立和Octree的联系
            OctreeSystemInit();

            lastPosition = transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            CheckPositionChange();
            CheckSurroundNodeList();
        }

        /// <summary>
        /// 负责和Octree系统建立联系
        /// </summary>
        private void OctreeSystemInit()
        {
            //创建物体自身包围盒
            CharacterController c = GetComponentInChildren<CharacterController>();
            if (c != null)
                bounds = c.bounds;
            else
                bounds = GetComponent<Collider>().bounds;

            //创建检测范围包围盒
            checkBounds = new Bounds(bounds.center, new Vector3(range, range, range));

            //创建相应的OctreeObject
            octreeObject = new OctreeObject(bounds, this);

            //把自身的OctreeObject注册到数据管理单例的哈希表中
            GameDataMgr.Instance.octreeMonos.Add(octreeObject);
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
            //调试：绘制自身和目标的线条
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

        /// <summary>
        /// 位置更新同步并通知相应的OctreeObject进行更新
        /// </summary>
        private void CheckPositionChange()
        {
            //当位置发生变动
            if (transform.position != lastPosition)
            {
                lastPosition = transform.position;
                //更新包围盒位置
                checkBounds.center = bounds.center;
                bounds.center = transform.position;
                //替换OctreeObject的包围盒
                octreeObject.ChangeBounds(bounds);
                //通知位置发生变动，更新八叉树
                octreeObject.OctreeUpdate();
                //更新周围节点
                UpdateSurroundNode();
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            //Gizmos.DrawWireCube(bounds.center, bounds.size);
            Gizmos.DrawWireCube(checkBounds.center, checkBounds.size);
        }

        private void OnDisable()
        {
            //Debug.Log(gameObject.name);
            //octreeObject.BreakParent();
        }
        private void OnDestroy()
        {
            octreeObject.BreakParent();
        }
    }
}

