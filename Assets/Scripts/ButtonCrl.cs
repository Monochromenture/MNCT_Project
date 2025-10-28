using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonCrl : MonoBehaviour
{
    private bool isPaused = false; // 是否處於暫停狀態
    public GameObject pauseMenu; // 暫停頁面的 UI 物件
    private bool isToggleCooldown = false; // 防止重複觸發的旗標

    // 關閉遊戲
    public void QuitGame()
    {
        Application.Quit(); // 關閉遊戲（適用於 Build 版本）
        Debug.Log("Game is quitting..."); // 在 Unity Editor 內部測試
    }

    // 切換暫停狀態
    public void TogglePause()
    {
        if (isToggleCooldown) return; // 如果正在冷卻，直接返回

        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1; // 暫停或恢復遊戲

        if (pauseMenu != null)
        {
            pauseMenu.SetActive(isPaused); // 顯示或隱藏暫停頁面
        }

        Debug.Log(isPaused ? "Game is paused." : "Game is resumed.");

        StartCoroutine(ToggleCooldown()); // 啟動冷卻協程
    }

    // 加載指定場景
    public void BtnLoadScene(string targetScene)
    {
        Time.timeScale = 1; // 確保切換場景時遊戲恢復正常速度
        SceneManager.LoadScene(targetScene);
    }

    // 切換視窗模式或全螢幕模式
    public void ToggleFullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen; // 切換全螢幕模式
        Debug.Log(Screen.fullScreen ? "Switched to Fullscreen mode." : "Switched to Windowed mode.");
    }

    private void Update()
    {
        // 檢測 ESC 鍵是否被按下
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    // 防止重複觸發的冷卻協程
    private IEnumerator ToggleCooldown()
    {
        isToggleCooldown = true; // 設置冷卻中
        yield return new WaitForSecondsRealtime(0.2f); // 等待 0.2 秒（不受 Time.timeScale 影響）
        isToggleCooldown = false; // 冷卻結束
    }
}
