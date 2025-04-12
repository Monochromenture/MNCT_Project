using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("延遲切換時間 (秒)")]
    public float delayTime = 0f; // 可在 Unity Inspector 設定

    private GameObject player; // 存儲玩家引用

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player"); // 找到玩家物件
    }

    //  無參數版本 (可在 Unity Event 中選擇)
    public void LoadSceneWithDelay(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            StartCoroutine(LoadSceneCoroutine(sceneName));
        }
        else
        {
            Debug.LogWarning("未設定場景名稱，無法切換！");
        }
    }

    // 內部使用 (包含玩家隱藏)
    public void LoadSceneWithDelay(string sceneName, GameObject playerObject)
    {
        player = playerObject; // 允許外部指定玩家
        LoadSceneWithDelay(sceneName);
    }

    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        if (player != null)
        {
            player.SetActive(false); // 觸發後隱藏玩家
        }

        yield return new WaitForSeconds(delayTime); // 等待設定時間

        SceneManager.LoadScene(sceneName); // 切換場景
    }
}
