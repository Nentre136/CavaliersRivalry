using Org.BouncyCastle.Asn1.Ocsp;
using SocketGameProtocol;
using SocketServer.Controller;
using SocketServer.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SocketServer.Manager
{
    class ControllerManager
    {
        /// <summary>
        /// 通过不同请求类型 对应 不同控制器
        /// </summary>
        public Dictionary<RequestCode, BaseController> controllerDict {  get; private set; }

        /// <summary>
        /// 服务端对象
        /// </summary>
        private Server server;

        public ControllerManager(Server server)
        {
            this.server = server;
            // 初始化控制器字典
            controllerDict = new Dictionary<RequestCode, BaseController>();
            UserController userController = new UserController();
            GameHallContorller gameHallController = new GameHallContorller();
            RoomController roomController = new RoomController();
            GameController gameController = new GameController();
            //用户请求 对应 用户控制器
            controllerDict.Add(userController.GetRequestCode, userController);
            controllerDict.Add(gameHallController.GetRequestCode, gameHallController);
            controllerDict.Add(roomController.GetRequestCode, roomController);
            controllerDict.Add(gameController.GetRequestCode, gameController);
        }
        /// <summary>
        /// 处理请求并响应 默认为TCP处理
        /// </summary>
        /// <param name="pack"></param>
        /// <param name="client"></param>
        /// <param name="isUDP"></param>
        public void HandleRequest(MainPack pack, Client client,bool isUDP=false)
        {
            // 通过RequestCode获取对应的控制器
            if (controllerDict.TryGetValue(pack.RequestCode, out BaseController controller))
            {
                string metName = pack.ActionCode.ToString();
                // 通过反射获取对应控制器的方法
                MethodInfo method = controller.GetType().GetMethod(metName);
                if (method == null)
                {
                    Console.WriteLine("没有找到对应处理的方法");
                    return;
                }
                // 反射得到的方法只能用object类型传参
                // 定义方法的参数列表
                object[] param;

                // UDP协议
                if(isUDP)
                {
                    param = new object[] { server, client, pack };
                    method.Invoke(controller, param);
                }
                else// TCP协议
                {
                    param = new object[] { server, client, pack };
                    // 调用反射得到的方法并获得返回值
                    // 参数：调用方法的对象实例  方法参数列表
                    object ret = method.Invoke(controller, param);
                    // 存在方法返回值
                    if (ret != null)
                    {
                        // 将响应返回的信息发送给客户端
                        client.Send(ret as MainPack);
                    }
                }
            }
            else
            {
                Console.WriteLine("没有找到对应的Controller处理");
            }
        }
    }
}
