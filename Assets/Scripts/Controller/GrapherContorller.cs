using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapherContorller : MonoBehaviour
{
    private PlayerContorller playerContorller;
    private float grapherX = 0;
    private float grapherY = 0;
    /// <summary>
    /// 镜头灵敏度
    /// </summary>
    private float lensSensitivity = 2.0f;
    void Start()
    {
        playerContorller = transform.parent.GetComponent<PlayerContorller>();
    }
    void Update()
    {
        // 自由控制镜头
        if (!playerContorller.isSprintDirec && !playerContorller.isAttackDirec 
            && !playerContorller.isChooseAbility)
        {
            grapherX += Input.GetAxis("Mouse X") * lensSensitivity;
            grapherY -= Input.GetAxis("Mouse Y") * lensSensitivity;
            //限制上下旋转不超过90度
            grapherY = Mathf.Clamp(grapherY, -90, 35);
            //获取旋转到的四元值
            Quaternion rota = Quaternion.Euler(grapherY, grapherX, 0);
            transform.rotation = rota;
        }

    }
}
