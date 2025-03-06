using UnityEngine;
using System.Collections;

public class Weapon2 : MonoBehaviour
{
    public float attackCooldown = 0.5f; // 攻擊冷卻時間
    public float attackDuration = 0.2f; // 揮動時間
    public float attackAngle = 90f;     // 揮動角度
    private bool isAttacking = false;
    private float lastAttackTime;

    private Quaternion initialRotation;

    void Start()
    {
        initialRotation = transform.localRotation;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.time - lastAttackTime > attackCooldown)
        {
            lastAttackTime = Time.time;
            StartCoroutine(SwingWeapon());
        }
    }

    private IEnumerator SwingWeapon()
    {
        isAttacking = true;
        float elapsedTime = 0f;

        // 設定揮動範圍 (從上往下)
        Quaternion startRotation = Quaternion.Euler(0, 0, attackAngle / 2); // 例如 45°
        Quaternion endRotation = Quaternion.Euler(0, 0, -attackAngle / 2);  // 例如 -45°

        while (elapsedTime < attackDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / attackDuration;
            transform.localRotation = Quaternion.Lerp(startRotation, endRotation, t);
            yield return null;
        }

        transform.localRotation = initialRotation; // 重置位置
        isAttacking = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isAttacking && collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(1); // 給敵人扣 1 點血
            }
        }
        else if (isAttacking && collision.CompareTag("Boss"))
        {
            BossController boss = collision.GetComponent<BossController>();
            if (boss != null)
            {
                boss.TakeDamage(1); // 給 Boss 扣 1 點血
            }
        }
        else if (isAttacking && collision.CompareTag("Player"))
        {
            // 如果碰到玩家，不做任何操作，避免自傷
            return;
        }
    }
}
