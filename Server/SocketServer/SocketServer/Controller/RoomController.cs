using SocketGameProtocol;
using SocketServer.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketServer.Controller
{
    class RoomController : BaseController
    {
        public RoomController()
        {
            requestCode = RequestCode.Room;
        }
        /// <summary>
        /// 服务端退出房间请求响应
        /// </summary>
        /// <param name="server"></param>
        /// <param name="client"></param>
        /// <param name="pack"></param>
        /// <returns></returns>
        public MainPack ExitRoom(Server server, Client client, MainPack pack)
        {
            Room room = server.roomDict[pack.Str];
            if (room.roomPack.CurCount > 1)
            {
                room.roomPack.CurCount--;
                room.roomPack.RoomState = 1;
                room.clientList.Remove(client);
                room.clientIsSetout.Remove(client);
                client.roomName = "";
                pack.Str = client.userName + "退出了房间\n";
                // 向房间内其他用户广播client退出房间
                room.BroadCastUpdataPlayerList(client, pack);
            }
            else // 最后一个人退出，销毁房间
            {
                server.RemoveRoom(room);
            }
            // 找到对应房间则退出索引
            pack.ReturnCode = ReturnCode.Succeed;
            return pack;
        }
        /// <summary>
        /// 服务端聊天请求响应
        /// </summary>
        /// <param name="server"></param>
        /// <param name="client"></param>
        /// <param name="pack"></param>
        /// <returns></returns>
        public MainPack Chat(Server server, Client client, MainPack pack)
        {
            foreach(Room room in server.GetRoomList)
            {
                if(room.roomPack.RoomName == pack.RoomPack[0].RoomName)
                {
                    room.allMessage += pack.Str;
                    room.BroadCastMessage(client, pack);
                    // 发送成功不需要返回信息
                    return null;
                }
            }
            // 发送失败需要返回消息
            pack.ReturnCode = ReturnCode.Fail;
            return pack;
        }
        /// <summary>
        /// 服务端准备请求响应
        /// </summary>
        /// <param name="server"></param>
        /// <param name="client"></param>
        /// <param name="pack"></param>
        /// <returns></returns>
        public MainPack Setout(Server server, Client client, MainPack pack)
        {
            Room room = server.roomDict[pack.RoomPack[0].RoomName];
            room.BroadCastSetout(client,pack);
            return null;
        }
        /// <summary>
        /// 服务端对开始游戏请求响应
        /// </summary>
        /// <param name="server"></param>
        /// <param name="client"></param>
        /// <param name="pack"></param>
        /// <returns></returns>
        public MainPack StartGame(Server server, Client client, MainPack pack)
        {
            Room room = server.roomDict[pack.RoomPack[0].RoomName];
            if(client != room.clientList[0])
            {
                pack.Str = "只有房主才能开始游戏";
                pack.ReturnCode = ReturnCode.Fail;
                return pack;
            }

            foreach(Client c in room.clientList)
            {
                // 有玩家未准备
                if (!room.clientIsSetout[c])
                {
                    pack.ReturnCode = ReturnCode.Fail;
                    pack.Str = "有玩家尚未准备";
                    return pack;
                }
            }
            // 所有玩家准备完成
            pack.ReturnCode = ReturnCode.Succeed;
            // 向房间内其他玩家广播游戏开始
            room.BroadCastStartGame(client, pack);
            return pack;
        }

    }
}
