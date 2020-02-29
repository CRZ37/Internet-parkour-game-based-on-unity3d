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
    private void Update()
    {
        UpdateManager();
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
    public UserData GetUserData()
    {
        return playerMng.UserData;
    }
    public void SetLocalRoleType(Role_ResultRoleType type)
    {
        playerMng.SetLocalRoleType(type);
    }
    public GameObject GetCurrentRoleGameObject()
    {
        return playerMng.LocalRoleGameObject;
    }
    public void EnterPlayingSync(int index1,int index2, int hostHealth,float hostSkillTime,int clientHealth,float clientSkillTime)
    {
        playerMng.EnterPlayingSync(index1,index2,hostHealth,hostSkillTime,clientHealth,clientSkillTime);
    }
    public void StartPlayingSync()
    {
        playerMng.GameStart();
    }
    public void SetPlayersName(string hostName, string clientName)
    {
        playerMng.SetPlayersName(hostName, clientName);
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
    public void SetHealthState(int time)
    {
        playerMng.SetShopState(time, GetUserData().BigHealthTime, GetUserData().SkillTimeTime, GetUserData().BigSkillTimeTime);
    }
    public void SetBigHealthState(int time)
    {
        playerMng.SetShopState(GetUserData().HealthTime, time, GetUserData().SkillTimeTime, GetUserData().BigSkillTimeTime);
    }
    public void SetSkillState(int time)
    {
        playerMng.SetShopState(GetUserData().HealthTime, GetUserData().BigHealthTime, time, GetUserData().BigSkillTimeTime);
    }
    public void SetBigSkillState(int time)
    {
        playerMng.SetShopState(GetUserData().HealthTime, GetUserData().BigHealthTime, GetUserData().SkillTimeTime, time);
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
