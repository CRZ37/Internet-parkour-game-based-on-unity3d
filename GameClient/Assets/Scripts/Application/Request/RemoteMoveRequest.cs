using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class RemoteMoveRequest : BaseRequest
{
    //只负责接收
    private RemotePlayerMove remotePlayerMove;
    private MoveDirection moveDirection = MoveDirection.Null;
    public override void Awake()
    {
        actionCode = ActionCode.RemoteMove;
        base.Awake();
    }
    public void SetRemotePlayerMove(RemotePlayerMove remotePlayerMove)
    {
        this.remotePlayerMove = remotePlayerMove;
    }
    public override void OnResponse(string data)
    {
        moveDirection = (MoveDirection)int.Parse(data);
        remotePlayerMove.SyncRemotePlayer(moveDirection);
    }
}
