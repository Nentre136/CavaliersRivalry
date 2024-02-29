using SocketGameProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ע������
/// </summary>
public class RegisterRequest : BaseRequest
{
    /// <summary>
    /// ע�᷵�ؽ��
    /// </summary>
    private bool registerResult;
    public override void Start()
    {
        // ��ʼ���������� �� ����
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
    /// ������Ϣ����Ӧ
    /// </summary>
    /// <param name="pack"></param>
    public override void OnResponse(MainPack pack)
    {
        if(pack.ReturnCode == ReturnCode.Succeed)
        {
            face.uiManager.messageTips.ShowTips("ע��ɹ������¼�����˺�");
            // �л�ҳ���������̽��У����������޷�����unity���
            registerResult = true;
        }
        else if(pack.ReturnCode == ReturnCode.Fail)
        {
            face.uiManager.messageTips.ShowTips("ע��ʧ�ܣ���ǰ�˺��ѱ�ע��");
        }
    }
    /// <summary>
    /// ����ע������
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
