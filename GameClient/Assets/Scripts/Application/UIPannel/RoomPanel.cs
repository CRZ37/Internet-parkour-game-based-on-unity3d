using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Common;
using DG.Tweening;

public class RoomPanel : BasePanel
{
    private RectTransform bluePanel;
    private RectTransform redPanel;
    private Text hostPlayerUsername;
    private Text hostPlayerTotalCount;
    private Text hostPlayerWinCount;

    private Text clientPlayerUsername;
    private Text clientPlayerTotalCount;
    private Text clientPlayerWinCount;

    private UserData hostPlayer = null;
    private UserData hostUser;
    private UserData clientUser;

    private QuitRoomRequest quitRoomRequest;
    private StartGameRequest startGameRequest;
    private bool isPopPanel = false;
    private void Awake()
    {
        bluePanel = transform.Find("BluePanel").GetComponent<RectTransform>();
        redPanel = transform.Find("RedPanel").GetComponent<RectTransform>();
        hostPlayerUsername = transform.Find("BluePanel/Username").GetComponent<Text>();
        hostPlayerTotalCount = transform.Find("BluePanel/TotalCount").GetComponent<Text>();
        hostPlayerWinCount = transform.Find("BluePanel/WinCount").GetComponent<Text>();
        clientPlayerUsername = transform.Find("RedPanel/Username").GetComponent<Text>();
        clientPlayerTotalCount = transform.Find("RedPanel/TotalCount").GetComponent<Text>();
        clientPlayerWinCount = transform.Find("RedPanel/WinCount").GetComponent<Text>();
        quitRoomRequest = GetComponent<QuitRoomRequest>();
        startGameRequest = GetComponent<StartGameRequest>();

        transform.Find("StartButton").GetComponent<Button>().onClick.AddListener(OnStartClick);
        transform.Find("ExitButton").GetComponent<Button>().onClick.AddListener(OnExitClick);
    }
    private void Update()
    {
        //创建房间的时候
        if (hostPlayer != null)
        {
            SetHostPlayerRes(hostPlayer.Username, hostPlayer.TotalCount, hostPlayer.WinCount);
            ClearClientPlayerRes();
            hostPlayer = null;
        }
        //两个人加进来的时候，服务器会返回两个用户信息；当一个用户退出房间时，就只会返回房主的信息
        if (hostUser != null)
        {
            SetHostPlayerRes(hostUser.Username, hostUser.TotalCount, hostUser.WinCount);
            if (clientUser != null)
            {
                SetClientPlayerRes(clientUser.Username, clientUser.TotalCount, clientUser.WinCount);
                //有两个玩家加入后即可先设置好昵称,顺序：先host再client
                Game.Instance.SetPlayersName(hostUser, clientUser);
            }
            else
            {
                ClearClientPlayerRes();
                //不必清空昵称，人数不够不会开始
            }
            hostUser = null;
            clientUser = null;
        }
        if (isPopPanel)
        {
            UIMng.PopPanel();
            isPopPanel = false;
        }
    }
    public void SetLocalPlayerResSync()
    {
        hostPlayer = Game.Instance.GetUserData();
    }
    public void SetAllPlayerResSync(UserData hostUser, UserData clientUser)
    {
        this.hostUser = hostUser;
        this.clientUser = clientUser;
    }
    public void SetHostPlayerRes(string username, int totalCount, int winCount)
    {
        hostPlayerUsername.text = username;
        hostPlayerTotalCount.text = "总场数：" + totalCount;
        hostPlayerWinCount.text = "胜场：" + winCount;
    }

    public void SetClientPlayerRes(string username, int totalCount, int winCount)
    {
        clientPlayerUsername.text = username;
        clientPlayerTotalCount.text = "总场数：" + totalCount;
        clientPlayerWinCount.text = "胜场：" + winCount;
    }
    public void ClearClientPlayerRes()
    {
        clientPlayerUsername.text = "等待中...";
        clientPlayerTotalCount.text = "等待中...";
        clientPlayerWinCount.text = "等待中...";
    }
    private void OnStartClick()
    {
        startGameRequest.SendRequest();
    }
    private void OnExitClick()
    {
        quitRoomRequest.SendRequest();
    }
    public void OnExitResponse()
    {
        isPopPanel = true;
    }

    public void OnStartGameResponseSync(string data)
    {
        string[] strs = data.Split(',');
        ReturnCode returnCode = (ReturnCode)int.Parse(strs[0]);
        if (returnCode == ReturnCode.Success)
        {
            int index1 = int.Parse(strs[1]);
            int index2 = int.Parse(strs[2]);
            BGAnim.ShowPopAnimSync();
            UIMng.PushPanelSync(UIPanelType.Game);
            Game.Instance.EnterPlayingSync(index1, index2);
        }
        else
        {
            switch (returnCode)
            {
                case ReturnCode.Fail:
                    UIMng.ShowMessageSync("非房主无法开始游戏");
                    break;
                case ReturnCode.Lack:
                    UIMng.ShowMessageSync("人数不足无法开始游戏");
                    break;
            }
        }
    }
    public override void OnEnter()
    {
        gameObject.SetActive(true);
        bluePanel.localPosition = new Vector3(-1250, 0, 0);
        redPanel.localPosition = new Vector3(1250, 0, 0);
        bluePanel.DOLocalMove(new Vector3(-330, 0, 0), 0.4f);
        redPanel.DOLocalMove(new Vector3(330, 0, 0), 0.4f);
    }
    public override void OnPause()
    {
        gameObject.SetActive(false);
    }
    public override void OnResume()
    {
        gameObject.SetActive(true);
    }
}
