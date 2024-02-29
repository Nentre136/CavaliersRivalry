using SocketGameProtocol;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class RequestManager : BaseManager
{
    private Dictionary<ActionCode, BaseRequest> requestDict;
    public RequestManager(GameFace face) : base(face)
    {
        requestDict = new Dictionary<ActionCode, BaseRequest>();
    }
    /// <summary>
    /// 添加请求模块
    /// </summary>
    /// <param name="request"></param>
    public void AddRequest(BaseRequest request)
    {
        if(!requestDict.ContainsKey(request.GetActionCode))
            requestDict.Add(request.GetActionCode, request);
    }
    /// <summary>
    /// 删除请求模块
    /// </summary>
    /// <param name="action"></param>
    public void RemoveRequest(ActionCode action)
    { 
        if(requestDict.ContainsKey(action))
            requestDict.Remove(action); 
    }
    /// <summary>
    /// 处理服务端消息并响应
    /// </summary>
    /// <param name="pack"></param>
    public void HandleResponse(MainPack pack)
    {
        if(requestDict.TryGetValue(pack.ActionCode,out BaseRequest request))
        {
            request.OnResponse(pack);
        }
        else
        {
            Debug.Log("没有对应的处理");
        }
    }
}
