using SocketGameProtocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class ClientManager : BaseManager
{
    #region TCP服务
    private Socket socket;
    private Message message;
    // 服务端ip地址
    private string serveIp = "127.0.0.1";
    private int port = 8888;
    /// <summary>
    /// 用户登录名
    /// </summary>
    public string userName;
    public ClientManager(GameFace face) : base(face)
    {
    }
    public override void OnInit()
    {
        base.OnInit();
        message = new Message();
        // 连接服务端TCP
        InitSocket();
        // 等待接收消息
        StartRecevie();

        // 初始化UDP服务
        InitUDP();
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        CloseSocket();
    }
    /// <summary>
    /// 初始化socket
    /// </summary>
    private void InitSocket()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            // 连接服务端
            socket.Connect(serveIp, port);
            face.uiManager.messageTips.ShowTips("连接服务器成功");
        }
        catch(Exception e)
        {
            // 连接失败报错
            face.uiManager.messageTips.ShowTips("服务器连接失败");
            Debug.LogException(e);
        }
    }
    /// <summary>
    /// 关闭socket
    /// </summary>
    private void CloseSocket()
    {
        if(socket != null && socket.Connected) 
        { 
            socket.Close();
            Console.WriteLine("Close");
        }
    }
    /// <summary>
    /// 等待接收消息
    /// </summary>
    private void StartRecevie()
    {
        socket.BeginReceive(message.Buffer, message.StartIndex,
            message.RemSize,SocketFlags.None,
            RecevieCallback, null);
    }
    private void RecevieCallback(IAsyncResult iar)
    {
        try
        {
            if(socket==null || !socket.Connected)
                return;
            int len = socket.EndReceive(iar);
            if (len <= 0)
            {
                CloseSocket();
                return;
            }
            // 解析消息 回调函数的执行是异步处理的
            message.ReadBuffer(len, HandleResponse);
            StartRecevie();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        
    }
    /// <summary>
    /// 解析接收到消息后的回调函数
    /// </summary>
    /// <param name="pack"></param>
    private void HandleResponse(MainPack pack)
    {
        face.requestManager.HandleResponse(pack);
    }
    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="pack"></param>
    public void Send(MainPack pack)
    {
        try
        {
            socket.Send(Message.PackToData(pack));
        }
        catch(Exception e)
        {
            Debug.LogException(e);
            face.uiManager.messageTips.ShowTips("未检测到连接到服务器，请检查网络设置");
        }
    }
    #endregion

    #region UDP服务
    // UDP接套字
    private Socket socketUDP;
    private IPEndPoint iPEndPoint;
    // 服务端ip 用于向服务端发送数据报
    private EndPoint serverEndPoint;
    // 消息缓存
    private byte[] buffer = new byte[1024];
    // 接收消息线程
    private Thread receiveThread;
    /// <summary>
    /// UDP服务初始化
    /// </summary>
    private void InitUDP()
    {
        // 创建UDP接套字 消息类型为数据报(Dgram) 协议为UDP
        socketUDP = new Socket(AddressFamily.InterNetwork,SocketType.Dgram, ProtocolType.Udp);
        iPEndPoint = new IPEndPoint(IPAddress.Parse(serveIp), port);
        serverEndPoint = iPEndPoint;
        try
        {
            // 给UDP套接字设置默认的服务端ip地址和端口号
            // 并尝试进行一次测试通信
            socketUDP.Connect(serverEndPoint);
        }
        catch
        {
            Debug.Log("UDP连接失败!");
            return;
        }
        // 创建一个线程接收消息
        receiveThread = new Thread(ReceiveMessage);
        receiveThread.Start();
    }
    /// <summary>
    /// 接收消息
    /// </summary>
    private void ReceiveMessage()
    {
        while (true)
        {
            int len = socketUDP.ReceiveFrom(buffer, ref serverEndPoint);
            MainPack pack = (MainPack)MainPack.Descriptor.Parser.ParseFrom(buffer,0,len);
            // 解析pack包
            HandleResponse(pack);
        }
    }
    /// <summary>
    /// 使用UDP协议发送消息到服务端
    /// </summary>
    /// <param name="pack"></param>
    public void SendUDP(MainPack pack)
    {
        byte[] buffer = Message.PackToDataUDP(pack);
        // Send与Send的区别为：使用Send时必须进行socketUDP.Connect操作 将套接字连接到目标远程端点
        // 而SendTo需要指定目标远程端点的 EndPoint，不需要进行socketUDP.Connect操作
        socketUDP.Send(buffer, buffer.Length, SocketFlags.None);
    }
    #endregion
}
