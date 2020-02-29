using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class QuitBattleRequest : BaseRequest
{
    GamePanel gamePanel;
    public override void Awake()
    {
        requestCode = RequestCode.Game;
        actionCode = ActionCode.QuitBattle;
        gamePanel = GetComponent<GamePanel>();
        base.Awake();
    }
    public override void SendRequest()
    {
        SendRequest("QuitBattle");
    }
    public override void OnResponse(string data)
    {
        gamePanel.OnExitResponseSync();
    }
}
