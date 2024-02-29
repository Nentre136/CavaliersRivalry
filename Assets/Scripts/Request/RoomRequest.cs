using SocketGameProtocol;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomRequest : BaseRequest
{
    private RoomPanel roomPanel;
    private MainPack responsePack;
    public override void Start()
    {
        face = GameFace.Instance;
        requestCode = RequestCode.Room;
        // 他人加入房间请求
        actionCode = ActionCode.UpdataPlayerList;
        face.requestManager.AddRequest(this);
        // 他人退出房间请求
        actionCode = ActionCode.ExitRoom;
        face.requestManager.AddRequest(this);
        // 聊天请求
        actionCode = ActionCode.Chat;
        face.requestManager.AddRequest(this);
        // 准备请求
        actionCode = ActionCode.Setout;
        face.requestManager.AddRequest(this);
        // 取消准备
        actionCode = ActionCode.StartGame;
        face.requestManager.AddRequest(this);

        roomPanel = GetComponent<RoomPanel>();
        responsePack = new MainPack();
    }
    private void Update()
    {
        if (responsePack!=null)
        { 
            // 其他玩家成功加入房间 更新玩家列表
            if (responsePack.ActionCode == ActionCode.UpdataPlayerList)
            {
                roomPanel.UpdataPlayerList(responsePack);
                roomPanel.messageList.text += responsePack.Str;
                roomPanel.curCount = responsePack.RoomPack[0].CurCount;
                roomPanel.maxCount = responsePack.RoomPack[0].MaxCount;
                roomPanel.countText.text = roomPanel.curCount.ToString() + "/" + roomPanel.maxCount.ToString();
            }
            else if (responsePack.ActionCode == ActionCode.ExitRoom)
            {
                if (responsePack.ReturnCode == ReturnCode.Succeed)
                {
                    roomPanel.ClearPlayerList();
                    GameHallPanel gameHallPanel = face.uiManager.PopUIPanel() as GameHallPanel;
                    gameHallPanel.UpdataRoomList();
                }
            }
            else if(responsePack.ActionCode == ActionCode.Chat)
            {
                // 接收消息成功
                if (responsePack.ReturnCode == ReturnCode.Succeed)
                {
                    roomPanel.messageList.text += responsePack.Str;
                    roomPanel.chatScrol.value = 0;
                }
                else// 发送消息失败
                {
                    face.uiManager.messageTips.ShowTips("发送消息失败未能送达");
                }
            }
            else if(responsePack.ActionCode == ActionCode.Setout)
            {
                // 该玩家准备
                if(responsePack.ReturnCode == ReturnCode.Succeed)
                {
                    roomPanel.UpdateSetou(responsePack.Str,true);
                }
                else// 该玩家没有准备
                {
                    roomPanel.UpdateSetou(responsePack.Str, false);
                }
            }
            else if(responsePack.ActionCode == ActionCode.StartGame)
            {
                // 开始游戏
                if (responsePack.ReturnCode == ReturnCode.Succeed)
                {
                    roomPanel.messageList.text += "加载游戏ing......\n";
                    List<RoomPlayerPack> playerList = new List<RoomPlayerPack>(responsePack.RoomPlayerPack);
                    roomPanel.StartGame(playerList);
                }
                else// 开始游戏失败
                {
                    // 提示开始失败原因
                    face.uiManager.messageTips.ShowTips(responsePack.Str);
                }
            }
            responsePack = null;
        }
    }
    /// <summary>
    /// 接收服务端消息并响应
    /// </summary>
    /// <param name="pack"></param>
    public override void OnResponse(MainPack pack)
    {
        responsePack = pack;
    }
    /// <summary>
    /// 向服务端发送退出房间请求
    /// </summary>
    public void SendExitRoomRequest()
    {
        MainPack pack = new MainPack();
        pack.RequestCode = requestCode;
        pack.ActionCode = ActionCode.ExitRoom;
        // 发送房间名
        pack.Str = roomPanel.roomName;
        base.SendRequest(pack);
    }
    /// <summary>
    /// 发送聊天请求
    /// </summary>
    public void SendChatRequest(string message)
    {
        MainPack pack = new MainPack();
        pack.RequestCode = requestCode;
        pack.ActionCode = ActionCode.Chat;
        // 发送聊天内容
        pack.Str = message;
        RoomPack roomPack = new RoomPack();
        roomPack.RoomName = roomPanel.roomName;
        pack.RoomPack.Add(roomPack);
        base.SendRequest(pack);
    }
    /// <summary>
    /// 发送准备请求
    /// </summary>
    public void SendSetoutRequest()
    {
        MainPack pack = new MainPack();
        pack.RequestCode = requestCode;
        pack.ActionCode = ActionCode.Setout;
        pack.Str = face.clientManager.userName;
        RoomPack roomPack = new RoomPack();
        roomPack.RoomName = roomPanel.roomName;
        pack.RoomPack.Add(roomPack);
        base.SendRequest(pack);
    }
    /// <summary>
    /// 发送开始游戏请求
    /// </summary>
    public void SendStartGameRequest()
    {
        MainPack pack = new MainPack();
        pack.RequestCode = requestCode;
        pack.ActionCode = ActionCode.StartGame;
        RoomPack roomPack = new RoomPack();
        roomPack.RoomName = roomPanel.roomName;
        pack.RoomPack.Add(roomPack);
        base.SendRequest(pack);
    }
}
