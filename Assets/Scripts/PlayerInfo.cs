using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public ParticleController partCont;
    /// <summary>
    /// �������ֵ  ����Ϊ10�ı���
    /// </summary>
    public int maxHealth {  get; private set; }
    /// <summary>
    /// ��ǰ����ֵ
    /// </summary>
    private int curHealth;
    public int Health
    {
        get { return curHealth; }
        set
        {
            if (value == curHealth)
                return;

            if (value >= maxHealth)
                value = maxHealth;
            if (value <= 0)
            {
                value = 0;
                partCont.Death();
            }

            curHealth = value;
            partCont.inforPanel.ChangeHealthBar(curHealth);
        }
    }
    /// <summary>
    /// ������
    /// </summary>
    public float maxEnergy {  get; private set; }
    /// <summary>
    /// ��ǰ������
    /// </summary>
    private float curEnergy;
    public float Energy
    {
        get { return curEnergy; }
        set
        {
            if (value >= maxHealth)
                value = maxHealth;
            if (value <= 0)
                value = 0;
            
            curEnergy = value;
            partCont.inforPanel.ChangeEnergyBar(curEnergy);
        }
    }
    /// <summary>
    /// ������ָ��ٶ� ÿ����ָ�һ��power
    /// </summary>
    public float powerRecoverSpeed {  get; private set; }
    /// <summary>
    /// ����ʱ��
    /// </summary>
    public float defenseTime {  get; private set; }
    /// <summary>
    /// ��ս�˺�
    /// </summary>
    public int damage {  get; private set; }
    /// <summary>
    /// Զ���˺�
    /// </summary>
    public int remoteDamage {  get; private set; }
    /// <summary>
    /// �ƶ��ٶ�
    /// </summary>
    public float speed {  get; private set; }
    /// <summary>
    /// �����ٶ�
    /// </summary>
    public float skillSpeed {  get; private set; }
    /// <summary>
    /// ���ܳ���ʱ��
    /// </summary>
    public float skillTime {  get; private set; }
    /// <summary>
    /// ������ȴʱ��
    /// </summary>
    public float skillCD {  get; private set; }
    public float maxMoveDistance {  get; private set; }

    public float attackEnergy = 2.0f;
    public float attRemoEnergy = 2.0f;
    public float defenseEnergy = 2.0f;
    public float sprintEnergy = 1.0f;
    public float SkillEnergy = 6.0f;
    
    public PlayerInfo()
    {
        maxHealth = 120;
        curHealth = maxHealth;
        maxEnergy = 10.0f;
        curEnergy = maxEnergy;
        powerRecoverSpeed = 5.0f;
        defenseTime = 0.6f;
        damage = 10;
        remoteDamage = 20;
        speed = 20.0f;
        skillSpeed = 10.0f;
        skillTime = 5.0f;
        skillCD = 60.0f;
        maxMoveDistance = 10.0f;
    }
}
