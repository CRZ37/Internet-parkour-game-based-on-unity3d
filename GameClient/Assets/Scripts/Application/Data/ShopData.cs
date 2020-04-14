using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ShopData
{
    public int HealthPrice { get; set; }
    public int BigHealthPrice { get; set; }
    public int SkillTimePrice { get; set; }
    public int BigSkillTimePrice { get; set; }

    public int RoleMalePrice { get; set; }
    public int RoleCopPrice { get; set; }
    public int RoleRobotPrice { get; set; }
    public void SetPropertyShopState(int healthPrice, int bigHealthPrice, int skillTimePrice, int bigSkillTimePrice)
    {
        HealthPrice = healthPrice;
        BigHealthPrice = bigHealthPrice;
        SkillTimePrice = skillTimePrice;
        BigSkillTimePrice = bigSkillTimePrice;
    }
    public void SetRoleShopState(int roleMalePrice,int roleCopPrice,int roleRobotPrice)
    {
        RoleMalePrice = roleMalePrice;
        RoleCopPrice = roleCopPrice;
        RoleRobotPrice = roleRobotPrice;
    }
}

