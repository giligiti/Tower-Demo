using System.Collections.Generic;
using UnityEngine;

namespace Octree
{
    public class OctreeGenerator : MonoBehaviour
    {
        //定义起始空间的物体
        public List<GameObject> worldSpace;
        public float minNodeSize = 1f;
        private Octree ot;


        private void Awake()
        {
            //                                                             测试方法
        }
        private void Start()
        {
            //在start中调用，因为划定世界空间的物体需要在
            ot = new Octree(worldSpace, minNodeSize);
            //创建完就销毁
            foreach (var item in worldSpace)
            {
                Destroy(item);
            }
        }

        /// <summary>
        /// ///////////////////调试
        /// </summary>
        private void OnDrawGizmos()
        {
            //如果不是运行状态则直接返回，不绘制
            if (!Application.isPlaying) return;
            Gizmos.color = Color.green; 
            Gizmos.DrawWireCube(ot.bounds.center, ot.bounds.size); 

            ot.root.DrawNode();
        }
        

    }
}