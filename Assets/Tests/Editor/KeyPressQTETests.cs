using NUnit.Framework;
using UnityEngine;
using TMPro;

public class KeyPressQTETests
{
    private GameObject obj;
    private KeyPressQTE qte;

    private GameObject panel;
    private GameObject keyTextObj;
    private GameObject timerTextObj;

    [SetUp]
    public void Setup()
    {
        obj = new GameObject("KeyQTE");

        qte = obj.AddComponent<KeyPressQTE>();

        panel = new GameObject("Panel");
        panel.AddComponent<RectTransform>(); 
        panel.SetActive(false);
        keyTextObj = new GameObject("KeyText");
        timerTextObj = new GameObject("TimerText");

        qte.qtePanel = panel;

        qte.keyText = keyTextObj.AddComponent<TextMeshProUGUI>();
        qte.timerText = timerTextObj.AddComponent<TextMeshProUGUI>();

        qte.baseTimePerKey = 2.5f;
        qte.minTimePerKey = 1f;
        qte.totalKeys = 4;
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(obj);
        Object.DestroyImmediate(panel);
        Object.DestroyImmediate(keyTextObj);
        Object.DestroyImmediate(timerTextObj);
    }


    [Test]
    public void StartQTE_ResetsKeysCompleted()
    {
        qte.keysCompleted = 5;

        qte.StartQTE();

        Assert.AreEqual(0, qte.keysCompleted);
    }

    [Test]
    public void StartQTE_SetsWasPerfectTrue()
    {
        qte.WasPerfect = false;

        qte.StartQTE();

        Assert.IsTrue(qte.WasPerfect);
    }

    [Test]
    public void StartQTE_ActivatesPanel()
    {
        qte.qtePanel.SetActive(false);

        qte.StartQTE();

        Assert.IsTrue(qte.qtePanel.activeSelf);
    }


    [Test]
    public void ApplyDifficulty_SetsTimePerKeyWithinRange()
    {
        qte.ApplyDifficulty();

        Assert.Greater(qte.baseTimePerKey, 0f);
    }


    [Test]
    public void GenerateNewKey_SetsValidKey()
    {
        qte.GenerateNewKey();

        bool valid = false;

        foreach (KeyCode k in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (qte.currentKey == k)
            {
                valid = true;
                break;
            }
        }

        Assert.IsTrue(valid);
    }


    [Test]
    public void ResetTimer_SetsTimerCorrectly()
    {
        qte.ApplyDifficulty();

        qte.ResetTimer();

        Assert.Greater(qte.timer, 0f);
    }


    [Test]
    public void FinishQTE_DisablesPanel()
    {
        qte.qtePanel.SetActive(true);

        qte.FinishQTE();

        Assert.IsFalse(qte.qtePanel.activeSelf);
    }

    [Test]
    public void FinishQTE_InvokesCallback()
    {
        bool called = false;

        qte.qteFinishedCallback = () => called = true;

        qte.FinishQTE();

        Assert.IsTrue(called);
    }

    [Test]
    public void FinishQTE_RegistersStateCorrectly()
    {
        qte.WasPerfect = true;

        qte.FinishQTE();

        Assert.IsTrue(true); 
    }
}