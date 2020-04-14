using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Common;

public class LocalPlayerMove : MonoBehaviour
{
    private string multiTip = "";
    private string invincibleTip = "";
    private Image HPBar;
    //需要进行同步的request
    private LocalMoveRequest moveRequest;
    private TakeDamageRequest takeDamageRequest;
    private GameOverRequest gameOverRequest;
    private GetCoinRequest getCoinRequest;
    private RemotePlayerMove remotePlayerMove;
    //本地玩家的状态
    private string localPlayerName;
    private string remotePlayerName;
    private Transform localHealth;
    private Text localCoinText;
    private Text localItemText;
    //本局游戏金币数
    private int localCoin = 0;
    //重力
    private const float gravity = 9.8f;
    //跳跃高度的值
    private const float jumpValue = 3;
    //横向移动精度
    private const int targetCount = 10;
    //玩家的速度要不断加快
    private const float speedAddDis = 200;
    private const float speedAddRate = 0.5f;
    private const float maxSpeed = 32;
    private const float hitLimit = 0.2f;

    public float speed = 15;
    private CharacterController cc;
    private MoveDirection inputDir = MoveDirection.Null;
    //位置序号，1-6
    private int nowIndex = -1;
    private int targetIndex = -1;
    private float xDis;
    //横向移动速度
    //float xMoveSpeed = 10;
    private int count = 0;
    //跳跃
    private float yDis = 0;
    //玩家的速度要不断地加快
    private float speedAddCount = 0;
    private GameData gameData;
    private RoleData roleData;
    //玩家动画机
    private Animator anim;
    //现在是否正在被撞击
    private bool isHit = false;

    //item相关
    private int multiTime = 1;
    private float skillTime;
    private int health;
    private int maxHealth;
    private bool isInvincible = false;
    //双倍金币协程
    private IEnumerator MultiplyCor;
    //无敌协程
    private IEnumerator InvincibleCor;

    private bool canGameOver = true;
    //移动不能中断
    private bool isMoving = false;

    public float Speed
    {
        get => speed;
        set
        {
            speed = value;
            if (speed > maxSpeed)
            {
                speed = maxSpeed;
            }
        }
    }

    public int NowIndex { get => nowIndex; set => nowIndex = value; }

    public void SetRemotePlayerMove(RemotePlayerMove remotePlayerMove)
    {
        this.remotePlayerMove = remotePlayerMove;
    }
    public void SetGameDataAndRoleDataAndRequests(GameData gameData, RoleData roleData, LocalMoveRequest moveRequest, TakeDamageRequest takeDamageRequest, GetCoinRequest getCoinRequest, GameOverRequest gameOverRequest)
    {
        this.gameData = gameData;
        this.roleData = roleData;
        this.moveRequest = moveRequest;
        this.getCoinRequest = getCoinRequest;
        this.takeDamageRequest = takeDamageRequest;
        this.gameOverRequest = gameOverRequest;
        switch (roleData.Type)
        {
            case Role_ResultRoleType.Client:
                nowIndex = 2;
                targetIndex = 2;
                break;
            case Role_ResultRoleType.Host:
                nowIndex = 5;
                targetIndex = 5;
                break;
        }
    }
    public void SetLocalPlayerState(string localPlayerName, string remotePlayerName, Transform localHealth, Text localCoinText, Text localItemText)
    {
        this.localPlayerName = localPlayerName;
        this.remotePlayerName = remotePlayerName;
        this.localHealth = localHealth;
        this.localCoinText = localCoinText;
        this.localItemText = localItemText;
        HPBar = this.localHealth.Find("HPBar").GetComponent<Image>();
    }
    private void Start()
    {
        cc = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        skillTime = roleData.SkillTime;
        health = roleData.Health;
        Debug.Log(health);
        maxHealth = health;
        StartCoroutine(UpdateAction());
    }
    private void PlayJump()
    {
        anim.SetBool("canJump", true);
    }
    private void GoLeft()
    {
        anim.SetBool("canLeft", true);
    }
    private void GoRight()
    {
        anim.SetBool("canRight", true);
    }
    private void Roll()
    {
        anim.SetBool("canRoll", true);
    }
    //更新位置
    IEnumerator UpdateAction()
    {
        while (true)
        {
            if (gameData.IsPlay)
            {
                yDis -= gravity * Time.deltaTime;
                cc.Move((transform.forward * Speed + new Vector3(0, yDis, 0)) * Time.deltaTime);
                MoveControl();
                GetDirection();
                UpdatePosition();
                UpdateSpeed();
                //这里加一个X限定，确保如果移动操作过快，在还没有移动到目标位置的时候，主角不会移出跑道
                if (transform.position.x >= Consts.right3)
                {
                    transform.position = new Vector3(Consts.right3, transform.position.y, transform.position.z);
                    nowIndex = 6;
                }
                if (transform.position.x <= Consts.left3)
                {
                    transform.position = new Vector3(Consts.left3, transform.position.y, transform.position.z);
                    nowIndex = 1;
                }
            }
            //每帧循环一次
            yield return 0;
        }
    }

