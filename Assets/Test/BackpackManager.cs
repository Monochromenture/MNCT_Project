using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BackpackManager : MonoBehaviour
{
    public GameObject weaponSlotPrefab;  // 武器格子Prefab
    public Transform weaponGrid;         // GridLayoutGroup容器
    public WeaponSlotManager weaponSlotManager;  // HUD中的武器管理器

    public WeaponData[] allWeapons;  // 所有武器数据
    public bool[] obtainedWeapons;  // 武器是否已获得

    void Start()
    {
        GenerateWeaponSlots();
    }

    public void RefreshWeaponSlots()
    {
        // 删除所有旧的武器格子
        foreach (Transform child in weaponGrid)
        {
            Destroy(child.gameObject);
        }

        // 重新生成所有格子
        GenerateWeaponSlots();
    }

    void GenerateWeaponSlots()
    {
        Debug.Log($"开始生成武器格子，数量：{allWeapons.Length}");

        for (int i = 0; i < allWeapons.Length; i++)
        {
            Debug.Log($"生成第 {i + 1} 个武器格子");

            // 创建武器格子
            GameObject slot = Instantiate(weaponSlotPrefab, weaponGrid);
            Debug.Log($"生成的武器格子：{slot.name}");

            // 获取武器数据和是否获得的状态
            WeaponData weapon = allWeapons[i];
            bool obtained = obtainedWeapons[i];

            // 获取 Icon 和 Button
            Image icon = slot.transform.Find("Icon").GetComponent<Image>();
            Button button = slot.GetComponent<Button>();

            // 设置武器图标
            icon.sprite = obtained ? weapon.weaponIcon : weapon.unavailableIcon;

            // 获取按钮并设置点击事件
            if (obtained)
            {
                // 当武器已获得时，按钮可以点击，设置点击事件
                button.onClick.AddListener(() => EquipWeaponInSlot(weapon));
            }
            else
            {
                // 当武器未获得时，按钮禁用
                button.interactable = false;
            }
        }
    }

    void EquipWeaponInSlot(WeaponData weapon)
    {
        // 装备武器到当前或备用插槽
        weaponSlotManager.EquipWeapon(weapon);
    }
}
