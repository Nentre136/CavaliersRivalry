using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpState : MonoBehaviour
{
    public GameRequest gameRequest;
    public UpStateRequest upStateRequest;
    public PlayerInfo playerInfor;
    void Start()
    {
        gameRequest = transform.GetComponent<GameRequest>();
        upStateRequest = transform.GetComponent<UpStateRequest>();
        playerInfor = transform.GetComponent<PlayerInfo>();
        //InvokeRepeating("UpdateState", 1, 1.0f / 60.0f);
    }
    //private void UpdateState()
    //{
    //    Vector3 pos = transform.position;
    //    Vector3 rota = transform.rotation.eulerAngles;
    //    Vector3 rigRota = transform.Find("Rig").localRotation.eulerAngles;
    //    upStateRequest.SendStateRequest(pos, rota, rigRota, playerInfor);
    //}
}
