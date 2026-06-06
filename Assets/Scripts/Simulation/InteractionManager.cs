using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance;
    public List<InteractionPoint> points = new List<InteractionPoint>();

    void Awake()
    {
        Instance = this;
        Random.InitState((int)System.DateTime.Now.Ticks);
    }

    public InteractionPoint FindBestTarget(NPCBrain npc, InteractionPoint exclude = null)
    {
        if (points.Count == 0) return null;

        InteractionPoint best = null;
        float bestScore = float.MinValue;

        foreach (var p in points)
        {
            if (p == exclude) continue;

            float score = CalculateScore(npc, p);
            if (score > bestScore)
            {
                bestScore = score;
                best = p;
            }
        }
        npc.lastTarget = best;

        return best;
    }

    float CalculateScore(NPCBrain npc, InteractionPoint p)
    {
        float score = 0f;
        float personalityScore = 0f;
        switch (npc.personality)
        {
            case NPCBrain.NPCPersonality.Bodybuilder:
                switch (p.exerciseType)
                {
                    case InteractionPoint.ExerciseType.BenchPress: personalityScore += 10f; break;
                    case InteractionPoint.ExerciseType.Squat: personalityScore += 10f; break;
                    case InteractionPoint.ExerciseType.BicepCurls: personalityScore += 3f; break;
                    case InteractionPoint.ExerciseType.FrontRaises: personalityScore += 3f; break;
                    case InteractionPoint.ExerciseType.PushUps: personalityScore -= 2f; break;
                    case InteractionPoint.ExerciseType.Treadmill: personalityScore   -= 8f; break;
                    case InteractionPoint.ExerciseType.SitUps: personalityScore -= 2f; break;
                }
                break;

            case NPCBrain.NPCPersonality.CardioLover:
                switch (p.exerciseType)
                {
                    case InteractionPoint.ExerciseType.Treadmill: personalityScore += 10f; break;
                    case InteractionPoint.ExerciseType.SitUps: personalityScore += 4f; break;
                    case InteractionPoint.ExerciseType.PushUps: personalityScore += 3f; break;
                    case InteractionPoint.ExerciseType.BenchPress:  personalityScore -= 4f; break;
                    case InteractionPoint.ExerciseType.Squat: personalityScore -= 2f; break;
                    case InteractionPoint.ExerciseType.BicepCurls: personalityScore -= 3f; break;
                    case InteractionPoint.ExerciseType.FrontRaises: personalityScore -= 1f; break;
                }
                break;

            case NPCBrain.NPCPersonality.Calisthenics:
                switch (p.exerciseType)
                {
                    case InteractionPoint.ExerciseType.PushUps: personalityScore += 10f; break;
                    case InteractionPoint.ExerciseType.SitUps: personalityScore += 10f; break;
                    case InteractionPoint.ExerciseType.BenchPress: personalityScore += 2f; break;
                    case InteractionPoint.ExerciseType.Squat: personalityScore += 2f; break;
                    case InteractionPoint.ExerciseType.Treadmill: personalityScore -= 10f; break;
                    case InteractionPoint.ExerciseType.BicepCurls: personalityScore -= 1f; break;
                    case InteractionPoint.ExerciseType.FrontRaises: personalityScore -= 1f; break;
                }
                break;

            case NPCBrain.NPCPersonality.Lazy:
                personalityScore += 0f;

                if (p.equipment != null && !p.IsOccupied)
                    personalityScore += 6f;

                if (p.equipment != null)
                    personalityScore -= p.equipment.QueueCount * 4f;
                break;

            case NPCBrain.NPCPersonality.Balanced:

                personalityScore += 2f; 

                personalityScore += p.exerciseType switch
                {
                    InteractionPoint.ExerciseType.BenchPress => 3f,
                    InteractionPoint.ExerciseType.Squat => 3f,
                    InteractionPoint.ExerciseType.BicepCurls => 2f,
                    InteractionPoint.ExerciseType.FrontRaises => 2f,
                    InteractionPoint.ExerciseType.PushUps => 2f,
                    InteractionPoint.ExerciseType.SitUps => 2f,
                    InteractionPoint.ExerciseType.Treadmill => 3f,
                    _ => 0f
                };
                break;
        }

        if (p.equipment != null)
            score -= p.equipment.QueueCount * 1.5f;

        if (p.IsOccupied)
            score -= 3f;

        float dist = Vector3.Distance(npc.transform.position, p.transform.position);
        score -= dist * 0.05f;

        score += personalityScore;

        if (p == npc.lastTarget)
            score += 6f;
        score += Random.Range(-0.2f, 0.2f);

        return score;
    }

    public InteractionPoint FindTarget(InteractionPoint exclude = null)
    {
        var candidates = new List<InteractionPoint>();
        foreach (var p in points)
            if (p != exclude) candidates.Add(p);

        if (candidates.Count == 0) return null;

        var free = candidates.FindAll(p => !p.IsOccupied);
        var occupied = candidates.FindAll(p => p.IsOccupied);

        if (free.Count > 0 && occupied.Count > 0)
            return (Random.value < 0.65f)
                ? free[Random.Range(0, free.Count)]
                : occupied[Random.Range(0, occupied.Count)];

        if (free.Count > 0) return free[Random.Range(0, free.Count)];
        if (occupied.Count > 0) return occupied[Random.Range(0, occupied.Count)];
        return null;
    }
}