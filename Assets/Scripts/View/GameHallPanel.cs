using SocketGameProtocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.Examples;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameHallPanel : BasePanel
{
    private GameHallRequest gameHallRequest;
    private Transform roomList;
    private Button showCreateBtn, findBtn, backUserBtn,creatRoomBtn,closeBtn;
    private TMP_InputField createRoomName,findRoomName;
    private Slider userSlider;
    private GameObject roomPerfab;
    [SerializeField]
    private AnimationCurve showCurve;
    [SerializeField]
    private AnimationCurve hideCurve;
    [SerializeField]
    private float curveSpeed = 1.5f;
    private GameObject createPanel;
    public override void Start()
    {
        base.Start();
        transform.AddComponent<GameHallRequest>();
        gameHallRequest = GetComponent<GameHallRequest>();
        showCreateBtn.onClick.AddListener(() =>
        {
            StartCoroutine(ShowPanel(createPanel));
        });
        findBtn.onClick.AddListener(ClickFindBtn);
        backUserBtn.onClick.AddListener(ClickBackUserBtn);
        closeBtn.onClick.AddListener(() => { 
            StartCoroutine(HidePanel(createPanel));
        });
        creatRoomBtn.onClick.AddListener(ClickCreateBtn);
        roomPerfab = Resources.Load<GameObject>("roomItem");
    }
    public override void OnStart()
    {
        roomList = transform.Find("RoomList");
        showCreateBtn = transform.Find("Interact/showCreateBtn").GetComponent<Button>();
        findBtn = transform.Find("Interact/findBtn").GetComponent<Button>();
        findRoomName = transform.Find("Interact/findRoomName").GetComponent<TMP_InputField>();
        backUserBtn = transform.Find("Interact/backUserBtn").GetComponent<Button>();
        createPanel = transform.Find("CreateRoomPanel").gameObject;
        closeBtn = createPanel.transform.Find("closeBtn").GetComponent<Button>();
        creatRoomBtn = createPanel.transform.Find("creatRoomBtn").GetComponent<Button>();
        createRoomName = createPanel.transform.Find("createRoomName").GetComponent<TMP_InputField>();
        userSlider = createPanel.transform.Find("userSlider").GetComponent<Slider>();
    }

    /// <summary>
    /// ��������
    /// </summary>
    private void ClickCreateBtn()
    {
        // �������䷿��������Ϊ��
        if (createRoomName.text == "")
        {
            face.uiManager.messageTips.ShowTips("�������䣬����������Ϊ��");
            return;
        }
        gameHallRequest.SendCreateRoomRequest(createRoomName.text, (int)userSlider.value);
    }
    private IEnumerator ShowPanel(GameObject panel)
    {
        float clock = 0f;
        while (clock <= 1)
        {
            panel.transform.localScale = Vector3.one * showCurve.Evaluate(clock);
            clock += Time.deltaTime * curveSpeed;
            yield return null;
        }
    }
    private IEnumerator HidePanel(GameObject panel)
    {
        float clock = 0f;
        while (clock <= 1)
        {
            panel.transform.localScale = Vector3.one * hideCurve.Evaluate(clock);
            clock += Time.deltaTime * curveSpeed;
            yield return null;
        }
    }
    /// <summary>
    /// ��ѯ����
    /// </summary>
    private void ClickFindBtn()
    {
        gameHallRequest.SendFindRoomRequest(findRoomName.text);
    }
    /// <summary>
    /// ע����¼
    /// </summary>
    private void ClickBackUserBtn()
    {
        // ������ǰҳ��
        face.uiManager.PopUIPanel();
    }
    /// <summary>
    /// ������˷��ص���Ϣ����ɷ�����ʾ�ڷ����б���
    /// </summary>
    /// <param name="pack"></param>
    public void ShowRoomItem(MainPack pack)
    {
        // ��ɾ�����з���
        for (int i=0; i<roomList.childCount;i++)
        {
            Transform child = roomList.GetChild(i);
            Destroy(child.gameObject);
        }
        // ��ӷ���
        for (int i=0;i<pack.RoomPack.Count ;i++)
        {
            RoomPack roomPack = pack.RoomPack[i];
            GameObject room = GameObject.Instantiate(roomPerfab, roomList);
            room.transform.Find("roomName").GetComponent<Text>().text = roomPack.RoomName;
            room.transform.Find("roomNum").GetComponent<Text>().text
                = roomPack.CurCount + "/" + roomPack.MaxCount;
            string roomState = "";
            Color color = new Color();
            switch (roomPack.RoomState)
            {
                case 1:
                    roomState = "׼����";
                    color = Color.green;
                    break;
                case 2:
                    roomState = "��Ա";
                    color = Color.yellow;
                    break;
                case 3:
                    roomState = "��Ϸ��";
                    color = Color.red;
                    break;
            }
            room.transform.Find("roomState").GetComponent<Text>().text = roomState;
            room.transform.Find("roomState").GetComponent<Text>().color = color;
            // ��Ӽ��뷿�����
            room.GetComponent<Button>().onClick.AddListener(() => ClickJoinRoom(roomPack.RoomName));
            Debug.Log(roomPack.RoomName);
        }
    }
    private void ClickJoinRoom(string name)
    {
        gameHallRequest.SendJoinRoomRequest(name);
    }
    /// <summary>
    /// ˢ�·����б�
    /// </summary>
    public void UpdataRoomList()
    {
        gameHallRequest.SendFindRoomRequest("");
    }
}
