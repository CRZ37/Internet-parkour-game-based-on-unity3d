using System;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    [SerializeField]
    private int itemId;
    [SerializeField]
    private ShopPanel shopPanel;
    [SerializeField]
    private Text buyState;
    [SerializeField]
    private Text priceText;
    private int price;
    [SerializeField]
    private Button buyButton;

    private void Awake()
    {
        buyState.text = "价格随购买次数增加";
    }

    public void OnBuyClick()
    {
        Game.Instance.sound.PlayEffect("Click");
        //如果玩家持有金币大于物品金额，则购买成功
        if (Game.Instance.GetUserData().CoinNum >= price)
        {
            Debug.Log("已购买" + itemId + "号物品");
            string buyInfo = string.Format("{0},{1}", itemId, price);
            buyButton.enabled = false;
            shopPanel.OnBuyClick(buyInfo);
        }
        else
        {
            Debug.Log("购买失败");
        }       
    }
    public void UpdateItemPrice(int price)
    {
        this.price = price;
        priceText.text = "价格：" + price;
        buyButton.enabled = true;
    }
}
