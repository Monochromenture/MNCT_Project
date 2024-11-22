using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Inventory/Weapon")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public Sprite weaponIcon;  // 武器圖片
    public Sprite unavailableIcon;  // 未獲得時顯示的圖示
    public int damage;  // 傷害值
}
