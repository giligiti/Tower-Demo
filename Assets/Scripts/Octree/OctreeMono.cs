using UnityEngine;

namespace Octree
{
    public class OctreeMono : MonoBehaviour
    {
        [HideInInspector] public OctreeObject octreeObject;                   //表示本物体的抽象八叉树空间

        [HideInInspector] public Bounds bounds;                               //自身的包围盒
        
        protected Vector3 lastPosition = new Vector3();
        public bool isDead = false;

        private void Awake()
        {
            TryGetIDeath();
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        protected virtual void Start()
        {
            //建立和Octree的联系
            OctreeSystemInit();

            lastPosition = transform.position;
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            CheckPositionChange();
        }

        /// <summary>
        /// 负责和Octree系统建立联系
        /// </summary>
        private void OctreeSystemInit()
        {
            //创建物体自身包围盒
            CharacterController c = GetComponentInChildren<CharacterController>();

            if (c != null)
            {
                bounds = new Bounds();
                bounds.Encapsulate(c.bounds);
            }

            else bounds = GetComponent<Collider>().bounds;
                

            //创建相应的OctreeObject
            octreeObject = new OctreeObject(bounds, this);

            //把自身的OctreeObject注册到数据管理单例的哈希表中
            GameDataMgr.Instance.octreeMonos.Add(octreeObject);
        }

        /// <summary>
        /// 位置更新同步并通知相应的OctreeObject进行更新
        /// </summary>
        private void CheckPositionChange()
        {
            //当位置发生变动
            if (transform.position != lastPosition && !isDead)
            {
                lastPosition = transform.position;
                //改变包围盒位置
                bounds.center = transform.position;
                //替换OctreeObject的包围盒
                octreeObject.ChangeBounds(bounds);
                //通知位置发生变动，更新八叉树
                octreeObject.OctreeUpdate();
                
            }
        }

        protected virtual void TryGetIDeath()
        {
            if (TryGetComponent<IDeath>(out IDeath death))
            {
                death.SubscribeDeathEvent(Dead);
            }
        }
        /// <summary>
        /// 辅助函数，死亡后脱离八叉树系统
        /// </summary>
        private void Dead()
        {
            Debug.Log("dead");
            octreeObject.BreakParent(true);
            isDead = true;
        }

        protected virtual void OnDrawGizmos()
        {
            //Gizmos.DrawWireCube(bounds.center, bounds.size);//    表示实际占据空间
        }
        protected virtual void OnDisable()
        {
            Debug.Log($"{gameObject.name} 的 OnDisable 被调用！组件是否启用：{enabled}，对象是否激活：{gameObject.activeSelf}", this);
            octreeObject.BreakParent(true);
            octreeObject = null;
            isDead = false;
        }
    }
}

