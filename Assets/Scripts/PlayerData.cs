using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Game/PlayerData")]
public class PlayerData : ScriptableObject
{
    public float health;
    public float maxHealth;
    public int currentSaveSlot; // 存檔欄位（0-3）
    public bool isGameCompleted;

    // 這裡可以擴展存儲背包數據
}
