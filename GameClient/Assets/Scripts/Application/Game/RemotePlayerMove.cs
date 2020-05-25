using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Common;

public class RemotePlayerMove : MonoBehaviour
{
    private Image HPBar;
    private string multiTip = null;
    private string invincibleTip = null;
    private LocalPlayerMove localPlayerMove;
    //远程玩家的状态
    private Text remotePlayerName;
    private Transform remoteHealth;
    private Text remoteCoinText;
    private Text remoteItemText;
    //对方玩家的金币
    private int remoteCoin = 0;
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
    private const float hitLimit = 2;

    public float speed = 15;
    private CharacterController cc;
    private MoveDirection inputDir = MoveDirection.Null;
    //位置序号，1-6
    private int nowIndex = 2;
    private int targetIndex = 2;
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
    //item相关
    private float skillTime;
    private int health;
    private int maxHealth;
    //双倍金币协程
    private IEnumerator MultiplyCor;   
    //无敌协程
    private IEnumerator InvincibleCor;
    private bool canGameOver = true;
    //判断对方是否使用道具
    private bool useMultiply = false;
    private bool useInvincible = false;

    private Transform iPos;
    private Transform mPos;

    private Transform effectParent;
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
    public RoleData RoleData { get => roleData; set => roleData = value; }
    public int RemoteCoin { get => remoteCoin; set => remoteCoin = value; }

