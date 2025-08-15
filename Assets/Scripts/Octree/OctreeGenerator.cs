using System.Collections.Generic;
using UnityEngine;

namespace Octree
{
    public class OctreeGenerator : MonoBehaviour
    {
        //������ʼ�ռ������
        public List<GameObject> worldSpace;
        public float minNodeSize = 1f;
        private Octree ot;


        private void Awake()
        {
            //                                                             ���Է���
        }
        private void Start()
        {
            //��start�е��ã���Ϊ��������ռ��������Ҫ��
            ot = new Octree(worldSpace, minNodeSize);
            //�����������
            foreach (var item in worldSpace)
            {
                Destroy(item);
            }
        }

        /// <summary>
        /// ///////////////////����
        /// </summary>
        private void OnDrawGizmos()
        {
            //�����������״̬��ֱ�ӷ��أ�������
            if (!Application.isPlaying) return;
            Gizmos.color = Color.green; 
            Gizmos.DrawWireCube(ot.bounds.center, ot.bounds.size); 

            ot.root.DrawNode();
        }
        

    }
}