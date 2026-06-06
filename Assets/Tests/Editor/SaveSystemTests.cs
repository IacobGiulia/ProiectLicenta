using NUnit.Framework;
using UnityEngine;
using System.IO;

public class SaveSystemTests
{
    private PlayerStats CreateMockPlayer()
    {
        GameObject obj = new GameObject();
        PlayerStats stats = obj.AddComponent<PlayerStats>();

        stats.day = 10;
        stats.energy = 80f;
        stats.stamina = 40f;
        stats.strength = 60f;
        stats.progress = 50f;
        stats.totalExercises = 5;
        stats.correctExercises = 3;

        return stats;
    }

    [Test]
    public void SaveGame_ShouldCreateFile()
    {
        PlayerStats stats = CreateMockPlayer();

        SaveSystem.SaveGame(stats);

        string path = Application.persistentDataPath + "/savegame.json";

        Assert.IsTrue(File.Exists(path));
    }

    [Test]
    public void SaveGame_ShouldContainCorrectData()
    {
        PlayerStats stats = CreateMockPlayer();

        SaveSystem.SaveGame(stats);

        string path = Application.persistentDataPath + "/savegame.json";
        string json = File.ReadAllText(path);

        SaveData data = JsonUtility.FromJson<SaveData>(json);

        Assert.AreEqual(10, data.day);
        Assert.AreEqual(80f, data.energy);
        Assert.AreEqual(40f, data.stamina);
        Assert.AreEqual(60f, data.strength);
        Assert.AreEqual(50f, data.progress);
        Assert.AreEqual(5, data.totalExercises);
        Assert.AreEqual(3, data.correctExercises);
    }

    [Test]
    public void LoadGame_ShouldReturnSavedData()
    {
        PlayerStats stats = CreateMockPlayer();

        SaveSystem.SaveGame(stats);

        SaveData loaded = SaveSystem.LoadGame();

        Assert.IsNotNull(loaded);

        Assert.AreEqual(10, loaded.day);
        Assert.AreEqual(80f, loaded.energy);
        Assert.AreEqual(40f, loaded.stamina);
        Assert.AreEqual(60f, loaded.strength);
    }

    [TearDown]
    public void Cleanup()
    {
        string path = Application.persistentDataPath + "/savegame.json";

        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}