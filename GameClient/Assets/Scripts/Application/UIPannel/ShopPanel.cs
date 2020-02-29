using System;
using UnityEngine;
using UnityEngine.UI;
using Common;
using System.Collections.Generic;

public class ShopPanel : BasePanel
{
    [SerializeField]
    private ShopItem healthItem;
    [SerializeField]
    private ShopItem bigHealthItem;
    [SerializeField]
    private ShopItem skillItem;
    [SerializeField]
    private ShopItem bigSkillItem;
    private BuyItemRequest buyItemRequest;
    private UpdateShopRequest updateShopRequest;
    private RectTransform panelBG;
    private RectTransform content;
    private string shopData = null;
    private string itemAndCoinData = null;
    private Text healthText;
    private Text skillTimeText;

    private void Awake()
    {
        buyItemRequest = GetComponent<BuyItemRequest>();
        updateShopRequest = GetComponent<UpdateShopRequest>();
        panelBG = transform.GetComponent<RectTransform>();
        content = transform.Find("Content").GetComponent<RectTransform>();
        content.Find("CloseButton").GetComponent<Button>().onClick.AddListener(OnCloseClick);
        healthText = content.Find("UserHealth/HealthText").GetComponent<Text>();
        skillTimeText = content.Find("UserSkillTime/SkillTimeText").GetComponent<Text>();
    }

    private void OnCloseClick()
    {
        ClosePanelPUNISHING(panelBG, content, 120, 800);
    }

    public override void OnEnter()
    {
        UserData userData = Game.Instance.GetUserData();
        healthText.text = userData.Health.ToString();
        skillTimeText.text = userData.SkillTime.ToString();
        updateShopRequest.SendUpdateShopRequest();
        OpenPanelPUNISHING(panelBG, content, 800, 120, 1500);
    }

    public void OnBuyClick(string buyInfo)
    {
        buyItemRequest.SendBuyRequest(buyInfo);
    }
    public void UpdateShopStateSync(string shopData)
    {
        this.shopData = shopData;
    }
    public void UpdateItemStateSync(string itemAndCoinData)
    {
        this.itemAndCoinData = itemAndCoinData;
    }
    private void Update()
    {
        if (shopData != null)
        {
            UpdateShopState();
            shopData = null;
        }
        if (itemAndCoinData != null)
        {
            UpdateItemState();
            itemAndCoinData = null;
        }
    }
    //购买单个商品更新状态
    private void UpdateItemState()
    {
        //返回值为这个商品当前的被购买次数,格式：序号,值
        int itemId = int.Parse(itemAndCoinData.Split(',')[0]);
        int buyTime = int.Parse(itemAndCoinData.Split(',')[1]);
        float value = float.Parse(itemAndCoinData.Split(',')[2]);
        SetItemAndCoinState(itemId, buyTime, value);
    }
    private void SetItemAndCoinState(int itemId, int buyTime, float value = -1f)
    {
        UserData userData = Game.Instance.GetUserData();
        int price = -1;
        switch (itemId)
        {
            case 1:
                Game.Instance.SetHealthState(buyTime);
                price = 300 + 300 * buyTime;
                SetHealth(userData, (int)value);
                healthItem.UpdateItemState(userData.HealthTime, price);
                break;
            case 2:
                Game.Instance.SetBigHealthState(buyTime);
                price = 400 + 400 * buyTime;
                SetHealth(userData, (int)value);
                bigHealthItem.UpdateItemState(userData.BigHealthTime, price);
                break;
            case 3:
                Game.Instance.SetSkillState(buyTime);
                price = 500 + 500 * buyTime;
                SetSkillTime(userData, value);
                skillItem.UpdateItemState(userData.SkillTimeTime, price);
                break;
            case 4:
                Game.Instance.SetBigSkillState(buyTime);
                price = 700 + 700 * buyTime;
                SetSkillTime(userData, value);
                bigSkillItem.UpdateItemState(userData.BigSkillTimeTime, price);
                break;
        }
    }
    private void SetHealth(UserData userData, int value)
    {
        if (value != -1)
        {
            userData.Health = value;
        }
        healthText.text = userData.Health.ToString();
    }
    private void SetSkillTime(UserData userData, float value)
    {
        if (value != -1)
        {
            userData.SkillTime = value;
        }
        skillTimeText.text = userData.SkillTime.ToString();
    }
    private void UpdateShopState()
    {
        ReturnCode returnCode = (ReturnCode)int.Parse(shopData.Split(',')[0]);
        if (returnCode == ReturnCode.Success)
        {
            UIMng.ShowMessageSync("更新商店状态成功！");
            int healthTime = int.Parse(shopData.Split(',')[1]);
            int bigHealthTime = int.Parse(shopData.Split(',')[2]);
            int skillTimeTime = int.Parse(shopData.Split(',')[3]);
            int bigSkillTimeTime = int.Parse(shopData.Split(',')[4]);
            SetItemAndCoinState(1, healthTime);
            SetItemAndCoinState(2, bigHealthTime);
            SetItemAndCoinState(3, skillTimeTime);
            SetItemAndCoinState(4, bigSkillTimeTime);
        }
        else
        {
            UIMng.ShowMessageSync("更新商店状态失败！请退出后重新进入");
        }
    }
}
