using System.Collections;
using System.Collections.Generic;
using PathFind;
using UnityEngine;

public class Test : MonoBehaviour
{
    [HideInInspector]
    public PathNode starNode;
    [HideInInspector]
    public PathNode endNode;
    public bool Point = false;

    public Stack<PathNode> path = new Stack<PathNode>();

    AStar pathFinder;
    MainGrid<PathNode> grid;

    private void Start()
    {
        pathFinder = new AStar(48, 48, 2);
        grid = pathFinder.GetMainGridObj();

        Finder();
    }
    private void Update()
    {
        if (Point)
        {
            Point = false;
            Finder();
        }
    }
    private void Finder()
    {
        pathFinder.StartPathFind(transform.position, Player.Instance.transform.position, path);
        foreach (var node in path)
        {
            node.ChangeColor(Color.green);
        }
    }
    
}
