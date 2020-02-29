using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class BaseRequest : MonoBehaviour
{
    protected RequestCode requestCode = RequestCode.None;
    protected ActionCode actionCode = ActionCode.None;
    // Start is called before the first frame update
    public virtual void Awake()
    {
        Game.Instance.AddRequest(actionCode, this);
    }
    //发起请求
    public virtual void SendRequest()
    {

    }
    //接收数据
    public virtual void OnResponse(string data)
    {

    }

    public virtual void OnDestroy()
    {
        if(Game.Instance != null)
        {
            Game.Instance.RemoveRequest(actionCode);
        }       
    }

    protected void SendRequest(string data)
    {
        Game.Instance.SendRequest(requestCode, actionCode, data);
        Debug.Log(requestCode.ToString() + " " + actionCode.ToString());
    }
}
