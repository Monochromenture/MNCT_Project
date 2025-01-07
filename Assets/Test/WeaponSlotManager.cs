using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSlotManager : MonoBehaviour
{
    public Image WeaponImage1;  // 当前武器图片
    public Image WeaponImage2;  // 備用武器图片

    public WeaponData Weapon1;  // 武器1
    public WeaponData Weapon2;   // 武器2



    void Update()
    {
        SwitchWeapons();
    }


    // 点击武器时调用的装备方法
    public void EquipWeapon(WeaponData newWeapon)
    {
        // 1=空，給1
        if (Weapon1 == null)
        {
            Weapon1 = newWeapon;
            UpdateHUD(WeaponImage1, Weapon1);
            return;
        }

        // 1不是新，2空，給2
        if (Weapon1 != newWeapon && Weapon2 == null)
        {
            Weapon2 = newWeapon;
            UpdateHUD(WeaponImage2, Weapon2);
        }

        // 1是新，停
        if (Weapon1 == newWeapon )
        {
            return;
        }
        // 2是新，停
        if (Weapon2 == newWeapon)
        {
            return;
        }

        // 1,2不是新
        else if (Weapon2 != newWeapon && Weapon1 != newWeapon )
        {
            // 如果 Slot2 与点击的武器相同，替换 Slot1
            Weapon1 = newWeapon;
            UpdateHUD(WeaponImage1, Weapon1);
        }
    }

    private void UpdateHUD(Image slotImage, WeaponData weapon)
    {
        slotImage.sprite = weapon.weaponIcon;
    }

    

    // 切换当前武器与备用武器
    private void SwitchWeapons()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f) 
        {

        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
 
        }
    }


}
