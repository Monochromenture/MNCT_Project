using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirdropItem : MonoBehaviour
{
    public float fallSpeed = 5f;  // 掉落速度
    public float targetHeight = 1f;  // 停止掉落的高度
    public float lifetime = 10f;  // 物件存在時間

    void Start()
    {
        // 物件在 lifetime 秒後自動銷毀
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // 當 y 座標大於目標高度時，持續下降
        if (transform.position.y > targetHeight)
        {
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;
        }
        else
        {
            // 停止掉落，並鎖定 Y 座標
            transform.position = new Vector3(transform.position.x, targetHeight, transform.position.z);
        }
    }
}
