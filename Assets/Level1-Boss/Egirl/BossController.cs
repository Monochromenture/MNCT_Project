using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class BossController : MonoBehaviour
{
    public static event System.Action OnBossDamage;
    public static event System.Action OnBossRespawn;
    public float currentHealth => health;

    public float minX = -20f;
    public float maxX = 20f;
    public float minY = -5f;
    public float maxY = 20f;
    public float stopDistance = 2f;

    public enum BossState { Idle, Follow, Kick, Punch, Barrage, GloveThrow, Stunned, UwU, Gun, Slash, Defeated }
    private BossState currentState = BossState.Idle;

    public Animator animator;
    public Transform player;
    public float followSpeed = 4f;
    public float attackCooldown = 1.5f;
    private bool isAttacking = false;
    private int attackCount = 0;
    public float health = 100f;
    public float maxHealth = 100f;
    private bool secondPhase = false;

    public GameObject uwuProjectile;
    public GameObject[] airDropItems;
    public Transform airDropSpawnPoint;
    public GameObject portal;

    private float lastBarrageEndTime = 0f;
    private float barrageCooldown = 20f;
    private bool isBarrageAttacking = false;  // 新增旗標

    void Start()
    {
        currentState = BossState.Follow;
        StartCoroutine(AIUpdate());
    }

    IEnumerator AIUpdate()
    {
        while (health > 0)
        {
            animator.SetFloat("Speed", currentState == BossState.Follow ? followSpeed : 0f);

            switch (currentState)
            {
                case BossState.Follow:
                    FollowPlayer();
                    break;
                case BossState.Kick:
                    if (!isAttacking) StartCoroutine(KickAttack());
                    break;
                case BossState.Punch:
                    if (!isAttacking) StartCoroutine(PunchAttack());
                    break;
                case BossState.Barrage:
                    if (!isBarrageAttacking)
                        StartCoroutine(BarrageAttack());
                    break;
                case BossState.GloveThrow:
                    GloveThrowAttack();
                    break;
                case BossState.Stunned:
                    StartCoroutine(StunnedState());
                    break;
                case BossState.UwU:
                    UwUAttack();
                    break;
                case BossState.Gun:
                    StartCoroutine(GunAttack());
                    break;
                case BossState.Slash:
                    StartCoroutine(SlashAttack());
                    break;
            }
            yield return null;
        }
    }

    void FollowPlayer()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        float threshold = 0.1f;

        if (distance > stopDistance + threshold)
        {
            Vector2 targetPosition = Vector2.MoveTowards(transform.position, new Vector2(player.position.x, transform.position.y), followSpeed * Time.deltaTime);
            targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
            transform.position = targetPosition;

            animator.SetFloat("Speed", followSpeed);
            Flip(player.position.x - transform.position.x);
        }
        else
        {
            animator.SetFloat("Speed", 0f);
            int attackType = Random.Range(0, 2);
            currentState = attackType == 0 ? BossState.Punch : BossState.Kick;
        }
    }

    IEnumerator KickAttack()
    {
        isAttacking = true;
        animator.SetTrigger("KickTrigger");
        // 假設踢擊動畫長度為 0.8 秒
        float kickAnimDuration = 0.8f;
        yield return new WaitForSeconds(kickAnimDuration);
        // 動畫播放完後再累加攻擊次數
        attackCount++;
        // 等待剩餘的冷卻時間
        yield return new WaitForSeconds(attackCooldown - kickAnimDuration);
        isAttacking = false;
        ResetAttackState();
    }


    IEnumerator PunchAttack()
    {
        isAttacking = true;
        animator.SetTrigger("PunchTrigger");
        // 假設拳擊動畫長度為 0.8 秒
        float punchAnimDuration = 0.8f;
        yield return new WaitForSeconds(punchAnimDuration);
        // 動畫播放完後累加攻擊次數
        attackCount++;
        yield return new WaitForSeconds(attackCooldown - punchAnimDuration);
        isAttacking = false;
        ResetAttackState();
    }

    private bool firstBarrageTriggered = false;

    void ResetAttackState()
    {
        Debug.Log($"ResetAttackState called: attackCount = {attackCount}, firstBarrageTriggered = {firstBarrageTriggered}");
        if (!firstBarrageTriggered)
        {
            if (attackCount >= 10)
            {
                firstBarrageTriggered = true;
                attackCount = 0;
                currentState = BossState.Barrage;
                Debug.Log("Trigger first barrage attack");
                return;
            }
        }
        else
        {
            if (attackCount >= Random.Range(10, 16) && Time.time > lastBarrageEndTime + barrageCooldown)
            {
                attackCount = 0;
                currentState = BossState.Barrage;
                Debug.Log("Trigger subsequent barrage attack");
                return;
            }
        }
        currentState = BossState.Follow;
    }


    public GameObject barragePrefab;
    public Transform barrageSpawnPoint;

    IEnumerator BarrageAttack()
    {
        isBarrageAttacking = true;
        // 讓動畫持續播放，設定 IsBarrage 為 true
        animator.SetBool("IsBarrage", true);

        int barrageCount = Random.Range(5, 9); // 生成 5 到 8 個彈幕
        for (int i = 0; i < barrageCount; i++)
        {
            Vector3 spawnPos = barrageSpawnPoint.position + new Vector3(0, Random.Range(-3f, 3f), 0);
            Instantiate(barragePrefab, spawnPos, Quaternion.identity);
            yield return new WaitForSeconds(0.3f);
        }

        // 等待直到所有彈幕物件都被銷毀
        yield return new WaitUntil(() => BarrageTextController.activeBarrageCount <= 0);

        // 結束彈幕攻擊後，將 IsBarrage 設為 false 讓動畫結束
        animator.SetBool("IsBarrage", false);
        lastBarrageEndTime = Time.time;
        currentState = BossState.Follow;
        isBarrageAttacking = false;
    }





    void GloveThrowAttack()
    {
        animator.SetTrigger("GloveThrowTrigger");
        currentState = BossState.Stunned;
    }

    IEnumerator StunnedState()
    {
        animator.SetTrigger("StunnedTrigger");
        yield return new WaitForSeconds(3f);
        currentState = BossState.UwU;
    }

    void UwUAttack()
    {
        animator.SetTrigger("UwUAttackTrigger");
        Instantiate(uwuProjectile, transform.position, Quaternion.identity);
        currentState = BossState.Follow;
    }

    IEnumerator GunAttack()
    {
        animator.SetTrigger("GunTrigger");
        yield return new WaitForSeconds(3f);
        currentState = BossState.Follow;
    }

    IEnumerator SlashAttack()
    {
        animator.SetTrigger("SlashTrigger");
        yield return new WaitForSeconds(3f);
        currentState = BossState.Follow;
    }

    public void TakeDamage(float amount)
    {
        if (health <= 0) return;

        health -= amount;
        OnBossDamage?.Invoke();

        if (health <= maxHealth * 0.6f && !secondPhase)
        {
            secondPhase = true;
            currentState = BossState.GloveThrow;
        }

        if (health <= 0)
        {
            health = 0;
            currentState = BossState.Defeated;
            StartCoroutine(DefeatSequence());
        }
    }

    IEnumerator DefeatSequence()
    {
        animator.SetTrigger("KneelTrigger");
        yield return new WaitForSeconds(3f);
        Instantiate(portal, transform.position + Vector3.up * 2, Quaternion.identity);
    }

    private void Flip(float moveDirection)
    {
        if (moveDirection > 0)
            transform.localScale = new Vector3(-1, 1, 1);
        else if (moveDirection < 0)
            transform.localScale = new Vector3(1, 1, 1);
    }
}
