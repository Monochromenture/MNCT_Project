using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float leftX;  // 左邊界 X 座標
    public float rightX; // 右邊界 X 座標
    public float speed = 2f;
    private bool movingRight = true;
    private SpriteRenderer enemySprite;

    private void Start()
    {
        enemySprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        if (movingRight)
        {
            transform.position += Vector3.right * speed * Time.deltaTime;
            if (transform.position.x >= rightX)
            {
                movingRight = false;
                Flip();
            }
        }
        else
        {
            transform.position += Vector3.left * speed * Time.deltaTime;
            if (transform.position.x <= leftX)
            {
                movingRight = true;
                Flip();
            }
        }
    }

    private void Flip()
    {
        enemySprite.flipX = !enemySprite.flipX;  // 翻轉敵人
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
