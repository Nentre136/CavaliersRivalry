using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePanel : MonoBehaviour
{
    // 对应页面的面板无需在这初始化！在PushUIPanel时会自动初始化面板
    protected GameFace face;
    [NonSerialized]
    public UIType panelType;

    /// <summary>
    /// Start 执行添加监听或请求等操作 Start只在被创建出来时调用一次
    /// </summary>
    public virtual void Start()
    {
        face = GameFace.Instance;
    }
    /// <summary>
    /// 进入Panel 该函数执行顺序比Mono流的Start生命周期先
    /// </summary>
    public virtual void OnEnter()
    {
        gameObject.SetActive(true);
        GetComponent<BasePanel>().enabled = true;
    }
    /// <summary>
    /// 退出Panel
    /// </summary>
    public virtual void OnExit()
    {
        gameObject.SetActive(false);
        GetComponent<BasePanel>().enabled = false;
    }
    public virtual void OnDestory()
    {
        Destroy(gameObject);
    }
    /// <summary>
    /// 初始化OnStart 比Mono流的Start生命周期先执行
    /// </summary>
    public virtual void OnStart()
    {
    }
}
