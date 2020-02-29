using UnityEngine;
using Common;

public class GameOverRequest : BaseRequest
{
    GamePanel gamePanel;
    public override void Awake()
    {
        requestCode = RequestCode.Game;
        actionCode = ActionCode.GameOver;
        gamePanel = GetComponent<GamePanel>();
        base.Awake();
    }
    public void SendGameOverRes(string winnerName, int winnerScore,int winnerCoin,string loserName, int loserScore, int loserCoin, Role_ResultRoleType result)
    {
        //gamePannel负责解析
        SendRequest(winnerName + "|" + winnerScore + "|" + winnerCoin + "," +
                    loserName + "|" + loserScore + "|" + loserCoin + "," + 
                    ((int)result).ToString()); ;
    }
    public override void OnResponse(string data)
    {
        Debug.Log("游戏结束成功");
        gamePanel.OnGameOverResponseSync(data);
    }
}
