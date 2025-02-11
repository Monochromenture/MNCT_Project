using UnityEngine;

public class BouncePad2D : MonoBehaviour
{
    public float bounceForce = 80f;  // 彈跳力的強度
    public float bounceDampening = 0.5f;  // 彈跳減速系數，讓彈跳不那麼快速

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 確認碰到的是 GroundCheck，並且它的父物件是 Player
        if (other.name == "GroundCheck" && other.transform.parent.CompareTag("Player"))
        {
            Rigidbody2D playerRb = other.transform.parent.GetComponent<Rigidbody2D>();

            if (playerRb != null)
            {
                // 重置玩家的垂直速度，避免多次碰撞疊加
                playerRb.velocity = new Vector2(playerRb.velocity.x, 0);

                // 計算跳躍的反彈力，加入減速效果
                float adjustedBounceForce = bounceForce * (1 - Mathf.Abs(playerRb.velocity.y) * bounceDampening);

                // 給予向上的彈力
                playerRb.AddForce(Vector2.up * adjustedBounceForce, ForceMode2D.Impulse);
            }
        }
    }
}
