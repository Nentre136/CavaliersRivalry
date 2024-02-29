using SocketGameProtocol;
using SocketServer.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace SocketServer.Controller
{
    class GameHallContorller : BaseController
    {
        public GameHallContorller()
        {
            requestCode = RequestCode.GameHall;
        }
        /// <summary>
        /// 创建房间
        /// </summary>
        /// <param name="server"></param>
        /// <param name="client"></param>
        /// <param name="pack"></param>
        /// <returns></returns>
        public MainPack CreateRoom(Server server, Client client, MainPack pack)
        {
            // 创建房间
            Room room = new Room(client);
            // 创建成功
            if (room.CreateRoom(pack.RoomPack[0].RoomName, pack.RoomPack[0].MaxCount))
            {
                pack.RoomPack[0] = room.roomPack;
                client.roomName = room.roomPack.RoomName;
                pack.ReturnCode = ReturnCode.Succeed;
            }
            else
            {
                // 房间名字重复
                pack.ReturnCode = ReturnCode.Fail;
            }
            return pack;
        }
        /// <summary>
        /// 查询房间
        /// </summary>
        /// <param name="server"></param>
        /// <param name="client"></param>
        /// <param name="pack"></param>
        /// <returns></returns>
        public MainPack FindRoom(Server server, Client client, MainPack pack)
        {
            // 查询房间信息只有一个房间 只取首位
            // 查询名为空 随机返回12个已存在房间
            List<Room> resultRoomList = Server.Instance.GetRoomList;
            if (pack.RoomPack[0].RoomName == "" && resultRoomList.Count>0)
            {
                // 查询名为空的房间清除掉，避免返回空房间
                pack.RoomPack.Clear();
                // 已存在房间不超过12个全部添加
                if (resultRoomList.Count <= 12)
                {
                    foreach (Room room in resultRoomList)
                    {
                        pack.RoomPack.Add(room.roomPack);
                        // 记录房主
                        RoomPlayerPack playerPack = new RoomPlayerPack();
                        playerPack.PlayerName = room.clientList[0].userName;
                        pack.RoomPlayerPack.Add(playerPack);
                    }
                }
                else// 大于12个随机返回12个
                {
                    Random random = new Random();
                    for(int i =0; i < 12; i++)
                    {
                        int randomIndex = random.Next(resultRoomList.Count);
                        pack.RoomPack.Add(resultRoomList[randomIndex].roomPack);
                        // 记录房主
                        RoomPlayerPack playerPack = new RoomPlayerPack();
                        playerPack.PlayerName = resultRoomList[randomIndex].clientList[0].userName;
                        pack.RoomPlayerPack.Add(playerPack);
                        // 赋值完删除
                        resultRoomList.RemoveAt(randomIndex);
                    }
                }
                pack.ReturnCode = ReturnCode.Succeed;
                pack.Str = client.userName;
                return pack;
            }

            // 当房间名不为空内容
            foreach (Room room in resultRoomList)
            {
                if (room.roomPack.RoomName == pack.RoomPack[0].RoomName)
                {
                    pack.RoomPack.Clear();
                    pack.ReturnCode = ReturnCode.Succeed;
                    pack.RoomPack.Add(room.roomPack);
                    Console.WriteLine("找到了名字为'" + pack.RoomPack[0].RoomName + "'的房间");
                    return pack;
                }
            }
            pack.ReturnCode = ReturnCode.Fail;
            Console.WriteLine("未找到名字为'" + pack.RoomPack[0].RoomName + "'的房间");
            return pack;
        }
        /// <summary>
        /// 加入房间
        /// </summary>
        /// <param name="server"></param>
        /// <param name="client"></param>
        /// <param name="pack"></param>
        /// <returns></returns>
        public MainPack JoinRoom(Server server, Client client, MainPack pack)
        {
            foreach (Room room in server.GetRoomList)
            {
                if(room.roomPack.RoomName == pack.RoomPack[0].RoomName)
                {
                    // 可加入状态
                    if (room.JoinRoom())
                    {
                        pack.ReturnCode = ReturnCode.Succeed;
                        pack.RoomPack[0] = room.roomPack;
                        // 加入房间玩家列表
                        room.clientList.Add(client);
                        room.clientIsSetout[client] = false;
                        client.roomName = room.roomPack.RoomName;
                        pack.Str = client.userName + "加入了房间\n";
                        // 向房间内其他用户广播client加入了房间
                        room.BroadCastUpdataPlayerList(client, pack);
                        return pack;
                    }
                    else // 状态不可加入
                        break;
                }
            }
            // 加入失败
            pack.ReturnCode= ReturnCode.Fail;
            return pack;
        }
    }
}
