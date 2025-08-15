using System.Collections.Generic;
using UnityEngine;

namespace Octree
{
    public class Octree 
    {
        public OctreeNode root;         //���ڵ�
        public Bounds bounds;           //��ʾ���İ�Χ��
        private HashSet<OctreeObject> objects;                              //�洢��Ҫ���·���Ľڵ�//����������
        private HashSet<OctreeObject> allOtObject;                          //�洢�˲��������е�����

        public List<OctreeObject> actuallyLeaf;

        //�������ڵ������Χ��
        public Octree(List<GameObject> worldSpace, float minNodeSize)
        {
            CalculateBounds(worldSpace);
            CreateTree(minNodeSize);
            objects = GameDataMgr.Instance.octreeMonos;
            //�Ѹ��·���ע�ᵽmonoģ����
            MonoMgr.Instance.AddEventListener(Redivide, E_LifeFunction.update);

        }

        //�������ڵ�
        private void CreateTree(float minNodeSize) => root = new OctreeNode(bounds, minNodeSize);
        //������ڵ��Χ��
        private void CalculateBounds(List<GameObject> worldSpace)
        {
            //ʹ��Χ���������е�����
            foreach (var child in worldSpace)
            {
                bounds.Encapsulate(child.GetComponent<Collider>().bounds);
            }
            worldSpace = null;
            //ʹ��Χ�г�Ϊ������
            Vector3 size = bounds.size;
            Vector3 minSize = Vector3.one * Mathf.Max(size.x, size.y, size.z) * 0.5f;
            bounds.SetMinMax(bounds.center -  minSize, bounds.center + minSize);
            bounds.center = new Vector3(0,minSize.x - 5,0);
        }

        #region �˲���������Ƴ�OctreeObject

        /// <summary>
        /// �½ڵ����˲���ǰ��Ҫ����׼��
        /// </summary>
        /// <param name="otObject"></param>
        private void NewNodeInsertInit(OctreeObject otObject)
        {
            allOtObject ??= new HashSet<OctreeObject>();
            otObject.OctreeSystemGet(this);
            allOtObject.Add(otObject);
        }

        /// <summary>
        /// �ڵ����������б�
        /// </summary>
        /// <param name="object"></param>
        public void ReDivide(OctreeObject otObject)
        {
            NewNodeInsertInit(otObject);
            objects.Add(otObject);
        }

        /// <summary>
        /// �ڵ���뷽��
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
        /// �ڵ��Ƴ�
        /// </summary>
        /// <param name="otObject"></param>
        public void RemoveOtObject(OctreeObject otObject)
        {
            allOtObject.Remove(otObject);
        }
        #endregion

        #region �ռ仮�ּ�����

        public void RoomSearch(Bounds otObj, HashSet<OctreeNode> objectSet)
        {
            root.RoomCheck(otObj, objectSet);
        }


        #endregion


    }
}