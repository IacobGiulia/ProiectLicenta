using System.Collections;
using UnityEngine;

public enum InteractionType
{
    BicepCurl,
    TreadmillRun,
    FrontRaises,
    BenchPress,
    BarbellSquat,
    PushUp,
    SitUp
}

public class Interactable : MonoBehaviour
{
    [Header("Equipment Lock (NPC Queue)")]
    [Tooltip("Equipment component attached. NPCs will queue up when the player is using it.")]
    public Equipment equipment;

    [Header("Rest System")]

    public float restDuration = 20f;

    public GameObject canvasUI;
    public GameObject playerObject;
    private Animator playerAnimator;
    public InteractionType interactionType;
    public PlayerController playerController;

    [Header("Bench Press Settings")]
    public Transform benchTarget;           
    public PlayerBarbellBench playerBarbellScript;
    public GameObject benchBarbell;

    [Header("Barbell Squat Settings")]
    public PlayerBarbellSquat playerSquatBarbellScript;
    public GameObject squatBarbell;
    public Transform squatTarget;

    [Header("Push Up Settings")]
    public Transform pushUpTarget;

    [Header("Sit Up Settings")]
    public Transform sitUpTarget;

    public PlayerStats playerStats;
    public RestMessageUI restMessageUI;

    private bool isOnTreadmill = false;

    [Header("Bar QTE")]
    private TimingQTE activeQTE;

    [Header("Key Press QTE")]
    public KeyPressQTE keyPressQTE;

    public TreadmillPaceSystem treadmillSystem;

    private void Start()
    {
        if (canvasUI != null)
            canvasUI.SetActive(false);

        if (playerObject != null)
            playerAnimator = playerObject.GetComponentInChildren<Animator>();
    }

    public void ShowUI(bool show)
    {
        if (canvasUI != null)
            canvasUI.SetActive(show);
    }

    void LockForPlayer()
    {
        if (equipment != null)
            equipment.PlayerAcquire();
    }

    bool CanPerform(float cost)
    {
        if (playerStats == null)
            return false;

        if (playerStats.energy < cost)
        {
            if (restMessageUI != null)
                restMessageUI.ShowMessage("Not enough energy!");
            return false;
        }

        return true;
    }

    void UnlockForPlayer()
    {
        if (equipment != null)
            equipment.PlayerRelease();
    }

