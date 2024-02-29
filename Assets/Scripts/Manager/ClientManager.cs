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
    #region TCP����
    private Socket socket;
    private Message message;
    // �����ip��ַ
    private string serveIp = "127.0.0.1";
    private int port = 8888;
    /// <summary>
    /// �û���¼��
    /// </summary>
    public string userName;
    public ClientManager(GameFace face) : base(face)
    {
    }
    public override void OnInit()
    {
        base.OnInit();
        message = new Message();
        // ���ӷ����TCP
        InitSocket();
        // �ȴ�������Ϣ
        StartRecevie();

        // ��ʼ��UDP����
        InitUDP();
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        CloseSocket();
    }
    /// <summary>
    /// ��ʼ��socket
    /// </summary>
    private void InitSocket()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            // ���ӷ����
            socket.Connect(serveIp, port);
            face.uiManager.messageTips.ShowTips("���ӷ������ɹ�");
        }
        catch(Exception e)
        {
            // ����ʧ�ܱ���
            face.uiManager.messageTips.ShowTips("����������ʧ��");
            Debug.LogException(e);
        }
    }
    /// <summary>
    /// �ر�socket
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
    /// �ȴ�������Ϣ
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
            // ������Ϣ �ص�������ִ�����첽�����
            message.ReadBuffer(len, HandleResponse);
            StartRecevie();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        
    }
    /// <summary>
    /// �������յ���Ϣ��Ļص�����
    /// </summary>
    /// <param name="pack"></param>
    private void HandleResponse(MainPack pack)
    {
        face.requestManager.HandleResponse(pack);
    }
    /// <summary>
    /// ������Ϣ
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
            face.uiManager.messageTips.ShowTips("δ��⵽���ӵ���������������������");
        }
    }
    #endregion

    #region UDP����
    // UDP������
    private Socket socketUDP;
    private IPEndPoint iPEndPoint;
    // �����ip ���������˷������ݱ�
    private EndPoint serverEndPoint;
    // ��Ϣ����
    private byte[] buffer = new byte[1024];
    // ������Ϣ�߳�
    private Thread receiveThread;
    /// <summary>
    /// UDP�����ʼ��
    /// </summary>
    private void InitUDP()
    {
        // ����UDP������ ��Ϣ����Ϊ���ݱ�(Dgram) Э��ΪUDP
        socketUDP = new Socket(AddressFamily.InterNetwork,SocketType.Dgram, ProtocolType.Udp);
        iPEndPoint = new IPEndPoint(IPAddress.Parse(serveIp), port);
        serverEndPoint = iPEndPoint;
        try
        {
            // ��UDP�׽�������Ĭ�ϵķ����ip��ַ�Ͷ˿ں�
            // �����Խ���һ�β���ͨ��
            socketUDP.Connect(serverEndPoint);
        }
        catch
        {
            Debug.Log("UDP����ʧ��!");
            return;
        }
        // ����һ���߳̽�����Ϣ
        receiveThread = new Thread(ReceiveMessage);
        receiveThread.Start();
    }
    /// <summary>
    /// ������Ϣ
    /// </summary>
    private void ReceiveMessage()
    {
        while (true)
        {
            int len = socketUDP.ReceiveFrom(buffer, ref serverEndPoint);
            MainPack pack = (MainPack)MainPack.Descriptor.Parser.ParseFrom(buffer,0,len);
            // ����pack��
            HandleResponse(pack);
        }
    }
    /// <summary>
    /// ʹ��UDPЭ�鷢����Ϣ�������
    /// </summary>
    /// <param name="pack"></param>
    public void SendUDP(MainPack pack)
    {
        byte[] buffer = Message.PackToDataUDP(pack);
        // Send��Send������Ϊ��ʹ��Sendʱ�������socketUDP.Connect���� ���׽������ӵ�Ŀ��Զ�̶˵�
        // ��SendTo��Ҫָ��Ŀ��Զ�̶˵�� EndPoint������Ҫ����socketUDP.Connect����
        socketUDP.Send(buffer, buffer.Length, SocketFlags.None);
    }
    #endregion
}
