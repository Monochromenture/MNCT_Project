using System.Collections;
using UnityEngine;

public class TriggerAnimation : MonoBehaviour
{
    [Header("角色無法移動的時間 (秒)")]
    public float disableTime = 0f; // 預設為 0 秒，可在 Unity 介面調整

    private PlayerMovement playerMovement; // 取得玩家移動腳本
    private Rigidbody2D playerRb; // 取得玩家剛體

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // 確保是玩家觸發
        {
            playerMovement = other.GetComponent<PlayerMovement>();
            playerRb = other.GetComponent<Rigidbody2D>(); // 取得剛體

            if (playerMovement != null)
            {
                playerMovement.enabled = false; // 停止移動腳本
            }

            if (playerRb != null)
            {
                playerRb.velocity = Vector2.zero; // 強制停止移動
                playerRb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation; // 完全鎖定位置
            }

            StartCoroutine(EnableMovementAfterDelay());
        }
    }

    private IEnumerator EnableMovementAfterDelay()
    {
        yield return new WaitForSeconds(disableTime); // 等待指定時間

        if (playerRb != null)
        {
            playerRb.constraints = RigidbodyConstraints2D.FreezeRotation; // 只鎖定旋轉，允許移動
            playerRb.velocity = Vector2.zero; // 確保速度為 0，防止異常滑動
        }

        if (playerMovement != null)
        {
            playerMovement.enabled = true; // 恢復移動腳本
        }
    }
}

