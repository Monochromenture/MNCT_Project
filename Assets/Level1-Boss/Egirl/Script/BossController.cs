using System.Collections;
using System.Collections.Generic;
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

    public enum BossState { Idle, Follow, Kick, Punch, Barrage, GloveThrow, UwU, Pickup, GunAttack, Slash, Defeated }
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
        player = GameObject.FindGameObjectWithTag("Player").transform;

    }

    private void Update()
    {

    }

    IEnumerator AIUpdate()
    {
        while (health > 0)
        {
            animator.SetFloat("Speed", currentState == BossState.Follow ? followSpeed : 0f);
           // Debug.Log($"AIUpdate: CurrentState = {currentState}");


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
                case BossState.GunAttack:
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
            currentState = BossState.Idle;
            Debug.Log("第二階段：切換到 Idle 狀態。");
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
                Debug.Log("觸發第一次彈幕攻擊");
                return;
            }
        }
        else
        {
            if (attackCount >= subsequentBarrageThreshold)
            {
                attackCount = 0;
                currentState = BossState.Barrage;
                Debug.Log("觸發後續彈幕攻擊");
                return;
            }
        }

        // 確保在手套攻擊期間不觸發彈幕
        if (!isGloveThrowing)
        {
            currentState = BossState.Follow;
        }
        else
        {
            Debug.Log("因為手套攻擊，彈幕被阻止。");
        }
    }






    IEnumerator Idle()
    {
        if (secondPhase)
        {
            currentState = BossState.Pickup; // 進入撿取物資狀態
            animator.SetTrigger("PickupTrigger");
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
    private bool isGloveThrowing = false;  // 是否正在進行手套攻擊


    IEnumerator BarrageAttack()
    {
        // 如果 Boss 已經在進行手套攻擊，則跳過彈幕攻擊
        if (isGloveThrowing)
        {
            Debug.Log("彈幕攻擊被跳過，因為手套攻擊正在進行中。");
            yield break; // 如果手套攻擊正在進行，則跳過 BarrageAttack
        }

        FacePlayer(); // 確保 Boss 朝向玩家
        isBarrageAttacking = true;
        animator.SetBool("IsBarrage", true);

        int barrageCount = Random.Range(5, 9); // 生成 5 到 8 個彈幕
        for (int i = 0; i < barrageCount; i++)
        {
            Vector3 spawnPos = barrageSpawnPoint.position + new Vector3(0, Random.Range(-3f, 3f), 0);
            Instantiate(barragePrefab, spawnPos, Quaternion.identity);
            yield return new WaitForSeconds(0.3f);
        }

        // 等待彈幕結束
        yield return new WaitUntil(() => BarrageTextController.activeBarrageCount <= 0);

        animator.SetBool("IsBarrage", false);
        lastBarrageEndTime = Time.time;

        isBarrageAttacking = false; // 允許下次進行彈幕攻擊
        currentState = BossState.Follow; // 切換回跟隨狀態
        Debug.Log("BarrageAttack 結束，切換回 Follow。");
    }




    // 定義空投物資生成時的 X 範圍與頂端 Y 座標
    public float airdropSpawnMinX = -12f;
    public float airdropSpawnMaxX = 12f;
    public float airdropSpawnY = 10f;


    public GameObject gloveProjectilePrefab;  // 手套預製物件
    public float gloveThrowAnimDuration = 0.8f;   // 手套動畫長度
    public float stunnedDuration = 3f;            // Stunned 狀態持續時間



    IEnumerator GloveThrowAttack()
    {

        isGloveThrowing = true;

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

        // Stop the Boss from following the player while performing the attack
        currentState = BossState.Idle;  // Change the state to Idle or another state where the Boss doesn't follow the player

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

        // 恢復 Boss 行為狀態
        currentState = BossState.UwU;  // Or whatever state you want to transition to after the attack

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
        currentState = BossState.Pickup;
        animator.SetTrigger("IdleTrigger");


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

    private bool isPicking = false;
    private bool hasPickedUp = false; // 確保 Boss 只會撿一次，避免重複
    private GameObject currentAirdrop = null; // 當前空投物資
    private int sniperChance = 70; // 狙擊槍掉落機率 (70%)

    IEnumerator PickupAirdrop()
    {
        isPicking = true;  // 開始撿取
        if (hasPickedUp || currentAirdrop != null) yield break; // 確保每次只撿一個

        hasPickedUp = true; // 避免多次撿取
        animator.SetTrigger("PickupTrigger"); // 播放跳躍撿取動畫
        yield return new WaitForSeconds(0.5f); // 等待跳躍時間

        // 隨機選擇掉落物資 (狙擊槍機率較高)
        GameObject chosenItem = Random.Range(0, 100) < sniperChance ? airDropItems[0] : airDropItems[1];

        // 在 Boss 頭上生成空投物資
        Vector3 spawnPos = transform.position + new Vector3(0, 3f, 0);
        currentAirdrop = Instantiate(chosenItem, spawnPos, Quaternion.identity);
        Debug.Log($"空投物資生成: {chosenItem.name} at {spawnPos}");

        yield return new WaitForSeconds(0.5f); // 給 Boss 撿取的時間

        // 直接觸發對應攻擊
        if (chosenItem.name.Contains("Sniper"))
        {
            Debug.Log("Boss 撿到狙擊槍，進行 GunAttack");
            Destroy(currentAirdrop);
            StartCoroutine(GunAttack());
        }
        else if (chosenItem.name.Contains("Wand"))
        {

            Debug.Log("Boss 撿到魔法杖，進行 SlashAttack");
            Destroy(currentAirdrop);
            StartCoroutine(SlashAttack());
        }

        //yield return new WaitForSeconds(5f); // 等待攻擊結束，避免空投物資重疊
        //hasPickedUp = false; // 允許再次生成空投物資
        //isPicking = false;
        //currentAirdrop = null; // 清除當前空投物資

    }



    public GameObject aimingContainerPrefab; // 瞄準圈的容器預製體
    private bool isGunAttacking = false; // 防止重複執行
    private float hitRadius = 3f; // 讓 hitRadius 成為類別變數

    IEnumerator GunAttack()
    {
        if (isGunAttacking) yield break; // 防止重複執行
        isGunAttacking = true;

        Debug.Log("Egirl 開始槍攻擊！");
        currentState = BossState.GunAttack;
        animator.SetBool("IsGun", true);

        yield return new WaitForSeconds(1f); // 攻擊前的延遲

        int ringCount = 3; // 產生三個瞄準圈
        float ringSpawnDelay = 0.5f; // 每個圈的間隔時間
        float shrinkDuration = 0.5f; // 縮小所需時間

        GameObject previousAimingContainer = null; // 用來儲存上一個 AimingContainer

        for (int i = 0; i < ringCount; i++)
        {
            // 在生成新的 AimingContainer 之前，摧毀上一個
            if (previousAimingContainer != null)
            {
                Destroy(previousAimingContainer); // 刪除上一個 AimingContainer
            }

            Vector3 playerPos = player.transform.position; // 記錄玩家當前位置

            // 產生 AimingContainer
            GameObject aimingContainer = Instantiate(aimingContainerPrefab, playerPos, Quaternion.identity);
            previousAimingContainer = aimingContainer; // 記錄當前的 AimingContainer

            Transform aimingRing = aimingContainer.transform.Find("AimingRing"); // 取得 AimingRing

            if (aimingRing == null)
            {
                Debug.LogError("AimingContainer 缺少 AimingRing！");
                Destroy(aimingContainer);
                continue;
            }

            // 讓 AimingRing 縮小，並在縮小結束時判定傷害
            StartCoroutine(ShrinkAimingRing(aimingRing, shrinkDuration));

            yield return new WaitForSeconds(ringSpawnDelay);
        }

        // **這裡不再額外等待判定傷害，讓 ShrinkAimingRing() 直接處理**


        isGunAttacking = false; // 恢復狀態
        animator.SetBool("IsGun", false);
        currentState = BossState.Idle;
        animator.SetTrigger("IdleTrigger");
        Debug.Log("Egirl 結束槍攻擊，回到 Pickup 狀態。");


        yield return new WaitForSeconds(1f); //攻擊結束冷卻時間
        hasPickedUp = false; // 允許再次生成空投物資
        isPicking = false;
        currentAirdrop = null; // 清除當前空投物資

    }

    IEnumerator ShrinkAimingRing(Transform aimingRing, float duration)
    {
        Vector3 initialScale = aimingRing.localScale;
        Vector3 targetScale = new Vector3(0.3f, 0.3f, 1); // 最小縮放值

        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            aimingRing.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        aimingRing.localScale = targetScale; // 確保最終縮放大小

        // **縮小完成後立即判定傷害**
        Vector3 ringPos = aimingRing.position;
        float distance = Vector3.Distance(player.transform.position, ringPos);

        Debug.Log($"[GunAttack] 玩家位置: {player.transform.position}, 瞄準圈位置: {ringPos}, 距離: {distance}, 判定半徑: {hitRadius}");

        if (distance <= hitRadius)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.TakeDamage(1);
                Debug.Log("[GunAttack] 瞄準圈縮小結束，擊中玩家，扣除 2 點血量！");
            }
            else
            {
                Debug.LogError("[GunAttack] PlayerController 取得失敗！");
            }
        }
        else
        {
            Debug.Log("[GunAttack] 玩家在縮小結束時不在範圍內，未扣血！");
        }

        // **確保 AimingContainer 被刪除**
        if (aimingRing.parent != null)
        {
            Destroy(aimingRing.parent.gameObject);
            Debug.Log("[GunAttack] AimingContainer 已成功刪除！");
        }
        else
        {
            Debug.LogWarning("[GunAttack] 瞄準圈的父物件已不存在！");
        }
    }



    public GameObject platformPrefab; // 平台的預製物件
    public int platformCount = 5; // 固定平台數量
    public float platformSpacing = 3f; // 平台之間的X間隔
    public float platformMinY = -2f, platformMaxY = 2f; // 平台的Y軸範圍
    public float platformSpeed = 2f; // 平台移動速度
    private List<GameObject> activePlatforms = new List<GameObject>();
    private bool isPlatformPhase = false;

    IEnumerator SlashAttack()
    {

        // 讓 Boss 移動到畫面右側
        Vector3 targetPosition = new Vector3(maxX - 3f, transform.position.y, transform.position.z);
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, followSpeed * Time.deltaTime);
            yield return null;
        }

        // 播放 Slash 動畫
        animator.SetBool("IsSlash" ,true);
        yield return new WaitForSeconds(1f);

        // 生成平台
        isPlatformPhase = true;
        GeneratePlatforms();

        // 傳送玩家到最近的平台
        TransportPlayerToNearestPlatform();

        // 等待直到所有平台消失
        yield return new WaitUntil(() => activePlatforms.Count == 0);

        animator.SetBool("IsSlash", false);
        currentState = BossState.Idle;


        yield return new WaitForSeconds(1f); //攻擊結束冷卻時間
        hasPickedUp = false; // 允許再次生成空投物資
        isPicking = false;
        currentAirdrop = null; // 清除當前空投物資
    }

    void TransportPlayerToNearestPlatform()
    {
        if (player == null || activePlatforms.Count == 0) return;

        GameObject nearestPlatform = activePlatforms[0];
        float minDistance = Mathf.Abs(player.position.x - nearestPlatform.transform.position.x);

        // 找出最近的平台
        foreach (var platform in activePlatforms)
        {
            float distance = Mathf.Abs(player.position.x - platform.transform.position.x);
            if (distance < minDistance)
            {
                nearestPlatform = platform;
                minDistance = distance;
            }
        }

        // 讓玩家傳送到平台上方
        Vector3 targetPosition = nearestPlatform.transform.position + Vector3.up * 1.8f;
        // 立即設定位置
        player.position = targetPosition;

    }

    void GeneratePlatforms()
    {
        if (activePlatforms.Count > 0) return; // 防止過多生成

        float startX = minX;
        for (int i = 0; i < platformCount; i++)
        {
            float posY = Random.Range(platformMinY, platformMaxY);
            Vector3 spawnPos = new Vector3(startX + (i * platformSpacing), posY, 0);
            GameObject platform = Instantiate(platformPrefab, spawnPos, Quaternion.identity);
            activePlatforms.Add(platform);
        }
        StartCoroutine(MovePlatforms());
    }


    IEnumerator MovePlatforms()
    {
        while (isPlatformPhase && activePlatforms.Count > 0)
        {
            for (int i = activePlatforms.Count - 1; i >= 0; i--)
            {
                if (activePlatforms[i] != null)
                {
                    activePlatforms[i].transform.position += Vector3.right * platformSpeed * Time.deltaTime;

                    // 檢查平台是否超過 Boss 的位置
                    if (activePlatforms[i].transform.position.x > maxX)
                    {
                        Destroy(activePlatforms[i]);
                        activePlatforms.RemoveAt(i);
                    }
                }
            }
            yield return null;
        }
        EndPlatformPhase();
    }

    void EndPlatformPhase()
    {
        isPlatformPhase = false;
        foreach (var platform in activePlatforms)
        {
            if (platform != null)
            {
                Destroy(platform);
            }
        }
        activePlatforms.Clear();
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
