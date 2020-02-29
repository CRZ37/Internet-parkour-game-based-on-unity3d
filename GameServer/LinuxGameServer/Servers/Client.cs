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
        private Room room;
        private User user;
        private Result result;
        private Coin coin;
        private PlayerState playerState;
        private ShopState shopState;
        private int locationIndex = -1;
        private ResultDAO resultDAO = new ResultDAO();
        private CoinDAO coinDAO = new CoinDAO();
        private PlayerStateDAO playerStateDAO = new PlayerStateDAO();
        private ShopStateDAO shopStateDAO = new ShopStateDAO();

        public MySqlConnection MysqlConn { get => mysqlConn; set => MysqlConn = value; }
        public Room Room { get => room; set => room = value; }
        public int LocationIndex { get => locationIndex; set => locationIndex = value; }

        public Client() { }
        public Client(Socket clientSocket, Server server)
        {
            this.clientSocket = clientSocket;
            this.server = server;
            mysqlConn = ConnHelper.Connect();
        }
        public void SetUserData(User user, Result result, Coin coin, PlayerState playerState, ShopState shopState)
        {
            this.user = user;
            this.result = result;
            this.coin = coin;
            this.playerState = playerState;
            this.shopState = shopState;
        }
        public string GetUserData()
        {
            return user.Id + "," + user.Username + "," + result.TotalCount + "," + result.WinCount + ","
                + coin.CoinNum + "," + playerState.Health + "," + playerState.SkillTime;
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
                if (room != null)
                {
                    room.QuitGame(this);
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
            return room.IsHost(this);
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
            resultDAO.UpdateOrAddResult(mysqlConn, result);
            //通知客户端更新数据
            Send(ActionCode.UpdateResult, string.Format("{0},{1}", result.TotalCount, result.WinCount));
        }
        public void UpdateCoin(int coinNum)
        {
            coin.CoinNum += coinNum;
            coinDAO.UpdateOrAddCoin(mysqlConn, coin);
            Send(ActionCode.UpdateCoin, string.Format("{0}", coin.CoinNum));
        }
            
        public void UpdateShopState(string data)
        {
            int itemIndex = int.Parse(data.Split(',')[0]);
            int itemPrice = int.Parse(data.Split(',')[1]);
            coin.CoinNum -= itemPrice;
            switch (itemIndex)
            {
                case 1:
                    shopState.HealthTime++;
                    playerState.Health += 1;
                    ManyDAO();
                    Send(ActionCode.BuyItem, string.Format("{0},{1},{2}",1,shopState.HealthTime, playerState.Health));
                    break;
                case 2:
                    shopState.BigHealthTime++;
                    playerState.Health += 2;
                    ManyDAO();
                    Send(ActionCode.BuyItem, string.Format("{0},{1},{2}", 2,shopState.BigHealthTime, playerState.Health));
                    break;
                case 3:
                    shopState.SkillTimeTime++;
                    playerState.SkillTime += 0.5f;
                    ManyDAO();
                    Send(ActionCode.BuyItem, string.Format("{0},{1},{2}", 3,shopState.SkillTimeTime, playerState.SkillTime));
                    break;
                case 4:
                    shopState.BigSkillTimeTime++;
                    playerState.SkillTime += 1;
                    ManyDAO();
                    Send(ActionCode.BuyItem, string.Format("{0},{1},{2}", 4,shopState.BigSkillTimeTime, playerState.SkillTime));
                    break;
            }
            Send(ActionCode.UpdateCoin, string.Format("{0}", coin.CoinNum));
        }
        private void ManyDAO()
        {
            coinDAO.UpdateOrAddCoin(mysqlConn, coin);
            shopStateDAO.UpdateOrAddShopState(mysqlConn, shopState);
            playerStateDAO.UpdateOrAddPlayerState(mysqlConn, playerState);
        }
    }
}
