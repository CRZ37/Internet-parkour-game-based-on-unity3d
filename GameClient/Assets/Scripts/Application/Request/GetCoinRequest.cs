using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class GetCoinRequest : BaseRequest
{
    private RemotePlayerMove remotePlayerMove;
    public void SetRemotePlayerMove(RemotePlayerMove remotePlayerMove)
    {
        this.remotePlayerMove = remotePlayerMove;
    }
    public override void Awake()
    {
        requestCode = RequestCode.Game;
        actionCode = ActionCode.GetCoin;
        base.Awake();
    }
    public void SendCoinRequest(int coin)
    {
        SendRequest(coin.ToString());
    }
    public override void OnResponse(string data)
    {
        int remoteCoin = int.Parse(data);
        remotePlayerMove.SyncRemoteCoin(remoteCoin);
    }
}
