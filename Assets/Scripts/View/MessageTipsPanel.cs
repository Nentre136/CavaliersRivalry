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
        // 避免非主进程内调用Unity组件运行，将逻辑在update实现
        if(isShow)
        {
            // 等待时间
            if (clock < waitTime)
            {
                tips.text = showText;
                tips.color = tmpColor;
                clock += Time.deltaTime;
                return;
            }
            if(clock>= waitTime && clock<waitTime + vanishTime)
            {
                // 获取消失进度
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
    /// 显示messageTips到屏幕上方
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
