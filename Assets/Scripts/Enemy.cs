using UnityEngine;

public class Enemy : MonoBehaviour
{
    audiomanager audiomanager;

    private void Awake()
    {
        audiomanager = GameObject.FindGameObjectWithTag("Audio").GetComponent<audiomanager>();
    }

    public int health = 1;

    public void TakeDamage(int damage)
    {
        audiomanager.PlaySFX(audiomanager.enemyhitted);
        health -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage!");

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " has died!");
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = gameObject.GetComponent<PlayerController>();

            playerController.TakeDamage(1);
        }
    }

}
