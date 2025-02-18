using System;
using UnityEngine;
using System.IO;


[Serializable]
public class SaveData
{
    public float health;
    public float maxHealth;
    public bool isGameCompleted;
    public string saveTime; // 新增存檔時間
}

public static class SaveSystem
{
    private static string savePath = Application.persistentDataPath + "/saveSlot";

    public static void SaveGame(PlayerData data)
    {
        SaveData saveData = new SaveData
        {
            health = data.health,
            maxHealth = data.maxHealth,
            isGameCompleted = data.isGameCompleted,
            saveTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") // 存儲存檔時間
        };

        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(savePath + data.currentSaveSlot + ".json", json);
        Debug.Log("Game Saved: " + savePath + data.currentSaveSlot + ".json");
    }

    public static void LoadGame(PlayerData data)
    {
        string filePath = savePath + data.currentSaveSlot + ".json";
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);

            // 將讀取的數據賦值回玩家數據
            data.health = saveData.health;
            data.maxHealth = saveData.maxHealth;
            data.isGameCompleted = saveData.isGameCompleted;

            Debug.Log("Game Loaded: " + filePath);
        }
        else
        {
            Debug.LogWarning("No save file found.");
        }
    }
}
