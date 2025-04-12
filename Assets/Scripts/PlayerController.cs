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
    private SpriteRenderer playerSr;
    private bool isGrounded;
    private bool isInvincible = false;
    public float invincibilityDuration = 1.5f;

    public float health, maxHealth;
    public Vector2 respawnPoint;
    public Vector2 SceneLoadrespawnPoint;

    public static event Action OnPlayerDamage;
    public static event Action OnPlayerDeath;
    public static event Action OnPlayerRespawn;

    public List<ColorType> unlockedColors = new List<ColorType>();
    private audiomanager audiomanager;

    private void Awake()
    {
        audiomanager = GameObject.FindGameObjectWithTag("Audio").GetComponent<audiomanager>();
    }

    private void Start()
    {
        rigid2D = GetComponent<Rigidbody2D>();
        playerSr = GetComponent<SpriteRenderer>();
        health = maxHealth;
        respawnPoint = transform.position;
    }

    public void TakeDamage(float amount)
    {
        if (health <= 0 || isInvincible) return;

        audiomanager.PlaySFX(audiomanager.hitted);
        health -= amount;
        OnPlayerDamage?.Invoke();

        if (health <= 0)
        {
            audiomanager.PlaySFX(audiomanager.death);
            health = 0;
            Debug.Log("You're dead");
            OnPlayerDeath?.Invoke();
            StartCoroutine(RespawnPlayer());
        }
        else
        {
            StartCoroutine(TemporaryInvincibility());
        }
    }

    private IEnumerator TemporaryInvincibility()
    {
        isInvincible = true;
        for (float i = 0; i < invincibilityDuration; i += 0.2f)
        {
            playerSr.enabled = !playerSr.enabled;
            yield return new WaitForSeconds(0.2f);
        }
        playerSr.enabled = true;
        isInvincible = false;
    }

    private IEnumerator RespawnPlayer()
    {
        rigid2D.velocity = Vector2.zero;
        rigid2D.simulated = false;

        for (int i = 0; i < 6; i++)
        {
            playerSr.enabled = !playerSr.enabled;
            yield return new WaitForSeconds(0.1f);
        }
        playerSr.enabled = true;

        transform.position = respawnPoint;
        health = maxHealth;
        OnPlayerRespawn?.Invoke();
        StartCoroutine(TemporaryInvincibility());
        rigid2D.simulated = true;
    }

    public void SetRespawnPoint(Vector2 newRespawnPoint)
    {
        respawnPoint = newRespawnPoint;
        Debug.Log("Respawn point updated to: " + respawnPoint);
    }

    public void UnlockColor(ColorType color)
    {
        if (!unlockedColors.Contains(color))
        {
            unlockedColors.Add(color);
        }
    }


}
