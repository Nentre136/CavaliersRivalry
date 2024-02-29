using SocketGameProtocol;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BaseRequest : MonoBehaviour
{
    protected GameFace face;
    protected RequestCode requestCode;
    protected ActionCode actionCode;
    public ActionCode GetActionCode {  get { return actionCode; } }
    public virtual void Start()
    {
        face = GameFace.Instance;
        // ͨ�������ʼ�� ÿ�����µ����󱻴���������ӵ�requestManager���ֵ���
        face.requestManager.AddRequest(this);
    }
    public virtual void OnDestroy()
    {
        face.requestManager.RemoveRequest(actionCode);
    }
    /// <summary>
    /// ������Ϣ����Ӧ
    /// </summary>
    /// <param name="pack"></param>
    public virtual void OnResponse(MainPack pack)
    {
    }
    /// <summary>
    /// ʹ��TCPЭ�������˷�������
    /// </summary>
    /// <param name="pack"></param>
    public virtual void SendRequest(MainPack pack)
    {
        // ���õ�face��ClientManagerģ��ķ���
        face.clientManager.Send(pack);
    }
    /// <summary>
    /// ʹ��UDPЭ�������˷�������
    /// </summary>
    /// <param name="pack"></param>
    public virtual void SendRequestUDP(MainPack pack)
    {
        // UDPЭ����Ҫ��UDP�����ע�� �ĸ��ͻ��˷��͵����ݱ�(userNam)
        pack.UserName = face.clientManager.userName;
        face.clientManager.SendUDP(pack);
    }
}
