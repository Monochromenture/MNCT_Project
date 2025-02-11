using UnityEngine;

public class DamageObject : MonoBehaviour
{
    public int damageAmount = 20;  // 每次觸發扣除的生命值

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 檢查是否碰到標籤為 "Player" 的物件
        if (other.CompareTag("Player"))
        {
            // 取得玩家的 PlayerHealth 組件
            PlayerController health = other.GetComponent<PlayerController>();

            if (health != null)
            {
                // 對玩家造成傷害
                health.TakeDamage(damageAmount);
            }
        }
    }
}
