using System.Collections;
using UnityEngine;
using TMPro; // 引入 TMP 命名空間

public class CheckpointUIManager : MonoBehaviour
{
    public TextMeshProUGUI checkpointText; // TMP 文字元件
    public CanvasGroup canvasGroup; // 控制透明度

    private void Start()
    {
        canvasGroup.alpha = 0; // 初始隱藏
    }

    public void ShowCheckpointMessage()
    {
        StopAllCoroutines();
        StartCoroutine(ShowMessage());
    }

    private IEnumerator ShowMessage()
    {
        checkpointText.text = "重生點已更新！"; // 訊息內容
        yield return FadeUI(1, 0.3f); // 淡入
        yield return new WaitForSeconds(2f); // 顯示 2 秒
        yield return FadeUI(0, 0.3f); // 淡出
    }

    private IEnumerator FadeUI(float targetAlpha, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }
}
