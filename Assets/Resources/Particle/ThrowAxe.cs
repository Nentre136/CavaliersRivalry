using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ThrowAxe : MonoBehaviour
{
    public int damage = 0;
    /// <summary>
    /// …À∫¶¿¥‘¥
    /// </summary>
    public GameObject damageBeform;
    private float rotaSpeed = 1200.0f;
    private float flightSpeed = 40.0f;
    private Vector3 rota;
    public Vector3 throwDirec = Vector3.zero;
    private void Start()
    {
        rota = new Vector3 (0, transform.rotation.eulerAngles.y+90, 0);
    }
    void Update()
    {
        if (damageBeform!=null)
        {
            transform.position += throwDirec * flightSpeed * Time.deltaTime;
            rota.z += rotaSpeed * Time.deltaTime;
            rota.z %= 360;
            transform.rotation = Quaternion.Euler(rota);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (damageBeform == null)
            return;

        if (other.tag == "Player" && other.gameObject != damageBeform)
        {
            other.GetComponent<ParticleController>().BeDamage(damage);
            damageBeform.GetComponent<ParticleController>().throwAxe.SetActive(true);
            damageBeform = null;
            Destroy(gameObject);
        }
        else if (other.gameObject != damageBeform)
        {
            damageBeform.GetComponent<ParticleController>().throwAxe.SetActive(true);
            Destroy(gameObject);
        }
    }
}
