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
    public float wallJumpForce = 12f;
    public Vector2 wallJumpDirection = new Vector2(-1, 1);

    [Header("Jump Settings")]
    public bool enableDoubleJump = true;  // 是否開啟二段跳
    public bool enableAirJump = true;     // 是否開啟補救跳躍（離開地面後仍能跳一次）
    public bool canWallJump = true;

    [Header("Wall Sliding Settings")]
    public bool enableWallSlide = true;
    public float wallSlideSpeed = 2f;

    private Rigidbody2D rb;
    private bool canDoubleJump;
    private bool jumpPressed = false;
    private bool hasAirJumped = false; // 追蹤玩家是否已經執行過補救跳躍
    private bool isWallSliding = false;

    [Header("Ground Check Settings")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public LayerMask platformLayer;
    public Transform wallCheck;
    public float wallCheckRadius = 0.2f;
    public LayerMask wallLayer;

    public GameObject weapon1;
    public GameObject weapon2;

    audiomanager audiomanager;

    private bool canMove = true;
    public Rigidbody2D rigid2D;

    private void Awake()
    {
        audiomanager = GameObject.FindGameObjectWithTag("Audio").GetComponent<audiomanager>();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rigid2D = rb;
    }

    void Update()
    {
        if (!canMove)
            return;

        MovePlayer();
        ClampPlayerPosition();

        // 監聽跳躍按鍵
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            jumpPressed = true;
        }
    }

    private void FixedUpdate()
    {


        if (!canMove) return;  // 加入這一行，防止 Stun 時角色繼續動作

        HandleJump();
        HandleWallSlide();
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

    private void HandleJump()
    {
        bool isGrounded = IsGrounded();
        bool isTouchingWall = IsTouchingWall();
        bool isPlatform = IsPlatform();

        if (isGrounded || isPlatform)
        {
            canDoubleJump = true; // 角色落地時重置雙跳
            hasAirJumped = false; // 角色落地時重置補救跳躍
        }

        if (isTouchingWall)
        {
            canDoubleJump = true;
            hasAirJumped = false;
        }

        if (jumpPressed)
        {
            if (isTouchingWall && canWallJump)
            {
                audiomanager.PlaySFX(audiomanager.Jump);
                WallJump();
            }
            else if (isGrounded || isPlatform)
            {
                audiomanager.PlaySFX(audiomanager.Jump);
                Jump();
            }
            else if (enableAirJump && !hasAirJumped && canDoubleJump)
            {
                audiomanager.PlaySFX(audiomanager.Jump);
                Jump();
                hasAirJumped = true; // 標記補救跳躍已執行
                canDoubleJump = false;
            }
            else if (enableDoubleJump && canDoubleJump && !hasAirJumped)
            {
                audiomanager.PlaySFX(audiomanager.Jump);
                Jump();
                canDoubleJump = false; // 禁用雙跳
                hasAirJumped = true;
            }

            jumpPressed = false;
        }
    }

    private void HandleWallSlide()
    {
        bool isGrounded = IsGrounded();
        bool isTouchingWall = IsTouchingWall();

        if (enableWallSlide && isTouchingWall && !isGrounded && rb.velocity.y < 0)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0f);  // 重置垂直速度，避免影響跳躍高度
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void WallJump()
    {
        if (!canWallJump) return;

        float jumpDirection = PlayerSr.flipX ? 1f : -1f;

        rb.velocity = Vector2.zero;
        Vector2 jumpForce = new Vector2(jumpDirection * wallJumpForce, wallJumpForce * 1.2f);
        rb.AddForce(jumpForce, ForceMode2D.Impulse);

        isWallSliding = false;
        hasAirJumped = false;
        canDoubleJump = true;
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private bool IsPlatform()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, platformLayer);
    }

    private bool IsTouchingWall()
    {
        return Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, wallLayer);
    }

    private void ClampPlayerPosition()
    {
        Vector3 position = transform.position;
        position.x = Mathf.Clamp(position.x, minBounds.x, maxBounds.x);
        position.y = Mathf.Clamp(position.y, minBounds.y, maxBounds.y);
        transform.position = position;
    }

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

    private Coroutine stunCoroutine;

    public void Stun(float duration)
    {
        if (stunCoroutine != null)
        {
            StopCoroutine(stunCoroutine); // 停止之前的 Stun
        }
        stunCoroutine = StartCoroutine(StunCoroutine(duration));
    }

    IEnumerator StunCoroutine(float duration)
    {
        DisableMovement(); // 停止移動
        yield return new WaitForSeconds(duration);
        EnableMovement();
    }
}

