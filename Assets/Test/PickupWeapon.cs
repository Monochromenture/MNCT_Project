using UnityEngine;

public class PickupWeapon : MonoBehaviour
{
    public WeaponData weaponData; // 對應的武器數據 (ScriptableObject)

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // 確保是玩家碰到
        {
            // 獲取玩家的背包管理器
            BackpackManager backpack = other.GetComponent<BackpackManager>();

            if (backpack != null)
            {
                // 將武器加入玩家的已獲得列表
                for (int i = 0; i < backpack.allWeapons.Length; i++)
                {
                    if (backpack.allWeapons[i] == weaponData)
                    {
                        backpack.obtainedWeapons[i] = true;
                        Debug.Log($"玩家獲得了武器: {weaponData.weaponName}");
                        break;
                    }
                }

                backpack.RefreshWeaponSlots();
                Destroy(gameObject);
            }
        }
    }
}
