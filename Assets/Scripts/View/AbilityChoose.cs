using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class AbilityChoose : MonoBehaviour
{
    /// <summary>
    /// ѡ���Ŀ��
    /// </summary>
    public RectTransform chooseTarget;
    private RectTransform beaconLine;
    private RectTransform beaconArrows;
    // �����߳�
    private float maxLineLenght = 200.0f;
    private float lineHeight = 4.5f;
    /// <summary>
    /// ��ͷ������
    /// </summary>
    private float sensitivity = 4f;
    /// <summary>
    /// ����ʱ��
    /// </summary>
    private float adsorbTime = 0.4f;
    /// <summary>
    /// ���
    /// </summary>
    private Vector2 startPoint;
    /// <summary>
    /// ��ǰ�������
    /// </summary>
    public Vector2 curMousePosition;

    private RectTransform mid;
    private RectTransform attack;
    private RectTransform attackRemote;
    private RectTransform defense;
    private RectTransform sprint;
    private RectTransform skill;
    private RectTransform keyTips;
    private void Awake()
    {
        beaconLine = transform.Find("UIBeacon/Line").GetComponent<RectTransform>();
        beaconArrows = transform.Find("UIBeacon/Arrows").GetComponent<RectTransform>();
        mid = transform.Find("Mid").GetComponent<RectTransform>();
        attack = transform.Find("Attack").GetComponent<RectTransform>();
        attackRemote = transform.Find("Attack_Remote").GetComponent<RectTransform>();
        defense = transform.Find("Defense").GetComponent<RectTransform>();
        sprint = transform.Find("Sprint").GetComponent<RectTransform>();
        skill = transform.Find("Skill").GetComponent<RectTransform>();
        keyTips = transform.Find("KeyTips").GetComponent<RectTransform>();
        chooseTarget = null;

        startPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, mid.position); 
        curMousePosition = startPoint + new Vector2(20, 0);
    }
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        curMousePosition += new Vector2(mouseX, mouseY) * sensitivity;
        // �������Ʋ�������Χ
        curMousePosition.x = Mathf.Clamp(curMousePosition.x, startPoint.x, startPoint.x + maxLineLenght);
        curMousePosition.y = Mathf.Clamp(curMousePosition.y, startPoint.y - maxLineLenght, startPoint.y + maxLineLenght);
        Vector2 offset = curMousePosition - startPoint;
        if (offset.magnitude > maxLineLenght)
        {
            offset = offset.normalized * maxLineLenght;
            curMousePosition = startPoint + offset;
        }
        SetLine(curMousePosition);
        AdsorbAndChoose();
    }
    private void OnEnable()
    {
        StartCoroutine(KeyTabDownAnima());
    }
    /// <summary>
    /// ��ʼ��ָ���
    /// </summary>
    public void InitBeacon()
    {
        startPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, mid.position);
        curMousePosition = startPoint + new Vector2(20, 0);
        if (chooseTarget != null)
        {
            chooseTarget.GetComponent<Toggle>().isOn = false;
            chooseTarget = null;
        }
    }
    /// <summary>
    /// ����Ŀ��˵�
    /// </summary>
    /// <param name="endPoint"></param>
    private void SetLine(Vector2 endPoint)
    {
        float lineLength = Vector2.Distance(startPoint, endPoint);
        //������
        float lineAngle = Mathf.Atan2(endPoint.y - startPoint.y, endPoint.x - startPoint.x) * Mathf.Rad2Deg;
        //������
        float lineRadians = lineAngle * Mathf.Deg2Rad;
        beaconLine.sizeDelta = new Vector2(lineLength, lineHeight);
        // ����Ļ����תΪUI���ֲ�����
        Vector2 localStartPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle
            (mid, startPoint, Camera.main, out localStartPos);

        beaconLine.anchoredPosition = localStartPos;
        beaconLine.localRotation = Quaternion.Euler(0f, 0f, lineAngle);

        // ���ü�ͷλ�úͽǶ�
        beaconArrows.anchoredPosition = new
            Vector2((lineLength+5) * Mathf.Cos(lineRadians), (lineLength + 5) * Mathf.Sin(lineRadians));
        beaconArrows.localRotation = Quaternion.Euler(0, 0, lineAngle);
    }
    /// <summary>
    /// ������ѡ��
    /// </summary>
    public void AdsorbAndChoose()
    {
        Vector2 adsorbPos;
        if (ScreenPointInTargetPoint(curMousePosition, attack))
        {
            if (!attack.GetComponent<Toggle>().isOn)
            {
                attack.GetComponent<Toggle>().isOn = true;
                chooseTarget = attack;
                adsorbPos = CalcuAdsorbPos(attack);
                StartCoroutine(Adsorb(adsorbPos));
            }
        }
        else if(ScreenPointInTargetPoint(curMousePosition,attackRemote))
        {
            if (!attackRemote.GetComponent<Toggle>().isOn)
            {
                attackRemote.GetComponent<Toggle>().isOn = true;
                chooseTarget = attackRemote;
                adsorbPos = CalcuAdsorbPos(attackRemote);
                StartCoroutine(Adsorb(adsorbPos));
            }
        }
        else if(ScreenPointInTargetPoint(curMousePosition,defense))
        {
            if (!defense.GetComponent<Toggle>().isOn)
            {
                defense.GetComponent<Toggle>().isOn = true;
                chooseTarget = defense;
                adsorbPos = CalcuAdsorbPos(defense);
                StartCoroutine(Adsorb(adsorbPos));
            }
        }
        else if (ScreenPointInTargetPoint(curMousePosition, sprint))
        {
            if (!sprint.GetComponent<Toggle>().isOn)
            {
                sprint.GetComponent<Toggle>().isOn = true;
                chooseTarget = sprint;
                adsorbPos = CalcuAdsorbPos(sprint);
                StartCoroutine(Adsorb(adsorbPos));
            }
        }
        else if(ScreenPointInTargetPoint(curMousePosition, skill))
        {
            if (!skill.GetComponent<Toggle>().isOn)
            {
                skill.GetComponent<Toggle>().isOn = true;
                chooseTarget = skill;
                adsorbPos = CalcuAdsorbPos(skill);
                StartCoroutine(Adsorb(adsorbPos));
            }
        }
        else
        {
            if(chooseTarget != null)
            {
                chooseTarget.GetComponent<Toggle>().isOn = false;
                chooseTarget = null;
            }
        }
    }
    /// <summary>
    /// �ж�һ����Ļ�����Ƿ���Ŀ��RectTransform������
    /// </summary>
    /// <param name="screenPoint"></param>
    /// <param name="targetPoint"></param>
    /// <returns></returns>
    private bool ScreenPointInTargetPoint(Vector2 screenPoint,RectTransform targetPoint)
    {
        Vector2 localPoint;
        //����Ļ����ת��Ϊ���targetPoint�ı�������
        RectTransformUtility.ScreenPointToLocalPointInRectangle
            (targetPoint, screenPoint, Camera.main, out localPoint);
        //targetPoint.rect��targetPoint�ı�������
        //����ֱ�Ӻ�screenPointֱ���ж� ��Ҫת��screenPointΪ���targetPoint������
        //�жϵ����������Ƿ���rectTrans.rect������
        if (targetPoint.rect.Contains(localPoint))
            return true;
        else
            return false;
    }
    /// <summary>
    /// �����Ӧ���ܵ��������겢����
    /// </summary>
    /// <param name="adsorbTarget"></param>
    /// <returns></returns>
    private Vector2 CalcuAdsorbPos(RectTransform adsorbTarget)
    {
        // ��ͷ��ֵ
        float offset = 23.0f;
        Vector2 adsorbPos;
        Vector2 endPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, adsorbTarget.position);
        float lineRadians = Mathf.Atan2(endPoint.y - startPoint.y, endPoint.x - startPoint.x);
        float lenght = Vector2.Distance(endPoint, startPoint) - offset;
        adsorbPos = new Vector2(
            (lenght * Mathf.Cos(lineRadians)) + startPoint.x,
            (lenght * Mathf.Sin(lineRadians)) + startPoint.y
            );
        return adsorbPos;
    }
    IEnumerator Adsorb(Vector2 adsorbPos)
    {
        float click = 0;
        while (click<adsorbTime)
        {
            curMousePosition = adsorbPos;
            click += Time.deltaTime;
            yield return null;
        }
        yield break;
    }
    /// <summary>
    /// Tab����ʾ��������
    /// </summary>
    private IEnumerator KeyTabDownAnima()
    {
        bool tmp = false;
        while (true)
        {
            keyTips.Find("Key_Up").gameObject.SetActive(tmp);
            keyTips.Find("Key_Down").gameObject.SetActive(!tmp);

            yield return new WaitForSeconds(0.8f);
            tmp = !tmp;
        }
    }
}
