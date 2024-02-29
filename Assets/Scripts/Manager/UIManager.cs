using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : BaseManager
{
    /// <summary>
    /// ��ŵ�ǰ��ʾUI ��ջ˳��洢
    /// </summary>
    private Stack<BasePanel> uiPanelsStack;
    /// <summary>
    /// ͨ��UIType���Ҷ�Ӧ��Panelʵ��
    /// </summary>
    private Dictionary<UIType, BasePanel> uiPanelDict;
    private Transform canvasTransform;
    /// <summary>
    /// ��Ϣ��ʾ���
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
    /// ����UIPanel��ҳ�沢�Զ���ת ���ض�Ӧ���
    /// </summary>
    /// <param name="uIType"></param>
    public BasePanel PushUIPanel(UIType uiType)
    {
        // ��ջ�ڴ���Panel
        if (uiPanelDict.Count > 0)
        {
            BasePanel topPanel = uiPanelsStack.Peek();
            if(topPanel != null)
                topPanel.OnExit();
        }

        // ��ʾPanel
        BasePanel targetPanel = GetUIPanel(uiType);
        Debug.Log("��ǰ��ʾ�����Ϊ"+targetPanel);
        uiPanelsStack.Push(targetPanel);
        targetPanel.OnEnter();
        return targetPanel;
    }
    /// <summary>
    /// �˻��ϸ�UIPanel��ʾ �����ص�ǰҳ��Panel
    /// </summary>
    public BasePanel PopUIPanel()
    {
        if (uiPanelsStack.Count > 0)
        {
            BasePanel topPanel = uiPanelsStack.Pop();
            topPanel.OnExit();

            // �ָ��ϸ�Panel
            BasePanel currentPanel = uiPanelsStack.Peek();
            currentPanel.OnEnter();
            return currentPanel;
        }
        else
        {
            Debug.Log("��ǰҳ��Ϊ��ʼҳ�棬�޷��ٷ���");
            return null;
        }
    }
    /// <summary>
    /// ��ȡ��ǰ��ʾUI���
    /// </summary>
    /// <returns></returns>
    public BasePanel GetCurrentPanel()
    {
        return uiPanelsStack.Peek();
    }
    /// <summary>
    /// ��ȡUIPanel
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private BasePanel GetUIPanel(UIType type)
    {
        // �����ֵ���ֱ����ȡ
        if(uiPanelDict.TryGetValue(type, out BasePanel panel))
        {
            return panel;
        }
        else
        {
            // �ֵ�δ��¼�����
            GameObject perfab = LoadUIPanelPerfab(type);
            // �����ǰû��Canvas ��ˢ��Canvas
            if (canvasTransform == null)
                canvasTransform = GameObject.FindGameObjectsWithTag("Canvas")[0].transform;
            panel = GameObject.Instantiate(perfab, canvasTransform).GetComponent<BasePanel>();
            panel.gameObject.name = type.ToString();
            // �����ֵ�
            uiPanelDict.Add(type, panel);
            panel.panelType = type;
            // �״μ�����Ҫ��ʼ��
            panel.OnStart();
            return panel;
        }
    }
    private GameObject LoadUIPanelPerfab(UIType uiType)
    {
        return Resources.Load<GameObject>(uiType.ToString());
    }
    /// <summary>
    /// ������Ϣ��ʾ���
    /// </summary>
    /// <returns></returns>
    public MessageTipsPanel BuildMessageTipsPanel()
    {
        GameObject perfab = Resources.Load<GameObject>("MessageTipsPanel");
        // �����ǰû��Canvas ��ˢ��Canvas
        if(canvasTransform == null)
            canvasTransform = GameObject.FindGameObjectsWithTag("Canvas")[0].transform;

        MessageTipsPanel tipsPanel = GameObject.Instantiate(perfab, canvasTransform)
            .GetComponent<MessageTipsPanel>();
        tipsPanel.gameObject.name = "MessageTipsPanel";
        return tipsPanel;
    }
}
