using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 资源管理器
/// </summary>
public class ResouceManager : MonoBehaviour
{
    /// <summary>
    /// 是否正在进行资源加载
    /// </summary>
    public bool IsLoadComplete {  get; private set; }
    private void Start()
    {
        IsLoadComplete = false;
    }
    /// <summary>
    /// 加载物体获取实例
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public GameObject LoadGameObject(string path,Transform parnt=null)
    {
        GameObject perfab = Resources.Load<GameObject>(path);
        if (parnt != null)
        {
            GameObject instance = Instantiate(perfab,parnt);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.rotation = parnt.rotation;
            return instance;
        }
        else
            return Instantiate(perfab);
    }
    /// <summary>
    /// 异步加载游戏资源
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public void LoadAssetAsync(string path)
    {
        StartCoroutine(LoadResource(path));
    }
    /// <summary>
    /// 异步加载AB包内游戏资源 未写
    /// </summary>
    /// <param name="ABPath"></param>
    /// <param name="assetName"></param>
    /// <returns></returns>
    public GameObject LoadAssetBundleAsync(string ABPath,string assetName)
    {
        return null;
    }

    private IEnumerator LoadResource(string path, Transform parnt = null)
    {
        IsLoadComplete = true;
        ResourceRequest request = Resources.LoadAsync(path);
        // 等待资源加载完成
        yield return request;
        GameObject prefab = request.asset as GameObject;
        if (prefab != null)
        {
            if(parnt != null)
                Instantiate(prefab, parnt);
            else
                Instantiate(prefab);
        }
        IsLoadComplete = false;
        yield break;
    }
}
