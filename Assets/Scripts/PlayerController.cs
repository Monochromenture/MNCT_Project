using System.Collections;
using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float speed;
    public Rigidbody2D rigid2D;
    public float speed_x_constraint;

    private bool isGrounded;
    private SpriteRenderer playerSr;

    public float health, maxHealth;
    public Vector2 respawnPoint;  // 新增復活點變數

    public static event Action OnPlayerDamage;
    public static event Action OnPlayerDeath;

    private void Start()
    {
        rigid2D = GetComponent<Rigidbody2D>();
        playerSr = GetComponent<SpriteRenderer>();  // 取得 SpriteRenderer
        health = maxHealth;
        respawnPoint = transform.position;  // 設定初始復活點
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        OnPlayerDamage?.Invoke();

        if (health <= 0)
        {
            health = 0;
            Debug.Log("You're dead");
            OnPlayerDeath?.Invoke();
            StartCoroutine(RespawnPlayer());  // 啟動閃爍 & 重生協程
        }
    }

    public static event Action OnPlayerRespawn;  // 新增復活事件

    private IEnumerator RespawnPlayer()
    {
        rigid2D.velocity = Vector2.zero;  // 停止移動
        rigid2D.simulated = false;  // 暫時關閉剛體碰撞
        Color originalColor = playerSr.color;

        // 閃爍效果
        for (int i = 0; i < 6; i++)
        {
            playerSr.enabled = !playerSr.enabled;
            yield return new WaitForSeconds(0.1f);
        }
        playerSr.enabled = true;

        // 重生
        transform.position = respawnPoint;  // 移動到復活點
        health = maxHealth;  // 恢復血量
        OnPlayerRespawn?.Invoke();  // 觸發事件，讓 UI 刷新
        rigid2D.simulated = true;  // 重新啟動剛體碰撞
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(1);
        }
    }




}
