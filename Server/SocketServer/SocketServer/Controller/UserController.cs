using SocketGameProtocol;
using SocketServer.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketServer.Controller
{
    class UserController : BaseController
    {
        public UserController() 
        {
            requestCode = RequestCode.User;
        }
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="server"></param>
        /// <param name="client"></param>
        /// <param name="pack"></param>
        /// <returns></returns>
        public MainPack Register(Server server,Client client,MainPack pack)
        {
            // 根据是否注册成功修改包信息 再将原包返回
            if (client.UserData.Register(pack))
            {
                pack.ReturnCode = ReturnCode.Succeed;
            }
            else
            {
                pack.ReturnCode = ReturnCode.Fail;
            }
            return pack;
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="server"></param>
        /// <param name="client"></param>
        /// <param name="pack"></param>
        /// <returns></returns>
        public MainPack Login(Server server, Client client, MainPack pack)
        {
            // 根据是否登录成功修改包信息 再将原包返回
            if (client.UserData.Login(pack))
            {
                pack.ReturnCode = ReturnCode.Succeed;
                client.userName = pack.RegisterPack.UserName;
            }
            else
            {
                pack.ReturnCode = ReturnCode.Fail;
            }
            return pack;
        }
    }
}
