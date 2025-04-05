using UnityEngine;
using System.Collections;

public class Weapon1 : MonoBehaviour
{
    public float attackCooldown = 0.5f; // 攻擊冷卻時間
    public float attackDuration = 0.2f; // 揮動時間
    public float attackAngle = 90f;     // 揮動角度
    private bool isAttacking = false;
    private float lastAttackTime;

    private Quaternion initialRotation;
    private Collider2D weaponCollider;
    private float lastDamageTime; // 記錄上次傷害時間

    void Start()
    {
        initialRotation = transform.localRotation;
        weaponCollider = GetComponent<Collider2D>(); // 取得武器 Collider
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
        weaponCollider.enabled = true; // 啟用碰撞
        float elapsedTime = 0f;

        Quaternion startRotation = Quaternion.Euler(0, 0, attackAngle / 2);
        Quaternion endRotation = Quaternion.Euler(0, 0, -attackAngle / 2);

        while (elapsedTime < attackDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / attackDuration;
            transform.localRotation = Quaternion.Lerp(startRotation, endRotation, t);
            yield return null;
        }

        transform.localRotation = initialRotation;
        isAttacking = false;

        weaponCollider.enabled = false; // 關閉碰撞
        yield return new WaitForSeconds(0.1f); // 延遲0.1秒再開啟碰撞
        weaponCollider.enabled = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isAttacking && collision.CompareTag("Enemy") && Time.time - lastDamageTime > attackCooldown)
        {
            EgirlController boss = collision.GetComponent<EgirlController>();
            if (boss != null)
            {
                boss.TakeDamage(1); // 扣血
                lastDamageTime = Time.time; // 更新上次扣血時間
                Debug.Log($"Stay Hit: {collision.name}");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isAttacking && collision.CompareTag("Enemy"))
        {
            EgirlController boss = collision.GetComponent<EgirlController>();
            if (boss != null)
            {
                boss.TakeDamage(1);
                lastDamageTime = Time.time; // 第一次攻擊也要更新扣血時間
                Debug.Log($"Hit: {collision.name}");
            }
        }
    }
}
