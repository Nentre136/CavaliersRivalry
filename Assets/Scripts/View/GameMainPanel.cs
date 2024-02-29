using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMainPanel : BasePanel
{
    /// <summary>
    /// ����ѡ�����
    /// </summary>
    public AbilityChoose abilityChoose;
    /// <summary>
    /// ��Ϣ���
    /// </summary>
    public InforPanel inforPanel;
    /// <summary>
    /// ESC�������
    /// </summary>
    public GameSettingPanel settingPanel;
    /// <summary>
    /// ȡ��������ʾ ����X��ȡ��ѡ��
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
    /// ��ʼ�����ɽ�ɫ 
    /// </summary>
    public void InitCreateCharacter()
    {
        StartCoroutine(GameFace.Instance.gameManager.BuildCharacter());
    }
    /// <summary>
    /// �رհ�����ʾ
    /// </summary>
    public void CloseUseKeyTips()
    {
        useKeyTips.SetActive(false);
    }
    /// <summary>
    /// �򿪰�����ʾ
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
