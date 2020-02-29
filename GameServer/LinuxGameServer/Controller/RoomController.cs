using System.Text;
using GameServer.Common;
using GameServer.Servers;

namespace GameServer.Controller
{
    class RoomController : BaseController
    {
        public RoomController()
        {
            requestCode = RequestCode.Room;
        }
        //处理登录请求
        public string CreateRoom(string data, Client client, Server server)
        {
            //hostPlayer初始位置序号
            client.LocationIndex = 5;
            server.CreateRoom(client);
            return ((int)ReturnCode.Success) + "," + ((int)Role_ResultRoleType.Host);
        }
        public string ListRoom(string data, Client client, Server server)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Room room in server.RoomList)
            {
                if (room.IsWaitingJoin())
                {
                    sb.Append(room.GetHostOwner() + "|");
                }
            }
            //如果没有处于等待加入状态的房间
            if (sb.Length == 0) 
            {
                sb.Append("NoRoom");
            }
            else
            {
                //移除最后一个"|"
                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }
        public string JoinRoom(string data, Client client, Server server)
        {
            int id = int.Parse(data);
            Room room = server.GetRoomById(id);
            if (room == null)
            {
                //方间已销毁
                return ((int)ReturnCode.NotFound).ToString();
            }
            else if (!room.IsWaitingJoin())
            {
                //房间已经满员
                return ((int)ReturnCode.Fail).ToString();
            }
            else
            {   //clientPlayer初始位置序号
                client.LocationIndex = 2;
                room.AddClient(client);
                string roomData = room.GetRoomData();
                //广播给另一个客户端消息，更新房间
                room.BroadcastMessage(client, ActionCode.UpdateRoom, roomData);
                //返回的时候要附加角色类型
                return ((int)ReturnCode.Success) + "," + ((int)Role_ResultRoleType.Client) + "-" + roomData;
            }
        }
        public string QuitRoom(string data, Client client, Server server)
        {
            bool isHouseOwner = client.IsHost();
            Room room = client.Room;
            if (isHouseOwner)
            {
                //房主退出，销毁房间
                room.BroadcastMessage(client, ActionCode.QuitRoom, ((int)ReturnCode.Success).ToString());
                room.Close();
                return ((int)ReturnCode.Success).ToString();
            }
            else
            {
                //玩家退出
                client.Room.RemoveClient(client);
                room.BroadcastMessage(client, ActionCode.UpdateRoom, room.GetRoomData());
                return ((int)ReturnCode.Success).ToString();
            }
        }
    }
}
