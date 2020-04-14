using GameServer.Common;
using GameServer.Controller;
using GameServer.DAO;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace GameServer.Servers
{
    class Server
    {
        //ip地址
        private IPEndPoint iPEndPoint;
        private Socket serverSocket;
        //存储用来响应连接的client
        private List<Client> clientList = new List<Client>();
        //所有的房间
        private List<Room> roomList = new List<Room>();
        private ControllerManager controllerManager;
        //保存所有已登录的用户，用来验证是否重复登录
        private List<int> userIdList = new List<int>();
        public static readonly object objLock = new object();
        public List<Room> RoomList { get => roomList; set => roomList = value; }

        private CoinDAO coinDAO = new CoinDAO();
        private ItemPriceDAO itemPriceDAO = new ItemPriceDAO();
        private PlayerStateDAO playerStateDAO = new PlayerStateDAO();
        private ResultDAO resultDAO = new ResultDAO();
        private RoleShopStateDAO roleShopStateDAO = new RoleShopStateDAO();
        private ShopStateDAO shopStateDAO = new ShopStateDAO();
        private UserDAO userDAO = new UserDAO();


        public Server() { }
        public Server(string ipStr, int port)
        {
            controllerManager = new ControllerManager(this);
            SetIPAndPort(ipStr, port);
        }
        public void SetIPAndPort(string ipStr, int port)
        {
            iPEndPoint = new IPEndPoint(IPAddress.Parse(ipStr), port);
        }
        public void Start()
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //绑定ip
            serverSocket.Bind(iPEndPoint);
            //不限制客户端数量
            serverSocket.Listen(0);
            Console.WriteLine("||********服务器已启动，等待客户端连接...********||\n");
            //异步接受传入的连接
            serverSocket.BeginAccept(AcceptCallBack, null);
        }
        private void AcceptCallBack(IAsyncResult ar)
        {
            Console.WriteLine("|****客户端已连接,创建新的client来响应****|");
            //创建一个单独的客户端进行处理
            Socket clientSocket = serverSocket.EndAccept(ar);
            Client client = new Client(clientSocket, this,coinDAO,itemPriceDAO,playerStateDAO,resultDAO,roleShopStateDAO,shopStateDAO,userDAO);
            //启动客户端
            client.Start();
            clientList.Add(client);
            //异步接受传入的连接
            serverSocket.BeginAccept(AcceptCallBack, null);
        }
        public void RemoveClient(Client client)
        {
            lock (objLock)
            {
                clientList.Remove(client);
            }
        }
        //给客户端响应
        public void SendResponse(Client client, ActionCode actionCode, string data)
        {
            client.Send(actionCode, data);
        }
        public void HandleRequest(RequestCode requestCode, ActionCode actionCode, string data, Client client)
        {
            controllerManager.HandleRequest(requestCode, actionCode, data, client);
        }
        /// <summary>
        /// 创建房间
        /// </summary>
        /// <param name="client">房主</param>
        public void CreateRoom(Client client)
        {
            lock (objLock)
            {               
                Room room = new Room(this);
                room.AddClient(client);
                roomList.Add(room);
            }
        }
        public void RemoveRoom(Room room)
        {
            lock (objLock)
            {
                if (roomList != null && room != null)
                {
                    roomList.Remove(room);
                }
            }           
        }
        public Room GetRoomById(int id)
        {
            foreach (Room room in roomList)
            {
                if (room.GetId() == id)
                {
                    return room;
                }
            }
            return null;
        }
        public bool RemoveUser(int id)
        {
            lock (objLock)
            {
                return userIdList.Remove(id);
            }
        }
        public void AddUser(int id)
        {
            lock (objLock)
            {
                userIdList.Add(id);
            }
        }
        public bool IsLogin(int id)
        {
            lock (objLock)
            {
                return userIdList.Contains(id);
            }
        }
    }
}
