using GameServer.Common;
using System;
using System.Net.Sockets;
using MySql.Data.MySqlClient;
using GameServer.Tool;
using GameServer.Model;
using GameServer.DAO;

namespace GameServer.Servers
{
    class Client
    {
        private Socket clientSocket;
        //持有一个对server的引用
        private Server server;
        private Message msg = new Message();
        private MySqlConnection mysqlConn;
        private User user;
        private Result result;

        public MySqlConnection MysqlConn { get => mysqlConn; set => MysqlConn = value; }
        public Room Room { get; set; }
        public int LocationIndex { get; set; }
        public ResultDAO ResultDAO { get; set; }
        public CoinDAO CoinDAO { get; set; }
        public PlayerStateDAO PlayerStateDAO { get; set; }
        public ShopStateDAO ShopStateDAO { get; set; }
        public RoleShopStateDAO RoleShopStateDAO { get; set; }
        public ItemPriceDAO ItemPriceDAO { get; set; }
        public UserDAO UserDAO { get; set; }
        public Coin Coin { get; set; }
        public PlayerState PlayerState { get; set; }

        public Client() { }
        public Client(Socket clientSocket, Server server,CoinDAO coinDAO,ItemPriceDAO itemPriceDAO,PlayerStateDAO playerStateDAO,ResultDAO resultDAO,RoleShopStateDAO roleShopStateDAO,ShopStateDAO shopStateDAO,UserDAO userDAO)
        {
            this.clientSocket = clientSocket;
            this.server = server;
            mysqlConn = ConnHelper.Connect();
            CoinDAO = coinDAO;
            ItemPriceDAO = itemPriceDAO;
            PlayerStateDAO = playerStateDAO;
            ResultDAO = resultDAO;
            RoleShopStateDAO = roleShopStateDAO;
            ShopStateDAO = shopStateDAO;
            UserDAO = userDAO;
        }
        public void SetUserData(User user, Result result, Coin coin, PlayerState playerState)
        {
            this.user = user;
            this.result = result;
            Coin = coin;
            PlayerState = playerState;
        }
        public string GetUserData()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", 
                user.Id, user.Username, result.TotalCount, result.WinCount, 
                Coin.CoinNum, PlayerState.Health, PlayerState.SkillTime,
                PlayerState.RoleSelect);
        }

        public void Start()
        {
            //试图跟callback里的return起了同样的作用，但会多出一条“开始解析数据ReceiveCallBack...”，所以还是return比较好
            //if (clientSocket.Connected == false)
            //{
            //    return;
            //}
            //Console.WriteLine("开始接收数据BeginReceive...");
            clientSocket.BeginReceive(msg.Data, msg.EndIndex, msg.RemainSize, SocketFlags.None, ReceiveCallBack, null);
        }
        private void ReceiveCallBack(IAsyncResult ar)
        {
            try
            {
                int count = clientSocket.EndReceive(ar);
                Console.WriteLine("数据长度：" + count);
                if (count == 0)
                {
                    //如果发送过来的数据长度是0，说明客户端已经断开连接
                    //也可以使用Poll()方法，看看一定时间内（比如10毫秒）
                    //是否能从客户端读到消息来判断连接是否断开   
                    Console.WriteLine("客户端已关闭连接，关闭服务器与其对应的client");
                    Close();                 
                    return;
                }
                //读取数据
                Console.WriteLine("开始解析数据ReceiveCallBack...");
                msg.ReadMessage(count, OnProcessMessage);
                //处理接收到的数据
                Start();
            }
            catch (Exception e)
            {
                //出现异常直接close
                Close();
                Console.WriteLine(e);
            }
        }
        //断开连接
        private void Close()
        {
            //先关闭与数据库的连接
            ConnHelper.CloseConnection(mysqlConn);
            if (clientSocket != null)
            {
                clientSocket.Close();
                //如果玩家加入了房间，先退出/销毁房间
                if (Room != null)
                {
                    Room.QuitGame(this);
                }
                //从登录用户列表中去掉此用户
                if (user != null)
                {
                    server.RemoveUser(user.Id);
                }
                //从服务器移除此对应client
                server.RemoveClient(this);
                Console.WriteLine("\n|****移除对应的client，一次完整服务器响应流程结束****|\n");
            }
        }
        public void OnProcessMessage(RequestCode requestCode, ActionCode actionCode, string data)
        {
            server.HandleRequest(requestCode, actionCode, data, this);
        }
        //服务器端数据返回客户端
        public void Send(ActionCode actionCode, string data)
        {
            byte[] bytes = Message.PackData(actionCode, data);
            clientSocket.Send(bytes);
        }
        public int GetUserId()
        {
            return user.Id;
        }
        public bool IsHost()
        {
            return Room.IsHost(this);
        }
        //操作数据库
        public void UpdateResult(WinLoseType type)
        {
            //首先添加总场数
            result.TotalCount++;
            if (type == WinLoseType.Win)
            {
                result.WinCount++;
            }
            ResultDAO.UpdateOrAddResult(mysqlConn, result);
            //通知客户端更新数据
            Send(ActionCode.UpdateResult, string.Format("{0},{1}", result.TotalCount, result.WinCount));
        }
        public void UpdateCoin(int coinNum)
        {
            Coin.CoinNum += coinNum;
            CoinDAO.UpdateOrAddCoin(mysqlConn, Coin);
            Send(ActionCode.UpdateCoin, string.Format("{0}", Coin.CoinNum));
        }
        //TODO:2.也可以增加血量属性以及血量更新的方法，如果此Client血量归零，就通知Room开始结算成绩，也就是客户端的GameOverRequest由服务器来控制
    }
}
