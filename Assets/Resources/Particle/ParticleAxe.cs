using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAxe : MonoBehaviour
{
    /// <summary>
    /// ����˺�
    /// </summary>
    public int damage = 0;
    /// <summary>
    /// �˺���Դ
    /// </summary>
    public GameObject damageBeform = null;
    private void OnTriggerEnter(Collider other)
    {
        if (damageBeform == null)
            return;

        // �˺�����
        if(other.tag == "Player" && other.gameObject != damageBeform)
        {
            other.GetComponent<ParticleController>().BeDamage(damage);
            damageBeform = null;
        }
    }
}
