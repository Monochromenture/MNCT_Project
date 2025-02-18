using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float moveRange = 3f;  // 移動範圍（從中心點往兩側移動的距離）
    public float speed = 2f;

    private float startPos;
    private SpriteRenderer enemySprite;
    private bool movingRight = true;

    private void Start()
    {
        startPos = transform.position.x;  // 記錄起始位置
        enemySprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        float newX = startPos + Mathf.PingPong(Time.time * speed, moveRange * 2) - moveRange;
        movingRight = newX > transform.position.x;  // 判斷當前移動方向

        transform.position = new Vector3(newX, transform.position.y, transform.position.z);

        // 翻轉角色朝向
        enemySprite.flipX = !movingRight;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(1);  // 讓玩家扣 1 點血
            }
        }
    }
}
