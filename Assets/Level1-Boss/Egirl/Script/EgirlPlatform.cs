using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EgirlPlatform : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                FixedJoint2D joint = collision.gameObject.AddComponent<FixedJoint2D>();
                joint.connectedBody = GetComponent<Rigidbody2D>(); // 讓玩家跟隨平台
                joint.autoConfigureConnectedAnchor = true;
                joint.enableCollision = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            FixedJoint2D joint = collision.GetComponent<FixedJoint2D>();
            if (joint != null)
            {
                Destroy(joint); // 讓玩家恢復獨立移動
            }
        }
    }
}
