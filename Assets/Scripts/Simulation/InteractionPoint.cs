using UnityEngine;

public class InteractionPoint : MonoBehaviour
{
    [Header("Referinte")]
    public Equipment equipment;

    [Tooltip("Nodul din graf de unde NPC-ul pleacă spre acest interaction point și unde se întoarce după workout.")]
    public Node gatewayNode;

    [Header("Workout")]
    public string animationTrigger;
    public float workoutDuration = 5f;
    public Transform workTarget;

    [Header("Bara")]
    public GameObject rackBar;

    public enum BarType { None, BenchBar, SquatBar }

    [Tooltip("Ce tip de bară trebuie activată pe NPC pentru acest exercițiu.")]
    public BarType requiredBar = BarType.None;

    [Tooltip("Delay în secunde înainte să apară bara pe NPC.")]
    public float barDelay = 0f;

    public enum ExerciseType
    {
        BenchPress,
        Squat,
        Treadmill,
        BicepCurls,
        FrontRaises,
        PushUps,
        SitUps
    }

    [Header("Exercise Type")]
    public ExerciseType exerciseType;

    public bool IsOccupied => equipment != null && equipment.IsOccupied;

    private void Start()
    {
        if (InteractionManager.Instance != null &&
            !InteractionManager.Instance.points.Contains(this))
        {
            InteractionManager.Instance.points.Add(this);
        }
    }

    public bool TryReserve(NPCBrain npc)
    {
        if (equipment == null)
        {
            Debug.LogError($"[InteractionPoint:{gameObject.name}] equipment e NULL! NPC {npc.name} trece fără lock.");
            return true;
        }

        
        return equipment.TryAcquire(npc);
    }

    public void Release(NPCBrain npc)
    {
        equipment?.Release(npc);
    }

    public void AbandonQueue(NPCBrain npc)
    {
        equipment?.RemoveFromQueue(npc);
    }
}