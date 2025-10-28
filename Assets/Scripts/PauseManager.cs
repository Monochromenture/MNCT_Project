using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance; // 單例模式

    public GameObject pauseMenu; // 暫停頁面的 UI 物件

    private bool isPaused = false; // 是否處於暫停狀態

    private void Awake()
    {
        // 確保只有一個 PauseManager 實例
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 切換場景時保留
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // 檢測 ESC 鍵是否被按下
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f; // 暫停遊戲
            if (pauseMenu != null)
            {
                pauseMenu.SetActive(true); // 顯示暫停頁面
            }
        }
        else
        {
            Time.timeScale = 1f; // 恢復遊戲
            if (pauseMenu != null)
            {
                pauseMenu.SetActive(false); // 隱藏暫停頁面
            }
        }
    }
}
