using GameServer.Servers;
using System;
namespace GameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            //服务器：（公网）139.199.133.2（私网）172.21.0.13
            //127.0.0.1
            Server server = new Server("127.0.0.1", 6666);
            server.Start();
            Console.ReadKey();
        }
    }
}
