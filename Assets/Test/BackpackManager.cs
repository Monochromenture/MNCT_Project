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

            // 获取 Icon 和 EquippedText（TextMeshPro）
            Image icon = slot.transform.Find("Icon").GetComponent<Image>();
            TextMeshProUGUI equippedText = slot.transform.Find("EquippedText").GetComponent<TextMeshProUGUI>();

            // 如果没有找到 EquippedText 组件，打印错误信息
            if (equippedText == null)
            {
                Debug.LogError("EquippedText 组件为空！请检查 WeaponSlotPrefab 设置");
            }

            // 设置武器图标
            icon.sprite = obtained ? weapon.weaponIcon : weapon.unavailableIcon;

            // 根据获得状态显示或隐藏 "EquippedText"
            equippedText.gameObject.SetActive(obtained && (weaponSlotManager.currentWeapon == weapon || weaponSlotManager.backupWeapon == weapon));

            // 禁用按钮如果武器未获得
            Button button = slot.GetComponent<Button>();
            if (obtained)
            {
                button.onClick.AddListener(() => EquipWeaponInSlot(weapon, slot));
            }
            else
            {
                button.interactable = false;
            }
        }
    }

    void EquipWeaponInSlot(WeaponData weapon, GameObject slot)
    {
        Transform equippedText = slot.transform.Find("EquippedText");

        // 切换装备状态的显示
        if (equippedText != null)
        {
            // 检查当前武器是否已经装备
            bool isEquipped = weaponSlotManager.currentWeapon == weapon || weaponSlotManager.backupWeapon == weapon;

            // 切换显示或隐藏 "EquippedText"
            equippedText.gameObject.SetActive(!isEquipped);  // 如果已装备则隐藏，未装备则显示
        }

        // 装备武器
        weaponSlotManager.EquipWeapon(weapon, true);
    }
}
