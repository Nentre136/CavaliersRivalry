using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GotoTarget : MonoBehaviour
{
    public Camera _camera;
    private LineRenderer lineRenderer;
    private Transform arrows;
    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        arrows = transform.Find("Arrows");
    }
    private void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = _camera.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        int layerMask = 1 << LayerMask.NameToLayer("PositionPanel");

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            Vector3 mousePositionWorld = hit.point;
            mousePositionWorld.y = lineRenderer.transform.position.y;
            Vector3 localTarget = lineRenderer.transform.InverseTransformPoint(mousePositionWorld);
            localTarget.y = 0;
            // 设置线段末尾
            lineRenderer.SetPosition(1, localTarget);
            // 设置箭头位置
            arrows.position = mousePositionWorld;
            // 设置箭头方向
            Quaternion targetRotation = Quaternion.LookRotation(mousePositionWorld - lineRenderer.transform.position);
            targetRotation = Quaternion.Euler(targetRotation.eulerAngles + new Vector3(90,0,45));
            arrows.rotation = targetRotation;
        }
    }
}
