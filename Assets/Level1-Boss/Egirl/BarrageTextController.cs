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
    public ColorType colorType; // 由文字決定的顏色類型
    public LayerMask playerLayer;
    public float minY = -3f; // 最低世界座標 Y 值
    public float maxY = 3f;  // 最高世界座標 Y 值

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
        AssignColorType(selectedText);

        // 設定隨機世界座標的 Y 值（X、Z 保持原位置）
        Vector3 pos = transform.position;
        pos.y = Random.Range(minY, maxY);
        transform.position = pos;

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
            if (player != null && !player.unlockedColors.Contains(colorType))
            {
                player.TakeDamage(10);
            }
            Destroy(gameObject);
        }
    }

    private void AssignColorType(string text)
    {
        switch (text)
        {
            case "Magenta":
                colorType = ColorType.Magenta;
                textMesh.color = Color.magenta;
                break;
            case "Yellow":
                colorType = ColorType.Yellow;
                textMesh.color = Color.yellow;
                break;
            case "Blue":
                colorType = ColorType.Blue;
                textMesh.color = Color.blue;
                break;
            default:
                textMesh.color = Color.white;
                break;
        }
    }
}

public enum ColorType { Magenta, Yellow, Blue }
