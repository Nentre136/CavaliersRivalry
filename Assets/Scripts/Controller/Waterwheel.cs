using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Ë®³µ¿ØÖÆÆ÷
/// </summary>
public class Waterwheel : MonoBehaviour
{
    [SerializeField]
    private float rotaSpeed = 1.0f;
    private Transform body;
    private Vector3 rota;
    void Start()
    {
        body = transform.Find("waterwheel").GetComponent<Transform>();
        rota = transform.rotation.eulerAngles;
    }
    void Update()
    {
        float RX = rota.x;
        RX += rotaSpeed * Time.deltaTime;
        RX %= 360;
        rota.x = RX;
        body.rotation = Quaternion.Euler(rota);
    }
}
