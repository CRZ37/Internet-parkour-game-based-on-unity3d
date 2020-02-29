using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class UpdateResultRequest : BaseRequest
{
    RoomListPanel roomListPanel;
    public override void Awake()
    {
        actionCode = ActionCode.UpdateResult;
        roomListPanel = GetComponent<RoomListPanel>();
        base.Awake();
    }

    public override void OnResponse(string data)
    {
        string[] strs = data.Split(',');
        int totalCount = int.Parse(strs[0]);
        int winCount = int.Parse(strs[1]);
        roomListPanel.OnUpdateResultResponseSync(totalCount,winCount);
    }
}
