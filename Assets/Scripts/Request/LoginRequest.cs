using SocketGameProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginRequest : BaseRequest
{
    /// <summary>
    /// ��¼���صĽ��
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
        // ���ش���
        if (loginResult)
        {
            // ... ��Դ���� .... �첽
            face.uiManager.PushUIPanel(UIType.GameHallPanel2);
            loginResult = false;
        }
    }
    /// <summary>
    /// ������Ϣ����Ӧ
    /// </summary>
    /// <param name="pack"></param>
    public override void OnResponse(MainPack pack)
    {
        if (pack.ReturnCode == ReturnCode.Succeed)
        {
            face.uiManager.messageTips.ShowTips("��¼�ɹ�����ӭ����");
            // �洢�û��� �������
            face.clientManager.userName = pack.RegisterPack.UserName;
            // �л�ҳ���������̽��У����������޷�����unity���
            loginResult = true;
        }
        else if (pack.ReturnCode == ReturnCode.Fail)
        {
            face.uiManager.messageTips.ShowTips("��¼ʧ�ܣ�������ȷ�������û���������");
        }
    }
    /// <summary>
     /// ����ע������
     /// </summary>
    public void SendRequest(string user, string password)
    {
        MainPack pack = new MainPack();
        pack.RequestCode = requestCode;
        pack.ActionCode = actionCode;
        // ��¼��ע��ʹ�õ���Ϣ����ͬ ��ֻ��Ҫ�˺š�����
        RegisterPack registerPack = new RegisterPack();
        registerPack.UserName = user;
        registerPack.Password = password;
        pack.RegisterPack = registerPack;
        base.SendRequest(pack);
    }
}
