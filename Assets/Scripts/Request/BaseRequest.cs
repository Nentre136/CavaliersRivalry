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
        // 通过基类初始化 每当有新的请求被创建都能添加到requestManager的字典中
        face.requestManager.AddRequest(this);
    }
    public virtual void OnDestroy()
    {
        face.requestManager.RemoveRequest(actionCode);
    }
    /// <summary>
    /// 接收消息并响应
    /// </summary>
    /// <param name="pack"></param>
    public virtual void OnResponse(MainPack pack)
    {
    }
    /// <summary>
    /// 使用TCP协议向服务端发送请求
    /// </summary>
    /// <param name="pack"></param>
    public virtual void SendRequest(MainPack pack)
    {
        // 调用的face中ClientManager模块的发送
        face.clientManager.Send(pack);
    }
    /// <summary>
    /// 使用UDP协议向服务端发送请求
    /// </summary>
    /// <param name="pack"></param>
    public virtual void SendRequestUDP(MainPack pack)
    {
        // UDP协议需要向UDP服务端注明 哪个客户端发送的数据报(userNam)
        pack.UserName = face.clientManager.userName;
        face.clientManager.SendUDP(pack);
    }
}
