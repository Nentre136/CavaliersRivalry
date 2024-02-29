using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePanel : MonoBehaviour
{
    // ��Ӧҳ���������������ʼ������PushUIPanelʱ���Զ���ʼ�����
    protected GameFace face;
    [NonSerialized]
    public UIType panelType;

    /// <summary>
    /// Start ִ����Ӽ���������Ȳ��� Startֻ�ڱ���������ʱ����һ��
    /// </summary>
    public virtual void Start()
    {
        face = GameFace.Instance;
    }
    /// <summary>
    /// ����Panel �ú���ִ��˳���Mono����Start����������
    /// </summary>
    public virtual void OnEnter()
    {
        gameObject.SetActive(true);
        GetComponent<BasePanel>().enabled = true;
    }
    /// <summary>
    /// �˳�Panel
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
    /// ��ʼ��OnStart ��Mono����Start����������ִ��
    /// </summary>
    public virtual void OnStart()
    {
    }
}
