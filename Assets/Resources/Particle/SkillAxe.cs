using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class SkillAxe : MonoBehaviour
{
    private ParticleSystem particle;
    public int damage= 0;
    public GameObject damageBeform;
    private float coolingTime = 1.0f;
    private float rotaSpeed = 420.0f;
    private float rotaY;
    private float clock = 0;
    void Start()
    {
        particle = GetComponent<ParticleSystem>();
        rotaY = transform.rotation.eulerAngles.y;
        clock = 0;
        Invoke("PausePart", 0.12f);
    }
    void Update()
    {
        if(clock > 0)
            clock -= Time.deltaTime;

        rotaY += rotaSpeed * Time.deltaTime;
        rotaY %= 360;
        transform.rotation = Quaternion.Euler(new Vector3(0,rotaY,0));
        transform.position = damageBeform.transform.position;
    }
    private void PausePart()
    {
        particle.Pause();
        particle.time = 0.12f;
    }
    private void OnTriggerStay(Collider other)
    {
        if (clock <= 0)
        {
            // …À∫¶¬ﬂº≠
            if (other.tag == "Player" && other.gameObject != damageBeform)
            {
                other.GetComponent<ParticleController>().BeDamage(damage);
                // ÷ÿ÷√º∆ ±∆˜
                clock = coolingTime;
            }
        }
    }
}
