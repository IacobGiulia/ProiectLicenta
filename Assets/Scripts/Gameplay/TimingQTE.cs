using System;
using System.Collections;
using UnityEngine;

public class TimingQTE : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform marker;
    public RectTransform successZone;
    public GameObject qtePanel;

    [Header("Base Settings")]
    public float baseMoveSpeed = 300f;
    public float maxMoveSpeed = 700f;

    public float baseZoneWidth = 220f;
    public float minZoneWidth = 80f;

    public int requiredHits = 3;

    private bool running = false;
    private float direction = 1f;

    private int currentHits = 0;
    private int successfulHits = 0;

    public bool IsFinished { get; private set; }
    public bool WasSuccessful { get; private set; }

    public Action qteFinishedCallback;

    [ContextMenu("Start QTE")]
    public void StartQTE()
    {
        Debug.Log("StartQTE called!");

        IsFinished = false;
        WasSuccessful = false;

        running = true;
        direction = 1f;

        currentHits = 0;
        successfulHits = 0;

        marker.anchoredPosition = new Vector2(-300, 0);

        ApplyDifficulty();        
        RandomizeSuccessZone();

        qtePanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        StartCoroutine(MoveMarker());
    }

    public void ApplyDifficulty()
    {
        if (DifficultyManager.Instance == null)
            return;


        float d = DifficultyManager.Instance.difficulty;

        float moveSpeed = Mathf.Lerp(baseMoveSpeed, maxMoveSpeed, d);
        float zoneWidth = Mathf.Lerp(baseZoneWidth, minZoneWidth, d);

        this.baseMoveSpeed = moveSpeed;

        successZone.sizeDelta = new Vector2(zoneWidth, successZone.sizeDelta.y);

        Debug.Log($"[QTE] Difficulty applied: {d}");
    }

    IEnumerator MoveMarker()
    {
        while (running)
        {
            marker.anchoredPosition +=
                new Vector2(baseMoveSpeed * Time.deltaTime * direction, 0);

            if (marker.anchoredPosition.x > 300)
                direction = -1;

            if (marker.anchoredPosition.x < -300)
                direction = 1;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                bool hit = CheckSuccess();

                currentHits++;

                if (hit)
                {
                    successfulHits++;
                    Debug.Log($"GOOD HIT ({successfulHits}/{requiredHits})");
                }
                else
                {
                    Debug.Log("MISS");
                }

                RandomizeSuccessZone();

                if (currentHits >= requiredHits)
                {
                    FinishQTE();
                    yield break;
                }
            }

            yield return null;
        }
    }

    public void RandomizeSuccessZone()
    {
        float minX = -250f;
        float maxX = 250f;

        float randomX = UnityEngine.Random.Range(minX, maxX);

        successZone.anchoredPosition =
            new Vector2(randomX, successZone.anchoredPosition.y);
    }

    public bool CheckSuccess()
    {
        float markerX = marker.anchoredPosition.x;

        float zoneMin =
            successZone.anchoredPosition.x - (successZone.sizeDelta.x / 2);

        float zoneMax =
            successZone.anchoredPosition.x + (successZone.sizeDelta.x / 2);

        return markerX >= zoneMin && markerX <= zoneMax;
    }

    public void FinishQTE()
    {
        running = false;
        IsFinished = true;

        WasSuccessful = (successfulHits == requiredHits);

        Debug.Log("QTE COMPLETED!");
        Debug.Log("SUCCESS: " + WasSuccessful);

        if (DifficultyManager.Instance != null)
            DifficultyManager.Instance.RegisterMinigameResult(WasSuccessful);

        qteFinishedCallback?.Invoke();

        qtePanel.SetActive(false);
    }
}