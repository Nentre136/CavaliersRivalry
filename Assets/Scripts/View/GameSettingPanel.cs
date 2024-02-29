using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSettingPanel : MonoBehaviour
{
    private Button settingBtn, exitBtn;
    [SerializeField]
    private AnimationCurve showCurve;
    [SerializeField]
    private AnimationCurve hideCurve;
    [SerializeField]
    private float curveSpeed = 2.5f;
    void Start()
    {
        settingBtn = transform.Find("settingBtn").GetComponent<Button>();
        exitBtn = transform.Find("exitBtn").GetComponent<Button>();

        exitBtn.onClick.AddListener(() =>
        {
            // ÍË³öÓÎÏ·
            string roomName = GameFace.Instance.gameManager.roomName;
            GameFace.Instance.gameManager.myCharacter.GetComponent<GameRequest>().SendExitGameRequest(roomName);
            SceneManager.LoadScene("EnterScene");
        });
    }
    public void ShowPanel()
    {
        StartCoroutine(_ShowPanel(gameObject));
    }
    public void HidePanel()
    {
        StartCoroutine(_HidePanel(gameObject));
    }
    private IEnumerator _ShowPanel(GameObject panel)
    {
        float clock = 0f;
        while (clock <= 1)
        {
            panel.transform.localScale = Vector3.one * showCurve.Evaluate(clock);
            clock += Time.deltaTime * curveSpeed;
            yield return null;
        }
    }
    private IEnumerator _HidePanel(GameObject panel)
    {
        float clock = 0f;
        while (clock <= 1)
        {
            panel.transform.localScale = Vector3.one * hideCurve.Evaluate(clock);
            clock += Time.deltaTime * curveSpeed;
            yield return null;
        }
    }
}
