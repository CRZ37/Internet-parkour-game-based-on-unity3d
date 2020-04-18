using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Common;
using System;

struct RoleProperty
{
    public string rolePath;
    public int roleMultiplyCoinTime;
    public int roleInvincibleTime;

    public RoleProperty(string rolePath, int roleMultiplyCoinTime, int roleInvincibleTime)
    {
        this.rolePath = rolePath;
        this.roleMultiplyCoinTime = roleMultiplyCoinTime;
        this.roleInvincibleTime = roleInvincibleTime;
    }
}

public class PlayerManager : BaseManager
{
    //本地玩家的状态
    private GameObject hostPlayerState;
    private Text clientName;
    private Transform hostHealthTran;
    private Text hostCoinText;
    private Text hostItemText;
    //远程玩家的状态
    private GameObject clientPlayerState;
    private Text hostName;
    private Transform clientHealthTran;
    private Text clientCoinText;
    private Text clientItemText;
    //client在左边
    private Vector3 clientPos = new Vector3(-4.968f, 0.31f, 0.8f);
    //host在右边
    private Vector3 hostPos = new Vector3(4.968f, 0.31f, 0.8f);
    private Transform players;
    //private int hostHealth = -1;
    //private float hostSkillTime = -1;
    //private int clientHealth = -1;
    //private float clientSkillTime = -1;
    //private string hostRoleSelect = null;
    //private string clientRoleSelect = null;
    private UserData hostUser;
    private UserData clientUser;
    public UserData UserData { get; set; }
    public ShopData ShopData { get; set; }
    public GameData GameData { get; set; }
    private Dictionary<Role_ResultRoleType, RoleData> roleDataDict = new Dictionary<Role_ResultRoleType, RoleData>();
    private Dictionary<string, RoleProperty> rolePropertyDict = new Dictionary<string, RoleProperty>();
    private Role_ResultRoleType localRoleType;
    private GameObject remoteRoleGameObject;
    private GameObject gamePanelgo;
    private CamFollowPlayer camFollowPlayer;
    private bool isEnterPlaying = false;
    private bool canPlayPlayingBG = false;
    //初次跑道序号
    private int index1 = -1;
    private int index2 = -1;
    public PlayerManager()
    {
        rolePropertyDict.Add("1", new RoleProperty("Players/RoleMale", 0, 0));
        rolePropertyDict.Add("2", new RoleProperty("Players/RoleCop", 1, 1));
        rolePropertyDict.Add("3", new RoleProperty("Players/RoleRobot", 2, 2));
    }
    public override void Update()
    {
        if (isEnterPlaying)
        {
            EnterPlaying();
            isEnterPlaying = false;
        }
        if (canPlayPlayingBG)
        {
            Game.Instance.sound.PlayBg("GamePlaying");
            canPlayPlayingBG = false;
        }
    }
    public void SetPropertyShopState(int healthPrice, int bigHealthPrice, int skillTimePrice, int bigSkillTimePrice)
    {
        ShopData.SetPropertyShopState(healthPrice, bigHealthPrice, skillTimePrice, bigSkillTimePrice);
    }
    public void SetRoleBuyState(int roleMaleBuyState, int roleCopBuyState, int roleRobotBuyState)
    {
        UserData.SetRoleBuyState(roleMaleBuyState, roleCopBuyState, roleRobotBuyState);
    }
    public void SetRoleSelectState(string roleSelectState)
    {
        UserData.SetRoleSeclctState(roleSelectState);
    }
    public void UpdateResult(int totalCount, int winCount)
    {
        UserData.TotalCount = totalCount;
        UserData.WinCount = winCount;
    }
    public void UpdateCoin(int coinNum)
    {
        UserData.CoinNum = coinNum;
    }
    public void EnterPlayingSync(int index1, int index2)
    {
        isEnterPlaying = true;
        this.index1 = index1;
        this.index2 = index2;
    }
    public void GameStart()
    {
        //开始游戏
        GameData.IsPlay = true;
        canPlayPlayingBG = true;
    }
    private void EnterPlaying()
    {
        //生成角色
        SpawnRoles();
        //相机跟随
        camFollowPlayer.FollowRole();
    }
    public void SetLocalRoleType(Role_ResultRoleType type)
    {
        localRoleType = type;
    }

