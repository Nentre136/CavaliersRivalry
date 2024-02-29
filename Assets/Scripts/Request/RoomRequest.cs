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
        // ���˼��뷿������
        actionCode = ActionCode.UpdataPlayerList;
        face.requestManager.AddRequest(this);
        // �����˳���������
        actionCode = ActionCode.ExitRoom;
        face.requestManager.AddRequest(this);
        // ��������
        actionCode = ActionCode.Chat;
        face.requestManager.AddRequest(this);
        // ׼������
        actionCode = ActionCode.Setout;
        face.requestManager.AddRequest(this);
        // ȡ��׼��
        actionCode = ActionCode.StartGame;
        face.requestManager.AddRequest(this);

        roomPanel = GetComponent<RoomPanel>();
        responsePack = new MainPack();
    }
    private void Update()
    {
        if (responsePack!=null)
        { 
            // ������ҳɹ����뷿�� ��������б�
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
                // ������Ϣ�ɹ�
                if (responsePack.ReturnCode == ReturnCode.Succeed)
                {
                    roomPanel.messageList.text += responsePack.Str;
                    roomPanel.chatScrol.value = 0;
                }
                else// ������Ϣʧ��
                {
                    face.uiManager.messageTips.ShowTips("������Ϣʧ��δ���ʹ�");
                }
            }
            else if(responsePack.ActionCode == ActionCode.Setout)
            {
                // �����׼��
                if(responsePack.ReturnCode == ReturnCode.Succeed)
                {
                    roomPanel.UpdateSetou(responsePack.Str,true);
                }
                else// �����û��׼��
                {
                    roomPanel.UpdateSetou(responsePack.Str, false);
                }
            }
            else if(responsePack.ActionCode == ActionCode.StartGame)
            {
                // ��ʼ��Ϸ
                if (responsePack.ReturnCode == ReturnCode.Succeed)
                {
                    roomPanel.messageList.text += "������Ϸing......\n";
                    List<RoomPlayerPack> playerList = new List<RoomPlayerPack>(responsePack.RoomPlayerPack);
                    roomPanel.StartGame(playerList);
                }
                else// ��ʼ��Ϸʧ��
                {
                    // ��ʾ��ʼʧ��ԭ��
                    face.uiManager.messageTips.ShowTips(responsePack.Str);
                }
            }
            responsePack = null;
        }
    }
    /// <summary>
    /// ���շ������Ϣ����Ӧ
    /// </summary>
    /// <param name="pack"></param>
    public override void OnResponse(MainPack pack)
    {
        responsePack = pack;
    }
    /// <summary>
    /// �����˷����˳���������
    /// </summary>
    public void SendExitRoomRequest()
    {
        MainPack pack = new MainPack();
        pack.RequestCode = requestCode;
        pack.ActionCode = ActionCode.ExitRoom;
        // ���ͷ�����
        pack.Str = roomPanel.roomName;
        base.SendRequest(pack);
    }
    /// <summary>
    /// ������������
    /// </summary>
    public void SendChatRequest(string message)
    {
        MainPack pack = new MainPack();
        pack.RequestCode = requestCode;
        pack.ActionCode = ActionCode.Chat;
        // ������������
        pack.Str = message;
        RoomPack roomPack = new RoomPack();
        roomPack.RoomName = roomPanel.roomName;
        pack.RoomPack.Add(roomPack);
        base.SendRequest(pack);
    }
    /// <summary>
    /// ����׼������
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
    /// ���Ϳ�ʼ��Ϸ����
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
