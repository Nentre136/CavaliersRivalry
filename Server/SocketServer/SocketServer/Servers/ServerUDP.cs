using SocketGameProtocol;
using SocketServer.Manager;
using SocketServer.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketServer.Servers
{
    class ServerUDP
    {
        /// <summary>
        /// ServerUDP单例
        /// </summary>
        public static ServerUDP Instance
        {
            get
            {
                return _instance;
            }
        }
        private static ServerUDP _instance;
        // UDP接套字
        Socket socketUDP;
        // 本地监听ip
        IPEndPoint bindEP;
        // 远程ip
        EndPoint remoteEP;
        // TCP的服务端
        Server server;

        // 消息缓存
        Byte[] buffer = new Byte[1024];
        // 接收消息线程
        Thread receiveThread;

        public ServerUDP(int port)
        {
            if (_instance == null)
                _instance = this;
            this.server = Server.Instance;
            // 创建UDP接套字 消息类型为数据报(Dgram) 协议为UDP
            socketUDP = new Socket(AddressFamily.InterNetwork,SocketType.Dgram, ProtocolType.Udp);
            bindEP = new IPEndPoint(IPAddress.Any, port);
            // 防止为null，临时占值
            remoteEP = (IPEndPoint)bindEP;
            socketUDP.Bind(bindEP);
            // 创建线程使用 ReceiveMesage 方法 可以不占用主线程
            receiveThread = new Thread(ReceiveMesaage);
            // 开始执行
            receiveThread.Start();
            Console.WriteLine("UDP服务已启动成功...");
        }
        ~ServerUDP()
        {
            if(receiveThread != null)
            {
                // 终止线程执行
                receiveThread.Abort();
                receiveThread = null;
            }
        }
        /// <summary>
        /// 接收消息
        /// </summary>
        public void ReceiveMesaage()
        {
            while(true)
            {
                // 将接收数据传入buffer中 返回数据长度
                // 并将消息发送方的EndPoint存入remoteEP中!
                int len = socketUDP.ReceiveFrom(buffer, ref remoteEP);
                // 将字节数据转换为protobuf定义的消息包
                // 由于UDP不存在粘包现象 所以不需要像TCP一样拆包处理
                MainPack pack = (MainPack) MainPack.Descriptor.Parser.ParseFrom(buffer,0,len);
                HandleRequest(pack, remoteEP);
            }
        }
        /// <summary>
        /// 处理消息
        /// </summary>
        /// <param name="pack"></param>
        /// <param name="ipEndPoint"></param>
        public void HandleRequest(MainPack pack,EndPoint ipEndPoint)
        {
            Client client = server.GetClient(pack.UserName);
            if (client.IEP == null)
            {
                client.IEP = ipEndPoint;
            }
            server.controllerManager.HandleRequest(pack, client,true);
        }
        /// <summary>
        /// UDP协议发送消息 将pack转换为数据报发送到point对应客户端ip地址
        /// </summary>
        /// <param name="pack"></param>
        /// <param name="point"></param>
        public void SendUDP(MainPack pack,EndPoint point)
        {
            byte[] buffer = Message.PackToDataUDP(pack);
            socketUDP.SendTo(buffer, buffer.Length, SocketFlags.None, point);
        }
    }
}
