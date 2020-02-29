using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using System;

public class BuyItemRequest : BaseRequest
{
    private ShopPanel shopPanel;  
    public override void Awake()
    {
        requestCode = RequestCode.Shop;
        actionCode = ActionCode.BuyItem;
        shopPanel = GetComponent<ShopPanel>();
        base.Awake();
    }
    public void SendBuyRequest(string buyInfo)
    {
        SendRequest(buyInfo);
    }
    public override void OnResponse(string data)
    {
        shopPanel.UpdateItemStateSync(data);
    }
}
