using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class SelectRoleItemRequest : BaseRequest
{
    private RoleShopPanel roleShopPanel;

    public override void Awake()
    {
        requestCode = RequestCode.RoleShop;
        actionCode = ActionCode.SelectRoleItem;
        roleShopPanel = GetComponent<RoleShopPanel>();
        base.Awake();
    }
    public void SendSelectRequest(string selectInfo)
    {
        SendRequest(selectInfo);
    }
    public override void OnResponse(string data)
    {
        roleShopPanel.UpdateRoleSelectStateSync(data);
    }
}
