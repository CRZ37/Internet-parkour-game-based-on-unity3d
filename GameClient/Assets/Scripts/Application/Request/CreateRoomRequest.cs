using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class CreateRoomRequest : BaseRequest
{
    private RoomPanel roomPanel;
    public override void Awake()
    {
        requestCode = RequestCode.Room;
        actionCode = ActionCode.CreateRoom;
        base.Awake();
    }
    public void SetPanel(BasePanel panel)
    {
        roomPanel = panel as RoomPanel;
    }
    //只需要有创建房间的请求，不需要带参数
    public override void SendRequest()
    {
        SendRequest("CreateRoom");
    }
    public override void OnResponse(string data)
    {
        string[] strs = data.Split(',');
        ReturnCode returnCode = (ReturnCode)int.Parse(strs[0]);
        Role_ResultRoleType roleType = (Role_ResultRoleType)int.Parse(strs[1]);
        Game.Instance.SetLocalRoleType(roleType);
        if(returnCode == ReturnCode.Success)
        {           
            roomPanel.SetLocalPlayerResSync();
        }
    }
}
