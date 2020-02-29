using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class UpdateShopRequest : BaseRequest
{
    private ShopPanel shopPanel;
    public override void Awake()
    {
        requestCode = RequestCode.Shop;
        actionCode = ActionCode.UpdateShop;
        shopPanel = GetComponent<ShopPanel>();
        base.Awake();
    }
    public void SendUpdateShopRequest()
    {
        SendRequest("UpdateShop");
    }
    public override void OnResponse(string data)
    {
        shopPanel.UpdateShopStateSync(data);
    }
}
