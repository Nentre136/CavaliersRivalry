using SocketGameProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpStateRequest : BaseRequest
{
    private MainPack responsePack;
    public override void Start()
    {
        requestCode = RequestCode.Game;
        actionCode = ActionCode.UpTransform;
        base.Start();
    }
    void Update()
    {
        if(responsePack != null)
        {
            if (responsePack.ActionCode == ActionCode.UpTransform)
                face.gameManager.SyncCharacState(responsePack);
            responsePack = null;
        }
    }
    public override void OnResponse(MainPack pack)
    {
        responsePack = pack;
    }
    /// <summary>
    /// 发送状态同步请求
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="rota"></param>
    /// <param name="RigRota"></param>
    public void SendStateRequest(Vector3 pos, Vector3 rota, Vector3 RigRota, PlayerInfo infor)
    {
        MainPack pack = new MainPack();
        RoomPlayerPack roomPlayerPack = new RoomPlayerPack();
        CharaStatePack charaStatePack = new CharaStatePack();
        charaStatePack.PosX = pos.x;
        charaStatePack.PosY = pos.y;
        charaStatePack.PosZ = pos.z;
        charaStatePack.RotaX = rota.x;
        charaStatePack.RotaY = rota.y;
        charaStatePack.RotaZ = rota.z;
        charaStatePack.RigRotaX = RigRota.x;
        charaStatePack.RigRotaY = RigRota.y;
        charaStatePack.RigRotaZ = RigRota.z;
        charaStatePack.Health = infor.Health;
        roomPlayerPack.PlayerName = face.clientManager.userName;
        roomPlayerPack.CharaStatePack = charaStatePack;
        pack.RoomPlayerPack.Add(roomPlayerPack);
        pack.RequestCode = requestCode;
        pack.ActionCode = ActionCode.UpTransform;
        pack.Str = face.gameManager.roomName;
        base.SendRequestUDP(pack);
    }
}
