using SocketGameProtocol;
using SocketServer.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketServer.Controller
{
    class GameController : BaseController
    {
        public GameController()
        {
            requestCode = RequestCode.Game;
        }
        /// <summary>
        /// 更新角色Transform
        /// </summary>
        /// <param name="client"></param>
        /// <param name="pack"></param>
        /// <returns></returns>
        public MainPack UpTransform(Server server, Client client, MainPack pack)
        {
            Room room = server.roomDict[pack.Str];
            room.BroadCastUpState(client,pack);
            return null;
        }
        /// <summary>
        /// 设置动画
        /// </summary>
        /// <param name="server"></param>
        /// <param name="client"></param>
        /// <param name="pack"></param>
        /// <returns></returns>
        public MainPack SetAnimator(Server server, Client client, MainPack pack)
        {
            Room room = server.roomDict[pack.RoomPack[0].RoomName];
            room.BroadCast(client, pack);
            return null;
        }
        /// <summary>
        /// 退出游戏
        /// </summary>
        /// <param name="server"></param>
        /// <param name="client"></param>
        /// <param name="pack"></param>
        /// <returns></returns>
        public MainPack ExitGame(Server server, Client client, MainPack pack)
        {
            Room room = server.roomDict[pack.Str];
            pack.RequestCode = RequestCode.Game;
            pack.ActionCode = ActionCode.ExitGame;
            pack.Str = client.userName;
            client.inGame = false;
            room.BroadCastExitGame(client, pack);
            return null;
        }
    }
}