    public GameObject LocalRoleGameObject { get; set; }

    public override void OnInit()
    {
        //游戏初始状态：非暂停，未开始
        GameData = new GameData(false);

        players = GameObject.Find("Players").transform;
        camFollowPlayer = GameObject.Find("CameraAndOthers/Camera").GetComponent<CamFollowPlayer>();
        clientPlayerState = GameObject.Find("Canvas/BackGround/Left").transform.Find("ClientPlayerState").gameObject;
        hostPlayerState = GameObject.Find("Canvas/BackGround/Right").transform.Find("HostPlayerState").gameObject;
        hostName = hostPlayerState.transform.Find("HostName").GetComponent<Text>();
        hostHealthTran = hostPlayerState.transform.Find("Health");
        hostCoinText = hostPlayerState.transform.Find("Coin").GetComponent<Text>();
        hostItemText = hostPlayerState.transform.Find("State").GetComponent<Text>();

        clientName = clientPlayerState.transform.Find("ClientName").GetComponent<Text>();
        clientHealthTran = clientPlayerState.transform.Find("Health");
        clientCoinText = clientPlayerState.transform.Find("Coin").GetComponent<Text>();
        clientItemText = clientPlayerState.transform.Find("State").GetComponent<Text>();
    }

    private void InitRoleDataDict()
    {
        //获取另一个客户端的角色选择
        string hostRoleSelect = rolePropertyDict[hostUser.RoleSelectState].rolePath;
        string clientRoleSelect = rolePropertyDict[clientUser.RoleSelectState].rolePath;
        roleDataDict.Add(Role_ResultRoleType.Host, new RoleData(Role_ResultRoleType.Host, hostRoleSelect, hostPos, hostUser.Health, hostUser.SkillTime,rolePropertyDict[hostUser.RoleSelectState].roleMultiplyCoinTime, rolePropertyDict[hostUser.RoleSelectState].roleInvincibleTime));
        roleDataDict.Add(Role_ResultRoleType.Client, new RoleData(Role_ResultRoleType.Client, clientRoleSelect, clientPos, clientUser.Health, clientUser.SkillTime, rolePropertyDict[hostUser.RoleSelectState].roleMultiplyCoinTime, rolePropertyDict[hostUser.RoleSelectState].roleInvincibleTime));
    }
    //设置玩家的姓名
    public void SetPlayersData(UserData hostUser, UserData clientUser)
    {
        hostName.text = hostUser.Username;
        clientName.text = clientUser.Username;
        this.hostUser = hostUser;
        this.clientUser = clientUser;
    }
    //初始化角色时脚本添加顺序十分重要，为了防止空指针，不再使用生命周期中的Start或Awake，改为手动赋值。
    public void SpawnRoles()
    {
        gamePanelgo = GameObject.FindGameObjectWithTag(Tag.GamePanel);
        InitRoleDataDict();
        GamePanel gamePanel = gamePanelgo.GetComponent<GamePanel>();
        gamePanel.initNumandAnim();
        LocalMoveRequest localMoveRequest = gamePanelgo.GetComponent<LocalMoveRequest>();
        RemoteMoveRequest remoteMoveRequest = gamePanelgo.GetComponent<RemoteMoveRequest>();
        TakeDamageRequest takeDamageRequest = gamePanelgo.GetComponent<TakeDamageRequest>();
        GetCoinRequest getCoinRequest = gamePanelgo.GetComponent<GetCoinRequest>();
        GameOverRequest gameOverRequest = gamePanelgo.GetComponent<GameOverRequest>();
        UseItemRequest useItemRequest = gamePanelgo.GetComponent<UseItemRequest>();
        LocalPlayerMove localPlayerMove = null;
        RemotePlayerMove remotePlayerMove = null;

        //生成玩家的时候添加脚本，一定要注意添加脚本时的初始化操作,防止遗漏一条一条操作
        foreach (RoleData roleData in roleDataDict.Values)
        {
            GameObject go = GameObject.Instantiate(roleData.RolePrefab, roleData.SpawnPos, Quaternion.identity);
            //设置父物体
            go.transform.SetParent(players);
            if (roleData.Type == localRoleType)
            {
                //增强可读性
                LocalRoleGameObject = go;
                //生成角色的时候顺便先生成两条跑道
                RoadChange roadChange = LocalRoleGameObject.AddComponent<RoadChange>();
                //添加本地玩家控制脚本
                localPlayerMove = LocalRoleGameObject.AddComponent<LocalPlayerMove>();
                localMoveRequest.SetLocalPlayerMove(localPlayerMove);
                localPlayerMove.SetGameDataAndRoleDataAndRequests(
                    GameData,
                    roleData,
                    localMoveRequest,
                    takeDamageRequest,
                    getCoinRequest,
                    gameOverRequest,
                    useItemRequest);
                CreateRoadRequest createRoadRequest = LocalRoleGameObject.AddComponent<CreateRoadRequest>();
                roadChange.SetCreateRoadRequest(createRoadRequest, index1, index2);
                //设置UI信息
                switch (roleData.Type)
                {
                    case Role_ResultRoleType.Host:
                        localPlayerMove.SetLocalPlayerState(hostName.text, clientName.text, hostHealthTran, hostCoinText, hostItemText,gamePanel.MultiplyNum,gamePanel.InvincibleNum,gamePanel.MultiplyAnim,gamePanel.InvincibleAnim);
                        break;
                    case Role_ResultRoleType.Client:
                        localPlayerMove.SetLocalPlayerState(clientName.text, hostName.text, clientHealthTran, clientCoinText, clientItemText, gamePanel.MultiplyNum, gamePanel.InvincibleNum, gamePanel.MultiplyAnim, gamePanel.InvincibleAnim);
                        break;
                }
            }
            else
            {
                remoteRoleGameObject = go;
                //添加远程玩家同步脚本
                remotePlayerMove = remoteRoleGameObject.AddComponent<RemotePlayerMove>();
                remotePlayerMove.SetGameDataAndRoleData(GameData, roleData);
                remoteMoveRequest.SetRemotePlayerMove(remotePlayerMove);
                takeDamageRequest.SetRemotePlayerMove(remotePlayerMove);
                getCoinRequest.SetRemotePlayerMove(remotePlayerMove);
                useItemRequest.SetRemotePlayerMove(remotePlayerMove);
                //设置UI信息
                switch (roleData.Type)
                {
                    case Role_ResultRoleType.Host:
                        remotePlayerMove.SetRemotePlayerState(hostHealthTran, hostCoinText, hostItemText);
                        break;
                    case Role_ResultRoleType.Client:
                        remotePlayerMove.SetRemotePlayerState(clientHealthTran, clientCoinText, clientItemText);
                        break;
                }
            }
        }
        //其它初始化操作
        localPlayerMove.SetRemotePlayerMove(remoteRoleGameObject.GetComponent<RemotePlayerMove>());
        remotePlayerMove.SetLocalPlayerMove(LocalRoleGameObject.GetComponent<LocalPlayerMove>());
    }
    public void DestroyRoles()
    {
        //相机复位
        camFollowPlayer.transform.localPosition = new Vector3(0, 8.97f, -8.73f);
        camFollowPlayer.StopFollow();
        GameObject.Destroy(LocalRoleGameObject);
        GameObject.Destroy(remoteRoleGameObject);
        //这里设置false是给中途就退出用的
        GameData.IsPlay = false;
        //清空dict
        roleDataDict.Clear();
    }
}
