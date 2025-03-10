using System.Collections;
using TMPro;
using Unity.VisualScripting;
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

    public enum BossState { Idle, Follow, Kick, Punch, Barrage, GloveThrow, UwU, Pickup, Gun, Slash, Defeated }
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
    private bool isBarrageAttacking = false;  // 新增旗標

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentState = BossState.Follow;
        StartCoroutine(AIUpdate());
    }

    private void Update()
    {

    }

    IEnumerator AIUpdate()
    {
        while (health > 0)
        {
            animator.SetFloat("Speed", currentState == BossState.Follow ? followSpeed : 0f);
            Debug.Log($"AIUpdate: CurrentState = {currentState}");


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
                    if (!isAttacking) StartCoroutine(GloveThrowAttack());
                    break;
                case BossState.UwU:
                    if (!isAttacking) StartCoroutine(UwUAttack());
                    break;
                case BossState.Pickup:
                    if (!isPicking) StartCoroutine(PickupAirdrop());
                    break;
                case BossState.Gun:
                    StartCoroutine(GunAttack());
                    break;
                case BossState.Slash:
                    StartCoroutine(SlashAttack());
                    break;
                case BossState.Idle:
                    StartCoroutine(Idle());
                    break;
            }
            yield return null;
        }
    }


    private bool firstBarrageTriggered = false;
    private int firstBarrageThreshold = 3;
    private int subsequentBarrageThreshold = 10;
    void ResetAttackState()
    {
        Debug.Log($"ResetAttackState: attackCount = {attackCount}, firstBarrageTriggered = {firstBarrageTriggered}, secondPhase = {secondPhase}");

        if (secondPhase)
        {
            // 當進入第二階段時，預設進入 Idle 狀態
            currentState = BossState.Idle;
            Debug.Log("第二階段：切換到 Idle 狀態");
            return;
        }

        // 第一階段行為
        if (!firstBarrageTriggered)
        {
            if (attackCount >= firstBarrageThreshold)
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
            if (attackCount >= subsequentBarrageThreshold)
            {
                attackCount = 0;
                currentState = BossState.Barrage;
                Debug.Log("Trigger subsequent barrage attack");
                return;
            }
        }
        currentState = BossState.Follow;
    }



    IEnumerator Idle()
    {
        if (secondPhase)
        {
            currentState = BossState.Pickup;
            yield break;
        }

        animator.SetTrigger("IdleTrigger");
        yield return new WaitForSeconds(5f);

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
        Debug.Log("KickAttack");

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
        Debug.Log("PunchAttack");

        yield return new WaitForSeconds(attackCooldown - punchAnimDuration);
        isAttacking = false;
        ResetAttackState();
    }

   

    public GameObject barragePrefab;
    public Transform barrageSpawnPoint;

    IEnumerator BarrageAttack()
    {
       FacePlayer();
        isBarrageAttacking = true;
        animator.SetBool("IsBarrage", true);

        int barrageCount = Random.Range(5, 9); // 生成 5 到 8 個彈幕
        for (int i = 0; i < barrageCount; i++)
        {
            Vector3 spawnPos = barrageSpawnPoint.position + new Vector3(0, Random.Range(-3f, 3f), 0);
            Instantiate(barragePrefab, spawnPos, Quaternion.identity);
            yield return new WaitForSeconds(0.3f);
        }
 
        // 等待彈幕消失
        yield return new WaitUntil(() => BarrageTextController.activeBarrageCount <= 0);

        animator.SetBool("IsBarrage", false);
        lastBarrageEndTime = Time.time;

        isBarrageAttacking = false;  // 解除鎖定，允許下次進入
        currentState = BossState.Follow;  // 確保切換狀態，避免卡住
        Debug.Log("BarrageAttack 結束，切換回 Follow");
    }

    private bool spawnAirdropsActive = false;

    // 定義空投物資生成時的 X 範圍與頂端 Y 座標
    public float airdropSpawnMinX = -12f;
    public float airdropSpawnMaxX = 12f;
    public float airdropSpawnY = 10f;


    IEnumerator SpawnAirdropsContinuously()
    {
        spawnAirdropsActive = true;
        while (spawnAirdropsActive)
        {
            if (airDropItems.Length > 0)
            {
                int randomIndex = Random.Range(0, airDropItems.Length);
                // 產生一個隨機的 X 座標，Y 座標固定為頂端位置
                float randomX = Random.Range(airdropSpawnMinX, airdropSpawnMaxX);
                Vector3 spawnPos = new Vector3(randomX, airdropSpawnY, 0f);
                Instantiate(airDropItems[randomIndex], spawnPos, Quaternion.identity);
                Debug.Log("空投物資生成於 " + spawnPos);
            }
            // 隨機等待 2 到 5 秒後再生成下一個
            yield return new WaitForSeconds(1f);
        }
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
            PlayerMovement pc = player.GetComponent<PlayerMovement>();
            if (pc != null)
            {
                pc.DisableMovement();
                Debug.Log("玩家移動已禁用");
            }
        }


        float bossOffset = 8f;   // Boss 距離畫面右邊的偏移量
        float playerOffset = 3f; // 玩家距離畫面左邊的偏移量

        // Boss 移動到畫面右側 (利用 maxX)
        Vector3 newBossPos = new Vector3(maxX - bossOffset, transform.position.y, transform.position.z);
        transform.position = newBossPos;
        Debug.Log("Boss 移動到畫面右側：" + newBossPos);

        // 玩家移動到畫面左側 (利用 minX)
        if (player != null)
        {
            Vector3 newPlayerPos = new Vector3(minX + playerOffset, player.position.y, player.position.z);
            player.position = newPlayerPos;
            Debug.Log("Player 移動到畫面左側：" + newPlayerPos);
        }


        isAttacking = true;
        Debug.Log("開始 GloveThrowAttack");

        // 播放 GloveThrow 動畫
        animator.SetTrigger("GloveThrowTrigger");
        Debug.Log("播放GloveThrow動畫");
        yield return new WaitForSeconds(3f);
        isAttacking = false;
        currentState = BossState.UwU;

        // 開始掉落空投物資
        // 如果空投生成協程尚未啟動，就啟動它
        if (!spawnAirdropsActive)
        {
            StartCoroutine(SpawnAirdropsContinuously());
        }

    }


    public Transform uwuSpawnPoint;
    private GameObject activeUwUProjectile = null;
    private bool hasSpawnedUwU = false;  // 新增旗標，確保只生成一次

    IEnumerator UwUAttack()
    {
        if (player != null)
        {
            PlayerMovement pc = player.GetComponent<PlayerMovement>();
            if (pc != null)
            {
                pc.EnableMovement();
                Debug.Log("玩家移動已禁用");
            }
        }

        currentState = BossState.UwU;
        animator.SetTrigger("UwUTrigger");

        // 確保只生成一次 UwU 物件
        if (!hasSpawnedUwU && uwuProjectile != null && player != null)
        {
            hasSpawnedUwU = true;
            Vector3 spawnPos = (uwuSpawnPoint != null) ? uwuSpawnPoint.position : transform.position;
            activeUwUProjectile = Instantiate(uwuProjectile, spawnPos, Quaternion.identity);
            Rigidbody2D rb = activeUwUProjectile.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                Vector2 direction = (player.position - spawnPos).normalized;
                rb.velocity = direction * 5f;
            }

            // 5秒後銷毀 UwU 物件
            Destroy(activeUwUProjectile, 5f);
        }

        // 延長動畫的持續時間，例如等待 8 秒
        yield return new WaitForSeconds(3f);

        secondPhase = true;
        currentState = BossState.Idle;
        animator.SetTrigger("IdleTrigger");


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

    private bool isInvincible = false;  // 是否處於無敵狀態
    public float invincibleDuration = 1.5f;  // 無敵時間
    public float flashDuration = 0.7f;  // 閃爍時間
    public float flashInterval = 0.1f;  // 閃爍間隔
    private SpriteRenderer spriteRenderer;


    public void TakeDamage(float amount)
    {
        if (isInvincible || isBarrageAttacking) return;  // 無敵時不受傷害

        if (health <= 0) return;

        health -= amount;
        Debug.Log("Boss 受傷, 現在血量：" + health);
        OnBossDamage?.Invoke();

        if (health <= 6 && !secondPhase)
        {
            Debug.Log("Boss 狀態應該切換到 GloveThrow");
            currentState = BossState.GloveThrow;
            StartCoroutine(GloveThrowAttack());
        }


        if (health <= 0)
        {
            health = 0;
            currentState = BossState.Defeated;
            StartCoroutine(DefeatSequence());
        }
        else
        {
            StartCoroutine(InvincibilityTimer()); // 開始無敵計時
            StartCoroutine(InvincibilityFlash()); // 觸發閃爍
        }
    }

    IEnumerator InvincibilityFlash()
    {
        float timer = 0f;

        while (timer < flashDuration) // 閃爍時間內進行閃爍
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(flashInterval);
            timer += flashInterval;
        }

        spriteRenderer.enabled = true; // 確保閃爍結束時可見
    }

    IEnumerator InvincibilityTimer()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleDuration); // 無敵時間倒數
        isInvincible = false;
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


    // 新增：空投撿取參數與旗標
    public float pickupSpeed = 3f;
    public float pickupDistance = 1.5f;
    private bool isPicking = false;


    IEnumerator PickupAirdrop()
    {
        isPicking = true;
        // 取得所有標籤為 "Airdrop" 的空投物資
        GameObject[] airdrops = GameObject.FindGameObjectsWithTag("Airdrop");
        if (airdrops.Length == 0)
        {
            Debug.Log("沒有找到空投物資");
            isPicking = false;
            currentState = BossState.Idle;
            yield break;
        }

        // 找出最近的空投物資
        GameObject targetAirdrop = null;
        float closestDistance = Mathf.Infinity;
        foreach (GameObject item in airdrops)
        {
            float distance = Vector2.Distance(transform.position, item.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                targetAirdrop = item;
            }
        }

        // 移動到空投物資
        while (targetAirdrop != null && Vector2.Distance(transform.position, targetAirdrop.transform.position) > pickupDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetAirdrop.transform.position, pickupSpeed * Time.deltaTime);
            yield return null;
        }

        // 當進入撿取範圍後，觸發 JumpGrab 動畫
        animator.SetTrigger("JumpGrab");
        Debug.Log("觸發 JumpGrab 動畫");
        yield return new WaitForSeconds(1f); // 假設動畫長度為 1 秒

        if (targetAirdrop != null)
        {
            Debug.Log("成功取得空投物資！");
            Destroy(targetAirdrop);
        }

        isPicking = false;
        // 撿取完後可以依據取得物品類型來決定下一步攻擊，這裡先回 Idle
        currentState = BossState.Idle;
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
