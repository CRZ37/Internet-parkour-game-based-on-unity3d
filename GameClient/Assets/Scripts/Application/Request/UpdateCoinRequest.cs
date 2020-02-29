using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using UnityEngine.UI;

public class UpdateCoinRequest : BaseRequest
{
    private Text coin;
    private string coinNum = "";
    public override void Awake()
    {
        requestCode = RequestCode.User;
        actionCode = ActionCode.UpdateCoin;
        coin = transform.Find("CoinNum").GetComponent<Text>();
        base.Awake();
    }
    //购买的时候调用
    public override void SendRequest()
    {
        SendRequest("UpdateCoin");
    }
    private void Update()
    {
        if(coinNum != "")
        {
            Game.Instance.UpdateCoin(int.Parse(coinNum));
            coin.text = coinNum;
        }
    }
    public override void OnResponse(string data)
    {
        coinNum = data;
    }
}
