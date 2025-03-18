using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimingRing : MonoBehaviour
{
    private Vector3 originalScale;
    private float shrinkTime;

    public void StartShrinking(float duration)
    {
        shrinkTime = duration;
        originalScale = transform.localScale;
        StartCoroutine(Shrink());
    }

    private IEnumerator Shrink()
    {
        float elapsedTime = 0f;
        while (elapsedTime < shrinkTime)
        {
            float t = elapsedTime / shrinkTime;
            transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localScale = Vector3.zero;
    }
}
