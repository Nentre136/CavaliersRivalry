using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMainPanel : BasePanel
{
    /// <summary>
    /// 能力选择面板
    /// </summary>
    public AbilityChoose abilityChoose;
    /// <summary>
    /// 信息面板
    /// </summary>
    public InforPanel inforPanel;
    /// <summary>
    /// ESC设置面板
    /// </summary>
    public GameSettingPanel settingPanel;
    /// <summary>
    /// 取消按键提示 按下X键取消选择
    /// </summary>
    private GameObject useKeyTips;
    public override void Start()
    {
        base.Start();
    }
    public override void OnStart()
    {
        abilityChoose = transform.Find("AbilityChoose").GetComponent<AbilityChoose>();
        inforPanel = transform.Find("InforPanel").GetComponent<InforPanel>();
        settingPanel = transform.Find("GameSettingPanel").GetComponent<GameSettingPanel>();
        useKeyTips = transform.Find("UseKeyTips").gameObject;
    }
    /// <summary>
    /// 初始化生成角色 
    /// </summary>
    public void InitCreateCharacter()
    {
        StartCoroutine(GameFace.Instance.gameManager.BuildCharacter());
    }
    /// <summary>
    /// 关闭按键提示
    /// </summary>
    public void CloseUseKeyTips()
    {
        useKeyTips.SetActive(false);
    }
    /// <summary>
    /// 打开按键提示
    /// </summary>
    public void OpenUseKeyTips()
    {
        useKeyTips.SetActive(true);
        StartCoroutine(_UseKeyTips());
    }
    private IEnumerator _UseKeyTips()
    {
        bool tmp = true;
        while (useKeyTips.activeSelf)
        {
            useKeyTips.transform.Find("KeyX_Up").gameObject.SetActive(tmp);
            useKeyTips.transform.Find("KeyX_Down").gameObject.SetActive(!tmp);
            useKeyTips.transform.Find("KeyMouse_Up").gameObject.SetActive(tmp);
            useKeyTips.transform.Find("KeyMouse_Down").gameObject.SetActive(!tmp);
            yield return new WaitForSeconds(0.8f);
            tmp = !tmp;
        }
        yield break;
    }
}
