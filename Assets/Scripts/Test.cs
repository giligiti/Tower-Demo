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

    //inspector�����Ͻ����޸�
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
    //��ʼѰ·��UI��ť����
    public void StartPathFinder()
    {
        Stopwatch sw = Stopwatch.StartNew();                                                                        //���ԣ���ʱ
        sw.Start();                                                                                                 //���ԣ���ʱ
        bool pathSuccess = pathFinder.StartPathFind(starNode, endNode, path);
        sw.Stop();                                                                                                  //���ԣ���ʱ
        float time = sw.ElapsedMilliseconds;                                                                        //���ԣ���ʱ
        if (pathSuccess)
        {
            UnityEngine.Debug.Log("Ѱ·�ɹ�����ʱ��"+time+"����");
        }
        else
        {
            UnityEngine.Debug.Log("Ѱ·ʧ�ܣ���ʱ��" + time + "����");
        }
        StartCoroutine(PathCallBack(path));
    }
    //���ø���״̬
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
