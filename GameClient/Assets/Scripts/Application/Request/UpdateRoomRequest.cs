using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class UpdateRoomRequest : BaseRequest
{
    private RoomPanel roomPanel;
    public override void Awake()
    {
        requestCode = RequestCode.Room;
        actionCode = ActionCode.UpdateRoom;
        roomPanel = GetComponent<RoomPanel>();
        base.Awake();
    }
    public override void OnResponse(string data)
    {
        
        UserData user1 = null;
        UserData user2 = null;
        string[] userDataArray = data.Split('|');
        user1 = new UserData(userDataArray[0]);
        if(userDataArray.Length > 1)
        {
            //如果有多于一个玩家在房间内，设置第二位玩家的信息
            user2 = new UserData(userDataArray[1]);
        }      
        roomPanel.SetAllPlayerResSync(user1, user2);
    }
}
