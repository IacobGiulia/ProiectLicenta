using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static string path = Application.persistentDataPath + "/savegame.json";

    public static void SaveGame(PlayerStats playerStats)
    {
        SaveData data = new SaveData();

        data.day = playerStats.day;

        data.energy = playerStats.energy;
        data.stamina = playerStats.stamina;
        data.strength = playerStats.strength;

        data.progress = playerStats.progress;

        data.totalExercises = playerStats.totalExercises;
        data.correctExercises = playerStats.correctExercises;

        string json = JsonUtility.ToJson(data, true);

        File.WriteAllText(path, json);

        Debug.Log("Game Saved!");
        Debug.Log(path);
    }

    public static SaveData LoadGame()
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);

            SaveData data = JsonUtility.FromJson<SaveData>(json);

            Debug.Log("Game Loaded!");

            return data;
        }

        Debug.LogWarning("Save file not found!");
        return null;
    }
}