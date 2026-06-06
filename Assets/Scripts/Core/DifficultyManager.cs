using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance;

    [Range(0f, 1f)]
    public float difficulty = 0.5f;

    private int totalGames = 0;
    private int totalSuccesses = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void RegisterMinigameResult(bool success)
    {
        totalGames++;

        if (success)
            totalSuccesses++;

        float successRate = (float)totalSuccesses / totalGames;

        difficulty = Mathf.Lerp(0.2f, 1f, successRate);

        Debug.Log($"Difficulty updated: {difficulty}");
    }
}
