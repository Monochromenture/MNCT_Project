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
    public float health = 10f;
    public float maxHealth = 10f;
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
                    StartCoroutine(GloveThrowAttack());
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


    public float attackOffset = 1f;    // 水平偏移距離
    public float verticalOffset = 0.5f;
    public float attackRadius = 0.5f;    // 攻擊範圍半徑
    public LayerMask playerLayer;        // 玩家所在的 Layer

    public void DoAttack()
    {
        Debug.Log("DoAttack() 被呼叫");
        // 根據 Boss 的 localScale.x 判斷攻擊方向：正值向右，負值向左
        float facingDirection = Mathf.Sign(transform.localScale.x);
        // 加上水平偏移與向下偏移
        Vector2 attackCenter = (Vector2)transform.position
                               + Vector2.right * attackOffset * facingDirection
                               + Vector2.down * verticalOffset;
        Debug.Log("攻擊中心: " + attackCenter + " 半徑: " + attackRadius);

        Collider2D hit = Physics2D.OverlapCircle(attackCenter, attackRadius, playerLayer);
        if (hit != null)
        {
            // 使用 GetComponent 直接抓取玩家
            PlayerController player = hit.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(1);
                Debug.Log("玩家受到攻擊，扣除1點血量");
            }
            else
            {
                Debug.Log("OverlapCircle 檢測到物件，但找不到 PlayerController");
            }
        }
        else
        {
            Debug.Log("OverlapCircle 沒有檢測到任何物件");
        }

        Debug.Log("verticalOffset: " + verticalOffset);


    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        // 根據 Boss 的 facing 判斷：transform.right * attackOffset 代表水平偏移，再加上垂直偏移
        Vector2 attackCenter = (Vector2)transform.position + (Vector2)(transform.right * attackOffset) + Vector2.down * verticalOffset;
        Gizmos.DrawWireSphere(attackCenter, attackRadius);
    }


    IEnumerator KickAttack()
    {
        isAttacking = true;
        animator.SetTrigger("KickTrigger");
 
        float kickAnimDuration = 0.75f;
        yield return new WaitForSeconds(kickAnimDuration);

        // 動畫播放完後累加攻擊次數
        attackCount++;

        yield return new WaitForSeconds(attackCooldown - kickAnimDuration);
        isAttacking = false;
        ResetAttackState();
    }


    IEnumerator PunchAttack()
    {
        isAttacking = true;
        animator.SetTrigger("PunchTrigger");

        float punchAnimDuration = 0.75f;
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
            if (attackCount >= Random.Range(10, 16) )
            {
                attackCount = 0;
                currentState = BossState.Barrage;
                Debug.Log("Trigger subsequent barrage attack");
                return;
            }
        }
        currentState = BossState.Follow;
    }

    //&& Time.time > lastBarrageEndTime + barrageCooldown

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


    public GameObject gloveProjectilePrefab;  // 手套預製物件
    public float gloveThrowAnimDuration = 0.8f;   // 手套動畫長度
    public float stunnedDuration = 3f;            // Stunned 狀態持續時間



    IEnumerator GloveThrowAttack()
    {
        // 先讓 Boss 面向玩家
        FacePlayer();

        // 禁用玩家移動
        if (player != null)
        {
            PlayerController pc = player.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.DisableMovement();
                Debug.Log("玩家移動已禁用");
            }
        }

        // 播放 GloveThrow 動畫
        animator.SetTrigger("GloveThrowTrigger");

        // 等待手套攻擊動畫播放完畢（假設 gloveThrowAnimDuration 為 0.8 秒）
        yield return new WaitForSeconds(gloveThrowAnimDuration);

        // 發射手套拋出物件（此物件在碰撞時僅觸發玩家閃爍，不扣血）
        // 切換到 Stunned 狀態
        currentState = BossState.Stunned;

        // 結束此協程，由 StunnedState 處理後續（恢復玩家移動、生成空投物資等）
    }

    public void GloveHitPlayer()
    {
        Debug.Log("手套擊中玩家（閃爍效果）");

        Collider2D hit = Physics2D.OverlapCircle(transform.position, attackRadius, playerLayer);
        if (hit != null)
        {
            PlayerController player = hit.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(0); // 觸發閃爍效果
                Debug.Log("玩家閃爍");
            }
        }
    }


    IEnumerator StunnedState()
    {
        animator.SetTrigger("StunnedTrigger");
        yield return new WaitForSeconds(stunnedDuration);

        // 恢復玩家移動
        if (player != null)
        {
            PlayerController pc = player.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.EnableMovement();
                Debug.Log("玩家移動已恢復");
            }
        }

        // 開始掉落粉絲的空投物資（這裡僅示意，請根據需求實作）
        SpawnAirdrops();

        currentState = BossState.Follow;
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
        Debug.Log("Boss 受傷, 現在血量：" + health);
        OnBossDamage?.Invoke();

        if (health <= 6 && !secondPhase)
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

    void FacePlayer()
    {
        if (player != null)
        {
            Vector3 direction = player.position - transform.position;
            if (direction.x > 0)
                transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
            else
                transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        }
    }

    void SpawnAirdrops()
    {
        if (airDropItems.Length == 0 || airDropSpawnPoint == null) return;

        int dropCount = Random.Range(1, 4); // 掉落 1 到 3 個物資
        for (int i = 0; i < dropCount; i++)
        {
            int randomIndex = Random.Range(0, airDropItems.Length);
            Instantiate(airDropItems[randomIndex], airDropSpawnPoint.position, Quaternion.identity);
            Debug.Log("空投物資生成");
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
