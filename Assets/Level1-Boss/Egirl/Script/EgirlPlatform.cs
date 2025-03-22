using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EgirlPlatform : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.SetParent(transform); // 設為子物件，跟隨平台移動
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.SetParent(null); // 解除父物件關係，恢復獨立移動
        }
    }
}
