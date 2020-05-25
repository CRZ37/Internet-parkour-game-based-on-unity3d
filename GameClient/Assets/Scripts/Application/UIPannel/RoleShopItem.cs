using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoleShopItem : MonoBehaviour
{
    [SerializeField]
    private int itemId;
    [SerializeField]
    private RoleShopPanel roleShopPanel;
    [SerializeField]
    private Text priceText;
    private int price;
    [SerializeField]
    private Button selectButton;
    [SerializeField]
    private Button buyButton;
    [SerializeField]
    private GameObject[] selectImages;

    public void OnSelectClick()
    {
        for (int i = 0; i < selectImages.Length; i++)
        {
            selectImages[i].SetActive(false);
        }
        selectImages[itemId - 1].SetActive(true);
        roleShopPanel.OnselectClick(itemId.ToString());
    }
    public void OnBuyClick()
    {
        Game.Instance.sound.PlayEffect("Click");
        if (Game.Instance.GetUserData().CoinNum >= price)
        {
            Debug.Log("已购买" + itemId + "号物品");
            roleShopPanel.OnBuyClick(string.Format("{0},{1}", itemId, priceText.text));
        }
        else
        {
            Debug.Log("购买失败");
        }
            
    }
    public void SetBuy()
    {
        buyButton.gameObject.SetActive(false);
        selectButton.gameObject.SetActive(true);
        //buyButton.transform.Find("Text").GetComponent<Text>().text = "已购买";
    }
    public void SetNoBuy()
    {
        selectButton.gameObject.SetActive(false);
    }
    public void SetSelect()
    {
        selectImages[itemId - 1].SetActive(true);
    }

    public void SetPrice(string price)
    {
        Debug.Log(priceText);
        priceText.text = price;
        this.price = int.Parse(price);
    }
}
