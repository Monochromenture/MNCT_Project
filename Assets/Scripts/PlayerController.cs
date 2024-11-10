using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;


[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{

    public float speed;
    public Rigidbody2D rigid2D;
    public float speed_x_constraint;

    private bool isGrounded;

    public Animator anim;
    public SpriteRenderer PlayerSr;

    public float health, maxHealth;

    public static event Action OnPlayerDamage;
    public static event Action OnPlayerDeath;

    private void Start()
    {
        rigid2D = this.gameObject.GetComponent<Rigidbody2D>();
        health = maxHealth;

    }

    private void Update()
    {
        PlayerMove();
    }

     
    public void PlayerMove()
    {
        float moveInput = Input.GetAxis("Horizontal"); 
        rigid2D.velocity = new Vector2(moveInput * speed, rigid2D.velocity.y);

        if (Mathf.Abs(moveInput) > 0.1f)
        {
            anim.SetBool("Walk", true);  
        }
        else
        {
            anim.SetBool("Walk", false); 
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rigid2D.AddForce(new Vector2(0, 500), ForceMode2D.Impulse);
        }


        if (Input.GetKey(KeyCode.D))
        {
            if (PlayerSr.flipX == true)
            {
                PlayerSr.flipX = false;
            }
            rigid2D.AddForce(new Vector2(100*speed, 0), ForceMode2D.Force);
        }
        if (Input.GetKey(KeyCode.A))
        {
            if(PlayerSr.flipX == false) 
            {
                PlayerSr.flipX = true;
            }
            rigid2D.AddForce(new Vector2(-100*speed, 0), ForceMode2D.Force);
        }
        if (Input.GetKey(KeyCode.S))
        {
            rigid2D.AddForce(new Vector2(0, -100), ForceMode2D.Force);
        }

        if (rigid2D.velocity.x > speed_x_constraint)
        {
            rigid2D.velocity = new Vector2(speed_x_constraint, rigid2D.velocity.y);
        }

        if (rigid2D.velocity.x < -speed_x_constraint)
        {
            rigid2D.velocity = new Vector2(-speed_x_constraint, rigid2D.velocity.y);
        }

    }

    public void TakeDamage(float amount) 
    { 
        health -= amount;
        OnPlayerDamage?.Invoke();

        if (health <=0) 
        { 
            health = 0;
            Debug.Log("You're dead");
            OnPlayerDeath?.Invoke();
        }

    
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")  
        {
            isGrounded = true;
        }

    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground") 
        {
            isGrounded = false;
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            TakeDamage(1);
        }
    }
}


