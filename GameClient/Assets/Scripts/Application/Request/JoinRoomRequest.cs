using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class JoinRoomRequest : BaseRequest
{
    private RoomListPanel roomListPanel;
    public override void Awake()
    {
        requestCode = RequestCode.Room;
        actionCode = ActionCode.JoinRoom;
        roomListPanel = GetComponent<RoomListPanel>();
        base.Awake();
    }
    public void SendRequest(int id)
    {
        SendRequest(id.ToString());
    }
    public override void OnResponse(string data)
    {
        string[] strs = data.Split('-');
        string[] strs2 = strs[0].Split(',');
        ReturnCode returnCode = (ReturnCode)int.Parse(strs2[0]);
        UserData user1 = null;
        UserData user2 = null;
        if(returnCode == ReturnCode.Success)
        {
            string[] userDataArray = strs[1].Split('|');
            user1 = new UserData(userDataArray[0]);
            user2 = new UserData(userDataArray[1]);
            Role_ResultRoleType roleType = (Role_ResultRoleType)int.Parse(strs2[1]);
            Game.Instance.SetLocalRoleType(roleType);
        }
        roomListPanel.OnJoinResponse(returnCode, user1, user2);
    }
}
