using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class BuyRoleItemRequest : BaseRequest
{
    private RoleShopPanel roleShopPanel;

    public override void Awake()
    {
        requestCode = RequestCode.RoleShop;
        actionCode = ActionCode.BuyRoleItem;
        roleShopPanel = GetComponent<RoleShopPanel>();
        base.Awake();
    }

    public void SendBuyRequest(string buyInfo)
    {
        SendRequest(buyInfo);
    }

    public override void OnResponse(string data)
    {
        roleShopPanel.UpdateRoleItemStateSync(data);
    }
}
