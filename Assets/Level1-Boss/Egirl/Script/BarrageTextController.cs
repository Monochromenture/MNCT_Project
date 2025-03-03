using System.Collections;
using UnityEngine;
using TMPro;

public class BarrageTextController : MonoBehaviour
{
    // 靜態計數器，追蹤當前場景中有多少個彈幕物件
    public static int activeBarrageCount = 0;

    public string[] randomTexts = { "Magenta", "Yellow", "Blue", "Attack", "Danger", "UwU" };
    public float speed = 3f;
    public float lifeTime = 5f;
    public ColorType colorType; // 由隨機分配的顏色決定
    public LayerMask playerLayer;
    public float minY = -3f; // 最低世界座標 Y 值
    public float maxY = 3f;  // 最高世界座標 Y 值
    public float gap = 0.5f; // 最小間隔

    // 用來記錄上一次分配的 Y 值，初始設為極小值以表示尚未分配過
    private static float lastAssignedY = float.MinValue;

    private TextMeshPro textMesh;

    private void Awake()
    {
        activeBarrageCount++;
        // 確保 Collider2D 為 Trigger
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;
    }

    private void Start()
    {
        // 取得非 UI 版的 TextMeshPro 組件
        textMesh = GetComponent<TextMeshPro>();

        // 隨機選取文字
        int randomIndex = Random.Range(0, randomTexts.Length);
        string selectedText = randomTexts[randomIndex];
        textMesh.text = selectedText;

        // 隨機分配一個顏色（Magenta, Yellow, Blue）
        AssignRandomColor();

        // 設定隨機世界座標的 Y 值（X、Z 保持原位置），確保間隔不小於 gap
        Vector3 pos = transform.position;
        float newY = Random.Range(minY, maxY);
        if (lastAssignedY != float.MinValue && Mathf.Abs(newY - lastAssignedY) < gap)
        {
            // 若新生成的 Y 值與上一次相差不足 gap，
            // 嘗試往上偏移 gap，如果超過 maxY，則往下調整
            if (newY + gap <= maxY)
                newY += gap;
            else
                newY -= gap;
            newY = Mathf.Clamp(newY, minY, maxY);
        }
        pos.y = newY;
        transform.position = pos;
        lastAssignedY = newY; // 更新上一次分配的 Y 值

        // 在 lifeTime 秒後自動銷毀
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        // 每幀向左移動
        transform.Translate(Vector2.left * speed * Time.deltaTime);
    }

    private void OnDestroy()
    {
        activeBarrageCount--;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(1);
            }
            Destroy(gameObject);
        }
    }

    private void AssignRandomColor()
    {
        // 定義可用的顏色選項
        ColorType[] possibleColors = new ColorType[] { ColorType.Magenta, ColorType.Yellow, ColorType.Blue };
        int rand = Random.Range(0, possibleColors.Length);
        colorType = possibleColors[rand];

        // 根據隨機選到的顏色設定文字顏色
        switch (colorType)
        {
            case ColorType.Magenta:
                textMesh.color = Color.magenta;
                break;
            case ColorType.Yellow:
                textMesh.color = Color.yellow;
                break;
            case ColorType.Blue:
                textMesh.color = Color.blue;
                break;
        }
    }
}

public enum ColorType { Magenta, Yellow, Blue }
