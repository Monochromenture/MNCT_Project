using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirdropItem : MonoBehaviour
{
    // 物件存在的時間（秒），可在 Inspector 中調整
    public float lifetime = 10f;

    void Start()
    {
        // 物件在 lifetime 秒後自動銷毀
        Destroy(gameObject, lifetime);
    }
}
