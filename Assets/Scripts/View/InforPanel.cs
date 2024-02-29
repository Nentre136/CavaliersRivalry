using Google.Protobuf.WellKnownTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class InforPanel : MonoBehaviour
{
    private Transform healthBar;
    private Transform energyBar;
    private GameObject healthShell;
    private GameObject energyShell;
    /// <summary>
    /// 存放有血红心 包括半血
    /// </summary>
    private Stack<Image> Health;
    /// <summary>
    /// 存放空血红心
    /// </summary>
    private Stack<Image> DeHealth;
    /// <summary>
    /// 有能量栈
    /// </summary>
    private Stack<Image> Energy;
    /// <summary>
    /// 空能量栈
    /// </summary>
    private Stack<Image> DeEnergy;
    public bool isHead {  get; private set; }
    public const int oneHeartHealth = 20;
    public const float oneLigEnergy = 1.0f;
    private int curHealth;
    private float curEnergy;

    void Awake()
    {
        healthBar = transform.Find("HealthBar");
        energyBar = transform.Find("EnergyBar");
        healthShell = Resources.Load<GameObject>("GameCombat/HealthShell");
        energyShell = Resources.Load<GameObject>("GameCombat/EnergyShell");
        Health = new Stack<Image>();
        DeHealth = new Stack<Image>();
        Energy = new Stack<Image>();
        DeEnergy = new Stack<Image>();
        isHead = false;
    }
    void Update()
    {
        if (isHead && Camera.main != null)
        {
            Vector3 direc = (Camera.main.transform.position - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(-direc);
        }
            
    }
    /// <summary>
    /// 初始化血量条 bool判断是否为头顶血条
    /// 半颗心10血
    /// </summary>
    /// <param name="value"></param>
    /// <param name="inHead"></param>
    public void InitHealthBar(int value,bool inHead = false)
    {
        if (value == 0)
            return;
        curHealth = value;
        int count = value / oneHeartHealth;
        for(int i=0;i<count;i++)
        {
            Image hs = Instantiate(healthShell, healthBar).GetComponent<Image>();
            hs.name = "H" + i.ToString();
            Health.Push(hs);
        }
        // 半颗心
        if(value % (oneHeartHealth * count) !=0)
        {
            Image hs = Instantiate(healthShell, healthBar).GetComponent<Image>();
            hs.fillAmount = 0.5f;
            hs.transform.Find("Health").GetComponent<Image>().fillAmount = 0.5f;
            hs.name = "H"+count.ToString();
            Health.Push(hs);
        }
        isHead = inHead;
        if (inHead)
        {
            int c = (value/ oneHeartHealth);
            c = value % oneHeartHealth > 0 ? c+1 : c;
            int width = c * 40 + (c - 1) * 10;
            transform.Find("HealthBar").GetComponent<RectTransform>().sizeDelta = new Vector2(width, 40);
        }
    }
    /// <summary>
    /// 初始化能量条 一个闪电一点能量
    /// </summary>
    /// <param name="value"></param>
    public void InitEnergyBar(float value)
    {
        if (value == 0)
            return;
        curEnergy = value;
        for (int i = 0; i < value; i++)
        {
            Image es = Instantiate(energyShell, energyBar).GetComponent<Image>();
            es.name = "E"+i.ToString();
            Energy.Push(es);
        }
    }
    /// <summary>
    /// 刷新血条
    /// </summary>
    /// <param name="value"></param>
    public void ChangeHealthBar(int value)
    {
        if (curHealth == value)
            return;

        if (value < curHealth)
        {
            int deValue = curHealth - value;
            float deCount = (deValue * 1.0f) / oneHeartHealth;
            while (Health.Count > 0 && deCount > 0)
            {
                Image hs = Health.Peek();
                if (deCount - hs.transform.Find("Health").GetComponent<Image>().fillAmount >= 0)
                {
                    deCount -= hs.transform.Find("Health").GetComponent<Image>().fillAmount;
                    hs.transform.Find("Health").GetComponent<Image>().fillAmount = 0;
                    if (DeHealth.Count > 0 && (Health.Peek() == DeHealth.Peek()))
                        Health.Pop();
                    else
                        DeHealth.Push(Health.Pop());
                }
                else
                {
                    hs.transform.Find("Health").GetComponent<Image>().fillAmount -= deCount;
                    deCount = 0;
                    if (DeHealth.Count <= 0 || (DeHealth.Count > 0 && (DeHealth.Peek() != Health.Peek())))
                        DeHealth.Push(Health.Peek());
                }
            }
        }
        else if (value > curHealth)// 回血
        {
            int addValue = value - curHealth;
            float addCount = (addValue * 1.0f) / oneHeartHealth;
            while (DeHealth.Count > 0 && addCount > 0)
            {
                Image hs = DeHealth.Peek();
                if (addCount + hs.transform.Find("Health").GetComponent<Image>().fillAmount >= 1)
                {
                    addCount -= (1 - hs.transform.Find("Health").GetComponent<Image>().fillAmount);
                    hs.transform.Find("Health").GetComponent<Image>().fillAmount = 1;
                    if (Health.Count > 0 && (DeHealth.Peek() == Health.Peek()))
                        DeHealth.Pop();
                    else
                        Health.Push(DeHealth.Pop());
                }
                else
                {
                    hs.transform.Find("Health").GetComponent<Image>().fillAmount += addCount;
                    addCount = 0;
                    if (Health.Count <= 0 || (Health.Count > 0 && (DeHealth.Peek() != Health.Peek())))
                        Health.Push(DeHealth.Peek());
                }
            }
        }
        curHealth = value;
    }
    /// <summary>
    /// 刷新能量条
    /// </summary>
    public void ChangeEnergyBar(float value)
    {
        if(value == curEnergy) 
            return;

        if (value < curEnergy)
        {
            float deValue = curEnergy - value;
            while (Energy.Count > 0 && deValue > 0)
            {
                Image es = Energy.Peek();
                if (deValue - es.transform.Find("Energy").GetComponent<Image>().fillAmount >= 0)
                {
                    deValue -= es.transform.Find("Energy").GetComponent<Image>().fillAmount;
                    es.transform.Find("Energy").GetComponent<Image>().fillAmount = 0;
                    if (DeEnergy.Count > 0 && (Energy.Peek() == DeEnergy.Peek()))
                        Energy.Pop();
                    else
                        DeEnergy.Push(Energy.Pop());
                }
                else
                {
                    es.transform.Find("Energy").GetComponent<Image>().fillAmount -= deValue;
                    deValue = 0;
                    if (DeEnergy.Count <= 0 || (DeEnergy.Count > 0 && (DeEnergy.Peek() != Energy.Peek())))
                        DeEnergy.Push(Energy.Peek());
                }
            }
        }
        else if (value > curEnergy)
        {
            float addValue = value - curEnergy;
            while (DeEnergy.Count > 0 && addValue > 0)
            {
                Image es = DeEnergy.Peek();
                if (addValue + es.transform.Find("Energy").GetComponent<Image>().fillAmount >= 1)
                {
                    addValue -= (1 - es.transform.Find("Energy").GetComponent<Image>().fillAmount);
                    es.transform.Find("Energy").GetComponent<Image>().fillAmount = 1;
                    if (Energy.Count > 0 && (DeEnergy.Peek() == Energy.Peek()))
                        DeEnergy.Pop();
                    else
                        Energy.Push(DeEnergy.Pop());
                }
                else
                {
                    es.transform.Find("Energy").GetComponent<Image>().fillAmount += addValue;
                    addValue = 0;
                    if (Energy.Count <= 0 || (Energy.Count > 0 && (DeEnergy.Peek() != Energy.Peek())))
                        Energy.Push(DeEnergy.Peek());
                }
            }
        }
        curEnergy = value;
    }
}