    void FreezePlayerForExercise()
    {
        if(playerController != null)
        {
            playerController.ResetMovement();
            playerController.canMove = false;
        }
    }
    IEnumerator ReleaseAfterAnimation(string stateName)
    {
        yield return null;
        yield return null;

        while (!playerAnimator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
            yield return null;

        while (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
            yield return null;
        playerAnimator.applyRootMotion = true;
        if (playerController != null)
        { 
            playerController.ResetMovement();
            playerController.canMove = true;
        }    

        UnlockForPlayer();
    }

    public void Interact()
    {
        if (playerAnimator == null) return;

        if (playerController != null && !playerController.canMove)
            return;

        if (equipment != null && equipment.Owner != null)
        {
            if (restMessageUI != null)
                restMessageUI.ShowMessage("Someone is already using this!");
            return;
        }

        if (playerStats != null && playerStats.isResting)
        {
            Debug.Log("Player is currently resting");
            if(restMessageUI != null)
                restMessageUI.ShowMessage(playerStats.restTimer);
            return;
        }

        switch (interactionType)
        {
            case InteractionType.BicepCurl:
                StartCoroutine(DoKeyQTEAndApplyStats("DoBicepCurl","BicepCurl", 5f, 0.5f, 0f));
                break;

            case InteractionType.TreadmillRun:
                if (!isOnTreadmill)
                    StartTreadmill();
                else
                    StopTreadmill();
                break;

            case InteractionType.FrontRaises:
                StartCoroutine(DoKeyQTEAndApplyStats("DoFrontRaises", "FrontRaises", 5f, 0.5f, 0f));
                break;

            case InteractionType.BenchPress:
                BenchPress();
                break;

            case InteractionType.BarbellSquat:
                BarbellSquat();
                break;

            case InteractionType.PushUp:
                PushUp();
                break;

            case InteractionType.SitUp:
                SitUp();
                break;
        }
    }

    private void BenchPress()
    {
        float energyCost = 10f;

        if (!CanPerform(energyCost))
            return;

        LockForPlayer();

        FreezePlayerForExercise();

        playerAnimator.applyRootMotion = true;
 

        if (benchBarbell != null)
        {
            benchBarbell.SetActive(false);
            Debug.Log("Bench barbell hidden");
        }

        if (playerBarbellScript != null)
        {
            
            playerBarbellScript.ShowBarbell(benchBarbell);
        }

        BoxCollider benchCollider = null;
        if (benchTarget != null)
        {
            benchCollider = benchTarget.GetComponent<BoxCollider>();
            if (benchCollider != null)
                benchCollider.enabled = false; 

            CharacterController cc = playerObject.GetComponent<CharacterController>();
            if (cc != null)
                cc.enabled = false;

            playerObject.transform.position = benchTarget.position;
            playerObject.transform.rotation = benchTarget.rotation;

            if (cc != null)
                cc.enabled = true;
        }


        playerAnimator.SetTrigger("DoBenchPress");
        Debug.Log(playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Bench Press"));
        playerAnimator.SetBool("QTEFinished", true);
        activeQTE = FindObjectOfType<TimingQTE>();
        if (activeQTE != null)
            StartCoroutine(WaitForQTE(activeQTE, isSquat: false, animStateName:"Bench Press"));

        if (benchCollider != null)
            benchCollider.enabled = true;
    }

    private void BarbellSquat()
    {
        float energyCost = 10f;

        if (!CanPerform(energyCost))
            return;


        LockForPlayer();

        FreezePlayerForExercise();

        playerAnimator.applyRootMotion = true;

        if (squatBarbell != null)
            squatBarbell.SetActive(false);

        if (playerSquatBarbellScript != null)
            playerSquatBarbellScript.ShowBarbell(squatBarbell);

        BoxCollider squatCollider = null;

        if (squatTarget != null)
        {
            CharacterController cc = playerObject.GetComponent<CharacterController>();
            if (cc != null)
                cc.enabled = false;

            playerObject.transform.position = squatTarget.position;
            playerObject.transform.rotation = squatTarget.rotation;

            if (cc != null)
                cc.enabled = true;
        }

        playerAnimator.SetTrigger("DoBarbellSquat");
        
        playerAnimator.SetBool("QTEFinished", true);

        activeQTE = FindObjectOfType<TimingQTE>();
        if (activeQTE != null)
            StartCoroutine(WaitForQTE(activeQTE, isSquat: true, animStateName: "Barbell Squat"));

        if (squatCollider != null)
            squatCollider.enabled = true;

    }

    private void PushUp()
    {
        LockForPlayer();

        FreezePlayerForExercise();

        playerAnimator.applyRootMotion = true;

        if (pushUpTarget != null)
        {
            CharacterController cc = playerObject.GetComponent<CharacterController>();
            if (cc != null)
                cc.enabled = false;

            playerObject.transform.position = pushUpTarget.position;
            playerObject.transform.rotation = pushUpTarget.rotation;

            if (cc != null)
                cc.enabled = true;
        }

        playerAnimator.SetTrigger("DoPushUp");
        StartCoroutine(StartPushUpAndSitUpQTE("Push Up"));

    }

    private void SitUp()
    {
        LockForPlayer();

        FreezePlayerForExercise();

        playerAnimator.applyRootMotion = true;

        if (sitUpTarget != null)
        {
            CharacterController cc = playerObject.GetComponent<CharacterController>();
            if (cc != null)
                cc.enabled = false;

            playerObject.transform.position = sitUpTarget.position;
            playerObject.transform.rotation = sitUpTarget.rotation;

            if (cc != null)
                cc.enabled = true;
        }

        playerAnimator.SetTrigger("DoSitUp");
        StartCoroutine(StartPushUpAndSitUpQTE("Sit Up"));

    }


    private void StartTreadmill()
    {
        float energyCost = 10f;

        if (!CanPerform(energyCost))
            return;


        LockForPlayer();
        FreezePlayerForExercise();
        isOnTreadmill = true;

        playerAnimator.applyRootMotion = false;
        playerAnimator.SetTrigger("RunOnTreadmill");

        if (treadmillSystem != null)
        {
            treadmillSystem.onRunFinished = () => StopTreadmill();
            treadmillSystem.StartRun();
        }
            

    }

    private void StopTreadmill()
    {
        isOnTreadmill = false;

        playerAnimator.ResetTrigger("RunOnTreadmill");

        if (treadmillSystem != null)
            treadmillSystem.StopRun();
        StartCoroutine(ReleaseAfterAnimation("TreadmillRun"));

    }

    private IEnumerator WaitForQTE(TimingQTE qte, bool isSquat, string animStateName)
    {


        if (playerAnimator != null)
        {
            playerAnimator.SetBool("QTEFinished", true);
        }

        bool qteDone = false;
        qte.qteFinishedCallback = () => qteDone = true;
        
        qte.StartQTE();

        while (!qteDone)
            yield return null;

        if (playerAnimator != null)
            playerAnimator.SetBool("QTEFinished", false); 

        if (playerStats != null)
        {
            float energyCost = isSquat ? 10f : 10f;
            float strengthGain = 0f;

            if (qte.WasSuccessful)
                strengthGain = isSquat ? 1f : 1f;

            bool wasCorrect = qte.WasSuccessful;
            playerStats.WorkOut(energyCost, 0f, strengthGain, 1f, wasCorrect);
        }

        if (playerStats != null)
            playerStats.StartRest();

        StartCoroutine(ReleaseAfterAnimation(animStateName));

    }

    private IEnumerator DoKeyQTEAndApplyStats(string animationTrigger, string animStateName, float energyCost, float strengthGain, float staminaGain)
    {
        LockForPlayer();
        FreezePlayerForExercise();

        playerAnimator.SetTrigger(animationTrigger);

        bool qteDone = false;
        keyPressQTE.qteFinishedCallback = () => qteDone = true;

        keyPressQTE.StartQTE();

        while (!qteDone)
            yield return null;

        bool wasCorrect = keyPressQTE.WasPerfect;

        if (wasCorrect)
        {
            playerStats.WorkOut(energyCost, staminaGain, strengthGain, 1f, true);
            Debug.Log("QTE PERFECT — stats increased!");
        }
        else
        {
            playerStats.WorkOut(energyCost, 0f, 0f, 0f, false);
            Debug.Log("QTE FAILED — only energy lost.");
        }


        if (playerStats != null)
            playerStats.StartRest();

        StartCoroutine(ReleaseAfterAnimation(animStateName));
    }

    private IEnumerator StartPushUpAndSitUpQTE(string animStateName)
    {
        MashQTE mashQTE = FindObjectOfType<MashQTE>();
        if (mashQTE == null)
        {
            if (playerController != null) playerController.canMove = true;
            UnlockForPlayer();
            yield break;
        }

        mashQTE.StartQTE();

        bool qteDone = false;
        mashQTE.mashQTESuccessCallback = (success) => qteDone = true;

        while (qteDone == false)
            yield return null;

        if (mashQTE.WasSuccessful)
        {
            Debug.Log("Push Up QTE SUCCESS!");
            playerStats.WorkOut(5f, 0.5f, 0f, 0.5f, true); 
        }
        else
        {
            Debug.Log("Push Up QTE FAILED!");
            playerStats.WorkOut(5f, 0f, 0f, 0f, false); 
        }

        if (playerStats != null)
            playerStats.StartRest();

        StartCoroutine(ReleaseAfterAnimation(animStateName));
    }

}
