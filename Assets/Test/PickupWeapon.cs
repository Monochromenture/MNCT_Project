using UnityEngine;

public class PickupWeapon : MonoBehaviour
{
    public WeaponData weaponData; // 对应的武器数据 (ScriptableObject)

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player")) // 确保是玩家碰到
        {
            // 获取玩家的背包管理器
            BackpackManager backpack = GameObject.Find("BackpackPanel").GetComponent<BackpackManager>();


            if (backpack != null)
            {
                // 将武器加入玩家的已获得列表
                for (int i = 0; i < backpack.allWeapons.Length; i++)
                {
                    if (backpack.allWeapons[i] == weaponData)
                    {
                        backpack.obtainedWeapons[i] = true;
                        Debug.Log($"玩家获得了武器: {weaponData.weaponName}");
                        break;
                    }
                }

                // 播放拾取音效或效果（可选）
                // AudioSource.PlayClipAtPoint(pickupSound, transform.position);

                // 摧毁武器物体
                Destroy(gameObject);
            }
        }
    }
}
