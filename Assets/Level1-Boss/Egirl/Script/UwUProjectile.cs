using UnityEngine;

public class UwUProjectile : MonoBehaviour
{
    public float damage = 1f;
    public float stunDuration = 1.5f;
    public float destroyTime = 5f;

    void Start()
    {
        Destroy(gameObject, destroyTime); // 自動銷毀
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            PlayerMovement playerm = collision.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.TakeDamage(damage);
                playerm.Stun(2); // 讓玩家暈眩
            }
            Destroy(gameObject);
        }
    }
}
