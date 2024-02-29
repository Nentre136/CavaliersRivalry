using SocketGameProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRequest : BaseRequest
{
    private MainPack responsePack;
    public override void Start()
    {
        requestCode = RequestCode.Game;
        face = GameFace.Instance;
        //actionCode = ActionCode.UpTransform;
        //face.requestManager.AddRequest(this);
        actionCode = ActionCode.SetAnimator;
        face.requestManager.AddRequest(this);
        actionCode = ActionCode.ExitGame;
        face.requestManager.AddRequest(this);
    }
    void Update()
    {
        if (responsePack != null)
        {
            if (responsePack.ActionCode == ActionCode.SetAnimator)
            {
                face.gameManager.SetTargetAnimation(responsePack);
            }
            else if (responsePack.ActionCode == ActionCode.ExitGame)
            {
                face.gameManager.ExitCharacter(responsePack.Str);
            }
            //else if(responsePack.ActionCode == ActionCode.UpTransform)
            //{
            //    face.gameManager.SyncCharacState(responsePack);
            //}
            responsePack = null;
        }
    }
    public override void OnResponse(MainPack pack)
    {
        responsePack = pack;
    }
    ///// <summary>
    ///// 发送状态同步请求
    ///// </summary>
    ///// <param name="pos"></param>
    ///// <param name="rota"></param>
    ///// <param name="RigRota"></param>
    //public void SendStateRequest(Vector3 pos,Vector3 rota,Vector3 RigRota,PlayerInfo infor)
    //{
    //    MainPack pack = new MainPack();
    //    RoomPlayerPack roomPlayerPack = new RoomPlayerPack();
    //    CharaStatePack charaStatePack = new CharaStatePack();
    //    charaStatePack.PosX = pos.x;
    //    charaStatePack.PosY = pos.y;
    //    charaStatePack.PosZ = pos.z;
    //    charaStatePack.RotaX = rota.x;
    //    charaStatePack.RotaY = rota.y;
    //    charaStatePack.RotaZ = rota.z;
    //    charaStatePack.RigRotaX = RigRota.x;
    //    charaStatePack.RigRotaY = RigRota.y;
    //    charaStatePack.RigRotaZ = RigRota.z;
    //    charaStatePack.Health = infor.Health;
    //    roomPlayerPack.PlayerName = face.clientManager.userName;
    //    roomPlayerPack.CharaStatePack = charaStatePack;
    //    pack.RoomPlayerPack.Add(roomPlayerPack);
    //    pack.RequestCode = requestCode;
    //    pack.ActionCode = ActionCode.UpTransform;
    //    pack.Str = face.gameManager.roomName;
    //    base.SendRequestUDP(pack);
    //}
    #region 发送动画请求
    /// <summary>
    /// 发送动画请求 Trigger
    /// </summary>
    /// <param name="animaName"></param>
    /// <param name="nameType"></param>
    public void SendAnimatorRequest(string animaName)
    {
        MainPack pack = new MainPack();
        pack.RequestCode = requestCode;
        pack.ActionCode = ActionCode.SetAnimator;
        RoomPack roomPack = new RoomPack();
        roomPack.RoomName = face.gameManager.roomName;
        pack.RoomPack.Add(roomPack);
        RoomPlayerPack roomPlayerPack = new RoomPlayerPack();
        roomPlayerPack.PlayerName = face.clientManager.userName;
        roomPlayerPack.AnimaName = animaName;
        roomPlayerPack.NameType = "Trigger";
        if(animaName == "Attack_Remote")
        {
            CharaStatePack charaStatePack = new CharaStatePack();
            Vector3 pos = transform.GetComponent<ParticleController>().remoteAttackDirec;
            charaStatePack.PosX = pos.x;
            charaStatePack.PosY = pos.y;
            charaStatePack.PosZ = pos.z;
            roomPlayerPack.CharaStatePack = charaStatePack;
        }
        pack.RoomPlayerPack.Add(roomPlayerPack);
        base.SendRequest(pack);
    }
    /// <summary>
    /// 发送动画请求 Bool
    /// </summary>
    /// <param name="animaName"></param>
    /// <param name="nameType"></param>
    /// <param name="boolParam"></param>
    public void SendAnimatorRequest(string animaName,bool boolParam)
    {
        MainPack pack = new MainPack();
        pack.RequestCode = requestCode;
        pack.ActionCode = ActionCode.SetAnimator;
        RoomPack roomPack = new RoomPack();
        roomPack.RoomName = face.gameManager.roomName;
        pack.RoomPack.Add(roomPack);
        RoomPlayerPack roomPlayerPack = new RoomPlayerPack();
        roomPlayerPack.PlayerName = face.clientManager.userName;
        roomPlayerPack.AnimaName = animaName;
        roomPlayerPack.NameType = "Bool";
        roomPlayerPack.BoolParam = boolParam;
        pack.RoomPlayerPack.Add(roomPlayerPack);
        base.SendRequest(pack);
    }
    /// <summary>
    /// 发送动画请求 Int
    /// </summary>
    /// <param name="animaName"></param>
    /// <param name="nameType"></param>
    /// <param name="intParam"></param>
    public void SendAnimatorRequest(string animaName,int intParam)
    {
        MainPack pack = new MainPack();
        pack.RequestCode = requestCode;
        pack.ActionCode = ActionCode.SetAnimator;
        RoomPack roomPack = new RoomPack();
        roomPack.RoomName = face.gameManager.roomName;
        pack.RoomPack.Add(roomPack);
        RoomPlayerPack roomPlayerPack = new RoomPlayerPack();
        roomPlayerPack.PlayerName = face.clientManager.userName;
        roomPlayerPack.AnimaName = animaName;
        roomPlayerPack.NameType = "Int";
        roomPlayerPack.IntParam = intParam;
        pack.RoomPlayerPack.Add(roomPlayerPack);
        base.SendRequest(pack);
    }
    #endregion
    /// <summary>
    /// 发送退出游戏请求
    /// </summary>
    public void SendExitGameRequest(string roomName)
    {
        MainPack pack=new MainPack();
        pack.RequestCode = requestCode;
        pack.ActionCode = ActionCode.ExitGame;
        pack.Str = roomName;
        base.SendRequest(pack);
    }
}
