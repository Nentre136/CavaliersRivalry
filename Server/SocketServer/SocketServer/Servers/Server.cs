using SocketGameProtocol;
using SocketServer.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketServer.Servers
{
    class Server
    {
        private static Server _instance;
        public static Server Instance
        {
            get
            {
                return _instance;
            }
        }
        private Socket socket;
        /// <summary>
        /// UDP服务端
        /// </summary>
        private ServerUDP serverUDP;
        /// <summary>
        /// 服务端连接的所有用户
        /// </summary>
        private List<Client> clientList = new List<Client>();
        /// <summary>
        /// 服务端存在的所有房间
        /// </summary>
        private List<Room> roomList = new List<Room>();
        /// <summary>
        /// 通过名字可以查找对应的房间
        /// </summary>
        public Dictionary<string,Room> roomDict = new Dictionary<string,Room>();
        public ControllerManager controllerManager;
        public Server(int port)
        {
            if(_instance == null)
                _instance = this;
            controllerManager = new ControllerManager(this);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Any,port));
            // 监听队列长度 0表示无限制
            socket.Listen(0);
            StartAccept();
            Console.WriteLine("TCP服务已启动...");
            // 启动UDP服务
            serverUDP = new ServerUDP(port);
        }
        private void StartAccept()
        {
            // 等待客户端连接
            socket.BeginAccept(AcceptCallback, null);
        }
        private void AcceptCallback(IAsyncResult iar)
        {
            Socket client = socket.EndAccept(iar);

            // 将连接的客户端添加到链表中
            clientList.Add(new Client(client,this));

            StartAccept();
        }
        public void RemoveClient(Client client)
        {
            clientList.Remove(client);
        }
        /// <summary>
        /// 添加房间进入房间列表中
        /// </summary>
        /// <param name="room"></param>
        public void AddRoomList(Room room)
        {
            if (room != null)
            {
                roomList.Add(room);
            }
            else
                Console.WriteLine("添加入房间列表的值为空");
        }
        public void RemoveRoom(Room room)
        {
            if (room != null)
            {
                roomList.Remove(room);
                roomDict.Remove(room.roomPack.RoomName);
            }
            else
                Console.WriteLine("删除房间值为空");
        }
        public List<Room> GetRoomList { get { return roomList; } }

        /// <summary>
        /// 查询userName对应的client
        /// </summary>
        /// <param name="clientName"></param>
        /// <returns></returns>
        public Client GetClient(string userName)
        {
            foreach(Client c in clientList)
            {
                if(c.userName == userName)
                {
                    return c;
                }
            }
            return null;
        }
    }
}
