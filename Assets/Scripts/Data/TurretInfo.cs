using UnityEngine;

//ÅÚËşÀàĞÍÃ¶¾Ù
public enum TurretType
{
    Constructor = 0,
    comp,

}

public class TurretInfo : InfoData
{
    public int id;
    public string name;
    public int atk;
    public TurretType type;
    public int atkRange;
    public int atkSpeed;
    public int roundSpeed;
    public int verticalSpeed;
    public float verticalAngleUp;
    public float verticalAngleDown;
    public string prefabPath;
}
