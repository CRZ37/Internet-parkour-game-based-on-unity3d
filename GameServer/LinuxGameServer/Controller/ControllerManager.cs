using GameServer.Common;
using GameServer.Servers;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace GameServer.Controller
{
    /// <summary>
    /// 把消息分发给Controller
    /// </summary>
    class ControllerManager
    {
        private Dictionary<RequestCode, BaseController> controllerDict = new Dictionary<RequestCode, BaseController>();
        private Server server;
        public ControllerManager(Server server)
        {
            this.server = server;
            InitController();
        }
        void InitController()
        {
            DefaultController defaultController = new DefaultController();
            controllerDict.Add(defaultController.RequestCode, defaultController);
            controllerDict.Add(RequestCode.User, new UserController());
            controllerDict.Add(RequestCode.Room, new RoomController());
            controllerDict.Add(RequestCode.Game, new GameController());
            controllerDict.Add(RequestCode.Shop, new ShopController());
            controllerDict.Add(RequestCode.RoleShop, new RoleShopController());
        }
        public void HandleRequest(RequestCode requestCode, ActionCode actionCode, string data, Client client)
        {
            Console.WriteLine("数据解析完成，开始HandleRequest...");
            BaseController controller;
            bool isGet = controllerDict.TryGetValue(requestCode, out controller);
            //如果参数给错了
            if (!isGet)
            {
                Console.WriteLine("无法得到" + requestCode + "对应的controller");
                return;
            }
            //获取方法名
            string methodName = Enum.GetName(typeof(ActionCode), actionCode);
            //通过反射得到方法
            MethodInfo mi = controller.GetType().GetMethod(methodName);
            //调用方法
            if (mi == null)
            {
                Console.WriteLine("方法写错了,在[" + controller.GetType() + "]中没有[" + methodName + "]方法");
                return;
            }
            object[] param = new object[] { data, client, server };
            object o = mi.Invoke(controller, param);
            //如果返回值为null或为""，不返回给客户端
            if (o == null || string.IsNullOrEmpty(o as string))
            {
                return;
            }
            server.SendResponse(client, actionCode, o as string);
            Console.WriteLine("数据已返回");
        }
    }
}
