using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
/// <summary>
/// 粒子特效控制器
/// </summary>
public class ParticleController : MonoBehaviour
{
    public InforPanel inforPanel;
    private PlayerInfo playerInfo;
    private Animator animator;
    private Rigidbody body;
    private Transform startPoint;
    private GameObject beacon;
    private Vector2 mousePosition;
    public Vector3 remoteAttackDirec = Vector3.zero;

    public GameObject axe1;
    public GameObject axe2;
    public GameObject throwAxe;
    public GameObject skillAxe;
    private Vector3 targetPos = Vector3.zero;
    public bool isDefense = false;
    public bool isDeath = false;

    void Start()
    {
        startPoint = transform.Find("PStart");
        body = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        playerInfo = GetComponent<PlayerInfo>();
        mousePosition = new Vector2(Screen.width / 2, Screen.height / 2);
    }
    void Update()
    {
    }
    /// <summary>
    /// 启用碰撞器，在waitTime秒后关闭
    /// </summary>
    /// <param name="collider"></param>
    /// <param name="waitTime"></param>
    /// <returns></returns>
    private IEnumerator StartDamage(BoxCollider collider, float waitTime)
    {
        collider.enabled = true;
        yield return new WaitForSeconds(waitTime);
        collider.enabled = false;
    }
    /// <summary>
    /// 启用碰撞器，在waitTime秒后关闭
    /// </summary>
    /// <param name="collider"></param>
    /// <param name="waitTime"></param>
    /// <returns></returns>
    private IEnumerator StartDamage(CapsuleCollider collider, float waitTime)
    {
        collider.enabled = true;
        yield return new WaitForSeconds(waitTime);
        collider.enabled = false;
    }
    public void CreateAxe1()
    {
        GameObject p = GameObject.Instantiate(axe1, startPoint);
        p.transform.rotation = startPoint.rotation;
        ParticleAxe collisionDamage = p.GetComponent<ParticleAxe>();
        collisionDamage.damage = playerInfo.damage;
        collisionDamage.damageBeform = transform.gameObject;
        StartCoroutine(StartDamage(p.GetComponent<BoxCollider>(), 0.4f));
        Destroy(p, 4.0f);
    }
    public void CreateAxe2()
    {
        Quaternion rota = Quaternion.Euler(startPoint.rotation.eulerAngles + new Vector3(-30, 0, -90));
        GameObject p = GameObject.Instantiate(axe2, startPoint.position, rota,startPoint);
        ParticleAxe collisionDamage = p.GetComponent<ParticleAxe>();
        collisionDamage.damage = playerInfo.damage;
        collisionDamage.damageBeform = transform.gameObject;
        StartCoroutine(StartDamage(p.GetComponent<BoxCollider>(), 0.4f));
        Destroy(p, 4.0f);
    }
    public void CraeteAxe3()
    {
        GameObject p = GameObject.Instantiate(skillAxe);
        SkillAxe collisionDamage = p.GetComponent<SkillAxe>();
        collisionDamage.damage = playerInfo.damage;
        collisionDamage.damageBeform = transform.gameObject;
        Destroy(p, playerInfo.skillTime);
    }
    public void ThrowAxe()
    {
        GameObject t_axe = Instantiate(throwAxe, startPoint.position, transform.Find("Rig").rotation);
        t_axe.transform.localScale = throwAxe.transform.lossyScale;
        throwAxe.SetActive(false);
        t_axe.GetComponent<BoxCollider>().enabled = true;
        ThrowAxe ta = t_axe.GetComponent<ThrowAxe>();
        ta.enabled = true;
        ta.damage = playerInfo.remoteDamage;
        ta.damageBeform = transform.gameObject;
        ta.throwDirec = remoteAttackDirec;
        Destroy(t_axe, 20.0f);
    }
    public void Defense()
    {
        StartCoroutine(_Defense());
    }
    private IEnumerator _Defense()
    {
        animator.SetBool("Defense", true);
        isDefense = true;
        yield return new WaitForSeconds(playerInfo.defenseTime);
        animator.SetBool("Defense", false);
        isDefense = false;
        yield break;
    }
    public void Skill()
    {
        SkillAnimation();
        StartCoroutine(_Skill());
    }
    private IEnumerator _Skill()
    {
        float clock = 0;
        while (clock <= playerInfo.skillTime)
        {
            if (beacon == null)
                beacon = GameFace.Instance.resouceManager.LoadGameObject("GameCombat/Beacon");
            beacon.transform.position = new Vector3(
                     transform.position.x,
                     5.5f,
                     transform.position.z);
            beacon.SetActive(true);

            mousePosition = new Vector2(Screen.width / 2, Screen.height / 2);
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
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
                Vector3 direc = (arrows.position - transform.position);
                direc.y = transform.position.y;
                direc = direc.normalized;
                Vector3 movePos = direc * playerInfo.skillSpeed * Time.deltaTime;
                body.MovePosition(body.position + new Vector3(movePos.x, 0, movePos.z));
                targetPos = arrows.position;
                transform.Find("Rig").LookAt(new Vector3(targetPos.x, transform.position.y, targetPos.z));
            }
            yield return null;
            clock += Time.deltaTime;
        }
        transform.Find("Rig").LookAt(new Vector3(targetPos.x, transform.position.y, targetPos.z));
        InitBeacon();
        yield break;
    }
    public void SkillAnimation()
    {
        CraeteAxe3();
        StartCoroutine(_SkillAnimation());
    }
    private IEnumerator _SkillAnimation()
    {
        animator.SetBool("Skill", true);
        while(!animator.GetCurrentAnimatorStateInfo(0).IsName("Skill"))
            yield return null;
        yield return new WaitForSeconds(playerInfo.skillTime);
        animator.SetBool("Skill", false);
    }
    private void InitBeacon()
    {
        beacon.SetActive(false);
        mousePosition = new Vector2(Screen.width / 2, Screen.height / 2);
    }
    /// <summary>
    /// 受到伤害
    /// </summary>
    public void BeDamage(int damage)
    {
        playerInfo.Health -= damage;
        if (damage >= 20)
            StartCoroutine(_HitAnima());
    }
    IEnumerator _HitAnima()
    {
        animator.SetInteger("Hit", Random.Range(1, 3));
        yield return new WaitForSeconds(0.1f);
        animator.SetInteger("Hit", 0);
        yield break;
    }
    public void Death()
    {
        if (!isDeath)
        {
            StartCoroutine(_Death());
            isDeath = true;
        }
    }
    IEnumerator _Death()
    {
        animator.SetInteger("Death", Random.Range(1, 3));
        yield return null;
        animator.SetInteger("Death", 0);
        yield return new WaitForSeconds(3.0f);
        if(gameObject.name == GameFace.Instance.clientManager.userName)
        {
            Camera.main.transform.parent = null;
            Transform gamePanel = GameFace.Instance.uiManager.GetCurrentPanel().transform;
            gamePanel.Find("DeathPanel").gameObject.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            gamePanel.Find("DeathPanel").GetComponent<Button>().onClick.AddListener(() =>
            {
                // 退出游戏
                string roomName = GameFace.Instance.gameManager.roomName;
                GameFace.Instance.gameManager.myCharacter.GetComponent<GameRequest>().SendExitGameRequest(roomName);
                SceneManager.LoadScene("EnterScene");
            });
        }
        gameObject.SetActive(false);
        yield break;
    }
}
