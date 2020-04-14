using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using UnityEngine.UI;
using System;

public class RoleShopPanel : BasePanel
{
    [SerializeField]
    private RoleShopItem roleMaleItem;
    [SerializeField]
    private RoleShopItem roleCopItem;
    [SerializeField]
    private RoleShopItem roleRobotItem;
    private BuyRoleItemRequest buyRoleItemRequest;
    private SelectRoleItemRequest selectRoleItemRequest;
    private UpdateRoleShopRequest updateRoleShopRequest;
    private RectTransform panelBG;
    private RectTransform content;
    private string roleShopData = null;
    private string roleItemData = null;
    private string roleSelectData = null;

    private void Awake()
    {
        buyRoleItemRequest = GetComponent<BuyRoleItemRequest>();
        Debug.Log(buyRoleItemRequest);
        selectRoleItemRequest = GetComponent<SelectRoleItemRequest>();
        updateRoleShopRequest = GetComponent<UpdateRoleShopRequest>();
        panelBG = transform.GetComponent<RectTransform>();
        content = transform.Find("Content").GetComponent<RectTransform>();
        content.Find("CloseButton").GetComponent<Button>().onClick.AddListener(OnCloseClick);
    }

    private void OnCloseClick()
    {
        ClosePanelPUNISHING(panelBG, content, 120, 1500);
    }
    public override void OnEnter()
    {
        //查看哪个人物是被选中的，显示它的选择框，也查看哪个人物被购买了，将其购买按钮禁用并显示“已购买”
        updateRoleShopRequest.SendUpdateRequest();
        OpenPanelPUNISHING(panelBG, content, 800, 120, 1500);
    }
    public void OnBuyClick(string buyInfo)
    {       
        buyRoleItemRequest.SendBuyRequest(buyInfo);
    }

    public void OnselectClick(string selectInfo)
    {
        selectRoleItemRequest.SendSelectRequest(selectInfo);
    }

    private void Update()
    {
        if(roleShopData != null)
        {
            UpdateRoleShopState();
            roleShopData = null;
        }
        if(roleItemData != null)
        {
            UpdateRoleItemState();
            roleItemData = null;
        }
        if(roleSelectData != null)
        {
            UpdateRoleSelectState();
            roleSelectData = null;
        } 
    }
    public void UpdateRoleSelectStateSync(string roleSelectData)
    {
        this.roleSelectData = roleSelectData;
    }
    private void UpdateRoleSelectState()
    {
        UIMng.ShowMessageSync("选择成功");
        SetRoleItemSelectState(roleSelectData);
    }

    public void UpdateRoleItemStateSync(string roleItemData)
    {
        this.roleItemData = roleItemData;
    }
    private void UpdateRoleItemState()
    {
        UIMng.ShowMessageSync("购买成功！");
        SetRoleItemState(roleItemData, "1");
    }
    public void UpdateRoleShopStateSync(string roleShopData)
    {
        this.roleShopData = roleShopData;
    }
    
    private void UpdateRoleShopState()
    {
        string[] updateDatas1 = roleShopData.Split('|');
        ReturnCode returnCode = (ReturnCode)int.Parse(updateDatas1[0]);
        if (returnCode == ReturnCode.Success)
        {
            UIMng.ShowMessageSync("更新人物商店状态成功！");
            string[] updateDatas2 = updateDatas1[1].Split(',');      
            string roleMaleBuyState = updateDatas2[0];
            string roleCopBuyState = updateDatas2[1];
            string roleRobotBuyState = updateDatas2[2];
            string roleMalePrice = updateDatas1[2];
            string roleCopPrice = updateDatas1[3];
            string roleRobotPrice = updateDatas1[4];
            string roleSelectState = updateDatas1[5];
            SetRoleItemPrice("1", roleMalePrice);
            SetRoleItemPrice("2", roleCopPrice);
            SetRoleItemPrice("3", roleRobotPrice);
            SetRoleItemState("1", roleMaleBuyState);
            SetRoleItemState("2", roleCopBuyState);
            SetRoleItemState("3", roleRobotBuyState);
            SetRoleItemSelectState(roleSelectState);
        }
        else
        {
            UIMng.ShowMessageSync("更新商店状态失败！请退出后重新进入");
        }
    }
    private void SetRoleItemPrice(string itemIndex,string price)
    {
        switch (itemIndex)
        {
            case "1":
                roleMaleItem.SetPrice(price);
                break;
            case "2":
                roleCopItem.SetPrice(price);
                break;
            case "3":
                roleRobotItem.SetPrice(price);
                break;
        }
    }
    private void SetRoleItemSelectState(string selectId)
    {
        Game.Instance.SetRoleSelectState(selectId);   
        switch (selectId)
        {
            case "1":
                roleMaleItem.SetSelect();
                break;
            case "2":
                roleCopItem.SetSelect();
                break;
            case "3":
                roleRobotItem.SetSelect();
                break;
        }
    }
    private void SetRoleItemState(string itemId, string buyState)
    {
        switch (itemId)
        {
            case "1":
                if(buyState.Equals("1"))
                {
                    roleMaleItem.SetBuy();
                    Game.Instance.SetRoleMaleBuyState();
                }
                else
                {
                    roleMaleItem.SetNoBuy();
                }
                break;
            case "2":
                if (buyState.Equals("1"))
                {
                    roleCopItem.SetBuy();
                    Game.Instance.SetRoleCopBuyState();
                }
                else
                {
                    roleCopItem.SetNoBuy();
                }
                break;
            case "3":
                if (buyState.Equals("1"))
                {
                    roleRobotItem.SetBuy();
                    Game.Instance.SetRoleRobotBuyState();
                }
                else
                {
                    roleRobotItem.SetNoBuy();
                }
                break;
        }
    }
}
