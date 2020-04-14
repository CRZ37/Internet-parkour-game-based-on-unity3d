using GameServer.DAO;
using GameServer.Model;
using GameServer.Servers;
using GameServer.Common;
using System.Collections.Generic;

namespace GameServer.Controller
{
    class ShopController : BaseController
    {
        
        public void BuyItem(string data, Client client, Server server)
        {
            string[] datas = data.Split(',');
            int itemIndex = int.Parse(datas[0]);
            int itemPrice = int.Parse(datas[1]);
            client.Coin.CoinNum -= itemPrice;
            ShopState shopState = client.ShopStateDAO.GetShopStateByUserid(client.MysqlConn, client.GetUserId());
            Dictionary<string, ItemPrice> itemPrices = client.ItemPriceDAO.GetItemPriceByShopId(client.MysqlConn, 1);
            switch (itemIndex)
            {
                case 1:
                    shopState.HealthTime++;
                    client.PlayerState.Health += 1;
                    ManyDAO(client,shopState);
                    int healthPrice = itemPrices["health"].Price * (shopState.HealthTime + 1);
                    client.Send(ActionCode.BuyItem, string.Format("{0},{1},{2}",1, client.PlayerState.Health,healthPrice));
                    break;
                case 2:
                    shopState.BigHealthTime++;
                    client.PlayerState.Health += 2;
                    ManyDAO(client, shopState);
                    int bighealthPrice = itemPrices["bighealth"].Price * (shopState.BigHealthTime + 1);
                    client.Send(ActionCode.BuyItem, string.Format("{0},{1},{2}",2, client.PlayerState.Health,bighealthPrice));
                    break;
                case 3:
                    shopState.SkillTimeTime++;
                    client.PlayerState.SkillTime += 0.5f;
                    ManyDAO(client, shopState);
                    int skillTimePrice = itemPrices["skilltime"].Price * (shopState.SkillTimeTime + 1);
                    client.Send(ActionCode.BuyItem, string.Format("{0},{1},{2}", 3, client.PlayerState.SkillTime,skillTimePrice));
                    break;
                case 4:
                    shopState.BigSkillTimeTime++;
                    client.PlayerState.SkillTime += 1;
                    ManyDAO(client, shopState);
                    int bigSkillTimePrice = itemPrices["bigskilltime"].Price * (shopState.BigSkillTimeTime + 1);
                    client.Send(ActionCode.BuyItem, string.Format("{0},{1},{2}", 4, client.PlayerState.SkillTime,bigSkillTimePrice));
                    break;
            }
            client.Send(ActionCode.UpdateCoin, client.Coin.CoinNum.ToString());
        }
        public string UpdateShop(string data, Client client, Server server)
        {
            ShopState shopState = client.ShopStateDAO.GetShopStateByUserid(client.MysqlConn, client.GetUserId());
            Dictionary<string, ItemPrice> itemPrices = client.ItemPriceDAO.GetItemPriceByShopId(client.MysqlConn, 1);
            if (shopState == null || itemPrices == null)
            {
                //获取购买信息失败
                return ((int)ReturnCode.Fail).ToString();
            }
            int healthPrice = itemPrices["health"].Price * (shopState.HealthTime + 1);
            int bighealthPrice = itemPrices["bighealth"].Price * (shopState.BigHealthTime + 1);
            int skillTimePrice = itemPrices["skilltime"].Price * (shopState.SkillTimeTime + 1);
            int bigSkillTimePrice = itemPrices["bigskilltime"].Price * (shopState.BigSkillTimeTime + 1);
            return string.Format("{0},{1},{2},{3},{4}", ((int)ReturnCode.Success).ToString(),
                healthPrice, bighealthPrice, skillTimePrice, bigSkillTimePrice);
        }
        private void ManyDAO(Client client,ShopState shopState)
        {
            client.CoinDAO.UpdateOrAddCoin(client.MysqlConn, client.Coin);
            client.ShopStateDAO.UpdateOrAddShopState(client.MysqlConn, shopState);
            client.PlayerStateDAO.UpdateOrAddPlayerState(client.MysqlConn, client.PlayerState);
        }
    }
}
