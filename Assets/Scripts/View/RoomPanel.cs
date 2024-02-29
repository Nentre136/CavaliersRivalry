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
        
        // 添加房间请求模块
        transform.AddComponent<RoomRequest>();
        roomRequest = GetComponent<RoomRequest>();
        isSetout = false;
    }
    /// <summary>
    /// 玩家加入房间 将userItem加入玩家列表 是否准备 并设置是否为房主
    /// </summary>
    /// <param name="playerName"></param>
    /// <param name="isSetout"></param>
    /// <param name="isMaster"></param>
    public void PlayerEnterRoom(string playerName,bool isSetout,bool isMaster=false)
    {
        GameObject item = GameObject.Instantiate(userItemPerfab,playerList);
        item.name = playerName;
        item.transform.Find("userName").GetComponent<Text>().text = "玩家id：" + playerName;
        item.transform.Find("isSetout").gameObject.SetActive(isSetout);
        item.transform.Find("master").gameObject.SetActive(isMaster);
        // 添加边框我方辨识度 绿色
        if (playerName == face.clientManager.userName)
        {
            Color color = new Color();
            color = Color.green;
            item.transform.Find("frame").GetComponent<Image>().color = color;
            item.transform.Find("you").gameObject.SetActive(true);
        }
    }
    /// <summary>
    /// 刷新房间玩家列表
    /// </summary>
    /// <param name="pack"></param>
    public void UpdataPlayerList(MainPack pack)
    {
        // 清空玩家列表
        ClearPlayerList();
        // 刷新列表
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
    /// 清空玩家列表
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
    /// 退出房间
    /// </summary>
    private void ClickExitRoom()
    {
        roomRequest.SendExitRoomRequest();
        isSetout = false;
        setoutBtn.transform.Find("Text").GetComponent<Text>().text = "准备";
    }
    /// <summary>
    /// 发送聊天信息
    /// </summary>
    private void ClickSendBtn()
    {
        if(inputMessage.text == "")
        {
            face.uiManager.messageTips.ShowTips("发送信息不可为空");
        }
        else
        {
            // 发送聊天消息 记录发送消息对象
            roomRequest.SendChatRequest(face.clientManager.userName + " 说：" + inputMessage.text + "\n");
            messageList.text += "我 说：" + inputMessage.text + "\n";
            inputMessage.text = "";
            chatScrol.value = 0;
        }
    }
    /// <summary>
    /// 输入消息后回车调用
    /// </summary>
    /// <param name="text"></param>
    private void OnEndEdit(string text)
    {
        if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
            ClickSendBtn();
    }
    /// <summary>
    /// 准备
    /// </summary>
    private void ClickSetoutBtn()
    {
        roomRequest.SendSetoutRequest();
        // 切换按钮状态
        if (!isSetout)
            setoutBtn.transform.Find("Text").GetComponent<Text>().text = "取消准备";
        else
            setoutBtn.transform.Find("Text").GetComponent<Text>().text = "准备";
        // 切换准备状态
        isSetout = !isSetout;
        UpdateSetou(face.clientManager.userName, isSetout);
    }
    /// <summary>
    /// 刷新玩家准备状态
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="isSetout"></param>
    public void UpdateSetou(string userName,bool isSetout)
    {
        playerList.Find(userName+"/isSetout").gameObject.SetActive(isSetout);
    }
    /// <summary>
    /// 开始游戏
    /// </summary>
    private void ClickStartBtn()
    {
        roomRequest.SendStartGameRequest();
    }
    /// <summary>
    /// 开始游戏
    /// </summary>
    public void StartGame(List<RoomPlayerPack> playerList)
    {
        face.gameManager.playerPacks = playerList;
        face.gameManager.SetGameInfo(roomName, curCount, maxCount);
        SceneManager.sceneLoaded += OnSceneLoaded;
        // 切换场景
        SceneManager.LoadScene("GameScene");
    }
    /// <summary>
    /// 切换场景时调用事件
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 游戏场景
        if (scene.name == "GameScene")
        {
            // 加载GamePanel等
            GameMainPanel gameMainPanel = face.uiManager.PushUIPanel(UIType.GameMainPanel) as GameMainPanel;
            face.uiManager.messageTips = face.uiManager.BuildMessageTipsPanel();
            // 加载地图
            face.resouceManager.LoadAssetAsync("GameCombat/Scene");
            face.resouceManager.LoadAssetAsync("GameCombat/SpawnPoint");
            gameMainPanel.InitCreateCharacter();
            Destroy(GameObject.Find("TmpCamera"));
        }
    }
}
