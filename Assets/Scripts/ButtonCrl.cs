using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonCrl : MonoBehaviour
{


    public void QuitGame()
    {
        Application.Quit(); // 關閉遊戲（適用於 Build 版本）
        Debug.Log("Game is quitting..."); // 在 Unity Editor 內部測試
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
