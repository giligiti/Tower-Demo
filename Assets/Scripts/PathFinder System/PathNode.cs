using TMPro;
using UnityEngine;

public class PathNode : MonoBehaviour, IGridNodes
{

    [HideInInspector] public PathNode fatherNode;        //记录父节点
    [HideInInspector] public int FCost => HCost + GCost + nodevalue;    //总代价
    [HideInInspector] public int HCost;                             //预计代价
    [HideInInspector] public int GCost;                             //到达代价
    private int nodevalue;
    [HideInInspector] public int Value { get => nodevalue; private set { text.text = value.ToString();nodevalue = value; } }
    int IGridNodes.Value { get => nodevalue;}

    //坐标值
    [HideInInspector] public int x;
    [HideInInspector] public int y;

    public SpriteRenderer Children;                     //图片组件
    public TextMeshPro text;                            //文本组件
    
    [HideInInspector] public bool isWall = false;       //是否为障碍
    //路径表示颜色
    private Color wallColor = Color.black;
    private Color normalColor = Color.white;

    private void Awake()
    {
        Children.color = normalColor;
    }

    /// <summary>
    /// 网格初始化
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void InitGridNodes(int x, int y, int value)
    {
        this.x = x;
        this.y = y;
        this.Value = value;
        
    }
    public void InitGridScale(int gridSize)
    {
        transform.localScale = new Vector3(gridSize - 0.1f, gridSize - 0.1f, gridSize - 0.1f);      //调整网格组件大小
        text.fontSize = gridSize * 7;                                                               //调整字体大小
        
    }

    public void InitText(TMP_FontAsset fontAsset, Material fontMaterial)
    {
        text.font = fontAsset;
        text.fontSharedMaterial = fontMaterial;
        text.enableAutoSizing = false;
    }

    //让方格成为障碍物
    public bool BecomeWall()
    {
        isWall = true;
        Children.color = wallColor;
        return isWall;
    }
    //重置方格
    public void ResetNode()
    {
        isWall=false;
        Value = 0;
        GCost = 0;
        HCost = 0;
        Children.color = normalColor;
    }

    public void ChangeColor(Color color)
    {
        Children.color = color;
    }

    public void ChangeValue(int value)
    {
        this.Value = value;
    }

    private bool Equals(PathNode other) => other != null && x == other.x && y == other.y;
    public override bool Equals(object other)
    {
        return Equals(other as PathNode);
    }
    public override int GetHashCode() => x.GetHashCode() ^ y.GetHashCode();
}

/// <summary>
/// 节点接口
/// </summary>
public interface IGridNodes
{
    public int Value { get; }
    void InitGridNodes(int x, int y, int value);

    void InitGridScale(int gridSize);

    void ChangeValue(int value);

    void ResetNode();
}
