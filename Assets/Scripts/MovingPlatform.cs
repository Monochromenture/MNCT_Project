using System.Collections;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Vector2 moveDirection = Vector2.right; // 移動方向（預設向右）
    public float moveDistance = 3f; // 移動範圍
    public float speed = 2f; // 移動速度
    public float waitTime = 1f; // 等待時間

    private Vector2 startPos; // 起始位置
    private Vector2 targetPos; // 目標位置
    private bool movingForward = true; // 移動方向
    private bool isWaiting = false; // 等待狀態

    private void Start()
    {
        startPos = transform.position;
        targetPos = startPos + moveDirection.normalized * moveDistance;
    }

    private void Update()
    {
        if (!isWaiting)
        {
            StartCoroutine(MovePlatform());
        }
    }

    private IEnumerator MovePlatform()
    {
        isWaiting = true;

        float timeElapsed = 0;
        Vector2 initialPos = transform.position;
        Vector2 destination = movingForward ? targetPos : startPos;

        while (timeElapsed < 1f)
        {
            transform.position = Vector2.Lerp(initialPos, destination, timeElapsed);
            timeElapsed += Time.deltaTime * speed;
            yield return null;
        }

        transform.position = destination;
        yield return new WaitForSeconds(waitTime); // 到達後等待
        movingForward = !movingForward;
        isWaiting = false;
    }

    // 讓角色站上時跟著移動
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}
