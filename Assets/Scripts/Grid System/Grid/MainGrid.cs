using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ��ʾȷ������ķ�ʽ
/// close��������Ľڵ�
/// actual:�����ڵĽڵ�
/// </summary>
public enum SearchGrindMode
{
    Close,
    actual,
}

//������ϵͳ
public class MainGrid<T> where T : class, IGridNodes
{
    private int height;                     //��������߶�
    private int width;                      //�������߶�
    private int gridSize;                   //���ӵĴ�С
    private GameObject gridParent;          //����Grid�ĸ�����

    private int[,] GridArray;               //�洢���������Ԥ��ֵ
    public T[,] GridNodesArray;             //�洢�ڵ�Ľڵ�ű�
    private Vector3 origin;                 //��������ĵ�

    public MainGrid()
    {

    }

    #region Ѱ·���

    //Խ��ͻ᷵�ؿսڵ�
    public T GetNode(int x, int y)
    {
        //�Գ�ԭ��仯
        ReCorrectGrid(ref x, ref y);
        //���Խ��
        if (CheckOutLine(x, y))
            return null;

        return GridNodesArray[x,y];
    }
    public T GetNode(Vector3 startPosition)
    {
        //�������յ�Ľڵ�
        Vector3Int s = GetWorldPositionToGrid(startPosition, SearchGrindMode.actual);
        return GetNode(s.x, s.z);
    }

    #endregion


    /// <summary>
    /// 
    /// </summary>
    /// <param name="height">y</param>
    /// <param name="width">x</param>
    /// <param name="gridSize">�����С</param>
    /// <param name="nodeAction">��Ҫ�Դ��������ĸ����������飨һ��������״̬��</param>
    public void CreatGrid(int width, int height, int gridSize, UnityAction<IGridNodes> nodeAction = null)
    {
        this.height = height;
        this.width = width;
        this.gridSize = gridSize;

        GridArray = new int[width, height];                     //���������ֵ

        GridNodesArray = new T[width, height];

        origin = GetGridToWorldPosition(width / 2, height / 2);

        gridParent = new GameObject("Grid"); // GameObject.FindGameObjectsWithTag("GridText")[0];

        Quaternion q = Quaternion.AngleAxis(90, Vector3.right);
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                //ʵ���������Ԥ����
                GameObject obj = GameObject.Instantiate(Resources.Load<GameObject>("Grid/GridCell"), GetGridToWorldPosition(i, j, true), q, gridParent.transform);

                //����Ľڵ����
                T p = obj.GetComponent<T>();

                //�����Ӧԭ��仯
                int x = i;int y = j;
                ReBuildGrid(ref x, ref y);
                //�ýڵ��е�xyֵ����ʵ��ֵ
                p.InitGridNodes(x, y, GridArray[i,j]);
                p.InitGridScale(gridSize);
                GridNodesArray[i, j] = p;

                //����
                DrawGridLine(i, j);
            }
        }

        //Ӧ�þ�̬������
        ApplyStaticBatching();

        Debug.DrawLine(GetGridToWorldPosition(width, 0), GetGridToWorldPosition(width, height), Color.white, 100f);
        Debug.DrawLine(GetGridToWorldPosition(0, height), GetGridToWorldPosition(width, height), Color.white, 100f);
    }

    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void DrawGridLine(int x, int y)
    {
        Debug.DrawLine(GetGridToWorldPosition(x, y), GetGridToWorldPosition(x, y + 1), Color.white, 100f);
        Debug.DrawLine(GetGridToWorldPosition(x, y), GetGridToWorldPosition(x + 1, y), Color.white, 100f);
    }

    /// <summary>
    /// ��ȡ����������������
    /// </summary>
    /// <param name="x">��</param>
    /// <param name="y">��</param>
    /// <param name="center">�Ƿ�Ҫ��ȡ��������λ��</param>
    /// <returns></returns>
    public Vector3 GetGridToWorldPosition(int x, int y, bool center = false)
    {
        Vector3 vector = new Vector3(x, 0, y) * gridSize - (origin == null ? Vector3.zero : origin);
        float g = gridSize / 2;
        if (center)
        {
            vector.x += g;
            vector.z += g;
        }

        return vector;
    }

    /// <summary>
    /// �������������ȡ��Ӧ������
    /// </summary>
    /// <param name="position">�������������</param>
    /// <returns></returns>
    public Vector3Int GetWorldPositionToGrid(Vector3 position, SearchGrindMode mode = SearchGrindMode.actual)
    {
        int x, y;
        switch (mode)
        {
            case SearchGrindMode.actual:
                //����ȡ��
                x = Mathf.FloorToInt(position.x / gridSize);
                y = Mathf.FloorToInt(position.z / gridSize);
                break;
            case SearchGrindMode.Close:
                //��������
                x = Mathf.RoundToInt(position.x / gridSize);
                y = Mathf.RoundToInt(position.z / gridSize);
                break;
            default:
                //����ȡ��
                x = Mathf.FloorToInt(position.x / gridSize);
                y = Mathf.FloorToInt(position.z / gridSize);
                break;
        }
        return new Vector3Int(x, 0, y);

    }

    #region Value�Ļ�ȡ���޸ķ���

    public int GetGridValue(int x, int y)
    {
        if (TranslateGridCoordinates(ref x, ref y))
            throw new Exception("����Խ��");

        return GridArray[x, y];
    }

    public int GetGridValue(Vector3 position)
    {
        Vector3Int v = GetWorldPositionToGrid(position);
        return GridNodesArray[v.x, v.z].Value;
    }

    //���������ֵ
    public void SetGridValue(int x, int y, int value)
    {
        //�Գ�ԭ��仯/���Խ��
        if (TranslateGridCoordinates(ref x, ref y))
        {
            Debug.Log("��������Խ��");
            return;
        }

        value = Mathf.Clamp(value, -100, 100);
        //�����������
        GridNodesArray[x,y].ChangeValue(value);
        Debug.Log("GridArray�е�ֵ��"+ value);
    }

    /// <summary>
    /// ����λ�õõ���Ӧ�����񣬲�����ֵ
    /// </summary>
    /// <param name="position"></param>
    /// <param name="value"></param>
    public void SetGridValue(Vector3 position, int value)
    {
        Vector3Int grid = GetWorldPositionToGrid(position);
        SetGridValue(grid.x, grid.z, value);
    }

    #endregion


    /// <summary>
    /// Ӧ�þ�̬������
    /// </summary>
    private void ApplyStaticBatching()
    {
        // Ӧ�þ�̬������
        StaticBatchingUtility.Combine(gridParent);
    }
    
    private bool TranslateGridCoordinates(ref int x, ref int y)
    {
        //�Գ�ԭ��仯
        ReCorrectGrid(ref x, ref y);
        //���Խ��
        if (CheckOutLine(x, y))
        {
            Debug.LogWarning("Խ�����");
            return false;
        }
        return true;

    }

    private void ReBuildGrid(ref int x, ref int y)
    {
        //��Ӧԭ��仯
        x -= width / 2;
        y -= height / 2;
    }

    public void ReCorrectGrid(ref int x, ref int y)
    {
        //�Գ�ԭ��仯
        x += width / 2;
        y += height / 2;
    }

    /// <summary>
    /// �ж������xy�Ƿ����
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private bool CheckOutLine(int x, int y)
    {
        return (x < 0 || y < 0 || x > width - 1 || y > height - 1);
    }

}
