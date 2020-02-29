using GameServer.Common;
using GameServer.Servers;
using GameServer.DAO;
using GameServer.Model;

namespace GameServer.Controller
{
    class UserController : BaseController
    {
        private UserDAO userDAO = new UserDAO();
        private ResultDAO resultDAO = new ResultDAO();
        private CoinDAO coinDAO = new CoinDAO();
        private PlayerStateDAO playerStateDAO = new PlayerStateDAO();
        private ShopStateDAO shopStateDAO = new ShopStateDAO();
        public UserController()
        {
            requestCode = RequestCode.User;
        }
        //处理登录请求
        public string Login(string data, Client client, Server server)
        {
            string[] strs = data.Split(',');
            User user = userDAO.VerifyUser(client.MysqlConn, strs[0], strs[1]);
            if (user == null)
            {
                return ((int)ReturnCode.Fail).ToString();
            }
            else if (server.IsLogin(user.Id))
            {
                return ((int)ReturnCode.IsLogin).ToString();
            }
            else
            {
                Result result = resultDAO.GetResultByUserid(client.MysqlConn, user.Id);
                Coin coin = coinDAO.GetCoinByUserid(client.MysqlConn, user.Id);
                PlayerState playerState = playerStateDAO.GetPlayerStateByUserid(client.MysqlConn, user.Id);
                ShopState shopState = shopStateDAO.GetShopStateByUserid(client.MysqlConn, user.Id);
                //把个人信息保存到对应的client中
                client.SetUserData(user, result, coin, playerState,shopState);
                //把id保存到登陆列表中，防止重复登录
                server.AddUser(user.Id);
                //将用户信息组拼起来返回
                return string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", ((int)ReturnCode.Success).ToString(),
                    user.Id, user.Username, result.TotalCount, result.WinCount, coin.CoinNum, playerState.Health, playerState.SkillTime);
            }
        }
        //处理注册请求
        public string Register(string data, Client client, Server server)
        {
            string[] strs = data.Split(',');
            string username = strs[0];
            string password = strs[1];
            if (userDAO.GetUserByUserName(client.MysqlConn, username))
            {
                return ((int)ReturnCode.Fail).ToString();
            }
            userDAO.AddUser(client.MysqlConn, username, password);
            return ((int)ReturnCode.Success).ToString();
        }
        //处理登出请求
        public string Logout(string data, Client client, Server server)
        {
            int userId = int.Parse(data);
            //从用户id列表中去除这个用户
            bool isRemove = server.RemoveUser(userId);
            if (isRemove)
            {
                return ((int)ReturnCode.Success).ToString();
            }
            else
            {
                return ((int)ReturnCode.NotFound).ToString();
            }
        }
    }
}
