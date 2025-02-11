using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BackpackManager : MonoBehaviour
{
    public GameObject weaponSlotPrefab;  // 武器格子Prefab
    public Transform weaponGrid;         // GridLayoutGroup容器
    public WeaponSlotManager weaponSlotManager;  // HUD中的武器管理器

    public WeaponData[] allWeapons;  // 所有武器數據
    public bool[] obtainedWeapons;  // 武器是否已獲得

    public GameObject weaponInfoPanel;  // 顯示武器資訊的面板
    public TextMeshProUGUI weaponDescriptionText;  // 顯示武器描述的文字

    void Start()
    {
        GenerateWeaponSlots();
        weaponInfoPanel.SetActive(false);  // 預設隱藏武器資訊面板
    }

    public void RefreshWeaponSlots()
    {
        // 刪除所有舊的武器格子
        foreach (Transform child in weaponGrid)
        {
            Destroy(child.gameObject);
        }

        // 重新生成所有格子
        GenerateWeaponSlots();
    }

    void GenerateWeaponSlots()
    {
        Debug.Log($"開始生成武器格子，數量：{allWeapons.Length}");

        for (int i = 0; i < allWeapons.Length; i++)
        {
            Debug.Log($"生成第 {i + 1} 個武器格子");

            // 創建武器格子
            GameObject slot = Instantiate(weaponSlotPrefab, weaponGrid);
            Debug.Log($"生成的武器格子：{slot.name}");

            // 獲取武器數據和是否獲得的狀態
            WeaponData weapon = allWeapons[i];
            bool obtained = obtainedWeapons[i];

            // 獲取 Icon 和 Button
            Image icon = slot.transform.Find("Icon").GetComponent<Image>();
            Button button = slot.GetComponent<Button>();

            // 設置武器圖標
            icon.sprite = obtained ? weapon.weaponIcon : weapon.unavailableIcon;

            // 獲取按鈕並設置點擊事件
            if (obtained)
            {
                // 當武器已獲得時，按鈕可以點擊，設置點擊事件
                button.onClick.AddListener(() => EquipWeaponInSlot(weapon));
            }
            else
            {
                // 當武器未獲得時，按鈕禁用
                button.interactable = false;
            }

            // 添加滑鼠懸停事件
            AddHoverEvents(slot, weapon);

        }
    }

    void AddHoverEvents(GameObject slot, WeaponData weapon)
    {
        int weaponIndex = System.Array.IndexOf(allWeapons, weapon);
        bool obtained = obtainedWeapons[weaponIndex];

        EventTrigger trigger = slot.GetComponent<EventTrigger>() ?? slot.AddComponent<EventTrigger>();

        // 滑鼠移入事件
        EventTrigger.Entry pointerEnter = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        pointerEnter.callback.AddListener((data) => { ShowWeaponInfo(weapon, obtained); });
        trigger.triggers.Add(pointerEnter);

        // 滑鼠移出事件
        EventTrigger.Entry pointerExit = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerExit
        };
        pointerExit.callback.AddListener((data) => { HideWeaponInfo(); });
        trigger.triggers.Add(pointerExit);
    }


    void ShowWeaponInfo(WeaponData weapon, bool obtained)
    {
        if (!obtained)
        {
            // 如果武器未獲得，直接返回，不顯示面板
            return;
        }

        weaponInfoPanel.SetActive(true);
        weaponDescriptionText.text = weapon.weaponDescription;

        // 使面板顯示在滑鼠右下角
        Vector2 mousePos = Input.mousePosition;
        Vector2 offset = new Vector2(50, -50);  // 調整偏移距離，使面板出現在滑鼠右下角
        weaponInfoPanel.transform.position = mousePos + offset;
    }

    void Update()
    {
        // 如果武器資訊面板正在顯示，讓它跟隨滑鼠移動
        if (weaponInfoPanel.activeSelf)
        {
            Vector2 mousePos = Input.mousePosition;
            Vector2 offset = new Vector2(180, -100);  // 保持偏移距離，確保顯示在滑鼠右下角
            weaponInfoPanel.transform.position = mousePos + offset;
        }
    }

    void HideWeaponInfo()
    {
        weaponInfoPanel.SetActive(false);
    }



    void EquipWeaponInSlot(WeaponData weapon)
    {
        // 裝備武器到當前或備用插槽
        weaponSlotManager.EquipWeapon(weapon);
    }
}
