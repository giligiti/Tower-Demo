using System;
using UnityEngine;

public class EffInfo : InfoData
{
    public int id;
    public string name;
    public bool isLoop;
    public int loop { set { isLoop = Convert.ToBoolean(value); } }
    public float delayTime;
    public int particleType;
    public string file_path;

}
