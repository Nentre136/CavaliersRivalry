using SocketGameProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHallRequest2 : BaseRequest
{
    /// <summary>
    /// 是否收到服务端响应
    /// </summary>
    private bool isResponse;
    private MainPack responsePack;
    private GameHallPanel2 hallPanel;
    public override void Start()
    {
        requestCode = RequestCode.GameHall;
        face = GameFace.Instance;
        // 三种ActionCode都是根据大厅请求
        actionCode = ActionCode.CreateRoom;
        face.requestManager.AddRequest(this);
        actionCode = ActionCode.FindRoom;
        face.requestManager.AddRequest(this);
        actionCode = ActionCode.JoinRoom;
        face.requestManager.AddRequest(this);
        hallPanel = transform.GetComponent<GameHallPanel2>();
        isResponse = false;
        responsePack = new MainPack();
    }
    private void Update()
    {
        if(isResponse && responsePack!=null)
        {
            // 创建
            if (responsePack.ActionCode == ActionCode.CreateRoom)
            {
                if (responsePack.ReturnCode == ReturnCode.Succeed)
                {
                    // 加载房间页面
                    RoomPanel roomPanel =  face.uiManager.PushUIPanel(UIType.RoomPanel) as RoomPanel;
                    roomPanel.ClearPlayerList();
                    roomPanel.messageList.text = "";
                    roomPanel.PlayerEnterRoom(face.clientManager.userName,false,true);
                    roomPanel.roomName = responsePack.RoomPack[0].RoomName;
                    roomPanel.curCount = responsePack.RoomPack[0].CurCount;
                    roomPanel.maxCount = responsePack.RoomPack[0].MaxCount;
                    roomPanel.countText.text = roomPanel.curCount.ToString() + "/" + roomPanel.maxCount.ToString();
                }
                else if (responsePack.ReturnCode == ReturnCode.Fail)
                {
                    face.uiManager.messageTips.ShowTips("创建房间失败，当前房间名字已被占用‘"+ responsePack.RoomPack[0].RoomName+"’");
                }
            }// 查询
            else if (responsePack.ActionCode == ActionCode.FindRoom)
            {
                if (responsePack.ReturnCode == ReturnCode.Fail)
                {
                    face.uiManager.messageTips.ShowTips("没有找到目标房间");
                }
                else
                {
                    // 添加房间ui
                    hallPanel.ShowRoomItem(responsePack);
                }
            }// 加入
            else if(responsePack.ActionCode == ActionCode.JoinRoom)
            {
                if (responsePack.ReturnCode == ReturnCode.Fail)
                {
                    face.uiManager.messageTips.ShowTips("当前尝试加入的房间已满或已开始游戏");
                }
                else if( responsePack.ReturnCode == ReturnCode.Succeed)
                {
                    // 加载房间内UI界面且刷新玩家列表
                    RoomPanel roomPanel = face.uiManager.PushUIPanel(UIType.RoomPanel) as RoomPanel;
                    roomPanel.messageList.text = "";
                    roomPanel.roomName = responsePack.RoomPack[0].RoomName;
                    roomPanel.curCount = responsePack.RoomPack[0].CurCount;
                    roomPanel.maxCount = responsePack.RoomPack[0].MaxCount;
                    roomPanel.countText.text = roomPanel.curCount.ToString() + "/" + roomPanel.maxCount.ToString();
                    roomPanel.UpdataPlayerList(responsePack);
                }
                else // 房间不存在
                {
                    face.uiManager.messageTips.ShowTips("当前尝试加入的房间不存在");
                }
            }
            isResponse = false;
            responsePack = null;
        }
    }
    /// <summary>
    /// 接收服务端消息并响应
    /// </summary>
    /// <param name="pack"></param>
    public override void OnResponse(MainPack pack)
    {
        // 接收响应
        isResponse=true;
        responsePack=pack;
    }
    /// <summary>
    /// 向服务端发送创建房间请求
    /// </summary>
    public void SendCreateRoomRequest(string roomName,int userCount)
    {
        MainPack pack = new MainPack();
        pack.RequestCode = requestCode;
        pack.ActionCode = ActionCode.CreateRoom;

        RoomPack roomPack = new RoomPack();
        roomPack.RoomName = roomName;
        roomPack.MaxCount = userCount;
        pack.RoomPack.Add(roomPack);
        base.SendRequest(pack);
    }
    /// <summary>
    /// 向服务端发送查找房间请求
    /// </summary>
    public void SendFindRoomRequest(string roomName)
    {
        MainPack pack = new MainPack();
        pack.RequestCode = requestCode;
        pack.ActionCode= ActionCode.FindRoom;

        RoomPack roomPack = new RoomPack();
        roomPack.RoomName = roomName;
        pack.RoomPack.Add(roomPack);
        base.SendRequest(pack);
    }
    /// <summary>
    /// 向服务端发送加入房间请求
    /// </summary>
    /// <param name="name"></param>
    public void SendJoinRoomRequest(string roomName)
    {
        MainPack pack = new MainPack();
        pack.RequestCode = RequestCode.GameHall;
        pack.ActionCode = ActionCode.JoinRoom;
        RoomPack roomPack=new RoomPack();
        roomPack.RoomName= roomName;
        pack.RoomPack.Add(roomPack);
        base.SendRequest(pack);
    }
}
