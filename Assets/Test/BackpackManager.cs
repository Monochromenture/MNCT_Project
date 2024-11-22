using UnityEngine;
using UnityEngine.UI;

public class BackpackManager : MonoBehaviour
{
    public GameObject weaponSlotPrefab; 
    public Transform weaponGrid;        
    public WeaponSlotManager weaponSlotManager;  

    public WeaponData[] allWeapons; 
    public bool[] obtainedWeapons;  

    void Start()
    {
        GenerateWeaponSlots();
    }

    private void Update()
    {
        RefreshWeaponSlots();
    }


    public void RefreshWeaponSlots()
    {

        foreach (Transform child in weaponGrid)
        {
            Destroy(child.gameObject);
        }


        GenerateWeaponSlots();
    }

    void GenerateWeaponSlots()
    {
        for (int i = 0; i < allWeapons.Length; i++)
        {
            GameObject slot = Instantiate(weaponSlotPrefab, weaponGrid);
            WeaponData weapon = allWeapons[i];
            bool obtained = obtainedWeapons[i];

            Image icon = slot.transform.Find("Icon").GetComponent<Image>();
            Button button = slot.GetComponent<Button>();
            Text equippedText = slot.transform.Find("EquippedText").GetComponent<Text>();

            icon.sprite = obtained ? weapon.weaponIcon : weapon.unavailableIcon;
            equippedText.gameObject.SetActive(false);

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
        if (weaponSlotManager.currentWeapon == weapon || weaponSlotManager.backupWeapon == weapon)
        {
            equippedText.gameObject.SetActive(false);
        }
        else
        {
            equippedText.gameObject.SetActive(true);
        }


        weaponSlotManager.EquipWeapon(weapon, true); 
    }
}
