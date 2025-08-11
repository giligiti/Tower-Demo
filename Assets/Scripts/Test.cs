using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class Test : MonoBehaviour
{
    public enum E_GridState
    {
        start,
        end,
        wall,
        normal,
    }
    [HideInInspector]
    public PathNode starNode;
    [HideInInspector]
    public PathNode endNode;
    public bool Point = false;
    [HideInInspector]
    Stack<PathNode> path = new Stack<PathNode>();

    AStar pathFinder;
    MainGrid<PathNode> grid;

    //inspector窗口上进行修改
    public E_GridState state = E_GridState.wall;
    private void Start()
    {
        pathFinder = new AStar(24, 24, 2);
        grid = pathFinder.GetMainGridObj();

    }
    private void Update()
    {
        OnMc();
    }
    private void OnMc()
    {
        if (Input.GetMouseButtonDown(0) && Point)
        {
            Ray x = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(x, out RaycastHit hitInfo, 200f);
            UnityEngine.Debug.DrawLine(Camera.main.transform.position, hitInfo.point, Color.red, 3f);
            PathNode node = grid.GetNode(hitInfo.point);
            if (node != null)
                ChangeState(node);
        }
    }
    
    public void ChangeState(PathNode node)
    {
        switch (state)
        {
            case E_GridState.start:
                node.ChangeColor(Color.yellow);
                starNode = node;
                break;
            case E_GridState.end:
                node.ChangeColor(Color.red);
                endNode = node;
                break;
            case E_GridState.wall:
                node.BecomeWall();
                break;
            case E_GridState.normal:
                node.ChangeColor(Color.white);
                node.isWall = false;
                node.ChangeValue(node.Value + 1);
                break;
        }
    }
    //开始寻路：UI按钮引用
    public void StartPathFinder()
    {
        Stopwatch sw = Stopwatch.StartNew();                                                                        //调试：计时
        sw.Start();                                                                                                 //调试：计时
        bool pathSuccess = pathFinder.StartPathFind(starNode, endNode, path);
        sw.Stop();                                                                                                  //调试：计时
        float time = sw.ElapsedMilliseconds;                                                                        //调试：计时
        if (pathSuccess)
        {
            UnityEngine.Debug.Log("寻路成功，耗时："+time+"毫秒");
        }
        else
        {
            UnityEngine.Debug.Log("寻路失败，耗时：" + time + "毫秒");
        }
        StartCoroutine(PathCallBack(path));
    }
    //重置格子状态
    public void ResetGridState()
    {
        path = new Stack<PathNode>();
        pathFinder.ResetPathFinder();
    }

    IEnumerator PathCallBack(Stack<PathNode> path)
    {

        foreach (var node in path)
        {
            node.ChangeColor(Color.green);
            yield return new WaitForSeconds(0.3f);
        }

    }
}
