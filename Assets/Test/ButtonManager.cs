using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject targetPanel; // 要控制的目標面板

    // 切換目標面板的啟用狀態
    public void TogglePanel()
    {
        if (targetPanel != null)
        {
            // 切換顯示與隱藏
            targetPanel.SetActive(!targetPanel.activeSelf);
        }
    }
}
