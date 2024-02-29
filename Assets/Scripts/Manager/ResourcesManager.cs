using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ��Դ������
/// </summary>
public class ResouceManager : MonoBehaviour
{
    /// <summary>
    /// �Ƿ����ڽ�����Դ����
    /// </summary>
    public bool IsLoadComplete {  get; private set; }
    private void Start()
    {
        IsLoadComplete = false;
    }
    /// <summary>
    /// ���������ȡʵ��
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
    /// �첽������Ϸ��Դ
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public void LoadAssetAsync(string path)
    {
        StartCoroutine(LoadResource(path));
    }
    /// <summary>
    /// �첽����AB������Ϸ��Դ δд
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
        // �ȴ���Դ�������
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
