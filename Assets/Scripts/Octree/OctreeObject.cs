using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Octree
{
    public class OctreeObject 
    {
        public Bounds bounds {  get; private set; }

        public Octree octree;

        public List<OctreeNode> parents = new List<OctreeNode>();               //�洢����������Ҷ�ӽڵ�

        public OctreeMono octMonoObj;                                           //��ʾ���OctreeObject��������������//���ڰ˲�����ѯ

        public UnityEvent OctreeComplete = new UnityEvent();                           

        public OctreeObject(Bounds obj, OctreeMono mono)
        {
            ChangeBounds(obj);
            this.octMonoObj = mono;
        }

        /// <summary>
        /// �õ������İ˲�����
        /// </summary>
        public void OctreeSystemGet(Octree octree)
        {
            this.octree = octree;
            OctreeComplete?.Invoke();
        }

        public void ChangeBounds(Bounds obj) => this.bounds = obj;

        /// <summary>
        /// ���boundsToCheck�͸�OctreeObject����İ�Χ���Ƿ��ཻ
        /// </summary>
        /// <param name="boundsToCheck"></param>
        /// <returns></returns>
        public bool Intersects(Bounds boundsToCheck) => bounds.Intersects(boundsToCheck);

        /// <summary>
        /// �趨����������Ҷ�ӽڵ�
        /// </summary>
        /// <param name="node"></param>
        public void BelongNode(OctreeNode node) => parents.Add(node);

        //��OctreeMono���õķ���

        /// <summary>
        /// ���°˲���
        /// </summary>
        public void OctreeUpdate()
        {
            //�Ͽ��ɹ���
            BreakParent(false);
            
            octree.ReDivide(this);
        }

        /// <summary>
        /// �������������ٻ���ʧ���ʱ��
        /// </summary>
        /// <param name="isbreakAll">�Ƿ��ټ���˲���</param>
        public void BreakParent(bool isbreakAll)
        {
            //�Ͽ��ɹ���
            if (parents.Count > 0)
            {
                foreach (OctreeNode node in parents)
                {
                    if (isbreakAll) node.RemoveObject(this);
                    if (!Intersects(node.bounds)) node.RemoveObject(this);
                }
                parents.Clear();
            }
            octree.RemoveOtObject(this);

        }

    }
}

