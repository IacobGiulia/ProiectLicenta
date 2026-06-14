using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MashQTE : MonoBehaviour
{
    [Header("Base Settings")]
    public float baseTimeLimit = 7f;
    public float minTimeLimit = 7f;

    public float baseProgressPerTap = 0.04f;
    public float minProgressPerTap = 0.03f;

    public float baseDecayRate = 0.02f;
    public float maxDecayRate = 0.05f;

    [Header("Audio")]
    public AudioSource qteAudioSource;
    public AudioClip successSound;
    public AudioClip failSound;

    [Header("UI References")]
    public Slider progressBar;
    public TextMeshProUGUI timerText;
    public GameObject mashPanel;

    private float timeLeft;
    private float timeLimit;

    private float progress;
    private float progressPerTap;
    private float decayRate;

    private bool active;

    public delegate void QTESuccessCallback(bool success);
    public QTESuccessCallback mashQTESuccessCallback;

    [HideInInspector]
    public bool WasSuccessful = false;

    public void StartQTE()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Debug.Log("TIME LIMIT = " + timeLimit);
        ApplyDifficulty();  

        if (mashPanel != null)
            mashPanel.SetActive(true);

        active = true;
        timeLeft = timeLimit;
        progress = 0f;

        progressBar.value = 0f;
    }

    public void ApplyDifficulty()
    {
        if (DifficultyManager.Instance == null)
            return;

        float d = DifficultyManager.Instance.difficulty;

        timeLimit = Mathf.Lerp(baseTimeLimit, minTimeLimit, d);

        progressPerTap = Mathf.Lerp(baseProgressPerTap, minProgressPerTap, d);

        decayRate = Mathf.Lerp(baseDecayRate, maxDecayRate, d);

        Debug.Log($"[MashQTE] Difficulty applied: {d}");
    }

    void Update()
    {
        if (!active) return;

        timeLeft -= Time.deltaTime;
        timerText.text = Mathf.Ceil(timeLeft).ToString();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            progress += progressPerTap;
        }

        progress -= decayRate * Time.deltaTime;
        progress = Mathf.Clamp01(progress);

        progressBar.value = progress;

        if (progress >= 1f)
        {
            Success();
        }

        if (timeLeft <= 0f)
        {
            Fail();
        }
    }

    public void Success()
    {
        active = false;
        WasSuccessful = true;

        if (qteAudioSource != null && successSound != null)
            qteAudioSource.PlayOneShot(successSound);

        Debug.Log("Mash QTE Success!");

        if (mashPanel != null)
            mashPanel.SetActive(false);

        if (DifficultyManager.Instance != null)
            DifficultyManager.Instance.RegisterMinigameResult(true);

        mashQTESuccessCallback?.Invoke(true);
    }

    public void Fail()
    {
        active = false;
        WasSuccessful = false;

        if (qteAudioSource != null && failSound != null)
            qteAudioSource.PlayOneShot(failSound);

        Debug.Log("Mash QTE Failed");

        if (mashPanel != null)
            mashPanel.SetActive(false);

        if (DifficultyManager.Instance != null)
            DifficultyManager.Instance.RegisterMinigameResult(false);

        mashQTESuccessCallback?.Invoke(false);
    }
}