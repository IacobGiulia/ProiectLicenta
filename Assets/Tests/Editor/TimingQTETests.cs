using NUnit.Framework;
using System;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class TimingQTETests
{
    private GameObject qteObject;
    private TimingQTE timingQTE;

    private GameObject markerObject;
    private GameObject successZoneObject;
    private GameObject panelObject;

    [SetUp]
    public void Setup()
    {
        qteObject = new GameObject("TimingQTE_TestObject");

        timingQTE = qteObject.AddComponent<TimingQTE>();

        markerObject = new GameObject("Marker");
        successZoneObject = new GameObject("SuccessZone");
        panelObject = new GameObject("Panel");

        markerObject.transform.SetParent(qteObject.transform);
        successZoneObject.transform.SetParent(qteObject.transform);

        timingQTE.marker = markerObject.AddComponent<RectTransform>();
        timingQTE.successZone = successZoneObject.AddComponent<RectTransform>();
        timingQTE.qtePanel = panelObject;

        timingQTE.baseMoveSpeed = 300f;
        timingQTE.maxMoveSpeed = 700f;

        timingQTE.baseZoneWidth = 220f;
        timingQTE.minZoneWidth = 80f;

        timingQTE.requiredHits = 3;

        timingQTE.successZone.sizeDelta = new Vector2(100f, 50f);
        timingQTE.successZone.anchoredPosition = Vector2.zero;

        timingQTE.marker.anchoredPosition = Vector2.zero;
    }

    [TearDown]
    public void TearDown()
    {
        UnityEngine.Object.DestroyImmediate(markerObject);
        UnityEngine.Object.DestroyImmediate(successZoneObject);
        UnityEngine.Object.DestroyImmediate(panelObject);
        UnityEngine.Object.DestroyImmediate(qteObject);
    }

    [Test]
    public void CheckSuccess_ReturnsTrue_WhenMarkerIsInsideZone()
    {
        timingQTE.successZone.anchoredPosition = new Vector2(0f, 0f);
        timingQTE.successZone.sizeDelta = new Vector2(100f, 50f);

        timingQTE.marker.anchoredPosition = new Vector2(0f, 0f);

        bool result = timingQTE.CheckSuccess();

        Assert.IsTrue(result);
    }

    [Test]
    public void CheckSuccess_ReturnsFalse_WhenMarkerIsOutsideZone()
    {
        timingQTE.successZone.anchoredPosition = new Vector2(0f, 0f);
        timingQTE.successZone.sizeDelta = new Vector2(100f, 50f);

        timingQTE.marker.anchoredPosition = new Vector2(200f, 0f);

        bool result = timingQTE.CheckSuccess();

        Assert.IsFalse(result);
    }

    [Test]
    public void CheckSuccess_ReturnsTrue_WhenMarkerIsExactlyOnLeftEdge()
    {
        timingQTE.successZone.anchoredPosition = new Vector2(0f, 0f);
        timingQTE.successZone.sizeDelta = new Vector2(100f, 50f);

        timingQTE.marker.anchoredPosition = new Vector2(-50f, 0f);

        bool result = timingQTE.CheckSuccess();

        Assert.IsTrue(result);
    }

    [Test]
    public void CheckSuccess_ReturnsTrue_WhenMarkerIsExactlyOnRightEdge()
    {
        timingQTE.successZone.anchoredPosition = new Vector2(0f, 0f);
        timingQTE.successZone.sizeDelta = new Vector2(100f, 50f);

        timingQTE.marker.anchoredPosition = new Vector2(50f, 0f);

        bool result = timingQTE.CheckSuccess();

        Assert.IsTrue(result);
    }

    [Test]
    public void CheckSuccess_ReturnsFalse_WhenMarkerIsSlightlyOutsideLeftEdge()
    {
        timingQTE.successZone.anchoredPosition = new Vector2(0f, 0f);
        timingQTE.successZone.sizeDelta = new Vector2(100f, 50f);

        timingQTE.marker.anchoredPosition = new Vector2(-50.1f, 0f);

        bool result = timingQTE.CheckSuccess();

        Assert.IsFalse(result);
    }

    [Test]
    public void CheckSuccess_ReturnsFalse_WhenMarkerIsSlightlyOutsideRightEdge()
    {
        timingQTE.successZone.anchoredPosition = new Vector2(0f, 0f);
        timingQTE.successZone.sizeDelta = new Vector2(100f, 50f);

        timingQTE.marker.anchoredPosition = new Vector2(50.1f, 0f);

        bool result = timingQTE.CheckSuccess();

        Assert.IsFalse(result);
    }

    [Test]
    public void CheckSuccess_WorksCorrectly_WithDifferentZonePositions()
    {
        timingQTE.successZone.anchoredPosition = new Vector2(150f, 0f);
        timingQTE.successZone.sizeDelta = new Vector2(120f, 50f);

        timingQTE.marker.anchoredPosition = new Vector2(160f, 0f);

        bool result = timingQTE.CheckSuccess();

        Assert.IsTrue(result);
    }


    [Test]
    public void FinishQTE_Sets_IsFinished_To_True()
    {
        timingQTE.FinishQTE();

        Assert.IsTrue(timingQTE.IsFinished);
    }

    [Test]
    public void FinishQTE_Disables_QTEPanel()
    {
        timingQTE.qtePanel.SetActive(true);

        timingQTE.FinishQTE();

        Assert.IsFalse(timingQTE.qtePanel.activeSelf);
    }

    [Test]
    public void FinishQTE_LeavesPanelDisabled_WhenAlreadyDisabled()
    {
        timingQTE.qtePanel.SetActive(false);

        timingQTE.FinishQTE();

        Assert.IsFalse(timingQTE.qtePanel.activeSelf);
    }

    [Test]
    public void FinishQTE_Invokes_Callback_WhenAssigned()
    {
        bool callbackInvoked = false;

        timingQTE.qteFinishedCallback = () =>
        {
            callbackInvoked = true;
        };

        timingQTE.FinishQTE();

        Assert.IsTrue(callbackInvoked);
    }

    [Test]
    public void FinishQTE_DoesNotThrow_WhenCallbackIsNull()
    {
        Assert.DoesNotThrow(() =>
        {
            timingQTE.FinishQTE();
        });
    }


    [Test]
    public void StartQTE_ActivatesPanel()
    {
        timingQTE.qtePanel.SetActive(false);

        timingQTE.StartQTE();

        Assert.IsTrue(timingQTE.qtePanel.activeSelf);
    }

    [Test]
    public void StartQTE_ResetsFinishedState()
    {
        timingQTE.FinishQTE();

        timingQTE.StartQTE();

        Assert.IsFalse(timingQTE.IsFinished);
    }

    [Test]
    public void StartQTE_ResetsWasSuccessfulState()
    {
        timingQTE.StartQTE();

        Assert.IsFalse(timingQTE.WasSuccessful);
    }

    [Test]
    public void StartQTE_ResetsMarkerPosition()
    {
        timingQTE.marker.anchoredPosition =
            new Vector2(200f, 0f);

        timingQTE.StartQTE();

        Assert.Less(
            timingQTE.marker.anchoredPosition.x,
            200f
        );
    }

    [Test]
    public void StartQTE_MakesCursorVisible()
    {
        timingQTE.StartQTE();

        Assert.IsTrue(Cursor.visible);
    }


    [Test]
    public void RandomizeSuccessZone_KeepsZoneInsideAllowedRange()
    {
        for (int i = 0; i < 100; i++)
        {
            timingQTE.RandomizeSuccessZone();

            float xPosition = timingQTE.successZone.anchoredPosition.x;

            Assert.GreaterOrEqual(xPosition, -250f);
            Assert.LessOrEqual(xPosition, 250f);
        }
    }


    [Test]
    public void InitialState_IsFinished_ShouldBeFalse()
    {
        Assert.IsFalse(timingQTE.IsFinished);
    }

    [Test]
    public void InitialState_WasSuccessful_ShouldBeFalse()
    {
        Assert.IsFalse(timingQTE.WasSuccessful);
    }
}

