using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                // 只有當新的存檔點和舊的不一樣時才更新
                if (player.respawnPoint != (Vector2)transform.position)
                {
                    player.SetRespawnPoint(transform.position);  // 設定新復活點

                    // 顯示 UI 訊息
                    CheckpointUIManager uiManager = FindObjectOfType<CheckpointUIManager>();
                    if (uiManager != null)
                    {
                        uiManager.ShowCheckpointMessage();
                    }
                }
            }
        }
    }
}
