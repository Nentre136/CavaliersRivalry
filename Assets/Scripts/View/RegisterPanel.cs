using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class RegisterPanel : BasePanel
{
    private RegisterRequest registerRequest;
    private InputField user, password;
    private Button registerBtn,switchBtn;
    public override void Start()
    {   

        base.Start();
        // ����ע���������
        transform.AddComponent<RegisterRequest>();
        registerRequest = GetComponent<RegisterRequest>();

        registerBtn.onClick.AddListener(() =>
        {
            if(user.text=="" || password.text=="")
            {
                face.uiManager.messageTips.ShowTips("������˺����벻��Ϊ��");
                return;
            }
            registerRequest.SendRequest(user.text, password.text);
        });

        // ע��ΪLogin���Ӽ� �л���ť�򷵻ظ���
        switchBtn.onClick.AddListener(() =>
        {
            face.uiManager.PopUIPanel();
        });

    }
    public override void OnStart()
    {
        user = transform.Find("user").GetComponent<InputField>();
        password = transform.Find("password").GetComponent<InputField>();
        registerBtn = transform.Find("registerBtn").GetComponent<Button>();
        switchBtn = transform.Find("switchBtn").GetComponent<Button>();
    }
}
