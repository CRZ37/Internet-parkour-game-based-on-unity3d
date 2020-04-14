using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Common;
using System;

public class RoomListPanel : BasePanel
{
    private RectTransform panelBG;
    private RectTransform content;
    private VerticalLayoutGroup roomLayout;
    private GameObject roomItemPrefab;
    private List<UserData> userList = null;
    private ListRoomRequest listRoomRequest;
    private JoinRoomRequest joinRoomRequest;
    private CreateRoomRequest createRoomRequest;
    private LogoutRequest logoutRequest;
    private UserData user1 = null;
    private UserData user2 = null;
    private int totalCount;
    private int winCount;
    private bool isUpdateResult = false;
    private bool isPopPanel = false;
    private bool isJoinRoom = false;
    private GameObject coin;
    private Text coinNum;
    private void Awake()
    {
        coin = GameObject.Find("Canvas/BackGround/Middle").transform.Find("Coin").gameObject;
        coinNum = coin.transform.Find("CoinNum").GetComponent<Text>();
        panelBG = transform.GetComponent<RectTransform>();
        content = transform.Find("Content").GetComponent<RectTransform>();
        roomLayout = content.Find("RoomList/ScrollRect/ViewPort/Layout").GetComponent<VerticalLayoutGroup>();
        roomItemPrefab = Resources.Load("UIPanel/RoomItem") as GameObject;
        content.Find("CloseButton").GetComponent<Button>().onClick.AddListener(OnCloseClick);
        content.Find("CreatRoomButton").GetComponent<Button>().onClick.AddListener(OnCreateRoomClick);
        content.Find("RefreshButton").GetComponent<Button>().onClick.AddListener(OnRefreshClick);
        content.Find("ShopButton").GetComponent<Button>().onClick.AddListener(OnShopClick);
        content.Find("RoleShopButton").GetComponent<Button>().onClick.AddListener(OnRoleShopClick);
        listRoomRequest = GetComponent<ListRoomRequest>();
        createRoomRequest = GetComponent<CreateRoomRequest>();
        joinRoomRequest = GetComponent<JoinRoomRequest>();
        logoutRequest = GetComponent<LogoutRequest>();
    }

    private void OnRoleShopClick()
    {
        PausePanelPUNISHING(panelBG, content, UIPanelType.RoleShop, 120, 1500);
    }

    private void OnShopClick()
    {
        PausePanelPUNISHING(panelBG, content, UIPanelType.Shop,120,1700);
    }
    private void OnCloseClick()
    {
        logoutRequest.SendRequest();
    }
    private void SetRequestParam(BasePanel panel)
    {
        createRoomRequest.SetPanel(panel);
        createRoomRequest.SendRequest();
    }
    private void OnCreateRoomClick()
    {
        PausePanelPUNISHING(panelBG, content, UIPanelType.Room, SetRequestParam);
    }
    private void OnRefreshClick()
    {
        listRoomRequest.SendRequest();
    }
    public override void OnResume()
    {
        ResumePanelPUNISHING(panelBG, content, 830, 1700);
        Game.Instance.sound.PlayBg("RoomListPanel");
        //更新玩家信息
        listRoomRequest.SendRequest();
    }
    public override void OnEnter()
    {
        coin.SetActive(true);
        OpenPanelPUNISHING(panelBG, content, 830, 120, 1700);
        Game.Instance.sound.PlayBg("RoomListPanel");
        //设置玩家信息
        SetLocalBattleRes();
        listRoomRequest.SendRequest();
    }
    public override void OnExit()
    {
        coin.SetActive(false);
        base.OnExit();
    }
    private void SetLocalBattleRes()
    {
        UserData userData = Game.Instance.GetUserData();
        content.Find("BattleRes/Username").GetComponent<Text>().text = userData.Username;
        content.Find("BattleRes/TotalCount").GetComponent<Text>().text = "总场数：" + userData.TotalCount;
        content.Find("BattleRes/WinCount").GetComponent<Text>().text = "胜场：" + userData.WinCount;
        coinNum.text = userData.CoinNum.ToString();
    }
    public void OnUpdateResultResponseSync(int totalCount, int winCount)
    {
        this.totalCount = totalCount;
        this.winCount = winCount;
        isUpdateResult = true;
    }
    public void OnUpdateResultResponse()
    {
        Game.Instance.UpdateResult(totalCount, winCount);
        SetLocalBattleRes();
    }
    private void SetRoomPanel(BasePanel panel)
    {
        ((RoomPanel)panel).SetAllPlayerResSync(user1, user2);
    }
    private void Update()
    {
        if (userList != null)
        {
            LoadRoomItem(userList);
            userList = null;
        }
        if (isJoinRoom)
        {
            isJoinRoom = false;
            PausePanelPUNISHING(panelBG, content, UIPanelType.Room, SetRoomPanel);
        }
        if (isPopPanel)
        {
            ClosePanelPUNISHING(panelBG, content, 120, 1700);
            isPopPanel = false;
        }
        if (isUpdateResult)
        {
            OnUpdateResultResponse();
            isUpdateResult = false;
        }
    }
    public void LoadRoomItemSync(List<UserData> userList)
    {
        this.userList = userList;
        UIMng.ShowMessageSync("房间列表已刷新!");
    }
    private void LoadRoomItem(List<UserData> userList)
    {
        RoomItem[] roomItems = roomLayout.GetComponentsInChildren<RoomItem>();
        foreach (RoomItem item in roomItems)
        {
            item.DestroySelf();
        }
        int count = userList.Count;
        Vector3 normalSize = new Vector3(1, 1, 1);
        //创建房间列表
        for (int i = 0; i < count; i++)
        {
            GameObject roomItem = Instantiate(roomItemPrefab);
            roomItem.transform.SetParent(roomLayout.transform);
            roomItem.GetComponent<RectTransform>().localScale = normalSize;
            Debug.Log("创建房间");
            //设置房间信息
            roomItem.GetComponent<RoomItem>().SetRoomInfo(userList[i].Id, userList[i].Username, userList[i].TotalCount, userList[i].WinCount, this);
        }
        //设置layout高度
        int roomCount = GetComponentsInChildren<RoomItem>().Length;
        Vector2 size = roomLayout.GetComponent<RectTransform>().sizeDelta;
        roomLayout.GetComponent<RectTransform>().sizeDelta = new Vector2(size.x,
            roomCount * (roomItemPrefab.GetComponent<RectTransform>().sizeDelta.y + roomLayout.spacing));
    }
    //回调，其使用委托也行
    public void OnJoinClick(int id)
    {
        joinRoomRequest.SendRequest(id);
    }
    public void OnJoinResponse(ReturnCode returnCode, UserData user1, UserData user2)
    {
        switch (returnCode)
        {
            case ReturnCode.NotFound:
                UIMng.ShowMessageSync("房间被销毁，请刷新");
                break;
            case ReturnCode.Fail:
                UIMng.ShowMessageSync("房间已满，无法加入");
                break;
            case ReturnCode.Success:
                this.user1 = user1;
                this.user2 = user2;
                isJoinRoom = true;
                break;
        }
    }
    public void OnLogoutResponse(ReturnCode returnCode)
    {
        switch (returnCode)
        {
            case ReturnCode.Success:
                isPopPanel = true;
                UIMng.ShowMessageSync("登出账号成功");
                break;
            case ReturnCode.NotFound:
                UIMng.ShowMessageSync("登出账号失败，登录状态异常！");
                break;
        }
    }
}
