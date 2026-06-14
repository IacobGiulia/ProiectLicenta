using UnityEngine;

public class InteractionPoint : MonoBehaviour
{
    [Header("Reference")]
    public Equipment equipment;

    [Tooltip("Graph node used by the NPC to reach this interaction point and return after the workout.")]
    public Node gatewayNode;

    [Header("Workout")]
    public string animationTrigger;
    public float workoutDuration = 5f;
    public Transform workTarget;

    [Header("Bar")]
    public GameObject rackBar;

    public enum BarType { None, BenchBar, SquatBar }

    [Tooltip("Bar type required")]
    public BarType requiredBar = BarType.None;

    [Tooltip("Delay before the bar appears on the NPC.")]
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