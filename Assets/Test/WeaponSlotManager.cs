using UnityEngine;
using UnityEngine.UI;

public class WeaponSlotManager : MonoBehaviour
{
    public Image weaponSlot1;  // 當前武器圖片
    public Image weaponSlot2;  // 備用武器圖片
    public Text weaponSlot1Text;  // 當前武器名稱
    public Text weaponSlot2Text;  // 備用武器名稱

    public WeaponData currentWeapon;  // 當前武器
    public WeaponData backupWeapon;   // 備用武器

    public void EquipWeapon(WeaponData newWeapon, bool isPrimary)
    {
        if (isPrimary)
        {
            currentWeapon = newWeapon;
            UpdateHUD(weaponSlot1, weaponSlot1Text, currentWeapon);
        }
        else
        {
            backupWeapon = newWeapon;
            UpdateHUD(weaponSlot2, weaponSlot2Text, backupWeapon);
        }
    }

    private void UpdateHUD(Image slotImage, Text slotText, WeaponData weapon)
    {
        slotImage.sprite = weapon.weaponIcon;
        slotText.text = weapon.weaponName;
    }

    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)  // 滑鼠滾輪向上
        {
            SwitchWeapons();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)  // 滑鼠滾輪向下
        {
            SwitchWeapons();
        }
    }

    private void SwitchWeapons()
    {
        // 切換當前武器與備用武器
        WeaponData temp = currentWeapon;
        EquipWeapon(backupWeapon, true);
        EquipWeapon(temp, false);
    }
}