using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : BaseManager
{
    /// <summary>
    /// 存放当前显示UI 按栈顺序存储
    /// </summary>
    private Stack<BasePanel> uiPanelsStack;
    /// <summary>
    /// 通过UIType查找对应的Panel实例
    /// </summary>
    private Dictionary<UIType, BasePanel> uiPanelDict;
    private Transform canvasTransform;
    /// <summary>
    /// 消息提示面板
    /// </summary>
    public MessageTipsPanel messageTips;

    public UIManager(GameFace face) : base(face)
    {
    }
    public override void OnInit()
    {
        base.OnInit();
        uiPanelsStack = new Stack<BasePanel>();
        uiPanelDict = new Dictionary<UIType, BasePanel>();
        canvasTransform = GameObject.Find("Canvas").transform;
        messageTips = BuildMessageTipsPanel();
        Debug.Log(messageTips);
    }
    /// <summary>
    /// 加载UIPanel到页面并自动跳转 返回对应面板
    /// </summary>
    /// <param name="uIType"></param>
    public BasePanel PushUIPanel(UIType uiType)
    {
        // 若栈内存在Panel
        if (uiPanelDict.Count > 0)
        {
            BasePanel topPanel = uiPanelsStack.Peek();
            if(topPanel != null)
                topPanel.OnExit();
        }

        // 显示Panel
        BasePanel targetPanel = GetUIPanel(uiType);
        Debug.Log("当前显示的面板为"+targetPanel);
        uiPanelsStack.Push(targetPanel);
        targetPanel.OnEnter();
        return targetPanel;
    }
    /// <summary>
    /// 退回上个UIPanel显示 并返回当前页面Panel
    /// </summary>
    public BasePanel PopUIPanel()
    {
        if (uiPanelsStack.Count > 0)
        {
            BasePanel topPanel = uiPanelsStack.Pop();
            topPanel.OnExit();

            // 恢复上个Panel
            BasePanel currentPanel = uiPanelsStack.Peek();
            currentPanel.OnEnter();
            return currentPanel;
        }
        else
        {
            Debug.Log("当前页面为起始页面，无法再返回");
            return null;
        }
    }
    /// <summary>
    /// 获取当前显示UI面板
    /// </summary>
    /// <returns></returns>
    public BasePanel GetCurrentPanel()
    {
        return uiPanelsStack.Peek();
    }
    /// <summary>
    /// 获取UIPanel
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private BasePanel GetUIPanel(UIType type)
    {
        // 存在字典中直接拿取
        if(uiPanelDict.TryGetValue(type, out BasePanel panel))
        {
            return panel;
        }
        else
        {
            // 字典未记录则加载
            GameObject perfab = LoadUIPanelPerfab(type);
            // 如果当前没有Canvas 则刷新Canvas
            if (canvasTransform == null)
                canvasTransform = GameObject.FindGameObjectsWithTag("Canvas")[0].transform;
            panel = GameObject.Instantiate(perfab, canvasTransform).GetComponent<BasePanel>();
            panel.gameObject.name = type.ToString();
            // 存入字典
            uiPanelDict.Add(type, panel);
            panel.panelType = type;
            // 首次加载需要初始化
            panel.OnStart();
            return panel;
        }
    }
    private GameObject LoadUIPanelPerfab(UIType uiType)
    {
        return Resources.Load<GameObject>(uiType.ToString());
    }
    /// <summary>
    /// 创建消息提示面板
    /// </summary>
    /// <returns></returns>
    public MessageTipsPanel BuildMessageTipsPanel()
    {
        GameObject perfab = Resources.Load<GameObject>("MessageTipsPanel");
        // 如果当前没有Canvas 则刷新Canvas
        if(canvasTransform == null)
            canvasTransform = GameObject.FindGameObjectsWithTag("Canvas")[0].transform;

        MessageTipsPanel tipsPanel = GameObject.Instantiate(perfab, canvasTransform)
            .GetComponent<MessageTipsPanel>();
        tipsPanel.gameObject.name = "MessageTipsPanel";
        return tipsPanel;
    }
}
