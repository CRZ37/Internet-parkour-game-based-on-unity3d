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
        ClosePanelPUNISHING(panelBG, content, 120, 1500);
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
        //返回值为是否成功，购买的物品序号，购买后当前的属性
        UIMng.ShowMessageSync("购买成功");
        string[] shopDatas = shopData.Split(',');
        int itemId = int.Parse(shopDatas[0]);
        float value = float.Parse(shopDatas[1]);
        int newprice = int.Parse(shopDatas[2]);
        SetItemAndCoinState(itemId, newprice, value);

    }
    private void SetItemAndCoinState(int itemId, int newprice, float value = -1f)
    {
        UserData userData = Game.Instance.GetUserData();
        int price = -1;
        switch (itemId)
        {
            case 1:
                Game.Instance.SetHealthPrice(newprice);
                price = newprice;
                SetHealth(userData, (int)value);
                healthItem.UpdateItemPrice(price);
                break;
            case 2:
                Game.Instance.SetBigHealthPrice(newprice);
                price = newprice;
                SetHealth(userData, (int)value);
                bigHealthItem.UpdateItemPrice(price);
                break;
            case 3:
                Game.Instance.SetSkillPrice(newprice);
                price = newprice;
                SetSkillTime(userData, value);
                skillItem.UpdateItemPrice(price);
                break;
            case 4:
                Game.Instance.SetBigSkillPrice(newprice);
                price = newprice;
                SetSkillTime(userData, value);
                bigSkillItem.UpdateItemPrice(price);
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
        string[] priceDatas = shopData.Split(',');
        ReturnCode returnCode = (ReturnCode)int.Parse(priceDatas[0]);
        if (returnCode == ReturnCode.Success)
        {
            UIMng.ShowMessageSync("更新商店状态成功！");
            int healthPrice = int.Parse(priceDatas[1]);
            int bigHealthPrice = int.Parse(priceDatas[2]);
            int skillTimePrice = int.Parse(priceDatas[3]);
            int bigSkillTimePrice = int.Parse(priceDatas[4]);
            SetItemAndCoinState(1, healthPrice);
            SetItemAndCoinState(2, bigHealthPrice);
            SetItemAndCoinState(3, skillTimePrice);
            SetItemAndCoinState(4, bigSkillTimePrice);
        }
        else
        {
            UIMng.ShowMessageSync("更新商店状态失败！请退出后重新进入");
        }
    }
}
