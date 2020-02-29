using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class RegisterRequest : BaseRequest
{
    private RegisterPanel registerPannel;
    public override void Awake()
    {
        requestCode = RequestCode.User;
        actionCode = ActionCode.Register;
        registerPannel = GetComponent<RegisterPanel>();
        base.Awake();
    }
    public void SendRequest(string username, string password)
    {
        string data = username + "," + password;
        SendRequest(data);
    }
    public override void OnResponse(string data)
    {
        ReturnCode returnCode = (ReturnCode)int.Parse(data);
        registerPannel.OnRegisterResponse(returnCode);
    }
}
