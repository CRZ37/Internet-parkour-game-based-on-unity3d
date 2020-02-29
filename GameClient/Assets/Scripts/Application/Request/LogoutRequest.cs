using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class LogoutRequest:BaseRequest
{
    private RoomListPanel roomListPanel;

    public override void Awake()
    {
        requestCode = RequestCode.User;
        actionCode = ActionCode.Logout;
        roomListPanel = GetComponent<RoomListPanel>();
        base.Awake();
    }
    public override void SendRequest()
    {
        string userId = Game.Instance.GetUserData().Id.ToString();
        SendRequest(userId);
    }
    public override void OnResponse(string data)
    {
        ReturnCode returnCode = (ReturnCode)int.Parse(data);
        roomListPanel.OnLogoutResponse(returnCode);
    }
}
