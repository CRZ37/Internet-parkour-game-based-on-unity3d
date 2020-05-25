using System.Collections.Generic;
using System.Text;
using GameServer.Common;
using System.Threading;

namespace GameServer.Servers
{
    /// <summary>
    /// 方间的四种状态
    /// </summary>
    enum RoomState
    {
        WaitingJoin,
        WaitingBattle,
        Battle,
        End
    }
    /// <summary>
    /// 每个房间内有两个客户端
    /// </summary>
    class Room
    {
        private List<Client> clientsInRoom = new List<Client>();
        private RoomState roomState = RoomState.WaitingJoin;
        private Server server;
        public static readonly object objLock = new object();
        public Room(Server server)
        {
            this.server = server;
        }
        public void Move(Client localClient, string data)
        {
            lock (objLock)
            {
                MoveDirection inputDir = (MoveDirection)int.Parse(data);
                switch (inputDir)
                {
                    case MoveDirection.Left:
                        localClient.LocationIndex--;
                        if (localClient.LocationIndex < 1
                            || clientsInRoom[0].LocationIndex == clientsInRoom[1].LocationIndex)
                        {
                            //如果移动后重合，那么不移动
                            localClient.LocationIndex++;
                        }
                        else
                        {
                            BroadcastMoveMessage(localClient, data);
                        }
                        break;
                    case MoveDirection.Right:
                        localClient.LocationIndex++;
                        if (localClient.LocationIndex > 6 || clientsInRoom[0].LocationIndex
                            == clientsInRoom[1].LocationIndex)
                        {
                            //如果移动后重合，那么不移动
                            localClient.LocationIndex--;
                        }
                        else
                        {
                            //否则发送信息
                            BroadcastMoveMessage(localClient, data);
                        }
                        break;
                    default:
                        BroadcastMoveMessage(localClient, data);
                        break;
                }
            }      
        }
        public bool IsWaitingJoin()
        {
            return roomState == RoomState.WaitingJoin;
        }
        public void AddClient(Client client)
        {
            lock (objLock)
            {
                clientsInRoom.Add(client);
                client.Room = this;
                if (clientsInRoom.Count >= 2)
                {
                    roomState = RoomState.WaitingBattle;
                }
            }          
        }
        public void RemoveClient(Client client)
        {
            lock (objLock)
            {
                client.Room = null;
                clientsInRoom.Remove(client);
                roomState = RoomState.WaitingJoin;
            }         
        }
        //返回房主信息
        public string GetHostOwner()
        {
            return clientsInRoom[0].GetUserData();
        }
        public void QuitGame(Client client)
        {
            lock (objLock)
            {
                if (client == clientsInRoom[0])
                {
                    Close();
                }
                else
                {
                    clientsInRoom.Remove(client);
                }
            }
        }
        public int GetId()
        {
            if (clientsInRoom.Count > 0)
            {
                return clientsInRoom[0].GetUserId();
            }
            return -1;
        }
        public string GetRoomData()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Client client in clientsInRoom)
            {
                sb.Append(client.GetUserData() + "|");
            }
            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }
        public void BroadcastMessage(Client excludeClient, ActionCode actionCode, string data)
        {
            foreach (Client client in clientsInRoom)
            {
                if (client != excludeClient)
                {
                    server.SendResponse(client, actionCode, data);
                }
            }
        }
        public void BroadcastMoveMessage(Client localClient, string data)
        {
            foreach (Client client in clientsInRoom)
            {
                if (client == localClient)
                {
                    server.SendResponse(client, ActionCode.LocalMove, data);
                }
                else
                {
                    server.SendResponse(client, ActionCode.RemoteMove, data);
                }
            }
        }
        public bool IsHost(Client client)
        {
            //list中的第一个是房主
            return client == clientsInRoom[0];
        }
        public void Close()
        {
            foreach (Client client in clientsInRoom)
            {
                client.Room = null;
            }
            server.RemoveRoom(this);
        }
        public void StartTimer()
        {
            //启动计时线程
            new Thread(RunTimer).Start();
        }
        private void RunTimer()
        {
            //计时之前先暂停一段时间，有两个目的，
            //一是先让开始计时的消息返回回去，二是这段间隙可以在客户端插入一些其他动画
            Thread.Sleep(1700);
            for (int i = 3; i > 0; i--)
            {
                //广播计时
                BroadcastMessage(null, ActionCode.ShowTimer, i.ToString());
                //每次暂停一秒
                Thread.Sleep(1000);
            }
            //计时结束后开始游戏
            BroadcastMessage(null, ActionCode.StartPlay, "StartPlay");
        }
        //平手/输了只增加总场数，输了再操作胜场
        //TODO:3.如果在服务器计算血量，就改成在本方法中结算分数，排出顺位，然后使用依次client.Send()来发送GameOverRequest
        public void UpdateResultCoin(string data)
        {
            string[] res = data.Split(',');
            Role_ResultRoleType type = (Role_ResultRoleType)int.Parse(res[2]);
            int coinNum1 = int.Parse(res[0].Split('|')[2]);
            int coinNum2 = int.Parse(res[1].Split('|')[2]);
            switch (type)
            {
                case Role_ResultRoleType.Host:
                    clientsInRoom[0].UpdateResult(WinLoseType.Win);
                    clientsInRoom[0].UpdateCoin(coinNum1);
                    clientsInRoom[1].UpdateResult(WinLoseType.Lose);
                    clientsInRoom[1].UpdateCoin(coinNum2);
                    break;
                case Role_ResultRoleType.Client:
                    clientsInRoom[0].UpdateResult(WinLoseType.Lose);
                    clientsInRoom[0].UpdateCoin(coinNum2);
                    clientsInRoom[1].UpdateResult(WinLoseType.Win);
                    clientsInRoom[1].UpdateCoin(coinNum1);
                    break;
                case Role_ResultRoleType.Tie:
                    clientsInRoom[0].UpdateResult(WinLoseType.Tie);
                    clientsInRoom[0].UpdateCoin(coinNum1);
                    clientsInRoom[1].UpdateResult(WinLoseType.Tie);
                    clientsInRoom[1].UpdateCoin(coinNum2);
                    break;
            }
        }
    }
}
