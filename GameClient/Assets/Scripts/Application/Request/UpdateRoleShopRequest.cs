using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class UpdateRoleShopRequest : BaseRequest
{
    private RoleShopPanel roleShopPanel;

    public override void Awake()
    {
        requestCode = RequestCode.RoleShop;
        actionCode = ActionCode.UpdateRoleShop;
        roleShopPanel = GetComponent<RoleShopPanel>();
        base.Awake();
    }
    public void SendUpdateRequest()
    {
        SendRequest("UpdateRoleShop");
    }
    public override void OnResponse(string data)
    {
        roleShopPanel.UpdateRoleShopStateSync(data);
    }
}
