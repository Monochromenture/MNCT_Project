using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonCrl : MonoBehaviour
{


    public void QuitGame()
    {
        Application.Quit(); // 關閉遊戲（適用於 Build 版本）
        Debug.Log("Game is quitting..."); // 在 Unity Editor 內部測試
    }

    private bool isPaused = false;
    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1; // 暫停或恢復遊戲
        Debug.Log(isPaused ? "Game is paused." : "Game is resumed.");
    }



    public void BtnLoadScene(string targetScene)
    {
        SceneManager.LoadScene(targetScene); 
    }



}
