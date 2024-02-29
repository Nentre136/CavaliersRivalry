using SocketGameProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHallRequest2 : BaseRequest
{
    /// <summary>
    /// �Ƿ��յ��������Ӧ
    /// </summary>
    private bool isResponse;
    private MainPack responsePack;
    private GameHallPanel2 hallPanel;
    public override void Start()
    {
        requestCode = RequestCode.GameHall;
        face = GameFace.Instance;
        // ����ActionCode���Ǹ��ݴ�������
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
            // ����
            if (responsePack.ActionCode == ActionCode.CreateRoom)
            {
                if (responsePack.ReturnCode == ReturnCode.Succeed)
                {
                    // ���ط���ҳ��
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
                    face.uiManager.messageTips.ShowTips("��������ʧ�ܣ���ǰ���������ѱ�ռ�á�"+ responsePack.RoomPack[0].RoomName+"��");
                }
            }// ��ѯ
            else if (responsePack.ActionCode == ActionCode.FindRoom)
            {
                if (responsePack.ReturnCode == ReturnCode.Fail)
                {
                    face.uiManager.messageTips.ShowTips("û���ҵ�Ŀ�귿��");
                }
                else
                {
                    // ��ӷ���ui
                    hallPanel.ShowRoomItem(responsePack);
                }
            }// ����
            else if(responsePack.ActionCode == ActionCode.JoinRoom)
            {
                if (responsePack.ReturnCode == ReturnCode.Fail)
                {
                    face.uiManager.messageTips.ShowTips("��ǰ���Լ���ķ����������ѿ�ʼ��Ϸ");
                }
                else if( responsePack.ReturnCode == ReturnCode.Succeed)
                {
                    // ���ط�����UI������ˢ������б�
                    RoomPanel roomPanel = face.uiManager.PushUIPanel(UIType.RoomPanel) as RoomPanel;
                    roomPanel.messageList.text = "";
                    roomPanel.roomName = responsePack.RoomPack[0].RoomName;
                    roomPanel.curCount = responsePack.RoomPack[0].CurCount;
                    roomPanel.maxCount = responsePack.RoomPack[0].MaxCount;
                    roomPanel.countText.text = roomPanel.curCount.ToString() + "/" + roomPanel.maxCount.ToString();
                    roomPanel.UpdataPlayerList(responsePack);
                }
                else // ���䲻����
                {
                    face.uiManager.messageTips.ShowTips("��ǰ���Լ���ķ��䲻����");
                }
            }
            isResponse = false;
            responsePack = null;
        }
    }
    /// <summary>
    /// ���շ������Ϣ����Ӧ
    /// </summary>
    /// <param name="pack"></param>
    public override void OnResponse(MainPack pack)
    {
        // ������Ӧ
        isResponse=true;
        responsePack=pack;
    }
    /// <summary>
    /// �����˷��ʹ�����������
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
    /// �����˷��Ͳ��ҷ�������
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
    /// �����˷��ͼ��뷿������
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
