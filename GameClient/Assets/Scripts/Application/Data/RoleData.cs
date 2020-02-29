using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class RoleData
{
    //玩家血量，碰撞6次后死亡
    public int Health { get; private set; }
    //技能的时间
    public float SkillTime { get; private set; }
    //玩家角色模型的生成位置
    public Vector3 SpawnPos { get; private set; }
    //玩家身份
    public Role_ResultRoleType Type { get; private set; }
    //玩家模型
    public GameObject RolePrefab { get; private set; }

    public RoleData(Role_ResultRoleType type, string rolePath,Vector3 spawnPos,int health=-1,float skillTime=-1)
    {
        Type = type;
        RolePrefab = Resources.Load(rolePath) as GameObject;      
        SpawnPos = spawnPos;
        Health = health;
        SkillTime = skillTime;
    }
}
