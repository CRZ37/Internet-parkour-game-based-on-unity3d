using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class StartGameRequest : BaseRequest
{
    private RoomPanel roomPanel;
    public override void Awake()
    {
        requestCode = RequestCode.Game;
        actionCode = ActionCode.StartGame;
        roomPanel = GetComponent<RoomPanel>();
        base.Awake();
    }

    public override void SendRequest()
    {
        SendRequest("StartGame");
    }

    public override void OnResponse(string data)
    {        
        roomPanel.OnStartGameResponseSync(data);        
    }
}
