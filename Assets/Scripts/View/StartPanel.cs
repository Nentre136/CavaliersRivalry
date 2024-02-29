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
            // ����ʾע��
            face.uiManager.PushUIPanel(UIType.LoginPanel);
        });
    }
    /// <summary>
    /// Ϊtips��Ӷ�Ч
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

            // ��Ҫ�ȴ�һ֡ ����Ῠס������
            yield return null; 
        }
        yield break;
    }
    public override void OnStart()
    {
        // �ɻ�OnStartΪʲô���Start��ִ��....
        nextBtn = GetComponent<Button>();
        tips = transform.Find("tips").GetComponent<Text>();
        StartCoroutine(AddTipsDynamicEffect());
    }


}
