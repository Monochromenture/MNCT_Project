using System.Collections;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public PlayerData playerData;

    public Rigidbody2D rigid2D;

    private bool isGrounded;
    private SpriteRenderer playerSr;

    public float health, maxHealth;
    public Vector2 respawnPoint;  // 新增復活點變數
    public Vector2 SceneLoadrespawnPoint;

    public static event Action OnPlayerDamage;
    public static event Action OnPlayerDeath;

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        rigid2D = GetComponent<Rigidbody2D>();
        playerSr = GetComponent<SpriteRenderer>();  // 取得 SpriteRenderer
        health = maxHealth;
        respawnPoint = transform.position;  // 設定初始復活點
        //SceneLoadrespawnPoint = new Vector2(0, 0);

    }




    public void TakeDamage(float amount)
    {
        health -= amount;
        Debug.Log("玩家扣血：" + amount + ", 剩餘血量：" + health);
        OnPlayerDamage?.Invoke();

        if (health <= 0)
        {
            health = 0;
            Debug.Log("You're dead");
            OnPlayerDeath?.Invoke();
            StartCoroutine(RespawnPlayer());
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

    public void SetRespawnPoint(Vector2 newRespawnPoint)
    {
        respawnPoint = newRespawnPoint;
        Debug.Log("Respawn point updated to: " + respawnPoint);
    }


    public List<ColorType> unlockedColors = new List<ColorType>();

    public void UnlockColor(ColorType color)
    {
        if (!unlockedColors.Contains(color))
        {
            unlockedColors.Add(color);
        }
    }



















    /*
         private void OnEnable()
        {
             SceneManager.sceneLoaded += OnSceneLoaded; // 註冊場景加載事件
        }

        private void OnDisable()
        {
             SceneManager.sceneLoaded -= OnSceneLoaded; // 取消註冊
        }

         private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
             // 場景加載後設定玩家重生點
            transform.position = SceneLoadrespawnPoint;
        }


         public void LoadtoNewScene(string targetScene)
         {
             SceneManager.LoadScene(targetScene); // 切換場景
             transform.position = SceneLoadrespawnPoint; // 切換後設置重生點
         }
     */







}