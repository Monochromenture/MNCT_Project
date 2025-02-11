using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSlotManager : MonoBehaviour
{
    public Image WeaponImage1;  // 当前武器图片
    public Image WeaponImage2;  // 備用武器图片

    public WeaponData Weapon1;  // 武器1
    public WeaponData Weapon2;   // 武器2
    public WeaponData Weapon0;   // 武器2



    void Update()
    {

    }


    // 點擊武器時購買方法
    public void EquipWeapon(WeaponData newWeapon)
    {
        // Weapon1為空，給Weapon1
        if (Weapon1 == null)
        {
            Weapon1 = newWeapon;
            UpdateHUD(WeaponImage1, Weapon1);
            return;
        }

        // Weapon1不是newWeapon，Weapon2為空，給Weapon2
        if (Weapon1 != newWeapon && Weapon2 == null)
        {
            Weapon2 = newWeapon;
            UpdateHUD(WeaponImage2, Weapon2);
            return;
        }

        // Weapon1是newWeapon，直接停止
        if (Weapon1 == newWeapon)
        {
            return;
        }

        // Weapon2是newWeapon，切換Weapon1和Weapon2
        if (Weapon2 == newWeapon)
        {
            WeaponData temp = Weapon1;
            Weapon1 = Weapon2;
            Weapon2 = temp;

            UpdateHUD(WeaponImage1, Weapon1);
            UpdateHUD(WeaponImage2, Weapon2);
            return;
        }

        // 如果Weapon1和Weapon2都不是newWeapon，每次點擊展開替換武器
        if (Weapon1 != newWeapon && Weapon2 != newWeapon)
        {
            // 用新武器替換Weapon1，原本Weapon1移至Weapon2
            Weapon2 = Weapon1;
            Weapon1 = newWeapon;

            UpdateHUD(WeaponImage1, Weapon1);
            UpdateHUD(WeaponImage2, Weapon2);
        }
    }

    // 更新屏幕上的武器圖示
   private void UpdateHUD(Image weaponImage, WeaponData weapon)
    {
        if (weapon != null)
        {
            weaponImage.sprite = weapon.weaponIcon;  // 設置武器圖示
            weaponImage.enabled = true;               // 啟用圖示顯示
        }
        else
        {
            weaponImage.enabled = false;              // 沒有武器時隱藏圖示
        }
    }

    public void SwapWeapons()
    {
        if (Weapon1 != null || Weapon2 != null)
        {
            WeaponData temp = Weapon1;
            Weapon1 = Weapon2;
            Weapon2 = temp;

            UpdateHUD(WeaponImage1, Weapon1);
            UpdateHUD(WeaponImage2, Weapon2);
        }
    }


    /*
    private void UpdateHUD(Image slotImage, WeaponData weapon)
    {
        slotImage.sprite = weapon.weaponIcon;
    }*/






}
