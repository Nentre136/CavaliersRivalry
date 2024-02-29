using SocketGameProtocol;
using SocketServer.Controller;
using SocketServer.DB;
using SocketServer.Tool;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketServer.Servers
{
    // 管理服务器客户端类
    class Client
    {
        private Server server;
        private Socket socket;
        private Message message;
        private UserData _userData;
        public string userName;
        public string roomName= "";
        public bool inGame = false;
        /// <summary>
        /// client对应的客户端ip地址
        /// </summary>
        public EndPoint IEP {  get; set; }
        /// <summary>
        /// 数据库
        /// </summary>
        public UserData UserData { get { return _userData; } }
        public Client(Socket socket,Server server) 
        {
            _userData = new UserData();
            message = new Message();

            this.socket = socket;
            this.server = server;
            StartRecevie();
        }
        private void StartRecevie()
        {
            socket.BeginReceive(message.Buffer, message.StartIndex,
                message.RemSize, SocketFlags.None,
                RecevieCallback,null);
        }
        private void RecevieCallback(IAsyncResult iar)
        {
            try// 捕获异常 防止客户端非正常退出报错卡住进程
            {
                // 检测是否断开连接 或 连接为空
                if (socket == null || socket.Connected == false)
                    return;
                
                int len = socket.EndReceive(iar);
                // 断开连接会返回len==0
                if (len == 0)
                {
                    // 非正常退出情况下 若用户在房间内 需要退出房间
                    ExitGame();
                    Close();
                    return;
                }
                // 解析消息
                message.ReadBuffer(len,HandleRequest);
                StartRecevie();
            }
            catch
            {
            }
        }
        /// <summary>
        /// 解析消息回调函数
        /// </summary>
        /// <param name="pack"></param>
        public void HandleRequest(MainPack pack)
        {
            Console.WriteLine(pack.RequestCode.ToString()+" "+pack.ActionCode.ToString());
            server.controllerManager.HandleRequest(pack, this);
        }
        /// <summary>
        /// 向客户端发送信息 TCP协议
        /// </summary>
        /// <param name="pack"></param>
        public void Send(MainPack pack)
        {
            // 通过与客户端连接的socket发送字节流信息
            socket.Send(Message.PackToData(pack));
        }
        /// <summary>
        /// 向客户端发送信息 TCP协议
        /// </summary>
        /// <param name="pack"></param>
        public void SendUDP(MainPack pack)
        {
            // 对应客户端的地址不能为空，否则无法发送到client对应的客户端手中
            if (IEP == null)
                return;
            // 将pack发送到客户端
            ServerUDP.Instance.SendUDP(pack, IEP);
        }
        public void Close()
        {
            Console.WriteLine("用户："+userName+" 断开连接");
            server.RemoveClient(this);
            socket.Close();
        }
        private void ExitGame()
        {
            if (inGame)
            {
                // 退出游戏逻辑
                GameController gameController = server.controllerManager.controllerDict[RequestCode.Game] as GameController;
                MainPack pack = new MainPack();
                pack.Str = roomName;
                gameController.ExitGame(server, this, pack);
                return;
            }
            // 当前用户位于房间内
            if (roomName != "")
            {
                RoomController roomController = server.controllerManager.controllerDict[RequestCode.Room] as RoomController;
                MainPack pack = new MainPack();
                pack.Str = roomName;
                roomController.ExitRoom(server,this,pack);
            }
        }
    }
}
