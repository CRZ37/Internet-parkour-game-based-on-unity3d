using Common;
using UnityEngine;

public class LoginRequest : BaseRequest
{
    private LoginPanel loginPannel;

    public override void Awake()
    {
        requestCode = RequestCode.User;
        actionCode = ActionCode.Login;
        loginPannel = GetComponent<LoginPanel>();
        base.Awake();
    }

    public void SendRequest(string username,string password)
    {
        string data = username + "," + password;
        SendRequest(data);
    }
    public override void OnResponse(string data)
    {
        string[] strs = data.Split(',');
        ReturnCode returnCode = (ReturnCode)int.Parse(strs[0]);
        loginPannel.OnLoginResponseSync(returnCode);
        Debug.Log(returnCode);
        //把数据保存到playerManager里面
        if (returnCode == ReturnCode.Success)
        {
            int id = int.Parse(strs[1]);
            string username = strs[2];
            int totalCount = int.Parse(strs[3]);
            int winCount = int.Parse(strs[4]);
            int coinNum = int.Parse(strs[5]);
            int health = int.Parse(strs[6]);
            float skillTime = float.Parse(strs[7]);
            UserData userData = new UserData(id,username, totalCount, winCount,coinNum,health,skillTime);
            Game.Instance.SetUserData(userData);
        }
    }
}
