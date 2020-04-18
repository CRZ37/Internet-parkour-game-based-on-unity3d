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
    //玩家进入游戏之前就拥有的双倍金币道具次数
    public int RoleMultiplyCoinTime { get; private set; }
    //玩家进入游戏之前就拥有的无敌道具次数
    public int RoleInvincibleTime { get; private set; }
    public RoleData(Role_ResultRoleType type, string rolePath,Vector3 spawnPos,int health,float skillTime,int roleDoubleCoinTime,int roleInvincibleTime)
    {
        Type = type;
        RolePrefab = Resources.Load(rolePath) as GameObject;      
        SpawnPos = spawnPos;
        Health = health;
        SkillTime = skillTime;
        RoleMultiplyCoinTime = roleDoubleCoinTime;
        RoleInvincibleTime = roleInvincibleTime;
    }
}
