using GameServer.DAO;
using GameServer.Model;
using GameServer.Servers;
using GameServer.Common;

namespace GameServer.Controller
{
    class ShopController : BaseController
    {
        private ShopStateDAO shopStateDAO = new ShopStateDAO();
        public void BuyItem(string data, Client client, Server server)
        {
            client.UpdateShopState(data);
        }
        public string UpdateShop(string data, Client client, Server server)
        {
            ShopState shopState = shopStateDAO.GetShopStateByUserid(client.MysqlConn,client.GetUserId());
            if(shopState == null)
            {
                //获取商店信息失败
                return ((int)ReturnCode.Fail).ToString();
            }
            return string.Format("{0},{1},{2},{3},{4}", ((int)ReturnCode.Success).ToString(),
                shopState.HealthTime, shopState.BigHealthTime, shopState.SkillTimeTime, shopState.BigHealthTime);
        }
    }
}
