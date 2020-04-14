using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserData
{
    //房间内页面使用此构造函数
    public UserData(string userData)
    {
        string[] strs = userData.Split(',');
        Id = int.Parse(strs[0]);
        Username = strs[1];
        TotalCount = int.Parse(strs[2]);
        WinCount = int.Parse(strs[3]);
        CoinNum = int.Parse(strs[4]);
        Health = int.Parse(strs[5]);
        SkillTime = float.Parse(strs[6]);
        RoleSelectState = strs[7];
    }

    //显示房间列表的时候使用此构造函数
    public UserData(int id, string username, int totalCount, int winCount)
        : this(id, username, totalCount, winCount, -1, -1, -1) { }

    //登录的时候使用此构造方法
    public UserData(int id, string username, int totalCount, int winCount, int coinNum, int health, float skillTime)
    {
        Id = id;
        Username = username;
        TotalCount = totalCount;
        WinCount = winCount;
        CoinNum = coinNum;
        Health = health;
        SkillTime = skillTime;
        //防止未初始化就访问
        RoleBuyState = new int[3]{ -1,-1,-1};
    }
    public void SetRoleBuyState(int roleMaleBuyState, int roleCopBuyState, int roleRobotBuyState)
    {
        RoleBuyState[0] = roleMaleBuyState;
        RoleBuyState[1] = roleCopBuyState;
        RoleBuyState[2] = roleRobotBuyState;
    }
    public void SetRoleSeclctState(string roleSelectState)
    {
        RoleSelectState = roleSelectState;
    }
    public int Id { get; set; }
    public string Username { get; set; }
    public int TotalCount { get; set; }
    public int WinCount { get; set; }
    public int CoinNum { get; set; }
    public int Health { get; set; }
    public float SkillTime { get; set; }
    public string RoleSelectState { get; set; }
    //防止未初始化
    public int[] RoleBuyState { get; set; }
    //get => RoleBuyState ?? (new int[] { -1, -1, -1 }); set { RoleBuyState = value; } 
}
