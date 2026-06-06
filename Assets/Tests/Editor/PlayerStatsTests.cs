using NUnit.Framework;
using UnityEngine;

public class PlayerStatsTests
{
    private PlayerStats stats;

    [SetUp]
    public void Setup()
    {
        GameObject obj = new GameObject();

        stats = obj.AddComponent<PlayerStats>();
    }

    [Test]
    public void Rank_ShouldBeLegendary_WhenDayIs20()
    {
        stats.day = 20;

        Assert.AreEqual("Legendary Athlete", stats.GetRank());
    }

    [Test]
    public void Progress_ShouldNotExceed100()
    {
        stats.progress = 95f;

        stats.AddProgress(20f);

        Assert.AreEqual(100f, stats.progress);
    }

    [Test]
    public void Workout_ShouldDecreaseEnergy()
    {
        stats.energy = 100f;

        stats.WorkOut(20f, 0f, 0f, 0f, false);

        Assert.AreEqual(80f, stats.energy);
    }

    [Test]
    public void StartNewDay_ShouldResetEnergyAndCounters()
    {
        stats.day = 5;
        stats.energy = 20f;
        stats.totalExercises = 10;
        stats.correctExercises = 7;

        stats.StartNewDay();

        Assert.AreEqual(6, stats.day);
        Assert.AreEqual(100f, stats.energy);
        Assert.AreEqual(0, stats.totalExercises);
        Assert.AreEqual(0, stats.correctExercises);
    }


    [Test]
    public void Workout_ShouldClampStaminaTo100()
    {
        stats.stamina = 95f;

        stats.WorkOut(0f, 10f, 0f, 0f, false);

        Assert.AreEqual(100f, stats.stamina);
    }

    [Test]
    public void Workout_ShouldNotIncreaseCorrectExercises_WhenFalse()
    {
        stats.correctExercises = 0;

        stats.WorkOut(0f, 0f, 0f, 0f, false);

        Assert.AreEqual(0, stats.correctExercises);
    }
}