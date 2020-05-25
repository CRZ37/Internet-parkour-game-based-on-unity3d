using System.Collections;
using UnityEngine;
using Common;

//使用了一部分单例模式也就是随时通过类获取唯一单例，单例模板没有写已存在单例就销毁此单例是因为脚本一直挂载到一个物体上，也就是说只产生一个实例。
//在这里起到的实际作用只是省略了GameObject.Find...一堆东西
//缺点：增加耦合性
[RequireComponent(typeof(ObjectPool))]
[RequireComponent(typeof(Sound))]
public class Game : MonoSingleton<Game>
{
    //[HideInInspector]
    public ObjectPool objectPool;
    //[HideInInspector]
    public Sound sound;

    //各种Manager(不做成单例模式，单例越多会导致耦合性越高，除了单机部分逻辑之外都不用单例模式了,
    //各个manager之间的调用使用本类作为中介。)
    private UIManager UIMng;
    private PlayerManager playerMng;
    private RequestManager requestMng;
    private ClientManager clientMng;  
    protected override void Awake()
    {
        base.Awake();
        objectPool = GetComponent<ObjectPool>();
        sound = GetComponent<Sound>();       
        InitManagers();        
    }
    public void InitManagers()
    {
        UIMng = new UIManager();
        playerMng = new PlayerManager();
        requestMng = new RequestManager();
        clientMng = new ClientManager();

        UIMng.OnInit();
        playerMng.OnInit();
        requestMng.OnInit();
        clientMng.OnInit();
    }
    /// <summary>
    /// 统一管理脚本的Update
    /// </summary>
    private void Update()
    {
        UpdateManager();
    }  
    private void UpdateManager()
    {
        UIMng.Update();
        playerMng.Update();
        requestMng.Update();
        clientMng.Update();
    }
    private void OnDestroy()
    {
        DestroyManagers();
    }
    //在需要销毁的时候调用
    public void DestroyManagers()
    {
        UIMng.OnDestroy();
        playerMng.OnDestroy();
        requestMng.OnDestroy();
        clientMng.OnDestroy();
    }

    public void AddRequest(ActionCode actionCode, BaseRequest request)
    {
        requestMng.addRequest(actionCode, request);
    }
    public void RemoveRequest(ActionCode actionCode)
    {
        requestMng.RemoveRequet(actionCode);
    }
    public void HandleResponse(ActionCode actionCode, string data)
    {
        requestMng.HandleResponse(actionCode, data);
    }
    public void SendRequest(RequestCode requestCode, ActionCode actionCode, string data)
    {
        clientMng.SendRequest(requestCode, actionCode, data);
    }
    public void SetUserData(UserData userData)
    {
        playerMng.UserData = userData;
    }
    public GameData GetGameData()
    {
        return playerMng.GameData;
    }
    public void SetGameData(GameData gameData)
    {
        playerMng.GameData = gameData;
    }
    public UserData GetUserData()
    {
        return playerMng.UserData;
    }
    public ShopData GetShopData()
    {
        Debug.Log(playerMng.ShopData);
        return playerMng.ShopData;
    }

    public void SetShopData(ShopData shopData)
    {
        playerMng.ShopData = shopData;
    }
    public void SetLocalRoleType(Role_ResultRoleType type)
    {
        playerMng.SetLocalRoleType(type);
    }
    public GameObject GetCurrentRoleGameObject()
    {
        return playerMng.LocalRoleGameObject;
    }
    public void EnterPlayingSync(int index1,int index2)
    {
        playerMng.EnterPlayingSync(index1,index2);
    }
    public void StartPlayingSync()
    {
        playerMng.GameStart();
    }
    public void SetPlayersName(UserData hostUser, UserData clientUser)
    {
        playerMng.SetPlayersData(hostUser, clientUser);
    }
    public void GameOver()
    {
        UIMng.BGAnim.ShowPushAnimSync();
        StartCoroutine(WaitForNextFrame());    
    }
    IEnumerator WaitForNextFrame()
    {
        //等0.1s再回收
        yield return new WaitForSeconds(0.1f);
        playerMng.DestroyRoles();
        objectPool.UnSpawnAll();
    }
    public void SetHealthPrice(int price)
    {
        playerMng.SetPropertyShopState(price, GetShopData().BigHealthPrice, GetShopData().SkillTimePrice, GetShopData().BigSkillTimePrice);
    }
    public void SetBigHealthPrice(int price)
    {
        playerMng.SetPropertyShopState(GetShopData().HealthPrice, price, GetShopData().SkillTimePrice, GetShopData().BigSkillTimePrice);
    }
    public void SetSkillPrice(int price)
    {
        playerMng.SetPropertyShopState(GetShopData().HealthPrice, GetShopData().BigHealthPrice, price, GetShopData().BigSkillTimePrice);
    }
    public void SetBigSkillPrice(int price)
    {
        playerMng.SetPropertyShopState(GetShopData().HealthPrice, GetShopData().BigHealthPrice, GetShopData().SkillTimePrice, price);
    }
    public void SetRoleSelectState(string roleSelectState)
    {
        playerMng.SetRoleSelectState(roleSelectState);
    }
    public void SetRoleMaleBuyState()
    {
        playerMng.SetRoleBuyState(1, GetUserData().RoleBuyState[1], GetUserData().RoleBuyState[2]);
    }
    public void SetRoleCopBuyState()
    {
        playerMng.SetRoleBuyState(GetUserData().RoleBuyState[0], 1, GetUserData().RoleBuyState[2]);
    }
    public void SetRoleRobotBuyState()
    {
        playerMng.SetRoleBuyState(GetUserData().RoleBuyState[0], GetUserData().RoleBuyState[1], 1);
    }
    public void UpdateResult(int totalCount, int winCount)
    {
        playerMng.UpdateResult(totalCount, winCount);
    }
    public void UpdateCoin(int coinNum)
    {
        playerMng.UpdateCoin(coinNum);
    }
}
