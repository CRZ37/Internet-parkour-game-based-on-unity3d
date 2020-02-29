using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class StartPlayRequest : BaseRequest
{
    public override void Awake()
    {
        actionCode = ActionCode.StartPlay;
        base.Awake();
    }
    public override void OnResponse(string data)
    {
        Game.Instance.StartPlayingSync();      
    }
}
