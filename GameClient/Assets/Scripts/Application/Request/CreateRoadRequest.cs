using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class CreateRoadRequest : BaseRequest
{
    private RoadChange roadChange;
    public override void Awake()
    {
        requestCode = RequestCode.Game;
        actionCode = ActionCode.CreateRoadAndItem;
        roadChange = GetComponent<RoadChange>();
        base.Awake();
    }

    public override void SendRequest()
    {
        SendRequest("CreateRoadAndItem");
    }

    public override void OnResponse(string data)
    {
        string[] strs = data.Split(',');
        int roadIndex = int.Parse(strs[0]);
        int itemIndex = int.Parse(strs[1]);
        roadChange.SpawnNewRoadAndItemSync(roadIndex,itemIndex);
    }
}
