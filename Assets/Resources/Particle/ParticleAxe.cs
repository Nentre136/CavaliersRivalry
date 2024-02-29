using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAxe : MonoBehaviour
{
    /// <summary>
    /// 造成伤害
    /// </summary>
    public int damage = 0;
    /// <summary>
    /// 伤害来源
    /// </summary>
    public GameObject damageBeform = null;
    private void OnTriggerEnter(Collider other)
    {
        if (damageBeform == null)
            return;

        // 伤害敌人
        if(other.tag == "Player" && other.gameObject != damageBeform)
        {
            other.GetComponent<ParticleController>().BeDamage(damage);
            damageBeform = null;
        }
    }
}
