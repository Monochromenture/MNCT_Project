using System.Collections;
using UnityEngine;

public class CameraZoomTrigger : MonoBehaviour
{
    private Camera cam;

    [Header("Zoom Settings")]
    public float zoomInSize = 3f;    // 縮放後的目標大小
    public float zoomOutSize = 5f;   // 恢復時的原始大小
    public float zoomSpeed = 1f;     // 縮放速度
    public float zoomDuration = 3f;  // 停留多久後恢復

    private bool isZooming = false;

    void Start()
    {
        cam = Camera.main; // 取得主攝影機
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isZooming)
        {
            StartCoroutine(ZoomInAndOut());
        }
    }

    private IEnumerator ZoomInAndOut()
    {
        isZooming = true;
        yield return StartCoroutine(ZoomCoroutine(zoomInSize));

        yield return new WaitForSeconds(zoomDuration); // 停留一段時間

        yield return StartCoroutine(ZoomCoroutine(zoomOutSize));
        isZooming = false;
    }

    private IEnumerator ZoomCoroutine(float targetSize)
    {
        float startSize = cam.orthographicSize;
        float elapsedTime = 0f;

        while (elapsedTime < zoomSpeed)
        {
            cam.orthographicSize = Mathf.Lerp(startSize, targetSize, elapsedTime / zoomSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cam.orthographicSize = targetSize; // 確保最後的大小正確
    }
}
