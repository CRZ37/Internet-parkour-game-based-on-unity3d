using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using System;

public class LocalMoveRequest : BaseRequest
{
    private LocalPlayerMove localPlayerMove;
    private MoveDirection moveDirection = MoveDirection.Null;
    public void SetLocalPlayerMove(LocalPlayerMove localPlayerMove)
    {
        this.localPlayerMove = localPlayerMove;
    }
    public override void Awake()
    {
        requestCode = RequestCode.Game;
        actionCode = ActionCode.LocalMove;       
        base.Awake();
    }
    public void SendRequest(MoveDirection direction)
    {
        //发送要移动的方位
        SendRequest(((int)direction).ToString());
    }
    public override void OnResponse(string data)
    {
        moveDirection = (MoveDirection)int.Parse(data);
        localPlayerMove.SyncLocalPlayer(moveDirection);
    }
}