    //获取玩家输入的方向
    private void GetDirection()
    {
        if (!isMoving)
        {
            //键盘识别方向,这儿就发送信息
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                moveRequest.SendRequest(MoveDirection.Up);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                moveRequest.SendRequest(MoveDirection.Left);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                moveRequest.SendRequest(MoveDirection.Right);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                moveRequest.SendRequest(MoveDirection.Down);
            }
        }
    }
    public void SyncLocalPlayer(MoveDirection inputDir)
    {
        this.inputDir = inputDir;
    }
    //TODO:这里在onresponse中调用，更新位置,
    private void UpdatePosition()
    {
        switch (inputDir)
        {
            case MoveDirection.Null:
                break;
            case MoveDirection.Up:
                //如果在地面上时才进行跳跃
                if (cc.isGrounded)
                {
                    moveRequest.SendRequest(inputDir);
                    yDis = jumpValue;
                    //播放跳跃音效
                    Game.Instance.sound.PlayEffect("jump");
                    //播放跳跃动画
                    AnimManager(inputDir);
                }
                break;
            case MoveDirection.Down:
                //播放翻滚动画
                AnimManager(inputDir);
                //摩擦地面音效
                Game.Instance.sound.PlayEffect("rubground");
                break;
            case MoveDirection.Left:
                isMoving = true;
                targetIndex--;
                xDis = -3.312f;
                //摩擦地面音效
                Game.Instance.sound.PlayEffect("rubground");
                //播放向左的动画
                AnimManager(inputDir);
                break;
            case MoveDirection.Right:
                isMoving = true;
                targetIndex++;
                xDis = 3.312f;
                //摩擦地面音效
                Game.Instance.sound.PlayEffect("rubground");
                //播放向右的动画
                AnimManager(inputDir);
                break;
            default:
                break;
        }
        //在移动之后要置空方向
        inputDir = MoveDirection.Null;
    }

    //判断当前是否需要移动
    void MoveControl()
    {
        if (nowIndex != targetIndex)
        {
            /*float move = xDis * 0.3f;
            float x = transform.position.x + move;
            transform.position = new Vector3(x,
                transform.position.y,
                transform.position.z);
            xDis -= move;
            Debug.Log(xDis + "," + move + "," + transform.position);*/

            //分帧移动过去
            float move = xDis / targetCount;
            transform.position += new Vector3(move, 0, 0);
            count++;

            //if (Mathf.Abs(xDis) < 0.005f)

            if (count == targetCount)
            {
                isMoving = false;
                xDis = 0;
                count = 0;
                nowIndex = targetIndex;

                switch (nowIndex)
                {
                    case 1:
                        transform.localPosition = new Vector3(Consts.left3, transform.position.y, transform.position.z);
                        break;
                    case 2:
                        transform.localPosition = new Vector3(Consts.left2, transform.position.y, transform.position.z);
                        break;
                    case 3:
                        transform.localPosition = new Vector3(Consts.left1, transform.position.y, transform.position.z);
                        break;
                    case 4:
                        transform.localPosition = new Vector3(Consts.right1, transform.position.y, transform.position.z);
                        break;
                    case 5:
                        transform.localPosition = new Vector3(Consts.right2, transform.position.y, transform.position.z);
                        break;
                    case 6:
                        transform.localPosition = new Vector3(Consts.right3, transform.position.y, transform.position.z);
                        break;
                }
                Debug.Log("当前x位置：" + transform.localPosition.x);
            }
        }
    }

    //更新玩家速度
    void UpdateSpeed()
    {
        speedAddCount += Speed * Time.deltaTime;
        if (speedAddCount > speedAddDis)
        {
            speedAddCount = 0;
            Speed = Speed + speedAddRate;
        }
    }

    //碰撞CD
    public void HitObstacles(int damage)
    {
        //如果是已经在减速状态或者正在无敌状态就不再cd
        if (isHit || isInvincible)
        {
            return;
        }
        health -= damage;
        takeDamageRequest.SendDamageRequest(health);
        //CD协程
        StartCoroutine(HitCD());
    }

    //CD协程
    IEnumerator HitCD()
    {
        isHit = true;
        yield return new WaitForSeconds(hitLimit);
        isHit = false;
    }

    //吃金币
    public void HitCoin()
    {
        Debug.Log("local吃到了金币！");
        localCoin += multiTime;
        getCoinRequest.SendCoinRequest(localCoin);
    }

    //吃双倍金币
    public void HitMultiply()
    {
        Debug.Log("双倍金币！！！");
        //如果在双倍金币时间结束前又吃到一个，就刷新技能时间，就是先停下现在的再开一个新的
        if (MultiplyCor != null)
        {
            StopCoroutine(MultiplyCor);
        }
        MultiplyCor = MultiplyCoroutine();
        StartCoroutine(MultiplyCor);
    }

    //双倍金币协程
    IEnumerator MultiplyCoroutine()
    {
        multiTip = "双倍金币";
        multiTime = 2;
        yield return new WaitForSeconds(skillTime);
        multiTip = "";
        multiTime = 1;
    }

    //无敌状态
    public void HitInvincible()
    {
        Debug.Log("无敌状态！！！！！！！！");
        //如果在双倍金币时间结束前又吃到一个，就刷新技能时间，就是先停下现在的再开一个新的
        if (InvincibleCor != null)
        {
            Debug.Log("无敌状态刷新！！！！！！！！");
            StopCoroutine(InvincibleCor);
        }
        InvincibleCor = InvincibleCoroutine();
        StartCoroutine(InvincibleCor);
    }
    //无敌协程
    IEnumerator InvincibleCoroutine()
    {
        invincibleTip = "无敌";
        isInvincible = true;
        yield return new WaitForSeconds(skillTime);
        invincibleTip = "";
        isInvincible = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == Tag.Dodge_Fence)
        {
            //如果撞到应该闪过去的障碍物，视为失败
            other.gameObject.SendMessage("HitPlayer", transform.position);
            //减速
            HitObstacles(1);
            //播放撞到障碍物的声音
            Game.Instance.sound.PlayEffect("HitObstacles");
        }
        else if (other.gameObject.tag == Tag.Jump_Fence)
        {
            //如果在跳跃状态下撞到本应该跳过去的障碍物，视为通过
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("jump"))
            {
                return;
            }
            //否则减速
            other.gameObject.SendMessage("HitPlayer", transform.position);
            HitObstacles(1);
            //播放撞到障碍物的声音
            Game.Instance.sound.PlayEffect("HitObstacles");
        }
        else if (other.gameObject.tag == Tag.Roll_Fence)
        {
            //如果在翻滚的状态下撞到本应该滚过去的障碍物，视为通过
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("roll"))
            {
                return;
            }
            //否则减速  
            other.gameObject.SendMessage("HitPlayer", transform.position);
            HitObstacles(1);
            //播放撞到障碍物的声音
            Game.Instance.sound.PlayEffect("HitObstacles");
        }
        else if (other.gameObject.tag == Tag.Block)
        {
            other.gameObject.SendMessage("HitPlayer", transform.position);
            HitObstacles(2);
            //播放撞到障碍物的声音
            Game.Instance.sound.PlayEffect("HitObstacles");
        }
        else if (other.gameObject.tag == Tag.Small_Block)
        {
            other.transform.parent.parent.gameObject.SendMessage("HitPlayer", transform.position);
            HitObstacles(1);
            //播放撞到障碍物的声音
            Game.Instance.sound.PlayEffect("HitObstacles");
        }
        else if (other.gameObject.tag == Tag.Before_Trigger)
        {
            //让车/人走
            other.transform.parent.gameObject.SendMessage("HitTrigger", SendMessageOptions.DontRequireReceiver);
        }
        else if (other.gameObject.tag == Tag.People)
        {
            //播放人跌倒的动画,减速
            other.gameObject.SendMessage("HitPlayer", transform.position);
            HitObstacles(2);
            //播放撞到障碍物的声音
            Game.Instance.sound.PlayEffect("HitObstacles");
        }
    }
    private void AnimManager(MoveDirection dir)
    {
        switch (dir)
        {
            case MoveDirection.Null:
                break;
            case MoveDirection.Up:
                PlayJump();
                break;
            case MoveDirection.Down:
                Roll();
                break;
            case MoveDirection.Left:
                GoLeft();
                break;
            case MoveDirection.Right:
                GoRight();
                break;
            default:
                break;
        }
    }
    private void Update()
    {
        if (gameData.IsPlay)
        {
            anim.enabled = true;
        }
        else
        {
            anim.enabled = false;
        }
        //每个客户端都只判断local的血量是否小于0即可,只发送一次
        if (health <= 0 && canGameOver)
        {
            //播放游戏结束音效
            Game.Instance.sound.PlayEffect("GameOver");
            gameData.IsPlay = false;
            //停止游戏后向服务器发送游戏结束的消息，并显示游戏结算面板，并在另一端也显示游戏结算面板，也可以减少服务器计算压力
            int localScore = localCoin;
            int remoteScore = remotePlayerMove.GetScore();
            if (localScore > remoteScore)
            {
                //local玩家赢了
                gameOverRequest.SendGameOverRes(localPlayerName, localScore, localCoin, remotePlayerName, remoteScore, remotePlayerMove.RemoteCoin, roleData.Type);
            }
            else if (remoteScore > localScore)
            {
                //remote玩家赢了
                gameOverRequest.SendGameOverRes(remotePlayerName, remoteScore, remotePlayerMove.RemoteCoin, localPlayerName, localScore, localCoin, remotePlayerMove.RoleData.Type);
            }
            else
            {
                //平手
                gameOverRequest.SendGameOverRes(localPlayerName, localScore, localCoin, remotePlayerName, remoteScore, remotePlayerMove.RemoteCoin, Role_ResultRoleType.Tie);
            }
            canGameOver = false;
            //顺序：胜利者昵称，胜利者分数，失败者昵称，失败者分数，赢了/输了/平手
        }
        //每一帧更新玩家信息
        localCoinText.text = "金币数：" + localCoin;
        localItemText.text = "状态：" + multiTip + invincibleTip;
        HPBar.fillAmount = health / (float)maxHealth;
        //如果在跳跃状态，跳跃设为false
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("jump"))
        {
            anim.SetBool("canJump", false);
        }
        else if (anim.GetCurrentAnimatorStateInfo(0).IsName("roll"))
        {
            anim.SetBool("canRoll", false);
        }
        else if (anim.GetCurrentAnimatorStateInfo(0).IsName("left"))
        {
            anim.SetBool("canLeft", false);
        }
        else if (anim.GetCurrentAnimatorStateInfo(0).IsName("right"))
        {
            anim.SetBool("canRight", false);
        }
    }
}
