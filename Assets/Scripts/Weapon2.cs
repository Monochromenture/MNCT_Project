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
    private Collider2D weaponCollider;
    private float lastDamageTime; // 記錄上次傷害時間

    private audiomanager audiomanager;

    private void Awake()
    {
        audiomanager = GameObject.FindGameObjectWithTag("Audio").GetComponent<audiomanager>();
    }

    void Start()
    {
        initialRotation = transform.localRotation;
        weaponCollider = GetComponent<Collider2D>(); // 取得武器 Collider
    }

    void Update()
    {
        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.V)) && Time.time - lastAttackTime > attackCooldown)
        {
            lastAttackTime = Time.time;
            StartCoroutine(SwingWeapon());
        }
    }

    private IEnumerator SwingWeapon()
    {
        audiomanager.PlaySFX(audiomanager.swing);
        isAttacking = true;
        weaponCollider.enabled = true; // 啟用碰撞
        float elapsedTime = 0f;

        Quaternion startRotation = Quaternion.Euler(0, 0, -attackAngle / 2);
        Quaternion endRotation = Quaternion.Euler(0, 0, attackAngle / 2);

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
        yield return new WaitForSeconds(0.1f);
        weaponCollider.enabled = true; // 0.1秒後重新開啟碰撞
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isAttacking)
        {
            if (collision.CompareTag("Enemy"))
            {
                Enemy enemy = collision.GetComponent<Enemy>();
                EgirlController boss = collision.GetComponent<EgirlController>();

                if (enemy != null)
                {
                    enemy.TakeDamage(1);
                }
                else if (boss != null && Time.time - lastDamageTime > attackCooldown)
                {
                    boss.TakeDamage(1);
                    lastDamageTime = Time.time;
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isAttacking && collision.CompareTag("Enemy") && Time.time - lastDamageTime > attackCooldown)
        {
            EgirlController boss = collision.GetComponent<EgirlController>();
            if (boss != null)
            {
                boss.TakeDamage(1);
                lastDamageTime = Time.time;
                Debug.Log($"Stay Hit: {collision.name}");
            }
        }
    }
}
