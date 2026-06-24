using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UIElements;

public class PlayerStats : MonoBehaviour
{
    [Header("Stats")]
    public float energy = 100f;
    public float stamina = 0f;
    public float strength = 0f;

    [Header("Rest Settings")]
    public float restTimer = 0f;
    public float restDuration = 20f;

    public bool isResting => restTimer > 0f;

    [Header("UI Panels")]
    public GameObject energyPanel;
    public GameObject staminaPanel;
    public GameObject strengthPanel;
    public GameObject dayPanel;

    [Header("References")]
    public UIStatsBar energyBar;
    public UIStatsBar staminaBar;
    public UIStatsBar strengthBar;

    [Header("Day System")]
    public int day = 1;
    public TextMeshProUGUI dayOverText;
    public TextMeshProUGUI dayText;
    public GameObject dayOverPanel;

    [Header("Daily Counters")]
    public int totalExercises = 0;
    public int correctExercises = 0;

    [Header("Progress System")]
    public float progress = 0f;
    public float maxProgress = 100f;

    [Header("Day Summary UI")]
    public CanvasGroup screenFade;
    public GameObject daySummaryPanel;
    public TMPro.TextMeshProUGUI daySummaryText;

    public UnityEngine.UI.Slider staminaSlider;
    public TMPro.TextMeshProUGUI staminaText;

    public UnityEngine.UI.Slider strengthSlider;
    public TMPro.TextMeshProUGUI strengthSliderText;

    public UnityEngine.UI.Slider progressSlider;
    public TMPro.TextMeshProUGUI progressTextSummary;

    public TextMeshProUGUI totalExercisesText;
    public TextMeshProUGUI correctExercisesText;
    public TextMeshProUGUI accuracyText;
    public TextMeshProUGUI ratingText;
    public TextMeshProUGUI focusMessageText;

    [Header("End Game UI")]
    public GameObject endGamePanel;
    public TextMeshProUGUI endTitleText;
    public TextMeshProUGUI endDaysText;
    public TextMeshProUGUI endRankText;

    private bool dayEnded = false;
    private bool gameFinished = false;
    private bool finalDayReached = false;

    private UnityEngine.UI.Button continueButton;
    private bool continuePressed = false;

    public bool IsBlockingPanelActive =>
        (daySummaryPanel != null && daySummaryPanel.activeSelf) ||
        (endGamePanel != null && endGamePanel.activeSelf) ||
        (dayOverPanel != null && dayOverPanel.activeSelf);

    void Start()
    {
        if (PlayerPrefs.GetInt("LoadGame", 0) == 1)
        {
            LoadGame();
            PlayerPrefs.DeleteKey("LoadGame");
        }

        UpdateDayText();
        if (dayOverPanel != null)
            dayOverPanel.SetActive(false);

        if (endGamePanel != null)
            endGamePanel.SetActive(false);

        if (daySummaryPanel != null)
        {
            continueButton = daySummaryPanel.GetComponentInChildren<UnityEngine.UI.Button>();
            if (continueButton != null)
                continueButton.onClick.AddListener(OnContinuePressed);
        }
    }

    void OnContinuePressed()
    {
        continuePressed = true;
    }

    void Update()
    {
        UpdateUI();
        CheckEnergy();
        UpdateRestTimer();

        if (Input.GetKeyDown(KeyCode.K)) SaveGame();
        if (Input.GetKeyDown(KeyCode.L)) LoadGame();
    }

    void UpdateUI()
    {
        if (energyBar != null) energyBar.SetValue(energy);
        if (staminaBar != null) staminaBar.SetValue(stamina);
        if (strengthBar != null) strengthBar.SetValue(strength);
    }

    void CheckEnergy()
    {
        if (energy <= 0 && !dayEnded)
        {
            dayEnded = true;
            StartCoroutine(EndDayRoutine());
        }
    }

    void UpdateRestTimer()
    {
        if (restTimer > 0f)
        {
            restTimer -= Time.deltaTime;
            if (restTimer < 0f)
                restTimer = 0f;
        }
    }

    public void StartRest(float duration = -1f)
    {
        if (duration <= 0f)
            duration = restDuration;
        restTimer = duration;
    }

    IEnumerator EndDayRoutine()
    {
        float fadeDuration = 3.5f;
        float elapsed = 0f;
        screenFade.alpha = 0f;
        screenFade.gameObject.SetActive(true);

        if (energyPanel != null) energyPanel.SetActive(false);
        if (staminaPanel != null) staminaPanel.SetActive(false);
        if (strengthPanel != null) strengthPanel.SetActive(false);
        if (dayPanel != null) dayPanel.SetActive(false);

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            screenFade.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }

        screenFade.alpha = 1f;

        float staminaPercent = stamina;
        float strengthPercent = strength;
        float progressPercent = (staminaPercent + strengthPercent) / 2f;


