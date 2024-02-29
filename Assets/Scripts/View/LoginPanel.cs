using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : BasePanel
{
    private LoginRequest loginRequest;
    private InputField user, password;
    private Button loginBtn, switchBtn;

    public override void Start()
    {
        base.Start();
        // 挂载登录请求组件
        transform.AddComponent<LoginRequest>();
        loginRequest = GetComponent<LoginRequest>();
        loginBtn.onClick.AddListener(() =>
        {
            if (user.text == "" || password.text == "")
            {
                face.uiManager.messageTips.ShowTips("输入的账号密码不能为空");
                return;
            }
            loginRequest.SendRequest(user.text, password.text);
        });

        // 弹出注册UI
        switchBtn.onClick.AddListener(() =>
        {
            face.uiManager.PushUIPanel(UIType.RegisterPanel);
        });
    }
    public override void OnStart()
    {
        user = transform.Find("user").GetComponent<InputField>();
        password = transform.Find("password").GetComponent<InputField>();
        loginBtn = transform.Find("loginBtn").GetComponent<Button>();
        switchBtn = transform.Find("switchBase/switchBtn").GetComponent<Button>();
    }
}
