using UnityEngine;

public class TreadmillPaceSystem : MonoBehaviour
{
    [Header("Base Speed Settings")]
    public float currentSpeed = 0f;
    public float minSpeed = 0f;
    public float maxSpeed = 10f;

    [Header("Difficulty Scaling")]
    public float baseAccelOnPress = 2f;
    public float minAccelOnPress = 1.2f;

    public float baseDecay = 1.5f;
    public float maxDecay = 2f;

    [Header("Ideal Zone")]
    public float baseIdealMin = 4f;
    public float baseIdealMax = 7f;

    public float hardIdealMin = 4.8f;
    public float hardIdealMax = 6.2f;

    [Header("Run Settings")]
    public float runDuration = 20f;

    [Header("Stats")]
    public PlayerStats playerStats;
    public float energyCost = 10f;
    public float staminaReward = 1f;

    public AudioSource qteAudioSource;
    public AudioClip successSound;
    public AudioClip failSound;

    [Header("UI")]
    public TreadmillUI treadmillUI;

    private bool isRunning = false;
    private bool hasFinished = false;
    private bool runSuccess = false;

    public bool RunSuccess => runSuccess;
    public bool IsRunning => isRunning;
    public float Timer => timer;

    private float timer;

    private float accelOnPress;
    private float naturalDecay;

    private float idealMin;
    private float idealMax;

    public float IdealMin => idealMin;
    public float IdealMax => idealMax;

    public System.Action onRunFinished;

    public void StartRun()
    {
        Cursor.lockState = CursorLockMode.None; 
        Cursor.visible = true;
        ApplyDifficulty();  

        isRunning = true;
        hasFinished = false;
        runSuccess = false;

        currentSpeed = 3f;
        timer = runDuration;

        if (treadmillUI != null)
            treadmillUI.SetTreadmill(this);

        Debug.Log("[Treadmill] Difficulty applied");
    }

    public void ApplyDifficulty()
    {
        float d = 0f;

        if (DifficultyManager.Instance != null)
            d = DifficultyManager.Instance.difficulty;

        d = Mathf.SmoothStep(0f, 1f, d);
        d *= 0.7f;

        accelOnPress =
            Mathf.Lerp(baseAccelOnPress, baseAccelOnPress * 0.6f, d);

        naturalDecay =
            Mathf.Lerp(baseDecay, maxDecay*0.6f, d);

        idealMin =
            Mathf.Lerp(baseIdealMin, hardIdealMin, d);

        idealMax =
            Mathf.Lerp(baseIdealMax, hardIdealMax, d);
    }

    public void StopRun()
    {
        if (!isRunning) return;

        isRunning = false;

        if (!hasFinished)
        {
            runSuccess = false;
            ApplyFinalStats();
        }

        if (treadmillUI != null)
            treadmillUI.Hide();

        currentSpeed = 0f;
    }

    void Update()
    {
        if (!isRunning) return;

        HandleInput();
        UpdateTimer();
    }

    void HandleInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            currentSpeed += accelOnPress * Time.deltaTime;
        }

        currentSpeed -= naturalDecay * Time.deltaTime;

        currentSpeed =
            Mathf.Clamp(currentSpeed, minSpeed, maxSpeed);
    }

    void UpdateTimer()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f && !hasFinished)
        {
            FinishRun();
        }
    }

    public void FinishRun()
    {
        if (hasFinished) return;

        isRunning = false;
        hasFinished = true;

        bool inIdealZone =
            currentSpeed >= idealMin &&
            currentSpeed <= idealMax;

        runSuccess = inIdealZone;

        if (qteAudioSource != null)
        {
            if (runSuccess && successSound != null)
                qteAudioSource.PlayOneShot(successSound);
            else if (!runSuccess && failSound != null)
                qteAudioSource.PlayOneShot(failSound);
        }

        if (DifficultyManager.Instance != null)
            DifficultyManager.Instance.RegisterMinigameResult(runSuccess);

        ApplyFinalStats();

        if (treadmillUI != null)
            treadmillUI.Hide();

        onRunFinished?.Invoke();
    }

    void ApplyFinalStats()
    {
        if (playerStats == null) return;

        float staminaGain = runSuccess ? staminaReward : 0f;

        playerStats.WorkOut(
            energyCost,   
            staminaGain,  
            0f,           
            1f,          
            runSuccess    
        );

        playerStats.StartRest();
    }
}