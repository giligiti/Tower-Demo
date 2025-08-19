using UnityEngine;

namespace Octree
{
    public class OctreeMono : MonoBehaviour
    {
        [HideInInspector] public OctreeObject octreeObject;                   //��ʾ������ĳ���˲����ռ�

        [HideInInspector] public Bounds bounds;                               //����İ�Χ��
        
        protected Vector3 lastPosition = new Vector3();
        public bool isDead = false;

        private void Awake()
        {
            TryGetIDeath();
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        protected virtual void Start()
        {
            //������Octree����ϵ
            OctreeSystemInit();

            lastPosition = transform.position;
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            CheckPositionChange();
        }

        /// <summary>
        /// �����Octreeϵͳ������ϵ
        /// </summary>
        private void OctreeSystemInit()
        {
            //�������������Χ��
            CharacterController c = GetComponentInChildren<CharacterController>();

            if (c != null)
            {
                bounds = new Bounds();
                bounds.Encapsulate(c.bounds);
            }

            else bounds = GetComponent<Collider>().bounds;
                

            //������Ӧ��OctreeObject
            octreeObject = new OctreeObject(bounds, this);

            //�������OctreeObjectע�ᵽ���ݹ������Ĺ�ϣ����
            GameDataMgr.Instance.octreeMonos.Add(octreeObject);
        }

        /// <summary>
        /// λ�ø���ͬ����֪ͨ��Ӧ��OctreeObject���и���
        /// </summary>
        private void CheckPositionChange()
        {
            //��λ�÷����䶯
            if (transform.position != lastPosition && !isDead)
            {
                lastPosition = transform.position;
                //�ı��Χ��λ��
                bounds.center = transform.position;
                //�滻OctreeObject�İ�Χ��
                octreeObject.ChangeBounds(bounds);
                //֪ͨλ�÷����䶯�����°˲���
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
        /// ��������������������˲���ϵͳ
        /// </summary>
        private void Dead()
        {
            Debug.Log("dead");
            octreeObject.BreakParent(true);
            isDead = true;
        }

        protected virtual void OnDrawGizmos()
        {
            //Gizmos.DrawWireCube(bounds.center, bounds.size);//    ��ʾʵ��ռ�ݿռ�
        }
        protected virtual void OnDisable()
        {
            Debug.Log($"{gameObject.name} �� OnDisable �����ã�����Ƿ����ã�{enabled}�������Ƿ񼤻{gameObject.activeSelf}", this);
            octreeObject.BreakParent(true);
            octreeObject = null;
            isDead = false;
        }
    }
}