    public void SetLocalPlayerMove(LocalPlayerMove localPlayerMove)
    {
        this.localPlayerMove = localPlayerMove;
    }
    public void SyncRemotePlayer(MoveDirection inputDir)
    {
        this.inputDir = inputDir;
    }
    public void SyncRemoteHealth(int health)
    {
        this.health = health;
    }
    public void SyncRemoteCoin(int remoteCoin)
    {
        this.remoteCoin = remoteCoin;
    }
    public void SetGameDataAndRoleData(GameData gameData, RoleData roleData)
    {
        this.gameData = gameData;
        this.roleData = roleData;
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
    public void SetRemotePlayerState(Transform remoteHealth, Text remoteCoinText, Text remoteItemText)
    {
        this.remoteHealth = remoteHealth;
        this.remoteCoinText = remoteCoinText;
        this.remoteItemText = remoteItemText;
        HPBar = this.remoteHealth.Find("HPBar").GetComponent<Image>();
    }
    private void Start()
    {
        cc = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        skillTime = roleData.SkillTime;
        health = roleData.Health;
        maxHealth = health;
        iPos = transform.Find("IPos").transform;
        mPos = transform.Find("MPos").transform;
        effectParent = GameObject.Find("EffectParent").transform;
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
                UseItem();
                MoveControl();
                UpdatePosition();
                UpdateSpeed();
                //这里加一个X限定，确保如果移动操作过快，在还没有移动到目标位置的时候，主角不会移出跑道，其实服务器判断后本地就不用判断了
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
    public void UseItemSync(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.MultiplyCoin:
                useMultiply = true;
                break;
            case ItemType.Invincible:
                useInvincible = true;
                break;
        }
    }
    void UseItem()
    {
        if (useMultiply)
        {
            Debug.Log("另一方双倍金币");
            //如果在双倍金币时间结束前又吃到一个，就刷新技能时间，就是先停下现在的再开一个新的
            //if (MultiplyCor != null)
            //{
            //    StopCoroutine(MultiplyCor);
            //}
            MultiplyCor = MultiplyCoroutine();
            StartCoroutine(MultiplyCor);
        }
        if (useInvincible)
        {
            Debug.Log("另一方无敌状态");
            //如果在双倍金币时间结束前又吃到一个，就刷新技能时间，就是先停下现在的再开一个新的
            //if (InvincibleCor != null)
            //{
            //    StopCoroutine(InvincibleCor);
            //}
            InvincibleCor = InvincibleCoroutine();
            StartCoroutine(InvincibleCor);
        }
    }
    //更新位置
    void UpdatePosition()
    {
        switch (inputDir)
        {
            case MoveDirection.Null:
                break;
            case MoveDirection.Up:
                //如果在地面上时才进行跳跃
                if (cc.isGrounded)
                {
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
                //如果没有处于最左边则可以向左
                if (targetIndex > 1)
                {
                    targetIndex--;
                    xDis = -3.312f;
                    //摩擦地面音效
                    Game.Instance.sound.PlayEffect("rubground");
                    //播放向左的动画
                    AnimManager(inputDir);
                }
                break;
            case MoveDirection.Right:
                //如果没有处于最右边则可以向右
                if (targetIndex < 6)
                {
                    targetIndex++;
                    xDis = 3.312f;
                    //摩擦地面音效
                    Game.Instance.sound.PlayEffect("rubground");
                    //播放向右的动画
                    AnimManager(inputDir);
                }
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
        //这里还要加一个参数，对方的位置EnemyIndex，如果targetIndex和EnemyIndex相等，直接return
        if (nowIndex != targetIndex)
        {

            /*float move = xDis * 0.3f;
            float x = transform.position.x + move;
            transform.position = new Vector3(x,
                transform.position.y,
                transform.position.z);
            xDis -= move;
            Debug.Log(xDis + "," + move + "," + transform.position);*/

            float move = xDis / targetCount;
            transform.position += new Vector3(move, 0, 0);
            count++;

            //if (Mathf.Abs(xDis) < 0.005f)

            if (count == targetCount)
            {
                xDis = 0;
                count = 0;
                nowIndex = targetIndex;
                switch (nowIndex)
                {
                    case 1:
                        transform.position = new Vector3(Consts.left3, transform.position.y, transform.position.z);
                        break;
                    case 2:
                        transform.position = new Vector3(Consts.left2, transform.position.y, transform.position.z);
                        break;
                    case 3:
                        transform.position = new Vector3(Consts.left1, transform.position.y, transform.position.z);
                        break;
                    case 4:
                        transform.position = new Vector3(Consts.right1, transform.position.y, transform.position.z);
                        break;
                    case 5:
                        transform.position = new Vector3(Consts.right2, transform.position.y, transform.position.z);
                        break;
                    case 6:
                        transform.position = new Vector3(Consts.right3, transform.position.y, transform.position.z);
                        break;
                }
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


    //吃金币
    public void HitCoin()
    {
        Debug.Log("remote吃到金币！");
    }

    //吃双倍金币
    public void HitMultiply()
    {
        Debug.Log("remote吃到双倍道具！");
    }

    //双倍金币协程
    IEnumerator MultiplyCoroutine()
    {
        useMultiply = false;

        multiTip = "双倍金币";

        GameObject effectGO = Game.Instance.objectPool.Spawn("FX_Multiply", effectParent);
        //effectGO.transform.position = mPos.position;
        effectGO.transform.parent = mPos;
        effectGO.transform.localPosition = Vector3.zero;

        yield return new WaitForSeconds(skillTime);

        Game.Instance.objectPool.Unspwan(effectGO);
        multiTip = null;
    }

    //无敌状态
    public void HitInvincible()
    {
        Debug.Log("remote吃到无敌道具！");
    }

    public void HitHeal()
    {
        Debug.Log("remote吃到回复！");
    }
    //无敌协程
    IEnumerator InvincibleCoroutine()
    {
        useInvincible = false;

        invincibleTip = "无敌";

        GameObject effectGO = Game.Instance.objectPool.Spawn("FX_Invincible", effectParent);
        //effectGO.transform.position = iPos.position;
        effectGO.transform.parent = iPos;
        effectGO.transform.localPosition = Vector3.zero;

        yield return new WaitForSeconds(skillTime);

        Game.Instance.objectPool.Unspwan(effectGO);
        invincibleTip = null;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == Tag.Dodge_Fence)
        {
            //如果撞到应该闪过去的障碍物，视为失败
            other.gameObject.SendMessage("HitPlayer", transform.position);
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
            //播放撞到障碍物的声音
            Game.Instance.sound.PlayEffect("HitObstacles");
        }
        else if (other.gameObject.tag == Tag.Roll_Fence)
        {
            //如果在翻滚的状态下撞到本应该滚过去的障碍物，视为通过
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("roll"))
            {
                Debug.Log("here");
                return;
            }
            //否则减速  
            other.gameObject.SendMessage("HitPlayer", transform.position);
            //播放撞到障碍物的声音
            Game.Instance.sound.PlayEffect("HitObstacles");
        }
        else if (other.gameObject.tag == Tag.Block)
        {
            other.gameObject.SendMessage("HitPlayer", transform.position);
            //播放撞到障碍物的声音
            Game.Instance.sound.PlayEffect("HitObstacles");
        }
        else if (other.gameObject.tag == Tag.Small_Block)
        {
            other.transform.parent.parent.gameObject.SendMessage("HitPlayer", transform.position);
            //播放撞到障碍物的声音
            Game.Instance.sound.PlayEffect("HitObstacles");
        }
        else if (other.gameObject.tag == Tag.Before_Trigger)
        {
            //remote不用负责触发
            //让车/人走
            //other.transform.parent.gameObject.SendMessage("HitTrigger", SendMessageOptions.DontRequireReceiver);
        }
        else if (other.gameObject.tag == Tag.People)
        {
            //播放人跌倒的动画,减速
            other.gameObject.SendMessage("HitPlayer", transform.position);
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
    public int GetScore()
    {
        return health * 2 + remoteCoin;
    }
    private void Update()
    {
        if (health <= 0 && canGameOver)
        {
            gameData.IsPlay = false;
            canGameOver = false;
        }
        //每一帧更新玩家信息
        remoteCoinText.text = "金币数：" + remoteCoin;
        remoteItemText.text = "状态：" + multiTip + invincibleTip;
        HPBar.fillAmount = health / (float)maxHealth;
        //判断是否暂停，暂停则停止动画播放
        if (gameData.IsPlay)
        {
            anim.enabled = true;
        }
        else
        {
            anim.enabled = false;
        }
        //如果在某个动画状态，那么不可进入这个状态
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