        float accuracy = 0f;
        if (totalExercises > 0)
            accuracy = (correctExercises * 100f) / totalExercises;

        string focusMessage;
        if (stamina > strength)
            focusMessage = "Cardio took the spotlight today. Maybe tomorrow you will lift some weights too!";
        else if (strength > stamina)
            focusMessage = "You really destroyed your muscles today. Try to hit the treadmill more tho!";
        else
            focusMessage = "Perfectly balanced workout. You've got true athlete energy!";

        string rating;
        if (accuracy < 30) rating = "Poor day";
        else if (accuracy < 60) rating = "Good day";
        else if (accuracy < 85) rating = "Great day";
        else rating = "Perfect day";

        daySummaryPanel.SetActive(true);
        daySummaryText.text = $"Day {day} Complete!";

        float animationTime = 1.5f;
        elapsed = 0f;

        while (elapsed < animationTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animationTime);

            staminaSlider.value = Mathf.Lerp(0f, staminaPercent, t);
            staminaText.text = $"{staminaSlider.value:0.#}%";

            strengthSlider.value = Mathf.Lerp(0f, strengthPercent, t);
            strengthSliderText.text = $"{strengthSlider.value:0.#}%";

            progressSlider.value = Mathf.Lerp(0f, progressPercent, t);
            progressTextSummary.text = $"{progressPercent:0.#}%";

            yield return null;
        }

        float finalProgress = (stamina + strength) / 2f;

        staminaSlider.value = staminaPercent;
        staminaText.text = $"{staminaPercent:0.#}%";

        strengthSlider.value = strengthPercent;
        strengthSliderText.text = $"{strengthPercent:0.#}%";

        progressSlider.value = finalProgress;
        progressTextSummary.text = $"{finalProgress:0.#}%";

        totalExercisesText.text = totalExercises.ToString();
        correctExercisesText.text = correctExercises.ToString();
        accuracyText.text = $"{accuracy:0.#}%";

        focusMessageText.text = focusMessage;
        ratingText.text = rating;

        continuePressed = false;
        while (!continuePressed)
            yield return null;

        elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            screenFade.alpha = 1f - Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }

        screenFade.alpha = 0f;
        screenFade.gameObject.SetActive(false);
        daySummaryPanel.SetActive(false);

        if (finalDayReached)
        {
            FinishGame();
            yield break;
        }

        StartNewDay();
    }

    public void StartNewDay()
    {
        day++;
        energy = 100f;
        dayEnded = false;
        totalExercises = 0;
        correctExercises = 0;
        restTimer = 0f;

        if (energyPanel != null) energyPanel.SetActive(true);
        if (staminaPanel != null) staminaPanel.SetActive(true);
        if (strengthPanel != null) strengthPanel.SetActive(true);
        if (dayPanel != null) dayPanel.SetActive(true);

        UpdateDayText();
    }

    void UpdateDayText()
    {
        if (dayText != null)
            dayText.text = "Day: " + day;
    }

    public void WorkOut(float energyCost, float staminaGain, float strengthGain, float progressGain, bool wasCorrect)
    {
        totalExercises++;
        if (wasCorrect) correctExercises++;

        energy -= energyCost;
        energy = Mathf.Clamp(energy, 0, 100);

        stamina += staminaGain;
        stamina = Mathf.Clamp(stamina, 0, 100);

        strength += strengthGain;
        strength = Mathf.Clamp(strength, 0, 100);

        AddProgress(progressGain);

        if (stamina >= 100 && strength >= 100)
            finalDayReached = true;

        if (energyBar != null) energyBar.SetValue(energy);
        if (staminaBar != null) staminaBar.SetValue(stamina);
        if (strengthBar != null) strengthBar.SetValue(strength);

    }

    public void AddProgress(float amount)
    {
        progress += amount;
        progress = Mathf.Clamp(progress, 0f, maxProgress);
    }

    public string GetRank()
    {
        if (day <= 25) return "Legendary Athlete";
        if (day <= 40) return "Elite Athlete";
        if (day <= 60) return "Professional";
        if (day <= 85) return "Dedicated Trainee";
        return "Amateur";
    }

    void FinishGame()
    {
        gameFinished = true;
        if (endGamePanel != null)
            endGamePanel.SetActive(true);

        endTitleText.text = "Congratulations, You Won!";
        endDaysText.text = $"Days Needed: {day}";
        endRankText.text = $"Rank: {GetRank()}";

        Time.timeScale = 0f;
    }

    public void SaveGame()
    {
        SaveSystem.SaveGame(this);
    }

    public void LoadGame()
    {
        SaveData data = SaveSystem.LoadGame();
        if (data != null)
        {
            day = data.day;
            energy = data.energy;
            stamina = data.stamina;
            strength = data.strength;
            progress = data.progress;
            totalExercises = data.totalExercises;
            correctExercises = data.correctExercises;

            UpdateUI();
            UpdateDayText();
        }
    }
}