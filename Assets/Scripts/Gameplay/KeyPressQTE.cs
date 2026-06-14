using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class KeyPressQTE : MonoBehaviour
{
    [Header("UI References")]
    public GameObject qtePanel;
    public TextMeshProUGUI keyText;
    public TextMeshProUGUI timerText;

    [Header("Base Settings")]
    public float baseTimePerKey = 2.5f;
    public float minTimePerKey = 1.0f;

    public int totalKeys = 8;

    [Header("Audio")]
    public AudioSource qteAudioSource;
    public AudioClip successSound;
    public AudioClip failSound;

    private readonly KeyCode[] possibleKeys =
    {
        KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F,
        KeyCode.Q, KeyCode.W, KeyCode.R, KeyCode.G,
        KeyCode.T, KeyCode.B, KeyCode.C, KeyCode.H,
        KeyCode.I, KeyCode.J, KeyCode.K, 
        KeyCode.M, KeyCode.N, KeyCode.O, KeyCode.P,
        KeyCode.U, KeyCode.V, KeyCode.X, KeyCode.Y, KeyCode.Z
    };

    public KeyCode currentKey;
    public int keysCompleted = 0;
    public float timer;

    public Action qteFinishedCallback;
    public bool WasPerfect { get;  set; }

    private float timePerKey;

    public void StartQTE()
    {
        keysCompleted = 0;
        WasPerfect = true;

        ApplyDifficulty();

        if (qtePanel != null)
            qtePanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        GenerateNewKey();
        ResetTimer();

        StartCoroutine(QTELoop());
    }

    public void ApplyDifficulty()
    {
        if (DifficultyManager.Instance == null)
        {
            timePerKey = baseTimePerKey;
            return;
        }

        float d = DifficultyManager.Instance.difficulty;

        timePerKey = Mathf.Lerp(baseTimePerKey, minTimePerKey, d);

        Debug.Log($"[KeyQTE] Difficulty applied: {d}, time/key: {timePerKey}");
    }

    IEnumerator QTELoop()
    {
        while (keysCompleted < totalKeys)
        {
            ResetTimer();
            bool answered = false;

            while (timer > 0f && !answered)
            {
                timer -= Time.deltaTime;
                timerText.text = timer.ToString("F1") + "s";

                foreach (KeyCode k in possibleKeys)
                {
                    if (Input.GetKeyDown(k))
                    {
                        answered = true;

                        if (k == currentKey)
                        {
                            if (qteAudioSource != null && successSound != null)
                                qteAudioSource.PlayOneShot(successSound);
                            Debug.Log($"CORRECT {keysCompleted + 1}/{totalKeys}");
                        }
                        else
                        {
                            if (qteAudioSource != null && failSound != null)
                                qteAudioSource.PlayOneShot(failSound);
                            Debug.Log($"WRONG: {k} vs {currentKey}");
                            WasPerfect = false;
                        }

                        keysCompleted++;

                        if (keysCompleted >= totalKeys)
                        {
                            FinishQTE();
                            yield break;
                        }

                        GenerateNewKey();
                        ResetTimer();
                        break;
                    }
                }

                yield return null;
            }

            if (!answered)
            {
                if (qteAudioSource != null && failSound != null)
                    qteAudioSource.PlayOneShot(failSound);
                Debug.Log($"TIME OUT: {currentKey}");
                WasPerfect = false;

                keysCompleted++;

                if (keysCompleted >= totalKeys)
                {
                    FinishQTE();
                    yield break;
                }

                GenerateNewKey();
                ResetTimer();
            }
        }
    }

    public void GenerateNewKey()
    {
        currentKey = possibleKeys[UnityEngine.Random.Range(0, possibleKeys.Length)];
        keyText.text = currentKey.ToString();
    }

    public void ResetTimer()
    {
        timer = timePerKey;
    }

    public void FinishQTE()
    {
        Debug.Log("Key QTE Finished!");

        if (qtePanel != null)
            qtePanel.SetActive(false);

        bool success = WasPerfect;

        if (DifficultyManager.Instance != null)
            DifficultyManager.Instance.RegisterMinigameResult(success);

        qteFinishedCallback?.Invoke();
    }
}