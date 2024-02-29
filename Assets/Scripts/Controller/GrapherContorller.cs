using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapherContorller : MonoBehaviour
{
    private PlayerContorller playerContorller;
    private float grapherX = 0;
    private float grapherY = 0;
    /// <summary>
    /// ��ͷ������
    /// </summary>
    private float lensSensitivity = 2.0f;
    void Start()
    {
        playerContorller = transform.parent.GetComponent<PlayerContorller>();
    }
    void Update()
    {
        // ���ɿ��ƾ�ͷ
        if (!playerContorller.isSprintDirec && !playerContorller.isAttackDirec 
            && !playerContorller.isChooseAbility)
        {
            grapherX += Input.GetAxis("Mouse X") * lensSensitivity;
            grapherY -= Input.GetAxis("Mouse Y") * lensSensitivity;
            //����������ת������90��
            grapherY = Mathf.Clamp(grapherY, -90, 35);
            //��ȡ��ת������Ԫֵ
            Quaternion rota = Quaternion.Euler(grapherY, grapherX, 0);
            transform.rotation = rota;
        }

    }
}
