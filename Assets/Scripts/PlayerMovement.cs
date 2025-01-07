using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;


public class PlayerMovement : MonoBehaviour
{

    public Animator anim;
    public SpriteRenderer PlayerSr;

    public Vector2 minBounds; // 移動的最小邊界
    public Vector2 maxBounds; // 移動的最大邊界


    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public bool enableDoubleJump = true;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool canDoubleJump;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        MovePlayer();
        HandleJump();
    }

    private void MovePlayer()
    {
        float moveInput = Input.GetAxis("Horizontal"); // A/D or Left/Right keys
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        if (Input.GetKey(KeyCode.D))
        {
            if (PlayerSr.flipX == true)
            {
                PlayerSr.flipX = false;
            }
        }
        if (Input.GetKey(KeyCode.A))
        {
            if (PlayerSr.flipX == false)
            {
                PlayerSr.flipX = true;
            }
        }

        if (Mathf.Abs(moveInput) > 0.1f)
        {
            anim.SetBool("Walk", true);
        }
        else
        {
            anim.SetBool("Walk", false);
        }

    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                Jump();
                canDoubleJump = true; // Reset double jump when grounded
            }
            else if (enableDoubleJump && canDoubleJump)
            {
                Jump();
                canDoubleJump = false; // Disable further double jumps
            }
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if player is grounded
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f) // Collision from below
            {
                isGrounded = true;
                break;
            }
        }
    }

    private void ClampPlayerPosition()
    {
        Vector3 position = transform.position;
        position.x = Mathf.Clamp(position.x, minBounds.x, maxBounds.x);
        position.y = Mathf.Clamp(position.y, minBounds.y, maxBounds.y);
        transform.position = position;
    }



    private void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }
}
