using GameServer.DAO;
using GameServer.Model;
using GameServer.Servers;
using GameServer.Common;
using System.Collections.Generic;
using System;

namespace GameServer.Controller
{
    class RoleShopController : BaseController
    {
        public void SelectRoleItem(string data, Client client, Server server)
        {          
            string roleItemIndex = data;
            switch (roleItemIndex)
            {
                case "1":
                    client.PlayerState.RoleSelect = "1";
                    break;
                case "2":
                    client.PlayerState.RoleSelect = "2";
                    break;
                case "3":
                    client.PlayerState.RoleSelect = "3";
                    break;
                default:
                    Console.WriteLine("RoleShopController中的SelectRoleItem的switch接收到非法参数");
                    break;
            }
            client.PlayerStateDAO.UpdateOrAddPlayerState(client.MysqlConn, client.PlayerState);
            client.Send(ActionCode.SelectRoleItem, data);
        }
        public void BuyRoleItem(string data, Client client, Server server)
        {
            string[] datas1 = data.Split(',');
            string roleItemIndex = datas1[0];
            int roleItemPrice = int.Parse(datas1[1]);      
            client.Coin.CoinNum -= roleItemPrice;
            RoleShopState roleShopState = client.RoleShopStateDAO.GetRoleShopStateByUserId(client.MysqlConn, client.GetUserId());
            string[] roleBuys = roleShopState.RoleBuy.Split(',');
            switch (roleItemIndex)
            {
                case "2":
                    roleBuys[1] = "1";
                    break;
                case "3":
                    roleBuys[2] = "1";
                    break;
                default:
                    Console.WriteLine("RoleShopController中的BuyRoleItem的switch接收到非法参数");
                    break;
            }
            roleShopState.RoleBuy = string.Join(",", roleBuys);
            client.RoleShopStateDAO.UpdateOrAddRoleShopState(client.MysqlConn, roleShopState);
            client.CoinDAO.UpdateOrAddCoin(client.MysqlConn, client.Coin);
            client.Send(ActionCode.BuyRoleItem, roleItemIndex);
            client.Send(ActionCode.UpdateCoin, client.Coin.CoinNum.ToString());
        }
        public string UpdateRoleShop(string data, Client client, Server server)
        {
            RoleShopState roleShopState = client.RoleShopStateDAO.GetRoleShopStateByUserId(client.MysqlConn, client.GetUserId());
            Dictionary<string, ItemPrice> itemPrices = client.ItemPriceDAO.GetItemPriceByShopId(client.MysqlConn, 2);
            if (roleShopState == null || itemPrices == null)
            {
                //获取购买信息失败
                return ((int)ReturnCode.Fail).ToString();
            }
            int roleMalePrice = itemPrices["rolemaleprice"].Price;
            int roleCopPrice = itemPrices["rolecopprice"].Price;
            int roleRobotPrice = itemPrices["rolerobotprice"].Price;
            Console.WriteLine(string.Format("{0}|{1}|{2}|{3}|{4}|{5}", ((int)ReturnCode.Success).ToString(),
                roleShopState.RoleBuy, roleMalePrice, roleCopPrice, roleRobotPrice, client.PlayerState.RoleSelect));
            return string.Format("{0}|{1}|{2}|{3}|{4}|{5}", ((int)ReturnCode.Success).ToString(),
                roleShopState.RoleBuy, roleMalePrice, roleCopPrice, roleRobotPrice, client.PlayerState.RoleSelect);
        }
    }
}
