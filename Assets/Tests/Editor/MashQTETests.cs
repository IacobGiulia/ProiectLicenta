using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MashQTETests
{
    private GameObject qteObject;
    private MashQTE mashQTE;

    private GameObject sliderObject;
    private GameObject textObject;
    private GameObject panelObject;

    [SetUp]
    public void Setup()
    {
        qteObject = new GameObject("MashQTE_TestObject");

        mashQTE = qteObject.AddComponent<MashQTE>();

        sliderObject = new GameObject("Slider");
        textObject = new GameObject("TimerText");
        panelObject = new GameObject("Panel");

        sliderObject.transform.SetParent(qteObject.transform);
        textObject.transform.SetParent(qteObject.transform);

        mashQTE.progressBar = sliderObject.AddComponent<Slider>();
        mashQTE.timerText = textObject.AddComponent<TextMeshProUGUI>();
        mashQTE.mashPanel = panelObject;

        mashQTE.baseTimeLimit = 7f;
        mashQTE.minTimeLimit = 3f;

        mashQTE.baseProgressPerTap = 0.04f;
        mashQTE.minProgressPerTap = 0.02f;

        mashQTE.baseDecayRate = 0.02f;
        mashQTE.maxDecayRate = 0.05f;

        mashQTE.progressBar.value = 0.5f;
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(sliderObject);
        Object.DestroyImmediate(textObject);
        Object.DestroyImmediate(panelObject);
        Object.DestroyImmediate(qteObject);
    }


    [Test]
    public void StartQTE_ActivatesPanel()
    {
        mashQTE.mashPanel.SetActive(false);

        mashQTE.StartQTE();

        Assert.IsTrue(mashQTE.mashPanel.activeSelf);
    }

    [Test]
    public void StartQTE_ResetsProgressBar()
    {
        mashQTE.progressBar.value = 0.8f;

        mashQTE.StartQTE();

        Assert.AreEqual(0f, mashQTE.progressBar.value);
    }

    [Test]
    public void StartQTE_DoesNotThrow_WhenPanelIsNull()
    {
        mashQTE.mashPanel = null;

        Assert.DoesNotThrow(() =>
        {
            mashQTE.StartQTE();
        });
    }


    [Test]
    public void Success_SetsWasSuccessfulToTrue()
    {
        mashQTE.Success();

        Assert.IsTrue(mashQTE.WasSuccessful);
    }

    [Test]
    public void Success_DisablesPanel()
    {
        mashQTE.mashPanel.SetActive(true);

        mashQTE.Success();

        Assert.IsFalse(mashQTE.mashPanel.activeSelf);
    }

    [Test]
    public void Success_InvokesCallbackWithTrue()
    {
        bool callbackResult = false;
        bool callbackInvoked = false;

        mashQTE.mashQTESuccessCallback = (success) =>
        {
            callbackInvoked = true;
            callbackResult = success;
        };

        mashQTE.Success();

        Assert.IsTrue(callbackInvoked);
        Assert.IsTrue(callbackResult);
    }

    [Test]
    public void Success_DoesNotThrow_WhenCallbackIsNull()
    {
        mashQTE.mashQTESuccessCallback = null;

        Assert.DoesNotThrow(() =>
        {
            mashQTE.Success();
        });
    }

    [Test]
    public void Success_DoesNotThrow_WhenPanelIsNull()
    {
        mashQTE.mashPanel = null;

        Assert.DoesNotThrow(() =>
        {
            mashQTE.Success();
        });
    }


    [Test]
    public void Fail_SetsWasSuccessfulToFalse()
    {
        mashQTE.WasSuccessful = true;

        mashQTE.Fail();

        Assert.IsFalse(mashQTE.WasSuccessful);
    }

    [Test]
    public void Fail_DisablesPanel()
    {
        mashQTE.mashPanel.SetActive(true);

        mashQTE.Fail();

        Assert.IsFalse(mashQTE.mashPanel.activeSelf);
    }

    [Test]
    public void Fail_InvokesCallbackWithFalse()
    {
        bool callbackResult = true;
        bool callbackInvoked = false;

        mashQTE.mashQTESuccessCallback = (success) =>
        {
            callbackInvoked = true;
            callbackResult = success;
        };

        mashQTE.Fail();

        Assert.IsTrue(callbackInvoked);
        Assert.IsFalse(callbackResult);
    }

    [Test]
    public void Fail_DoesNotThrow_WhenCallbackIsNull()
    {
        mashQTE.mashQTESuccessCallback = null;

        Assert.DoesNotThrow(() =>
        {
            mashQTE.Fail();
        });
    }

    [Test]
    public void Fail_DoesNotThrow_WhenPanelIsNull()
    {
        mashQTE.mashPanel = null;

        Assert.DoesNotThrow(() =>
        {
            mashQTE.Fail();
        });
    }


    [Test]
    public void WasSuccessful_DefaultValue_IsFalse()
    {
        Assert.IsFalse(mashQTE.WasSuccessful);
    }

    [Test]
    public void ProgressBar_IsAssignedCorrectly()
    {
        Assert.IsNotNull(mashQTE.progressBar);
    }

    [Test]
    public void TimerText_IsAssignedCorrectly()
    {
        Assert.IsNotNull(mashQTE.timerText);
    }

    [Test]
    public void MashPanel_IsAssignedCorrectly()
    {
        Assert.IsNotNull(mashQTE.mashPanel);
    }
}