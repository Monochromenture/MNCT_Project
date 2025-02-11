using System;
using UnityEngine;

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
    private bool jumpPressed = false;  // 記錄是否按下跳躍鍵

    [Header("Ground Check Settings")]
    public Transform groundCheck;  // 檢查地面的位置
    public float groundCheckRadius = 0.2f;  // 檢查半徑
    public LayerMask groundLayer;  // 地面層級

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        MovePlayer();
        ClampPlayerPosition(); // 限制角色位置

        // 處理跳躍輸入
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpPressed = true;  // 記錄按下跳躍鍵
        }
    }

    private void FixedUpdate()
    {
        HandleJump();  // 在 FixedUpdate 中處理跳躍
    }

    private void MovePlayer()
    {
        float moveInput = Input.GetAxis("Horizontal"); // A/D or Left/Right keys
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // 角色翻轉
        if (moveInput > 0 && PlayerSr.flipX) PlayerSr.flipX = false;
        if (moveInput < 0 && !PlayerSr.flipX) PlayerSr.flipX = true;

        // 設定走路動畫
        anim.SetBool("Walk", Mathf.Abs(moveInput) > 0.1f);
    }

    private void HandleJump()
    {
        if (jumpPressed)
        {
            if (IsGrounded())
            {
                Jump();
                canDoubleJump = true; // 重置雙跳
            }
            else if (enableDoubleJump && canDoubleJump)
            {
                Jump();
                canDoubleJump = false; // 禁止再次雙跳
            }

            jumpPressed = false; // 重置跳躍輸入，避免重複觸發
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0f);  // 重置垂直速度，避免連續跳躍影響
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);  // 物理跳躍
    }

    private bool IsGrounded()
    {
        // 使用 OverlapCircle 檢查角色是否站在地面上
        bool grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // 顯示調試訊息
        //Debug.Log("IsGrounded: " + grounded);

        return grounded;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 檢查角色是否落地
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    private void ClampPlayerPosition()
    {
        Vector3 position = transform.position;
        position.x = Mathf.Clamp(position.x, minBounds.x, maxBounds.x);
        position.y = Mathf.Clamp(position.y, minBounds.y, maxBounds.y);
        transform.position = position;
    }
}
