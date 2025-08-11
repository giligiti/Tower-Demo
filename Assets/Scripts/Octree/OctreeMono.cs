using System;
using System.Collections.Generic;
using ToolSpace;
using UnityEngine;
using UnityEngine.Events;


namespace Octree
{
    public class OctreeMono : MonoBehaviour
    {
        public OctreeObject octreeObject;                   //��ʾ������ĳ���˲����ռ�
        public Bounds bounds;
        public Bounds checkBounds;                          //����ڼ���Χ��//��ɸ�޴�
        public float range;                                 //������

        HashSet<OctreeNode> octSet = new HashSet<OctreeNode>();     //�洢���еİ˲����ڵ�

        
        HashSet<GameObject> TargetObjects = new HashSet<GameObject>();      //�õ�����λ����Ϣ,����һֱnew�������������ֶ�

        private Vector3 lastPosition = new Vector3();

        /// <summary>
        /// �õ�������Χ
        /// </summary>
        /// <param name="range"></param>
        public void InitRange()
        {
            if (!TryGetComponent<IGetFloat>(out IGetFloat AtkRange)) throw new Exception("OctreeMono�ű�δ�õ�������ⷶΧ");
            this.range = AtkRange.Range;
        }

        private void Awake()
        {
            
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            //�õ�������Χ
            InitRange();
            //������Octree����ϵ
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
        /// �����Octreeϵͳ������ϵ
        /// </summary>
        private void OctreeSystemInit()
        {
            //�������������Χ��
            CharacterController c = GetComponentInChildren<CharacterController>();
            if (c != null)
                bounds = c.bounds;
            else
                bounds = GetComponent<Collider>().bounds;

            //������ⷶΧ��Χ��
            checkBounds = new Bounds(bounds.center, new Vector3(range, range, range));

            //������Ӧ��OctreeObject
            octreeObject = new OctreeObject(bounds, this);

            //�������OctreeObjectע�ᵽ���ݹ������Ĺ�ϣ����
            GameDataMgr.Instance.octreeMonos.Add(octreeObject);
        }

        #region �ռ仮�ּ��

        /// <summary>
        /// �ṩһ�������Ŀ���λ����Ϣ
        /// </summary>
        /// <param name="action">ɸѡ����</param>
        /// <param name="priority">�Զ������ȼ�����</param>
        /// <returns></returns>
        public GameObject ProvideTargetPosition(Func<GameObject, bool> action = null, Func<float, float> priority = null)
        {
            TargetObjects.Clear();
            //�������ȶ���
            CustomePriorityQueue<GameObject, float> priorityQueue = new CustomePriorityQueue<GameObject, float>();
            //�������нڵ�
            foreach (var obj in octSet)
            {
                //����ڵ�δ�洢����������
                if (obj.isEmpty) continue;

                //�ѽڵ�洢�����嶼��ӵ�hashset��
                foreach (var octMono in obj.objects)
                {
                    TargetObjects.Add(octMono.octMonoObj.gameObject);
                }
            }
            //�������нڵ�
            foreach (GameObject obj in TargetObjects)
            {
                if (action != null)
                {
                    //�������������������������
                    if (!action(obj)) continue; 
                } 

                float distance = Vector3.Distance(transform.position, obj.transform.position);
                //ʵ���Զ������ȼ�����
                if (priority != null) distance = priority(distance);

                priorityQueue.Enqueue(obj, distance);
            }
            GameObject nearObj = priorityQueue.Dequeue();
            //���ԣ����������Ŀ�������
            Debug.DrawLine(transform.position, nearObj.transform.position, Color.red);

            return nearObj;                                                                    //�޸�
        }

        /// <summary>
        /// ��˲�����ѯ��Χ�ڵ㣬������Χ�ڵ��
        /// </summary>
        /// <returns></returns>
        public void UpdateSurroundNode()
        {
            if (octreeObject.octree == null) return;
            octSet.Clear();
            octreeObject.octree.RoomSearch(checkBounds, octSet);
        }

        /// <summary>
        /// ���洢�Ľڵ��Ƿ��ɷ�Ҷ�ӽڵ㣬������ά��
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
        /// λ�ø���ͬ����֪ͨ��Ӧ��OctreeObject���и���
        /// </summary>
        private void CheckPositionChange()
        {
            //��λ�÷����䶯
            if (transform.position != lastPosition)
            {
                lastPosition = transform.position;
                //���°�Χ��λ��
                checkBounds.center = bounds.center;
                bounds.center = transform.position;
                //�滻OctreeObject�İ�Χ��
                octreeObject.ChangeBounds(bounds);
                //֪ͨλ�÷����䶯�����°˲���
                octreeObject.OctreeUpdate();
                //������Χ�ڵ�
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

