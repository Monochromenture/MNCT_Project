using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class Trigger2D : MonoBehaviour
{
    [SerializeField] bool destroyOnTriggerEnter;
    [SerializeField] string tagFilter;
    [SerializeField] UnityEvent onTriggerEnter;
    [SerializeField] UnityEvent onTriggerExit;

    [Header("物件啟動/隱藏設定")]
    [SerializeField] GameObject targetObject;
    [SerializeField] float delayTime = 0f;
    [SerializeField] bool setActiveState = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!string.IsNullOrEmpty(tagFilter) && !other.gameObject.CompareTag(tagFilter)) return;

        onTriggerEnter.Invoke();

        // 使用協程來延遲 SetActive
        if (targetObject != null)
        {
            StartCoroutine(DelayedSetActive());
        }

        if (destroyOnTriggerEnter)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!string.IsNullOrEmpty(tagFilter) && !other.gameObject.CompareTag(tagFilter)) return;
        onTriggerExit.Invoke();
    }

    IEnumerator DelayedSetActive()
    {
        yield return new WaitForSeconds(delayTime);
        if (targetObject != null)
        {
            targetObject.SetActive(setActiveState);
        }
    }
}
