using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderTime : MonoBehaviour
{
    [Header("延遲切換時間 (秒)")]
    public float delayTime = 2f; // 可在 Inspector 設定

    public void LoadSceneWithDelay(string sceneName, GameObject player)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            StartCoroutine(LoadSceneCoroutine(sceneName, player));
        }
        else
        {
            Debug.LogWarning("未設定場景名稱，無法切換！");
        }
    }

    private IEnumerator LoadSceneCoroutine(string sceneName, GameObject player)
    {
        if (player != null)
        {
            player.SetActive(false); // 觸發後隱藏玩家
        }

        yield return new WaitForSeconds(delayTime); // 等待設定時間

        SceneManager.LoadScene(sceneName); // 切換場景
    }
}
