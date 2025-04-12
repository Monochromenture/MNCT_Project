using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour
{
    [Header("設定要切換的場景名稱")]
    public string sceneName; // 可在 Unity Inspector 設定

    [Header("延遲時間 (秒)")]
    public float delayTime = 30f; // 可在 Unity Inspector 設定延遲時間

    private Coroutine sceneCoroutine; // 用來追蹤 Coroutine 以便取消

    void Start()
    {
        sceneCoroutine = StartCoroutine(NextSceneCoroutine());
    }

    void Update()
    {
        // 按下 ESC 立即切換場景
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SkipScene();
        }
    }

    IEnumerator NextSceneCoroutine()
    {
        yield return new WaitForSeconds(delayTime);
        LoadScene();
    }

    private void SkipScene()
    {
        if (sceneCoroutine != null)
        {
            StopCoroutine(sceneCoroutine); // 取消計時的 Coroutine
        }
        LoadScene();
    }

    private void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("未設定場景名稱，無法切換場景！");
        }
    }
}