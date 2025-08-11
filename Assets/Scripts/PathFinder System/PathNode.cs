using TMPro;
using UnityEngine;

public class PathNode : MonoBehaviour, IGridNodes
{

    [HideInInspector] public PathNode fatherNode;        //��¼���ڵ�
    [HideInInspector] public int FCost => HCost + GCost + nodevalue;    //�ܴ���
    [HideInInspector] public int HCost;                             //Ԥ�ƴ���
    [HideInInspector] public int GCost;                             //�������
    private int nodevalue;
    [HideInInspector] public int Value { get => nodevalue; private set { text.text = value.ToString();nodevalue = value; } }
    int IGridNodes.Value { get => nodevalue;}

    //����ֵ
    [HideInInspector] public int x;
    [HideInInspector] public int y;

    public SpriteRenderer Children;                     //ͼƬ���
    public TextMeshPro text;                            //�ı����
    
    [HideInInspector] public bool isWall = false;       //�Ƿ�Ϊ�ϰ�
    //·����ʾ��ɫ
    private Color wallColor = Color.black;
    private Color normalColor = Color.white;

    private void Awake()
    {
        Children.color = normalColor;
    }

    /// <summary>
    /// �����ʼ��
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
        transform.localScale = new Vector3(gridSize - 0.1f, gridSize - 0.1f, gridSize - 0.1f);      //�������������С
        text.fontSize = gridSize * 7;                                                               //���������С
        
    }

    public void InitText(TMP_FontAsset fontAsset, Material fontMaterial)
    {
        text.font = fontAsset;
        text.fontSharedMaterial = fontMaterial;
        text.enableAutoSizing = false;
    }

    //�÷����Ϊ�ϰ���
    public bool BecomeWall()
    {
        isWall = true;
        Children.color = wallColor;
        return isWall;
    }
    //���÷���
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
/// �ڵ�ӿ�
/// </summary>
public interface IGridNodes
{
    public int Value { get; }
    void InitGridNodes(int x, int y, int value);

    void InitGridScale(int gridSize);

    void ChangeValue(int value);

    void ResetNode();
}
