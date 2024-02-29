using SocketGameProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginRequest : BaseRequest
{
    /// <summary>
    /// 登录返回的结果
    /// </summary>
    private bool loginResult;
    public override void Start()
    {
        requestCode = RequestCode.User;
        actionCode = ActionCode.Login;
        loginResult = false;
        base.Start();
    }
    private void Update()
    {
        // 加载大厅
        if (loginResult)
        {
            // ... 资源加载 .... 异步
            face.uiManager.PushUIPanel(UIType.GameHallPanel2);
            loginResult = false;
        }
    }
    /// <summary>
    /// 接收消息并响应
    /// </summary>
    /// <param name="pack"></param>
    public override void OnResponse(MainPack pack)
    {
        if (pack.ReturnCode == ReturnCode.Succeed)
        {
            face.uiManager.messageTips.ShowTips("登录成功，欢迎回来");
            // 存储用户名 方便调用
            face.clientManager.userName = pack.RegisterPack.UserName;
            // 切换页面在主进程进行，非主进程无法调用unity组件
            loginResult = true;
        }
        else if (pack.ReturnCode == ReturnCode.Fail)
        {
            face.uiManager.messageTips.ShowTips("登录失败，请重新确认您的用户名和密码");
        }
    }
    /// <summary>
     /// 发送注册请求
     /// </summary>
    public void SendRequest(string user, string password)
    {
        MainPack pack = new MainPack();
        pack.RequestCode = requestCode;
        pack.ActionCode = actionCode;
        // 登录与注册使用的消息包相同 都只需要账号、密码
        RegisterPack registerPack = new RegisterPack();
        registerPack.UserName = user;
        registerPack.Password = password;
        pack.RegisterPack = registerPack;
        base.SendRequest(pack);
    }
}
