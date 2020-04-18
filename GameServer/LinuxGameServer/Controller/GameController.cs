using System;
using GameServer.Common;
using GameServer.Servers;

namespace GameServer.Controller
{
    class GameController : BaseController
    {
        Random random = new Random();
        public GameController()
        {
            requestCode = RequestCode.Game;
        }
        public string StartGame(string data, Client client, Server server)
        {
            if (client.IsHost())
            {
                //如果人数不足，返回失败
                if (client.Room.IsWaitingJoin())
                {
                    return ((int)ReturnCode.Lack).ToString();
                }
                int index1 = random.Next(0, 3);
                int index2 = random.Next(0, 3);
                string strs = ((int)ReturnCode.Success).ToString() + "," + index1.ToString() + "," + index2.ToString();
                //广播给其它客户端，表示可以开启游戏了
                client.Room.BroadcastMessage(client, ActionCode.StartGame, strs);
                //开始倒计时
                client.Room.StartTimer();
                return strs;
            }
            else
            {
                return ((int)ReturnCode.Fail).ToString();
            }
        }
        //同步生成道路与道具
        public void CreateRoadAndItem(string data, Client client, Server server)
        {
            //只有房主能发出更新道路信息的请求
            if (client.IsHost())
            {
                //生成道路的序号
                int roadIndex = random.Next(1, 5);
                //生成道具的序号
                int itemIndex = random.Next(0, 3);
                string indexData = roadIndex.ToString() + "," +  itemIndex.ToString();
                //广播给所有客户端要生成的道路信息
                client.Room.BroadcastMessage(null, ActionCode.CreateRoadAndItem, indexData);
            }
        }
        //两个客户端同步移动
        public void LocalMove(string data, Client client, Server server)
        {
            client.Room.Move(client,data);
        }
        //两个客户端同步血量
        public void TakeDamage(string data, Client client, Server server)
        {
            //将一个client转发到其它client,可能会出现游戏结束的瞬间又碰到第二个障碍物的情况，这时候Room
            //已经close了
            if (client.Room != null)
            {
                client.Room.BroadcastMessage(client, ActionCode.TakeDamage, data);
            }           
        }
        //两个客户端同步金币
        public void GetCoin(string data, Client client, Server server)
        {
            //将一个client转发到其它client
            client.Room.BroadcastMessage(client, ActionCode.GetCoin, data);
        }
        //两个客户端同步使用道具
        public void UseItem(string data, Client client, Server server)
        {
            //将一个client转发到其它client
            client.Room.BroadcastMessage(client, ActionCode.UseItem, data);
        }
        public void GameOver(string data, Client client, Server server)
        {
            //将一个client转发到其它client,发送的client
            client.Room.BroadcastMessage(null, ActionCode.GameOver, data);
            //操作总场、胜场、金币        
            client.Room.UpdateResultCoin(data);                
            //全部处理结束后销毁房间
            client.Room.Close();
        }
        public void QuitBattle(string data, Client client, Server server)
        {
            client.Room.BroadcastMessage(null, ActionCode.QuitBattle, data);
            client.Room.Close();
        }
    }
}
