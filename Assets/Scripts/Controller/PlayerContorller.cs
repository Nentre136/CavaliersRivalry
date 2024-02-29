using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class PlayerContorller : MonoBehaviour
{
    private GameFace face;
    private GameRequest gameRequest;
    private UpStateRequest upStateRequest;
    public GameMainPanel gamePanel {  get; private set; }
    private Rigidbody body;
    private Animator animator;
    private ParticleController particleContro;
    private Camera _camare;
    private GameObject beacon;
    public PlayerInfo playerInfo;

    public bool isChooseAbility {  get; private set; }
    public bool isSprintDirec { get; private set; }
    public bool isAttackDirec {  get; private set; }
    public bool isRemoteDirec {  get; private set; }
    public bool isSprint {  get; private set; }
    private Vector3 targetPos = Vector3.zero;
    /// <summary>
    /// 重力比例
    /// </summary>
    private float gravityScale = 2.0f;
    /// <summary>
    /// 鼠标灵敏度
    /// </summary>
    public float mouseSensitivity = 7.0f;
    private Vector2 mousePosition;
    private void Start()
    {
        face = GameFace.Instance;
        gameRequest = GetComponent<GameRequest>();
        upStateRequest = GetComponent<UpStateRequest>();
        body = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        particleContro = GetComponent<ParticleController>();
        _camare = Camera.main;
        gamePanel = (face.uiManager.GetCurrentPanel() as GameMainPanel);
        playerInfo = GetComponent<PlayerInfo>();
        mousePosition = new Vector2(Screen.width / 2, Screen.height / 2);
        isChooseAbility = false;
        isSprintDirec = false;
        isAttackDirec = false;
        isRemoteDirec = false;
        isSprint = false;
        // 锁定并隐藏鼠标
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        InvokeRepeating("UpdateState", 1, 1.0f / 60.0f);
    }
    private void Update()
    {
        playerInfo.Energy += Time.deltaTime * 0.5f;
        // 当玩家按住Tab键 显示操作UI 使用UI逻辑
        if(Input.GetKey(KeyCode.Tab) && !isSprintDirec && !isAttackDirec 
            && !isRemoteDirec && animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            gamePanel.abilityChoose.gameObject.SetActive(true);
            isChooseAbility = true;
        }
        else if (Input.GetKeyUp(KeyCode.Tab))// 抬起Tab按键 选择行动
        {
            ChooseAbility();
            isChooseAbility = false;
        }
        else if (Input.GetKeyDown(KeyCode.Escape))// 退出游戏
        {
            Cursor.visible = !Cursor.visible;
            if(Cursor.visible)
            {
                Cursor.lockState = CursorLockMode.None;
                gamePanel.settingPanel.ShowPanel();
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                gamePanel.settingPanel.HidePanel();
            }
            //Cursor.lockState = Cursor.visible ? CursorLockMode.None : CursorLockMode.Locked;
        }
        

        // 选择方向距离
        if (isSprintDirec)
        {
            EnsureDirec();
            // 按下 X 键取消
            if (Input.GetKeyDown(KeyCode.X))
            {
                isSprintDirec = false;
                gamePanel.CloseUseKeyTips();
                InitBeacon();
            }
        }
        if (isSprint)
            CharacterSprint();

        if (isAttackDirec)
        {
            Attack();
            // 按下 X 键取消
            if (Input.GetKeyDown(KeyCode.X))
            {
                isAttackDirec = false;
                gamePanel.CloseUseKeyTips();
                InitBeacon();
            }
        }
        if (isRemoteDirec)
        {
            AttackRemote();
            // 按下 X 键取消
            if (Input.GetKeyDown(KeyCode.X))
            {
                isRemoteDirec = false;
                gamePanel.CloseUseKeyTips();
                InitBeacon();
            }
        }
    }
    void FixedUpdate()
    {
        if (body!=null)
        {
            Vector3 gravity = gravityScale * Physics.gravity;
            body.AddForce(gravity, ForceMode.Acceleration);
        }
    }
    /// <summary>
    /// 进行能力选择模块
    /// </summary>
    private void ChooseAbility()
    {
        if (gamePanel.abilityChoose.chooseTarget != null)
        {
            string abilityName = gamePanel.abilityChoose.chooseTarget.name;
            switch (abilityName)
            {
                case "Attack":
                    isAttackDirec = true;
                    gamePanel.OpenUseKeyTips();
                    break;
                case "Attack_Remote":
                    isRemoteDirec = true;
                    gamePanel.OpenUseKeyTips();
                    break;
                case "Defense":
                    if (playerInfo.Energy >= playerInfo.defenseEnergy)
                    {
                        gameRequest.SendAnimatorRequest("Defense", true);
                        particleContro.Defense();
                        playerInfo.Energy -= playerInfo.defenseEnergy;
                        playerInfo.Health += 50;
                    }
                    else
                        face.uiManager.messageTips.ShowTips("当前能量不足！");
                    break;
                case "Sprint":
                    isSprintDirec = true;
                    gamePanel.OpenUseKeyTips();
                    break;
                case "Skill":
                    if (playerInfo.Energy >= playerInfo.SkillEnergy)
                    {
                        particleContro.Skill();
                        gameRequest.SendAnimatorRequest("Skill", true);
                        playerInfo.Energy -= playerInfo.SkillEnergy;
                    }
                    else
                        face.uiManager.messageTips.ShowTips("当前能量不足！");
                    break;
                default:
                    break;
            }
        }
        // 初始化指针
        gamePanel.abilityChoose.InitBeacon();
        gamePanel.abilityChoose.gameObject.SetActive(false);
    }
    private void Attack()
    {
        if (beacon == null)
            beacon = face.resouceManager.LoadGameObject("GameCombat/Beacon");
        beacon.transform.position = new Vector3(
                 transform.position.x,
                 5.5f,
                 transform.position.z);
        beacon.SetActive(true);

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        mousePosition += new Vector2(mouseX * mouseSensitivity, mouseY * mouseSensitivity);
        Ray ray = _camare.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        int targetLayerMask = LayerMask.GetMask("PositionPanel");
        LineRenderer lineRenderer = beacon.GetComponent<LineRenderer>();
        Transform arrows = beacon.transform.Find("Arrows");
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetLayerMask))
        {
            Vector3 mousePositionWorld = hit.point;
            mousePositionWorld.y = lineRenderer.transform.position.y;
            Vector3 localPos = lineRenderer.transform.InverseTransformPoint(mousePositionWorld);
            localPos.y = 0;
            Vector3 lineDirec = (localPos - Vector3.zero).normalized * 5.0f;
            // 设置线段位置
            lineRenderer.SetPosition(1, lineDirec);
            // 设置箭头位置
            arrows.position = lineDirec + lineRenderer.transform.position;
            // 设置箭头方向
            Quaternion targetRotation = Quaternion.LookRotation(mousePositionWorld - lineRenderer.transform.position);
            targetRotation = Quaternion.Euler(targetRotation.eulerAngles + new Vector3(90, 0, 45));
            arrows.rotation = targetRotation;

            transform.Find("Rig").LookAt(new Vector3(
                arrows.position.x,
                transform.position.y,
                arrows.position.z));
            transform.Find("PStart").LookAt(new Vector3(
                arrows.position.x,
                transform.Find("PStart").position.y,
                arrows.position.z));
            // 选中攻击
            if (Input.GetMouseButtonDown(0) && playerInfo.Energy >= playerInfo.attackEnergy)
            {
                animator.SetTrigger("Attack");
                gameRequest.SendAnimatorRequest("Attack");
                gamePanel.CloseUseKeyTips();
                isAttackDirec = false;
                playerInfo.Energy -= playerInfo.attackEnergy;
                InitBeacon();
            }
            else if(Input.GetMouseButtonDown(0) && playerInfo.Energy < playerInfo.attackEnergy)
            {
                face.uiManager.messageTips.ShowTips("当前能量不足！");
            }
        }
    }
    private void AttackRemote()
    {
        if (beacon == null)
            beacon = face.resouceManager.LoadGameObject("GameCombat/Beacon");
        beacon.transform.position = new Vector3(
                 transform.position.x,
                 5.5f,
                 transform.position.z);
        beacon.SetActive(true);

        mousePosition = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = _camare.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        int targetLayerMask = LayerMask.GetMask("PositionPanel");
        LineRenderer lineRenderer = beacon.GetComponent<LineRenderer>();
        Transform arrows = beacon.transform.Find("Arrows");
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetLayerMask))
        {
            Vector3 mousePositionWorld = hit.point;
            mousePositionWorld.y = lineRenderer.transform.position.y;
            Vector3 localPos = lineRenderer.transform.InverseTransformPoint(mousePositionWorld);
            localPos.y = 0;
            Vector3 lineDirec = (localPos - Vector3.zero).normalized * 5.0f;

            // 设置线段位置
            lineRenderer.SetPosition(1, lineDirec);
            // 设置箭头位置
            arrows.position = lineDirec + lineRenderer.transform.position;
            // 设置箭头方向
            Quaternion targetRotation = Quaternion.LookRotation(mousePositionWorld - lineRenderer.transform.position);
            targetRotation = Quaternion.Euler(targetRotation.eulerAngles + new Vector3(90, 0, 45));
            arrows.rotation = targetRotation;

            transform.Find("Rig").LookAt(new Vector3(
                arrows.position.x,
                transform.position.y,
                arrows.position.z));
            // 选中攻击
            if (Input.GetMouseButtonDown(0) && playerInfo.Energy >= playerInfo.attRemoEnergy)
            {
                particleContro.remoteAttackDirec = (arrows.position - beacon.transform.position).normalized;
                animator.SetTrigger("Attack_Remote");
                gameRequest.SendAnimatorRequest("Attack_Remote");
                gamePanel.CloseUseKeyTips();
                isRemoteDirec = false;
                playerInfo.Energy -= playerInfo.attRemoEnergy;
                InitBeacon();
            }
            else if(Input.GetMouseButtonDown(0) && playerInfo.Energy < playerInfo.attRemoEnergy)
            {
                face.uiManager.messageTips.ShowTips("当前能量不足！");
            }
        }
    }
    /// <summary>
    /// 选择冲刺方向和距离并冲刺到目标位置
    /// </summary>
    private void EnsureDirec()
    {
        if(beacon == null)
            beacon = face.resouceManager.LoadGameObject("GameCombat/Beacon");
        beacon.transform.position = new Vector3(
                 transform.position.x,
                 5.5f,
                 transform.position.z);
        beacon.SetActive(true);

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        mousePosition += new Vector2(mouseX * mouseSensitivity, mouseY * mouseSensitivity);
        Ray ray = _camare.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        int targetLayerMask = LayerMask.GetMask("PositionPanel");
        LineRenderer lineRenderer = beacon.GetComponent<LineRenderer>();
        Transform arrows = beacon.transform.Find("Arrows");
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetLayerMask))
        {
            Vector3 mousePositionWorld = hit.point;
            mousePositionWorld.y = lineRenderer.transform.position.y;
            Vector3 localPos = lineRenderer.transform.InverseTransformPoint(mousePositionWorld);
            localPos.y = 0;
            // 限制移动距离
            float lineLenght = Vector3.Distance(localPos, Vector3.zero);
            if(lineLenght > playerInfo.maxMoveDistance)
            {
                Vector3 direction = (localPos - Vector3.zero).normalized;
                localPos = Vector3.zero + direction * playerInfo.maxMoveDistance;
                mousePosition = _camare.WorldToScreenPoint(localPos + beacon.transform.position);
            }

            // 设置线段位置
            lineRenderer.SetPosition(1, localPos);
            // 设置箭头位置
            arrows.position = localPos + lineRenderer.transform.position;
            // 设置箭头方向
            Quaternion targetRotation = Quaternion.LookRotation(mousePositionWorld - lineRenderer.transform.position);
            targetRotation = Quaternion.Euler(targetRotation.eulerAngles + new Vector3(90, 0, 45));
            arrows.rotation = targetRotation;

            // 选中则产生位移
            if (Input.GetMouseButtonDown(0) && playerInfo.Energy >= playerInfo.sprintEnergy)
            {
                targetPos = new Vector3(
                    arrows.position.x,
                    transform.position.y,
                    arrows.position.z);
                animator.SetTrigger("Sprint");
                gameRequest.SendAnimatorRequest("Sprint");
                gamePanel.CloseUseKeyTips();
                isSprint = true;
                isSprintDirec = false;
                playerInfo.Energy -= playerInfo.sprintEnergy;
                InitBeacon();
            }
            else if(Input.GetMouseButtonDown(0) && playerInfo.Energy < playerInfo.sprintEnergy)
            {
                face.uiManager.messageTips.ShowTips("当前能量不足！");
            }
        }
    }
    /// <summary>
    /// 角色位移
    /// </summary>
    private void CharacterSprint()
    {
        Vector3 direc = (targetPos - transform.position).normalized;
        Vector3 movePos = direc * playerInfo.speed * Time.deltaTime;
        body.MovePosition(body.position +new Vector3(movePos.x,0,movePos.z));
        transform.Find("Rig").LookAt(new Vector3(targetPos.x, transform.position.y, targetPos.z));
        float distance = Vector3.Distance(
            new Vector3(targetPos.x,0,targetPos.z),
            new Vector3(body.position.x,0,body.position.z));
        if (distance < 0.15f)
        {
            isSprint = false;
            transform.position = targetPos;
        }
    }
    /// <summary>
    /// 初始化箭头
    /// </summary>
    private void InitBeacon()
    {
        beacon.SetActive(false);
        mousePosition = new Vector2(Screen.width / 2, Screen.height / 2);
    }
    private void OnCollisionEnter(Collision collision)
    {
        // 撞墙停止
        if (LayerMask.LayerToName(collision.gameObject.layer) == "Wall" && isSprint)
        {
            isSprint = false;
            Vector3 direc = (targetPos - transform.position).normalized * (-1.0f);
            body.MovePosition(body.position + (direc * 0.1f));
            targetPos = transform.position;
        }
    }
    /// <summary>
    /// 同步位置信息
    /// </summary>
    private void UpdateState()
    {
        Vector3 pos = transform.position;
        Vector3 rota = transform.rotation.eulerAngles;
        Vector3 rigRota = transform.Find("Rig").localRotation.eulerAngles;
        upStateRequest.SendStateRequest(pos, rota, rigRota,playerInfo);
    }
}
