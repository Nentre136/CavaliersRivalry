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
        // 挂载注册请求组件
        transform.AddComponent<RegisterRequest>();
        registerRequest = GetComponent<RegisterRequest>();

        registerBtn.onClick.AddListener(() =>
        {
            if(user.text=="" || password.text=="")
            {
                face.uiManager.messageTips.ShowTips("输入的账号密码不能为空");
                return;
            }
            registerRequest.SendRequest(user.text, password.text);
        });

        // 注册为Login的子级 切换按钮则返回父级
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
