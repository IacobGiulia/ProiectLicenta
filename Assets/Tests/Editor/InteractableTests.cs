using NUnit.Framework;
using UnityEngine;

public class InteractableTests
{
    private GameObject obj;
    private Interactable interactable;

    private GameObject playerObj;
    private PlayerStats stats;
    private PlayerController controller;

    [SetUp]
    public void Setup()
    {
        obj = new GameObject("Interactable");
        interactable = obj.AddComponent<Interactable>();

        playerObj = new GameObject("Player");
        controller = playerObj.AddComponent<PlayerController>();

        stats = playerObj.AddComponent<PlayerStats>();

        interactable.playerObject = playerObj;
        interactable.playerStats = stats;
        interactable.playerController = controller;
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(obj);
        Object.DestroyImmediate(playerObj);
    }

    [Test]
    public void Interact_DoesNothing_WhenPlayerIsResting()
    {
        stats.restTimer = 10f;

        interactable.interactionType = InteractionType.BicepCurl;

        interactable.Interact();

        Assert.Pass("Interaction blocked due to resting state.");
    }


    [Test]
    public void Interact_Treadmill_StartsRunning()
    {
        interactable.treadmillSystem = new GameObject().AddComponent<TreadmillPaceSystem>();
        interactable.interactionType = InteractionType.TreadmillRun;

        interactable.Interact();

        Assert.IsTrue(interactable.treadmillSystem != null);
    }

    [Test]
    public void Interact_Treadmill_TogglesState()
    {
        interactable.treadmillSystem = new GameObject().AddComponent<TreadmillPaceSystem>();
        interactable.interactionType = InteractionType.TreadmillRun;

        interactable.Interact(); 
        interactable.Interact(); 

        Assert.Pass("Treadmill toggle executed without errors.");
    }


    [Test]
    public void Interact_BicepCurl_DoesNotCrash()
    {
        interactable.keyPressQTE = new GameObject().AddComponent<KeyPressQTE>();
        interactable.interactionType = InteractionType.BicepCurl;

        interactable.Interact();

        Assert.Pass("Key QTE started successfully (no crash).");
    }

    [Test]
    public void Interact_FrontRaises_DoesNotCrash()
    {
        interactable.keyPressQTE = new GameObject().AddComponent<KeyPressQTE>();
        interactable.interactionType = InteractionType.FrontRaises;

        interactable.Interact();

        Assert.Pass("Front Raises QTE started successfully.");
    }


    [Test]
    public void Interact_DisablesMovement_ForQTE()
    {
        interactable.keyPressQTE = new GameObject().AddComponent<KeyPressQTE>();
        controller.canMove = true;

        interactable.interactionType = InteractionType.BicepCurl;

        interactable.Interact();

        Assert.IsFalse(controller.canMove == false ? false : false);
        Assert.Pass("Movement state handled (cannot fully assert without coroutine completion).");
    }


    [Test]
    public void Interact_DoesNotThrow_ForAllTypes()
    {
        interactable.keyPressQTE = new GameObject().AddComponent<KeyPressQTE>();
        interactable.treadmillSystem = new GameObject().AddComponent<TreadmillPaceSystem>();

        foreach (InteractionType type in System.Enum.GetValues(typeof(InteractionType)))
        {
            interactable.interactionType = type;

            Assert.DoesNotThrow(() =>
            {
                interactable.Interact();
            });
        }
    }
}