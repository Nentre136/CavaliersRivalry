using SocketGameProtocol;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class GameManager : BaseManager
{
    public List<RoomPlayerPack> playerPacks;
    public Dictionary<string, GameObject> playerDict {  get; private set; }
    public GameObject myCharacter {  get; private set; }
    public string roomName {  get; private set; }
    public int curCount {  get; private set; }
    public int maxCount {  get; private set; }
    public GameManager(GameFace face) : base(face)
    {
        playerPacks = new List<RoomPlayerPack>();
        playerDict = new Dictionary<string, GameObject>();
    }
    /// <summary>
    /// 生成角色
    /// </summary>
    /// <returns></returns>
    public IEnumerator BuildCharacter()
    {
        // 等待资源加载完毕才能载入角色
        while (GameFace.Instance.resouceManager.IsLoadComplete)
            yield return null;

        Transform spawnPoint = GameObject.FindGameObjectsWithTag("SpawnPoint")[0].transform;
        GameObject camera = GameFace.Instance.resouceManager.LoadGameObject("GameCombat/MainCamera");
        int pb = 0;
        int pr = 4;
        for(int i=0; i < playerPacks.Count; i++)
        {
            // 蓝方
            if(i % 2 == 0)
            {
                GameObject character = GameFace.Instance.resouceManager.LoadGameObject("GameCombat/Barbarian");
                PlayerInfo playerInfo = character.GetComponent<PlayerInfo>();
                playerInfo.partCont = character.GetComponent<ParticleController>();
                character.name = playerPacks[i].PlayerName;
                playerDict.Add(character.name, character);
                character.transform.position = spawnPoint.GetChild(pb).transform.position;
                character.transform.rotation = spawnPoint.GetChild(pb++).transform.rotation;
                if (character.name == face.clientManager.userName)
                    myCharacter = character;
                else
                {
                    InforPanel head = character.transform.Find("InforPanel").GetComponent<InforPanel>();
                    playerInfo.partCont.inforPanel = head;
                    head.GetComponent<Canvas>().worldCamera = Camera.main;
                    head.InitHealthBar(playerInfo.maxHealth, true);
                }
            }
            else// 红方
            {
                GameObject character = GameFace.Instance.resouceManager.LoadGameObject("GameCombat/Barbarian");
                PlayerInfo playerInfo = character.GetComponent<PlayerInfo>();
                playerInfo.partCont = character.GetComponent<ParticleController>();
                character.name = playerPacks[i].PlayerName;
                playerDict.Add(character.name, character);
                character.transform.position = spawnPoint.GetChild(pr).transform.position;
                character.transform.rotation = spawnPoint.GetChild(pr++).transform.rotation;
                if (character.name == face.clientManager.userName)
                    myCharacter = character;
                else
                {
                    InforPanel head = character.transform.Find("InforPanel").GetComponent<InforPanel>();
                    playerInfo.partCont.inforPanel = head;
                    head.GetComponent<Canvas>().worldCamera = Camera.main;
                    head.InitHealthBar(playerInfo.maxHealth, true);
                }
            }
        }
        GameObject.Find("GameUICanvas").GetComponent<Canvas>().worldCamera = Camera.main;
        GameObject.Find("GameUICanvas").GetComponent<Canvas>().sortingLayerName = "UI";
        myCharacter.AddComponent<GameRequest>();
        myCharacter.AddComponent<UpStateRequest>();
        myCharacter.AddComponent<PlayerContorller>();
        myCharacter.transform.Find("PhotoGrapher").AddComponent<GrapherContorller>();
        // 在Eyes处 生成摄影机
        Transform eyes = myCharacter.transform.Find("PhotoGrapher/Eyes");
        camera.transform.parent = eyes;
        camera.transform.localPosition = Vector3.zero;
        camera.transform.localRotation = Quaternion.identity;
        camera.name = "MainCamera";
        GameMainPanel gamePanel =  GameFace.Instance.uiManager.GetCurrentPanel() as GameMainPanel;
        myCharacter.GetComponent<ParticleController>().inforPanel = gamePanel.inforPanel;
        gamePanel.inforPanel.InitHealthBar(myCharacter.GetComponent<PlayerInfo>().maxHealth);
        gamePanel.inforPanel.InitEnergyBar(myCharacter.GetComponent<PlayerInfo>().maxEnergy);
        yield break;
    }
    public void SetGameInfo(string roomName,int curCount,int maxCount)
    {
        this.roomName = roomName;
        this.curCount = curCount;
        this.maxCount = maxCount;
    }
    /// <summary>
    /// 状态同步
    /// </summary>
    /// <param name="pack"></param>
    public void SyncCharacState(MainPack pack)
    {
        GameObject character = playerDict[pack.RoomPlayerPack[0].PlayerName];
        Transform rigTransform = character.transform.Find("Rig");
        Transform psTransform = character.transform.Find("PStart");
        Vector3 cPos = Vector3.zero;
        Quaternion cRota = Quaternion.identity;
        Quaternion rigRota = Quaternion.identity;
        Quaternion psRota = Quaternion.identity;
        cPos = new Vector3(
            pack.RoomPlayerPack[0].CharaStatePack.PosX,
            pack.RoomPlayerPack[0].CharaStatePack.PosY,
            pack.RoomPlayerPack[0].CharaStatePack.PosZ);
        cRota = Quaternion.Euler(new Vector3(
            pack.RoomPlayerPack[0].CharaStatePack.RotaX,
            pack.RoomPlayerPack[0].CharaStatePack.RotaY,
            pack.RoomPlayerPack[0].CharaStatePack.RotaZ));
        rigRota = Quaternion.Euler(new Vector3(
            pack.RoomPlayerPack[0].CharaStatePack.RigRotaX,
            pack.RoomPlayerPack[0].CharaStatePack.RigRotaY,
            pack.RoomPlayerPack[0].CharaStatePack.RigRotaZ));
        psRota = Quaternion.Euler(new Vector3(
            pack.RoomPlayerPack[0].CharaStatePack.RigRotaX,
            pack.RoomPlayerPack[0].CharaStatePack.RigRotaY,
            pack.RoomPlayerPack[0].CharaStatePack.RigRotaZ));

        character.transform.position = Vector3.Lerp(character.transform.position, cPos, 0.25f);
        character.transform.rotation = Quaternion.Slerp(character.transform.rotation, cRota, 0.25f);
        rigTransform.localRotation = Quaternion.Slerp(rigTransform.localRotation,rigRota,0.25f);
        psTransform.localRotation = Quaternion.Slerp(psTransform.localRotation,psRota,0.25f);
        character.GetComponent<PlayerInfo>().Health = pack.RoomPlayerPack[0].CharaStatePack.Health;
    }
    /// <summary>
    /// 设置对应动画
    /// </summary>
    public void SetTargetAnimation(MainPack pack)
    {
        GameObject character = playerDict[pack.RoomPlayerPack[0].PlayerName];
        Animator animator = character.GetComponent<Animator>();
        switch (pack.RoomPlayerPack[0].NameType)
        {
            case "Trigger":
                SetTriggerParam(animator, pack);
                break;
            case "Bool":
                SetBoolParam(animator, pack.RoomPlayerPack[0].AnimaName, pack.RoomPlayerPack[0].BoolParam);
                break;
            case "Int":
                animator.SetInteger(pack.RoomPlayerPack[0].AnimaName, pack.RoomPlayerPack[0].IntParam);
                break;
        }
    }
    private void SetTriggerParam(Animator animator,MainPack pack)
    {
        if(pack.RoomPlayerPack[0].AnimaName == "Attack_Remote")
        {
            Vector3 direc = new Vector3(
                pack.RoomPlayerPack[0].CharaStatePack.PosX,
                pack.RoomPlayerPack[0].CharaStatePack.PosY,
                pack.RoomPlayerPack[0].CharaStatePack.PosZ
                );
            animator.GetComponent<ParticleController>().remoteAttackDirec = direc;
            animator.SetTrigger(pack.RoomPlayerPack[0].AnimaName);
        }
        else
        {
            animator.SetTrigger(pack.RoomPlayerPack[0].AnimaName);
        }
    }
    private void SetBoolParam(Animator animator,string animaName,bool param)
    {
        switch(animaName)
        {
            case "Defense":
                animator.GetComponent<ParticleController>().Defense();
                break;
            case "Skill":
                animator.GetComponent<ParticleController>().SkillAnimation();
                break;
        }
    }
    /// <summary>
    /// 角色退出游戏场景
    /// </summary>
    /// <param name="name"></param>
    public void ExitCharacter(string name)
    {
        GameObject ec = playerDict[name];
        playerDict.Remove(name);
        GameObject.Destroy(ec);
    }
}
