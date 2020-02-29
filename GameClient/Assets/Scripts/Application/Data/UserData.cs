using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserData
{
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
    }
    
    //只用来显示房间列表的UserData
    public UserData(int id, string username, int totalCount, int winCount)
        : this(id, username, totalCount, winCount, -1,-1,-1) { }

    public UserData(int id, string username, int totalCount, int winCount, int coinNum, int health, float skillTime)
    {
        Id = id;
        Username = username;
        TotalCount = totalCount;
        WinCount = winCount;
        CoinNum = coinNum;
        Health = health;
        SkillTime = skillTime;
    }
    public void SetShopState(int healthTime,int bigHealthTime,int skillTimeTime,int bigSkillTimeTime)
    {
        HealthTime = healthTime;
        BigHealthTime = bigHealthTime;
        SkillTimeTime = skillTimeTime;
        BigSkillTimeTime = bigSkillTimeTime;
    }
    public int Id { get; set; }
    public string Username { get; set; }
    public int TotalCount { get; set; }
    public int WinCount { get; set; }
    public int CoinNum { get; set; }
    public int Health { get; set; }
    public float SkillTime { get; set; }
    public int HealthTime { get; set; }
    public int BigHealthTime { get; set; }
    public int SkillTimeTime { get; set; }
    public int BigSkillTimeTime { get; set; }
}
