using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class TakeDamageRequest : BaseRequest
{
    private RemotePlayerMove remotePlayerMove;
    public void SetRemotePlayerMove(RemotePlayerMove remotePlayerMove)
    {
        this.remotePlayerMove = remotePlayerMove;
    }
    public override void Awake()
    {
        requestCode = RequestCode.Game;
        actionCode = ActionCode.TakeDamage;
        base.Awake();
    }
    public void SendDamageRequest(int damage)
    {
        SendRequest(damage.ToString());
    }
    public override void OnResponse(string data)
    {
        int health = int.Parse(data);
        remotePlayerMove.SyncRemoteHealth(health);
    }
}
