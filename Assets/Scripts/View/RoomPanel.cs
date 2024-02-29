using Google.Protobuf.Collections;
using SocketGameProtocol;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoomPanel : BasePanel
{
    public RoomRequest roomRequest {  get; private set; }
    private Transform playerList;
    private Button backBtn, startBtn, setoutBtn, sendBtn;
    private InputField inputMessage;
    public Scrollbar chatScrol {  get; private set; }
    public Text countText;
    public Text messageList;
    private GameObject userItemPerfab;
    public string roomName = "";
    public int curCount;
    public int maxCount;
    private bool isSetout;
    public override void Start()
    {
        base.Start();
    }
    public override void OnStart()
    {
        face = GameFace.Instance;
        playerList = transform.Find("PlayerList");
        backBtn = transform.Find("backBtn").GetComponent<Button>();
        startBtn = transform.Find("startBtn").GetComponent<Button>();
        setoutBtn = transform.Find("setoutBtn").GetComponent<Button>();
        sendBtn = transform.Find("Chat/sendBtn").GetComponent<Button>();
        inputMessage = transform.Find("Chat/inputMessage").GetComponent<InputField>();
        chatScrol = transform.Find("Chat/Scrollbar").GetComponent<Scrollbar>();
        messageList = transform.Find("Chat/MessageList/Content").GetComponent<Text>();
        countText = transform.Find("countText").GetComponent <Text>();
        userItemPerfab = Resources.Load<GameObject>("userItem");
        backBtn.onClick.AddListener(ClickExitRoom);
        sendBtn.onClick.AddListener(ClickSendBtn);
        setoutBtn.onClick.AddListener(ClickSetoutBtn);
        inputMessage.onEndEdit.AddListener(OnEndEdit);
        startBtn.onClick.AddListener(ClickStartBtn);
        
        // ��ӷ�������ģ��
        transform.AddComponent<RoomRequest>();
        roomRequest = GetComponent<RoomRequest>();
        isSetout = false;
    }
    /// <summary>
    /// ��Ҽ��뷿�� ��userItem��������б� �Ƿ�׼�� �������Ƿ�Ϊ����
    /// </summary>
    /// <param name="playerName"></param>
    /// <param name="isSetout"></param>
    /// <param name="isMaster"></param>
    public void PlayerEnterRoom(string playerName,bool isSetout,bool isMaster=false)
    {
        GameObject item = GameObject.Instantiate(userItemPerfab,playerList);
        item.name = playerName;
        item.transform.Find("userName").GetComponent<Text>().text = "���id��" + playerName;
        item.transform.Find("isSetout").gameObject.SetActive(isSetout);
        item.transform.Find("master").gameObject.SetActive(isMaster);
        // ��ӱ߿��ҷ���ʶ�� ��ɫ
        if (playerName == face.clientManager.userName)
        {
            Color color = new Color();
            color = Color.green;
            item.transform.Find("frame").GetComponent<Image>().color = color;
            item.transform.Find("you").gameObject.SetActive(true);
        }
    }
    /// <summary>
    /// ˢ�·�������б�
    /// </summary>
    /// <param name="pack"></param>
    public void UpdataPlayerList(MainPack pack)
    {
        // �������б�
        ClearPlayerList();
        // ˢ���б�
        for (int i = 0; i < pack.RoomPlayerPack.Count; i++)
        {
            RoomPlayerPack roomPlayer = pack.RoomPlayerPack[i];
            if (i == 0)
                PlayerEnterRoom(roomPlayer.PlayerName, roomPlayer.PlayerState,true);
            else
                PlayerEnterRoom(roomPlayer.PlayerName,roomPlayer.PlayerState);
        }
    }
    /// <summary>
    /// �������б�
    /// </summary>
    public void ClearPlayerList()
    {
        for (int i = 0; i < playerList.childCount; i++)
        {
            Transform child = playerList.GetChild(i);
            Destroy(child.gameObject);
        }
    }
    /// <summary>
    /// �˳�����
    /// </summary>
    private void ClickExitRoom()
    {
        roomRequest.SendExitRoomRequest();
        isSetout = false;
        setoutBtn.transform.Find("Text").GetComponent<Text>().text = "׼��";
    }
    /// <summary>
    /// ����������Ϣ
    /// </summary>
    private void ClickSendBtn()
    {
        if(inputMessage.text == "")
        {
            face.uiManager.messageTips.ShowTips("������Ϣ����Ϊ��");
        }
        else
        {
            // ����������Ϣ ��¼������Ϣ����
            roomRequest.SendChatRequest(face.clientManager.userName + " ˵��" + inputMessage.text + "\n");
            messageList.text += "�� ˵��" + inputMessage.text + "\n";
            inputMessage.text = "";
            chatScrol.value = 0;
        }
    }
    /// <summary>
    /// ������Ϣ��س�����
    /// </summary>
    /// <param name="text"></param>
    private void OnEndEdit(string text)
    {
        if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
            ClickSendBtn();
    }
    /// <summary>
    /// ׼��
    /// </summary>
    private void ClickSetoutBtn()
    {
        roomRequest.SendSetoutRequest();
        // �л���ť״̬
        if (!isSetout)
            setoutBtn.transform.Find("Text").GetComponent<Text>().text = "ȡ��׼��";
        else
            setoutBtn.transform.Find("Text").GetComponent<Text>().text = "׼��";
        // �л�׼��״̬
        isSetout = !isSetout;
        UpdateSetou(face.clientManager.userName, isSetout);
    }
    /// <summary>
    /// ˢ�����׼��״̬
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="isSetout"></param>
    public void UpdateSetou(string userName,bool isSetout)
    {
        playerList.Find(userName+"/isSetout").gameObject.SetActive(isSetout);
    }
    /// <summary>
    /// ��ʼ��Ϸ
    /// </summary>
    private void ClickStartBtn()
    {
        roomRequest.SendStartGameRequest();
    }
    /// <summary>
    /// ��ʼ��Ϸ
    /// </summary>
    public void StartGame(List<RoomPlayerPack> playerList)
    {
        face.gameManager.playerPacks = playerList;
        face.gameManager.SetGameInfo(roomName, curCount, maxCount);
        SceneManager.sceneLoaded += OnSceneLoaded;
        // �л�����
        SceneManager.LoadScene("GameScene");
    }
    /// <summary>
    /// �л�����ʱ�����¼�
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ��Ϸ����
        if (scene.name == "GameScene")
        {
            // ����GamePanel��
            GameMainPanel gameMainPanel = face.uiManager.PushUIPanel(UIType.GameMainPanel) as GameMainPanel;
            face.uiManager.messageTips = face.uiManager.BuildMessageTipsPanel();
            // ���ص�ͼ
            face.resouceManager.LoadAssetAsync("GameCombat/Scene");
            face.resouceManager.LoadAssetAsync("GameCombat/SpawnPoint");
            gameMainPanel.InitCreateCharacter();
            Destroy(GameObject.Find("TmpCamera"));
        }
    }
}
