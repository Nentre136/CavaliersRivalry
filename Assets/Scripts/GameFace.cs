using SocketGameProtocol;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 核心模块 集成各种管理器
/// </summary>
public class GameFace : MonoBehaviour
{
    public ClientManager clientManager { get; private set; }
    public RequestManager requestManager {  get; private set; }
    public UIManager uiManager { get; private set; }
    public GameManager gameManager { get; private set; }
    public ResouceManager resouceManager {  get; private set; }
    private static GameFace _instance;
    public static GameFace Instance {  get { return _instance; } }
    private void Awake()
    {
        _instance = this;
        resouceManager = gameObject.AddComponent<ResouceManager>();
        uiManager = new UIManager(this);
        clientManager = new ClientManager(this);
        requestManager = new RequestManager(this);
        gameManager = new GameManager(this);

        uiManager.OnInit();
        clientManager.OnInit();
        requestManager.OnInit();
        gameManager.OnInit();

        DontDestroyOnLoad(gameObject);
    }
    private void OnDestroy()
    {
        clientManager.OnDestroy();
        requestManager.OnDestroy();
    }

}
