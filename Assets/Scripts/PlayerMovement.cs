using Unity.VisualScripting;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    public Animator anim;
    public SpriteRenderer PlayerSr;

    public Vector2 minBounds; // 移動的最小邊界
    public Vector2 maxBounds; // 移動的最大邊界

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("Jump Settings")]
    public bool enableDoubleJump = true;  // 是否開啟二段跳
    public bool enableAirJump = true;     // 是否開啟補救跳躍（離開地面後仍能跳一次）

    private Rigidbody2D rb;
    private bool canDoubleJump;
    private bool jumpPressed = false;
    private bool hasAirJumped = false; // 追蹤玩家是否已經執行過補救跳躍

    [Header("Ground Check Settings")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    public GameObject weapon1;
    public GameObject weapon2;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {

        if (!canMove)
            return;

        MovePlayer();
        ClampPlayerPosition();

        // 監聽跳躍按鍵
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpPressed = true;
        }



    }

    private void FixedUpdate()
    {
        HandleJump();
    }

    private void MovePlayer()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // 角色翻轉
        if (moveInput > 0 && PlayerSr.flipX) PlayerSr.flipX = false;
        if (moveInput < 0 && !PlayerSr.flipX) PlayerSr.flipX = true;

        // 切換武器
        if (PlayerSr.flipX)
        {
            weapon1.SetActive(false);
            weapon2.SetActive(true);
        }
        else
        {
            weapon1.SetActive(true);
            weapon2.SetActive(false);
        }

        // 設定走路動畫
        anim.SetBool("Walk", Mathf.Abs(moveInput) > 0.1f);
    }

    public void HandleJump()
    {
        bool isGrounded = IsGrounded();

        if (isGrounded)
        {
            canDoubleJump = true; // 角色落地時重置雙跳
            hasAirJumped = false; // 角色落地時重置補救跳躍
        }

        if (jumpPressed)
        {
            if (isGrounded || (enableAirJump && !hasAirJumped))
            {
                Jump();
                hasAirJumped = true; // 標記補救跳躍已執行

                if (!isGrounded) // 如果已經在空中跳躍，則啟用雙跳
                {
                    canDoubleJump = false;
                }
            }
            else if (enableDoubleJump && canDoubleJump)
            {
                Jump();
                canDoubleJump = false; // 禁用雙跳
            }

            jumpPressed = false;
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0f);  // 重置垂直速度，避免影響跳躍高度
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void ClampPlayerPosition()
    {
        Vector3 position = transform.position;
        position.x = Mathf.Clamp(position.x, minBounds.x, maxBounds.x);
        position.y = Mathf.Clamp(position.y, minBounds.y, maxBounds.y);
        transform.position = position;
    }

    // 控制移動的旗標，預設可移動
    private bool canMove = true;
    public Rigidbody2D rigid2D;

    // 新增 DisableMovement() 與 EnableMovement() 方法

    public void DisableMovement()
    {
        canMove = false;
        rigid2D.velocity = Vector2.zero; // 停止移動
        anim.SetBool("Walk", false); // 確保關閉走路動畫
        anim.SetBool("Idle", true);  // 進入 Idle 動畫
        Debug.Log("玩家移動已禁用");
    }


    public void EnableMovement()
    {
        canMove = true;
        Debug.Log("玩家移動已恢復");
    }

    // 假設玩家移動邏輯在 Update() 中（這裡僅作示意）

    public void Stun(float duration)
    {
        StartCoroutine(StunCoroutine(duration));
    }

    IEnumerator StunCoroutine(float duration)
    {
        DisableMovement(); // 停止移動
        yield return new WaitForSeconds(duration);
        EnableMovement();
    }


}
