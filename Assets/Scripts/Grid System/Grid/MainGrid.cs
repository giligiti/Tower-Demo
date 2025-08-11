using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 表示确定网格的方式
/// close：按最近的节点
/// actual:按所在的节点
/// </summary>
public enum SearchGrindMode
{
    Close,
    actual,
}

//主网格系统
public class MainGrid<T> where T : class, IGridNodes
{
    private int height;                     //网格纵向高度
    private int width;                      //网格横向高度
    private int gridSize;                   //格子的大小
    private GameObject gridParent;          //所有Grid的父对象

    private int[,] GridArray;               //存储所有网格的预定值
    public T[,] GridNodesArray;             //存储节点的节点脚本
    private Vector3 origin;                 //网格的中心点

    public MainGrid()
    {

    }

    #region 寻路相关

    //越界就会返回空节点
    public T GetNode(int x, int y)
    {
        //对冲原点变化
        ReCorrectGrid(ref x, ref y);
        //检查越界
        if (CheckOutLine(x, y))
            return null;

        return GridNodesArray[x,y];
    }
    public T GetNode(Vector3 startPosition)
    {
        //获得起点终点的节点
        Vector3Int s = GetWorldPositionToGrid(startPosition, SearchGrindMode.actual);
        return GetNode(s.x, s.z);
    }

    #endregion


    /// <summary>
    /// 
    /// </summary>
    /// <param name="height">y</param>
    /// <param name="width">x</param>
    /// <param name="gridSize">网格大小</param>
    /// <param name="nodeAction">需要对创建出来的格子做的事情（一般是重置状态）</param>
    public void CreatGrid(int width, int height, int gridSize, UnityAction<IGridNodes> nodeAction = null)
    {
        this.height = height;
        this.width = width;
        this.gridSize = gridSize;

        GridArray = new int[width, height];                     //储存网格的值

        GridNodesArray = new T[width, height];

        origin = GetGridToWorldPosition(width / 2, height / 2);

        gridParent = new GameObject("Grid"); // GameObject.FindGameObjectsWithTag("GridText")[0];

        Quaternion q = Quaternion.AngleAxis(90, Vector3.right);
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                //实例化网格的预制体
                GameObject obj = GameObject.Instantiate(Resources.Load<GameObject>("Grid/GridCell"), GetGridToWorldPosition(i, j, true), q, gridParent.transform);

                //传入的节点相关
                T p = obj.GetComponent<T>();

                //坐标对应原点变化
                int x = i;int y = j;
                ReBuildGrid(ref x, ref y);
                //让节点中的xy值是真实的值
                p.InitGridNodes(x, y, GridArray[i,j]);
                p.InitGridScale(gridSize);
                GridNodesArray[i, j] = p;

                //画线
                DrawGridLine(i, j);
            }
        }

        //应用静态批处理
        ApplyStaticBatching();

        Debug.DrawLine(GetGridToWorldPosition(width, 0), GetGridToWorldPosition(width, height), Color.white, 100f);
        Debug.DrawLine(GetGridToWorldPosition(0, height), GetGridToWorldPosition(width, height), Color.white, 100f);
    }

    /// <summary>
    /// 画网格线
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void DrawGridLine(int x, int y)
    {
        Debug.DrawLine(GetGridToWorldPosition(x, y), GetGridToWorldPosition(x, y + 1), Color.white, 100f);
        Debug.DrawLine(GetGridToWorldPosition(x, y), GetGridToWorldPosition(x + 1, y), Color.white, 100f);
    }

    /// <summary>
    /// 获取网格具体的世界坐标
    /// </summary>
    /// <param name="x">列</param>
    /// <param name="y">行</param>
    /// <param name="center">是否要获取网格中心位置</param>
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
    /// 根据世界坐标获取相应的网格
    /// </summary>
    /// <param name="position">传入的世界坐标</param>
    /// <returns></returns>
    public Vector3Int GetWorldPositionToGrid(Vector3 position, SearchGrindMode mode = SearchGrindMode.actual)
    {
        int x, y;
        switch (mode)
        {
            case SearchGrindMode.actual:
                //向下取整
                x = Mathf.FloorToInt(position.x / gridSize);
                y = Mathf.FloorToInt(position.z / gridSize);
                break;
            case SearchGrindMode.Close:
                //四舍五入
                x = Mathf.RoundToInt(position.x / gridSize);
                y = Mathf.RoundToInt(position.z / gridSize);
                break;
            default:
                //向下取整
                x = Mathf.FloorToInt(position.x / gridSize);
                y = Mathf.FloorToInt(position.z / gridSize);
                break;
        }
        return new Vector3Int(x, 0, y);

    }

    #region Value的获取和修改方法

    public int GetGridValue(int x, int y)
    {
        if (TranslateGridCoordinates(ref x, ref y))
            throw new Exception("数组越界");

        return GridArray[x, y];
    }

    public int GetGridValue(Vector3 position)
    {
        Vector3Int v = GetWorldPositionToGrid(position);
        return GridNodesArray[v.x, v.z].Value;
    }

    //设置网格的值
    public void SetGridValue(int x, int y, int value)
    {
        //对冲原点变化/检查越界
        if (TranslateGridCoordinates(ref x, ref y))
        {
            Debug.Log("输入数组越界");
            return;
        }

        value = Mathf.Clamp(value, -100, 100);
        //网格的上下限
        GridNodesArray[x,y].ChangeValue(value);
        Debug.Log("GridArray中的值："+ value);
    }

    /// <summary>
    /// 根据位置得到相应的网格，并设置值
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
    /// 应用静态批处理
    /// </summary>
    private void ApplyStaticBatching()
    {
        // 应用静态批处理
        StaticBatchingUtility.Combine(gridParent);
    }
    
    private bool TranslateGridCoordinates(ref int x, ref int y)
    {
        //对冲原点变化
        ReCorrectGrid(ref x, ref y);
        //检查越界
        if (CheckOutLine(x, y))
        {
            Debug.LogWarning("越界错误");
            return false;
        }
        return true;

    }

    private void ReBuildGrid(ref int x, ref int y)
    {
        //对应原点变化
        x -= width / 2;
        y -= height / 2;
    }

    public void ReCorrectGrid(ref int x, ref int y)
    {
        //对冲原点变化
        x += width / 2;
        y += height / 2;
    }

    /// <summary>
    /// 判断输入的xy是否出界
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private bool CheckOutLine(int x, int y)
    {
        return (x < 0 || y < 0 || x > width - 1 || y > height - 1);
    }

}
