using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class AutoDisable : MonoBehaviour
{
    public float activeDuration = 2f; // 啟用後持續的時間（秒）
    private Collider2D colli2D; // 引用通用的 2D 碰撞器

    private void Awake()
    {
        // 獲取 Collider2D 組件
        colli2D = GetComponent<Collider2D>();
        if (colli2D == null)
        {
            Debug.LogError("Collider2D 未找到！請確保該物件上有 Collider2D 組件。");
        }
    }

    private void OnEnable()
    {
        StartCoroutine(DisableAfterDelay());
    }

    private void Update()
    {
        // 檢測滑鼠點擊
        if (Input.GetMouseButtonDown(0)) // 0 表示左鍵
        {
            if (!IsPointerOverCollider())
            {
                gameObject.SetActive(false); // 停用物件
            }
        }
    }

    // 協程：延遲後停用物件
    private IEnumerator DisableAfterDelay()
    {
        yield return new WaitForSeconds(activeDuration); // 等待指定時間
        gameObject.SetActive(false); // 停用物件
    }

    // 檢測滑鼠是否點擊在 Collider2D 上
    private bool IsPointerOverCollider()
    {
        if (colli2D == null) return false;

        // 將滑鼠位置轉換為世界座標
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 檢查滑鼠位置是否在 Collider2D 的範圍內
        return colli2D.OverlapPoint(mousePosition);
    }
}
