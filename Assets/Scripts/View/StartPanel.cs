using SocketGameProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartPanel : BasePanel
{
    private Button nextBtn;
    private Text tips;
    private float tipsFlickerSpeed = 0.5f;
    private float minAlphaProgress = 0.3f;
    public override void Start()
    {
        base.Start();

        nextBtn.onClick.AddListener(() => {
            // 先显示注册
            face.uiManager.PushUIPanel(UIType.LoginPanel);
        });
    }
    /// <summary>
    /// 为tips添加动效
    /// </summary>
    /// <returns></returns>
    IEnumerator AddTipsDynamicEffect()
    {
        while (this.enabled)
        {
            float time = Time.time * tipsFlickerSpeed;
            float progress = Mathf.PingPong(time, 1) + minAlphaProgress;
            Color tmpColor = tips.color;
            tmpColor.a = progress;
            tips.color = tmpColor;

            // 需要等待一帧 否则会卡住主进程
            yield return null; 
        }
        yield break;
    }
    public override void OnStart()
    {
        // 疑惑OnStart为什么会比Start先执行....
        nextBtn = GetComponent<Button>();
        tips = transform.Find("tips").GetComponent<Text>();
        StartCoroutine(AddTipsDynamicEffect());
    }


}
