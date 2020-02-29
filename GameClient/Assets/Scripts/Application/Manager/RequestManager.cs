using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class RequestManager : BaseManager
{
    private Dictionary<ActionCode, BaseRequest> actionDict = new Dictionary<ActionCode, BaseRequest>();

    public void addRequest(ActionCode actionCode,BaseRequest request)
    {
        actionDict.Add(actionCode, request);
    }
    public void RemoveRequet(ActionCode actionCode)
    {
        actionDict.Remove(actionCode);
    }
    public void HandleResponse(ActionCode actionCode, string data)
    {
        BaseRequest request = actionDict.TryGet(actionCode);
        if(request == null)
        {
            Debug.LogWarning("无法得到这个ActionCode[" + actionCode + "]对应的Request类");
            return;
        }
        request.OnResponse(data);
    }
}
