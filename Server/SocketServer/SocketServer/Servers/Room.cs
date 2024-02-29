using Google.Protobuf.Collections;
using MySqlX.XDevAPI;
using SocketGameProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketServer.Servers
{
    class Room
    {
        public RoomPack roomPack;
        /// <summary>
        /// 房间用于列表第一位为房主
        /// </summary>
        public List<Client> clientList;
        /// <summary>
        /// 记录玩家是否有准备
        /// </summary>
        public Dictionary<Client, bool> clientIsSetout;
        /// <summary>
        /// 存储所有聊天记录
        /// </summary>
        public string allMessage;
        /// <summary>
        /// 创建房间 对房间基础信息做初始化
        /// </summary>
        /// <param name="client"></param>
        public Room(Client client) 
        {
            roomPack = new RoomPack();
            roomPack.CurCount = 1;
            roomPack.RoomState = 1;
            clientList = new List<Client>();
            clientIsSetout = new Dictionary<Client, bool>();
            clientIsSetout[client] = false;
            clientList.Add(client);
        }
        public bool CreateRoom(string roomName, int maxCount)
        {
            // 房间名未被创建
            if (!Server.Instance.roomDict.ContainsKey(roomName))
            {
                roomPack.RoomName = roomName;
                roomPack.MaxCount = maxCount;
                Server.Instance.roomDict[roomName] = this;
                Server.Instance.AddRoomList(this);
                return true;
            }
            else// 房间名已被创建过 创建失败
                return false;
        }
        /// <summary>
        /// 加入房间返回是否能成功加入
        /// </summary>
        /// <returns></returns>
        public bool JoinRoom()
        {
            // 房间状态不能加入
            if(roomPack.RoomState!=1)
                return false;
            // 人数满了
            if (roomPack.CurCount == roomPack.MaxCount)
            {
                return false;
            }
            else if(roomPack.CurCount+1 == roomPack.MaxCount)
            {
                roomPack.CurCount++;
                roomPack.RoomState = 2;
                return true;
            }
            else
            {
                roomPack.CurCount++;
                return true;
            }
                
        }
        /// <summary>
        /// TCP 房间内向client以外的用户广播SendPack
        /// </summary>
        /// <param name="client"></param>
        /// <param name="sendPack"></param>
        public void BroadCast(Client client,MainPack sendPack)
        {
            foreach (Client c in clientList)
            {
                if (c != client)
                {
                    c.Send(sendPack);
                }
            }
        }
        /// <summary>
        /// 向除了client以外的用户广播刷新玩家列表
        /// </summary>
        /// <param name="client"></param>
        /// <param name="pack"></param>
        public void BroadCastUpdataPlayerList(Client client,MainPack pack)
        {
            MainPack sendPack = new MainPack();
            sendPack.RequestCode = RequestCode.Room;
            sendPack.ActionCode = ActionCode.UpdataPlayerList;
            sendPack.Str = pack.Str;
            sendPack.RoomPack.Add(roomPack);
            foreach (Client c in clientList)
            {
                RoomPlayerPack roomPlayer = new RoomPlayerPack();
                roomPlayer.PlayerName = c.userName;
                roomPlayer.PlayerState = clientIsSetout[c];
                sendPack.RoomPlayerPack.Add(roomPlayer);
                pack.RoomPlayerPack.Add(roomPlayer);
            }
            BroadCast(client, sendPack);
        }
        /// <summary>
        /// 向除了client以外的用户广播聊天信息
        /// </summary>
        /// <param name="client"></param>
        /// <param name="pack"></param>
        public void BroadCastMessage(Client client , MainPack pack)
        {
            MainPack sendPack = new MainPack();
            sendPack = pack;
            sendPack.ReturnCode = ReturnCode.Succeed;
            BroadCast(client, sendPack);
        }
        /// <summary>
        /// 向client以外的用户广播准备信息
        /// </summary>
        /// <param name="client"></param>
        /// <param name="pack"></param>
        public void BroadCastSetout(Client client,MainPack pack)
        {
            MainPack sendPack = new MainPack();
            sendPack = pack;
            // 当前用户未准备 则准备
            if (!clientIsSetout[client])
            {
                sendPack.ReturnCode = ReturnCode.Succeed;
                clientIsSetout[client] = true;
            }
            else
            {
                sendPack.ReturnCode = ReturnCode.Fail;
                clientIsSetout[client] = false;
            }
            BroadCast(client, sendPack);
        }
        /// <summary>
        /// 向client以外的用户广播开始游戏
        /// </summary>
        /// <param name="client"></param>
        /// <param name="pack"></param>
        public void BroadCastStartGame(Client client, MainPack pack)
        {
            // 存入玩家列表
            foreach (Client c in clientList)
            {
                RoomPlayerPack playerPack = new RoomPlayerPack();
                playerPack.PlayerName = c.userName;
                pack.RoomPlayerPack.Add(playerPack);
                c.inGame = true;
            }
            MainPack sendPack = pack;
            BroadCast(client, sendPack);
            roomPack.RoomState = 3;
        }
        /// <summary>
        /// 向client以外的用用户广播退出游戏
        /// </summary>
        /// <param name="client"></param>
        /// <param name="pack"></param>
        public void BroadCastExitGame(Client client,MainPack pack)
        {
            foreach(Client c in clientList)
            {
                if(c != client)
                    c.Send(pack);
            }
            clientList.Remove(client);
            roomPack.CurCount--;
            if (clientList.Count <= 0)
                Server.Instance.RemoveRoom(this);
        }
        /// <summary>
        /// 向client以外的用户广播信息同步 UDP
        /// </summary>
        /// <param name="client"></param>
        /// <param name="pack"></param>
        public void BroadCastUpState(Client client,MainPack pack)
        {
            foreach (Client c in clientList)
            {
                if (c != client)
                {
                    c.SendUDP(pack);
                }
            }
        }
    }
}
