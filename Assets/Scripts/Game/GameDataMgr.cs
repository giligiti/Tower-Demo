using System;
using System.Collections.Generic;
using Octree;
using UnityEngine;

public class GameDataMgr : BaseManager<GameDataMgr>
{
    public List<MonsterInfo> monsterInfos = new List<MonsterInfo>();

    public List<TurretInfo> turretInfos = new List<TurretInfo>();

    public List<EffInfo> effDatas = new List<EffInfo>();

    public List<GunInfo> gunInfos = new List<GunInfo>();
    //八叉树注册物体
    public HashSet<OctreeObject> octreeMonos = new HashSet<OctreeObject>();

    private GameDataMgr()
    {
        monsterInfos = JsonMgr.Instance.LoadData<List<MonsterInfo>>("MonsterInfo");

        turretInfos = JsonMgr.Instance.LoadData<List<TurretInfo>>("TurretInfo");

        effDatas = JsonMgr.Instance.LoadData<List<EffInfo>>("EffInfo");

        gunInfos = JsonMgr.Instance.LoadData<List<GunInfo>>("GunInfo");

    }

}
