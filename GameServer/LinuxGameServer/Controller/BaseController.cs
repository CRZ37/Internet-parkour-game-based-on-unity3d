using GameServer.Common;
using GameServer.Servers;

namespace GameServer.Controller
{
    abstract class BaseController
    {
        //默认请求类型,只读
        protected RequestCode requestCode = RequestCode.None;

        public RequestCode RequestCode { get => requestCode; }

        //默认处理方法,是否实现可选
        public virtual string DefaultHandle(string data, Client client, Server server)
        {
            return null;
        }
    }
}
