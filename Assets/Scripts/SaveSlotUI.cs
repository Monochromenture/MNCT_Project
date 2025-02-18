using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class SaveSlotUI : MonoBehaviour
{
    public PlayerData playerData;
    public Button[] saveSlotButtons; // 4 個存檔欄位的按鈕
    public Text[] saveSlotTexts; // 顯示存檔資訊（時間、血量等）

    private string savePath => Application.persistentDataPath + "/saveSlot";

    private void Start()
    {
        UpdateSaveSlots();
    }

    // **更新 UI 顯示存檔資訊**
    public void UpdateSaveSlots()
    {
        for (int i = 0; i < saveSlotButtons.Length; i++)
        {
            string filePath = savePath + i + ".json";
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                SaveData data = JsonUtility.FromJson<SaveData>(json);
                saveSlotTexts[i].text = $"Slot {i + 1}: {data.saveTime}\nHP: {data.health}/{data.maxHealth}";
            }
            else
            {
                saveSlotTexts[i].text = $"Slot {i + 1}: Empty";
            }
        }
    }

    // **存檔**
    public void SaveGame(int slot)
    {
        playerData.currentSaveSlot = slot;
        SaveSystem.SaveGame(playerData);
        UpdateSaveSlots();
    }

    // **讀檔**
    public void LoadGame(int slot)
    {
        playerData.currentSaveSlot = slot;
        SaveSystem.LoadGame(playerData);
        UpdateSaveSlots();
    }
}
