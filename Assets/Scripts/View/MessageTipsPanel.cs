using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MessageTipsPanel : BasePanel
{
    private Text tips;
    private string showText="";
    private float waitTime = 0.7f;
    private float vanishTime = 1.0f;
    private float clock = 0;
    private bool isShow;
    private Color tmpColor;
    private void Awake()
    {
        tips = transform.Find("tips").GetComponent<Text>();
        isShow = false;
    }
    private void Update()
    {
        // ������������ڵ���Unity������У����߼���updateʵ��
        if(isShow)
        {
            // �ȴ�ʱ��
            if (clock < waitTime)
            {
                tips.text = showText;
                tips.color = tmpColor;
                clock += Time.deltaTime;
                return;
            }
            if(clock>= waitTime && clock<waitTime + vanishTime)
            {
                // ��ȡ��ʧ����
                float reduceProgress = (clock - waitTime) / vanishTime;
                tmpColor.a -= reduceProgress * Time.deltaTime;
                tips.color = tmpColor;
                clock += Time.deltaTime;
            }
            if(clock >= waitTime + vanishTime)
            {
                tmpColor.a = 0;
                tips.color = tmpColor;
                tips.text = "";
                isShow = false;
            }
        }
    }
    /// <summary>
    /// ��ʾmessageTips����Ļ�Ϸ�
    /// </summary>
    /// <param name="str"></param>
    public void ShowTips(string str)
    {
        clock = 0;
        isShow = true;
        showText = str;
        tmpColor = tips.color;
        tmpColor.a = 1f;
    }
}
