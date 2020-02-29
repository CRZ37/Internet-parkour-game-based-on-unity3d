using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class ListRoomRequest : BaseRequest
{
    private RoomListPanel roomListPanel;
    public override void Awake()
    {
        requestCode = RequestCode.Room;
        actionCode = ActionCode.ListRoom;
        roomListPanel = GetComponent<RoomListPanel>();
        base.Awake();
    }
    public override void SendRequest()
    {
        SendRequest("ListRequest");
    }
    public override void OnResponse(string data)
    {
        List<UserData> userList = new List<UserData>();
        if (data != "NoRoom")
        {
            string[] userDataArr = data.Split('|');
            foreach (string userData in userDataArr)
            {
                string[] strs = userData.Split(',');
                //只用作显示房间，不用玩家状态
                userList.Add(new UserData(int.Parse(strs[0]), strs[1], int.Parse(strs[2]), int.Parse(strs[3])));
            }
        }       
        roomListPanel.LoadRoomItemSync(userList);
    }
}
