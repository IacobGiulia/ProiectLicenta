using NUnit.Framework;
using UnityEngine;

public class TreadmillTests
{
    private GameObject obj;
    private TreadmillPaceSystem treadmill;

    private PlayerStats stats;

    [SetUp]
    public void Setup()
    {
        obj = new GameObject("Treadmill");
        treadmill = obj.AddComponent<TreadmillPaceSystem>();

        stats = obj.AddComponent<PlayerStats>();
        stats.energy = 50;
        stats.stamina = 50;

        treadmill.playerStats = stats;
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(obj);
    }


    [Test]
    public void StartRun_SetsInitialSpeed()
    {
        treadmill.StartRun();

        Assert.AreEqual(3f, treadmill.currentSpeed);
    }

    [Test]
    public void StartRun_InitializesTimer()
    {
        treadmill.runDuration = 10f;

        treadmill.StartRun();

        Assert.AreEqual(10f, treadmill.Timer);
    }


    [Test]
    public void ApplyDifficulty_SetsDifferentValues()
    {
        treadmill.ApplyDifficulty();

        Assert.Greater(treadmill.IdealMax, treadmill.IdealMin);
    }


    [Test]
    public void FinishRun_SetsRunSuccessCorrectly()
    {
        treadmill.baseIdealMin = 0f;
        treadmill.baseIdealMax = 10f;
        treadmill.hardIdealMin = 0f;
        treadmill.hardIdealMax = 10f;

        treadmill.playerStats.energy = 50;
        treadmill.playerStats.stamina = 50;

        treadmill.StartRun();

        treadmill.currentSpeed = 5f;

        treadmill.FinishRun(); 

        Assert.IsTrue(treadmill.RunSuccess);
    }

    [Test]
    public void FinishRun_UpdatesPlayerStats_OnSuccess()
    {
        treadmill.playerStats.energy = 50;
        treadmill.playerStats.stamina = 50;

        treadmill.baseIdealMin = 0f;
        treadmill.baseIdealMax = 10f;
        treadmill.hardIdealMin = 0f;
        treadmill.hardIdealMax = 10f;

        treadmill.ApplyDifficulty();

        treadmill.StartRun();

        treadmill.currentSpeed = 5f; 

        treadmill.FinishRun(); 

        Assert.Greater(treadmill.playerStats.stamina, 50f);
    }

    [Test]
    public void FinishRun_DeductsEnergy()
    {
        treadmill.playerStats.energy = 50;
        treadmill.playerStats.stamina = 50;

        treadmill.baseIdealMin = 0f;
        treadmill.baseIdealMax = 10f;
        treadmill.hardIdealMin = 0f;
        treadmill.hardIdealMax = 10f;

        treadmill.StartRun();

        treadmill.currentSpeed = 5f;

        treadmill.FinishRun();

        Assert.Less(treadmill.playerStats.energy, 50f);
    }

    [Test]
    public void FinishRun_FailureDoesNotIncreaseStamina()
    {
        treadmill.StartRun();

        treadmill.currentSpeed = 1f; 

        Assert.AreEqual(50, treadmill.playerStats.stamina);
    }


    [Test]
    public void StopRun_SetsSpeedToZero()
    {
        treadmill.StartRun();
        treadmill.currentSpeed = 5f;

        treadmill.StopRun();

        Assert.AreEqual(0f, treadmill.currentSpeed);
    }
}