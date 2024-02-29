using SocketGameProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 注册请求
/// </summary>
public class RegisterRequest : BaseRequest
{
    /// <summary>
    /// 注册返回结果
    /// </summary>
    private bool registerResult;
    public override void Start()
    {
        // 初始化请求类型 和 方法
        requestCode = RequestCode.User;
        actionCode = ActionCode.Register;
        registerResult = false;
        base.Start();
    }
    private void Update()
    {
        if (registerResult)
        {
            face.uiManager.PopUIPanel();
            registerResult = false;
        }
    }
    /// <summary>
    /// 接收消息并响应
    /// </summary>
    /// <param name="pack"></param>
    public override void OnResponse(MainPack pack)
    {
        if(pack.ReturnCode == ReturnCode.Succeed)
        {
            face.uiManager.messageTips.ShowTips("注册成功，请登录您的账号");
            // 切换页面在主进程进行，非主进程无法调用unity组件
            registerResult = true;
        }
        else if(pack.ReturnCode == ReturnCode.Fail)
        {
            face.uiManager.messageTips.ShowTips("注册失败，当前账号已被注册");
        }
    }
    /// <summary>
    /// 发送注册请求
    /// </summary>
    public void SendRequest(string user,string password)
    {
        MainPack pack = new MainPack();
        pack.RequestCode = requestCode;
        pack.ActionCode = actionCode;
        RegisterPack registerPack = new RegisterPack();
        registerPack.UserName = user;
        registerPack.Password = password;
        pack.RegisterPack = registerPack;
        base.SendRequest(pack);
    }
}
