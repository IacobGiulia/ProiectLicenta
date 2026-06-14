using System.Collections.Generic;
using UnityEngine;

public class NPCBrain : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    public float rotationSpeed = 5f;
    public float nodeReachThreshold = 0.3f;
    public float interactionReachThreshold = 0.1f;

    [Header("Queue")]
    [Tooltip("Distance between NPCs in a queue")]
    public float queueSpacing = 2f;

    [Header("AI Personality")]
    public NPCPersonality personality;

    [Tooltip("Queue Timeout")]
    public float queueTimeout = 15f;

    [Header("Animation")]
    public Animator animator;

    [Tooltip("Barbell in the NPC's right hand for bench press.")]
    public GameObject benchBar;

    [Tooltip("Barbell on the NPC's back for squat.")]
    public GameObject squatBar;

    private InteractionPoint targetPoint;
    private InteractionPoint lastUsedPoint;
    public InteractionPoint lastTarget;
    private List<Node> path;
    private int currentNodeIndex;
    private float workoutTimer;
    private Node gatewayNode;

    private float barTimer = 0f;
    private bool waitingForBar = false;
    private GameObject pendingBar = null;
    private bool _waitingAfterPath = false;

    private int _queuePosition = 0;

    private Vector3 _queueWaitPosition;
    private bool _queueWaitPositionSet = false;

    private float _waitingInQueueTimer = 0f;

    private Node[] allNodes;

    public enum NPCPersonality
    {
        Bodybuilder,
        CardioLover,
        Calisthenics,
        Balanced,
        Lazy
    }
    private enum NPCState
    {
        MovingAlongPath,
        MovingToInteraction,
        WorkingOut,
        MovingToGateway,
        WaitingInQueue
    }

    private NPCState state = NPCState.MovingAlongPath;

    void Start()
    {
        personality = (NPCPersonality)Random.Range(
    0,
    System.Enum.GetValues(typeof(NPCPersonality)).Length
        );
        allNodes = Object.FindObjectsOfType<Node>();
        if (benchBar != null) benchBar.SetActive(false);
        if (squatBar != null) squatBar.SetActive(false);
        AssignNextTarget();
    }

    void Update()
    {
        switch (state)
        {
            case NPCState.MovingAlongPath:
                MoveAlongPath();
                break;

            case NPCState.MovingToInteraction:
                MoveDirectToInteraction();
                break;

            case NPCState.WorkingOut:
                DoWorkout();
                break;

            case NPCState.MovingToGateway:
                MoveToGateway();
                break;

            case NPCState.WaitingInQueue:
                UpdateWaitingInQueue();
                return; 
        }

        if (targetPoint == null && state != NPCState.MovingToGateway)
            AssignNextTarget();
    }

    public void UpdateQueuePosition(int newPosition)
    {
        _queuePosition = newPosition;
        _queueWaitPositionSet = false; 
    }

    void UpdateWaitingInQueue()
    {
        _waitingInQueueTimer += Time.deltaTime;
        if (_waitingInQueueTimer >= queueTimeout)
        {
            _waitingInQueueTimer = 0f;
            ClearTarget();
            state = NPCState.MovingToGateway;
            return;
        }

        if (!_queueWaitPositionSet && targetPoint != null)
        {
            _queueWaitPosition = CalculateQueuePosition(_queuePosition);
            _queueWaitPositionSet = true;
        }

        if (_queueWaitPositionSet)
        {
            float dist = DistXZ(transform.position, _queueWaitPosition);
            if (dist > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, _queueWaitPosition, moveSpeed * Time.deltaTime);
                RotateTowards(_queueWaitPosition, dist);
                if (animator != null) animator.SetFloat("MovementSpeed", 0.3f, 0.2f, Time.deltaTime);
            }
            else
            {

                if (targetPoint != null)
                    RotateTowards(targetPoint.transform.position, 1f);
                if (animator != null)
                {
                    animator.SetFloat("MovementSpeed", 0f, 0.2f, Time.deltaTime);
                    animator.SetBool("IsNPC", true);
                }
            }
        }
        else
        {
            if (animator != null)
            {
                animator.SetFloat("MovementSpeed", 0f, 0.2f, Time.deltaTime);
                animator.SetBool("IsNPC", true);
            }
        }
    }

    Vector3 CalculateQueuePosition(int positionIndex)
    {
        if (targetPoint == null || targetPoint.gatewayNode == null)
            return transform.position;

        Vector3 gatewayPos = targetPoint.gatewayNode.transform.position;

        Vector3 dirAwayFromEquipment = (gatewayPos - targetPoint.transform.position).normalized;
        if (dirAwayFromEquipment == Vector3.zero)
            dirAwayFromEquipment = -targetPoint.transform.forward;

        Vector3 lateral = Vector3.Cross(dirAwayFromEquipment, Vector3.up).normalized;
        float lateralOffset = (positionIndex % 2 == 0 ? 0f : 0.3f); 

        Vector3 pos = gatewayPos + dirAwayFromEquipment * (positionIndex * queueSpacing) + lateral * lateralOffset;
        pos.y = transform.position.y; 
        return pos;
    }


    void ClearTarget()
    {
        if (targetPoint == null) return;

        if (targetPoint.equipment != null && targetPoint.equipment.IsOwnedBy(this))
            targetPoint.Release(this);
        else if (targetPoint.equipment != null)
            targetPoint.AbandonQueue(this);

        targetPoint = null;
        _queueWaitPositionSet = false;
        _waitingInQueueTimer = 0f;
    }

    void AssignNextTarget()
    {
        if (targetPoint != null)
        {
            ClearTarget();
        }

        InteractionPoint candidate =
    InteractionManager.Instance.FindBestTarget(this, lastUsedPoint);
        if (candidate == null) return;

        bool reserved = candidate.TryReserve(this);
        targetPoint = candidate;
        _queuePosition = 0;
        _queueWaitPositionSet = false;
        _waitingInQueueTimer = 0f;

        Node targetNode = targetPoint.gatewayNode != null
            ? targetPoint.gatewayNode
            : FindClosestNode(targetPoint.transform.position);

        Node startNode = FindClosestNode(transform.position);
        gatewayNode = targetNode;

        if (!reserved)
        {
            if (targetPoint.equipment != null)
                _queuePosition = targetPoint.equipment.GetQueuePosition(this);

            _waitingAfterPath = true;

            if (startNode != null && targetNode != null && startNode != targetNode)
            {
                path = Pathfinding.Dijkstra(startNode, targetNode);
                currentNodeIndex = 0;
                state = NPCState.MovingAlongPath;
            }
            else
            {
                _waitingAfterPath = false;
                _queueWaitPosition = CalculateQueuePosition(_queuePosition);
                _queueWaitPositionSet = true;
                state = NPCState.WaitingInQueue;
                if (animator != null) animator.SetFloat("MovementSpeed", 0f);
            }
            return;
        }

        _waitingAfterPath = false;

        if (startNode == null || targetNode == null)
        {
            ClearTarget();
            return;
        }

        if (startNode == targetNode)
        {
            path = null;
            currentNodeIndex = 0;
            lastUsedPoint = null;
            state = NPCState.MovingToInteraction;
            return;
        }

        path = Pathfinding.Dijkstra(startNode, targetNode);
        if (path == null || path.Count == 0)
        {
            ClearTarget();
            return;
        }

        currentNodeIndex = 0;
        lastUsedPoint = null;
        state = NPCState.MovingAlongPath;
    }

    void MoveAlongPath()
    {
        if (path != null && currentNodeIndex < path.Count)
        {
            Vector3 targetPos = path[currentNodeIndex].transform.position;
            float distXZ = DistXZ(transform.position, targetPos);

            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            RotateTowards(targetPos, distXZ);

            if (animator != null) animator.SetFloat("MovementSpeed", 0.5f);
            if (distXZ < nodeReachThreshold) currentNodeIndex++;
            return;
        }

        if (_waitingAfterPath)
        {
            _waitingAfterPath = false;
            if (targetPoint != null && targetPoint.equipment != null)
                _queuePosition = targetPoint.equipment.GetQueuePosition(this);
            _queueWaitPosition = CalculateQueuePosition(_queuePosition);
            _queueWaitPositionSet = true;
            state = NPCState.WaitingInQueue;
            if (animator != null) animator.SetFloat("MovementSpeed", 0f);
        }
        else
        {
            state = NPCState.MovingToInteraction;
        }
    }

    void MoveDirectToInteraction()
    {
        if (targetPoint == null) return;

        if (targetPoint.equipment != null && !targetPoint.equipment.IsOwnedBy(this))
        {
            targetPoint = null;
            state = NPCState.MovingToGateway;
            return;
        }

        Vector3 interactionPos = new Vector3(
            targetPoint.transform.position.x,
            transform.position.y,
            targetPoint.transform.position.z);

        float dist = DistXZ(transform.position, interactionPos);

        transform.position = Vector3.MoveTowards(transform.position, interactionPos, moveSpeed * Time.deltaTime);
        RotateTowards(interactionPos, dist);

        if (animator != null) animator.SetFloat("MovementSpeed", 0.5f);

        if (dist < interactionReachThreshold)
            StartWorkout();
    }

    void MoveToGateway()
    {
        if (gatewayNode == null)
        {
            AssignNextTarget();
            return;
        }

        Vector3 gatewayPos = gatewayNode.transform.position;
        float distXZ = DistXZ(transform.position, gatewayPos);

        transform.position = Vector3.MoveTowards(transform.position, gatewayPos, moveSpeed * Time.deltaTime);
        RotateTowards(gatewayPos, distXZ);

        if (animator != null) animator.SetFloat("MovementSpeed", 0.5f);

        if (distXZ < nodeReachThreshold)
        {
            gatewayNode = null;
            AssignNextTarget();
        }
    }

    void RotateTowards(Vector3 targetPos, float distance)
    {
        if (distance < 0.05f) return;
        Vector3 dir = (targetPos - transform.position).normalized;
        if (dir == Vector3.zero) return;

        Quaternion lookRotation = Quaternion.LookRotation(dir);
        float speed = distance < 0.5f ? rotationSpeed * 3f : rotationSpeed;
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, speed * Time.deltaTime);
    }

    void StartWorkout()
    {
        workoutTimer = targetPoint.workoutDuration;
        path = null;
        currentNodeIndex = 0;
        state = NPCState.WorkingOut;

        if (targetPoint.workTarget != null)
        {
            transform.position = targetPoint.workTarget.position;
            transform.rotation = targetPoint.workTarget.rotation;
        }

        if (targetPoint.rackBar != null)
            targetPoint.rackBar.SetActive(false);

        GameObject bar = GetBarForType(targetPoint.requiredBar);
        if (bar != null)
        {
            if (targetPoint.barDelay > 0f)
            {
                waitingForBar = true;
                barTimer = targetPoint.barDelay;
                pendingBar = bar;
            }
            else
            {
                bar.SetActive(true);
            }
        }

        if (animator != null)
        {
            animator.SetFloat("MovementSpeed", 0f);
            animator.SetBool("IsNPC", true);
            if (!string.IsNullOrEmpty(targetPoint.animationTrigger))
                animator.SetTrigger(targetPoint.animationTrigger);
        }
    }

    void DoWorkout()
    {
        if (targetPoint == null)
        {
            state = NPCState.MovingToGateway;
            return;
        }

        if (waitingForBar)
        {
            barTimer -= Time.deltaTime;
            if (barTimer <= 0f)
            {
                waitingForBar = false;
                if (pendingBar != null)
                {
                    pendingBar.SetActive(true);
                    pendingBar = null;
                }
            }
        }

        workoutTimer -= Time.deltaTime;
        if (workoutTimer > 0f) return;

        waitingForBar = false;
        pendingBar = null;

        if (targetPoint.rackBar != null)
            targetPoint.rackBar.SetActive(true);

        GameObject bar = GetBarForType(targetPoint.requiredBar);
        if (bar != null) bar.SetActive(false);

        if (animator != null)
        {
            animator.SetBool("IsNPC", false);
            if (!string.IsNullOrEmpty(targetPoint.animationTrigger))
                animator.ResetTrigger(targetPoint.animationTrigger);
        }

        lastUsedPoint = targetPoint;
        targetPoint.Release(this);
        lastTarget = targetPoint;
        targetPoint = null;
        state = NPCState.MovingToGateway;
    }

    public void NotifyEquipmentAvailable()
    {
        if (state != NPCState.WaitingInQueue)
        {
            if (targetPoint != null && targetPoint.equipment != null && targetPoint.equipment.IsOwnedBy(this))
                targetPoint.Release(this);
            return;
        }

        if (targetPoint == null)
        {
            state = NPCState.MovingToGateway;
            return;
        }

        _waitingInQueueTimer = 0f;
        _queueWaitPositionSet = false;
        lastUsedPoint = null;
        state = NPCState.MovingToInteraction;
    }

    Node FindClosestNode(Vector3 pos)
    {
        if (allNodes == null || allNodes.Length == 0) return null;
        Node closest = null;
        float minDist = float.MaxValue;
        foreach (Node node in allNodes)
        {
            if (node == null) continue;
            float d = Vector3.Distance(pos, node.transform.position);
            if (d < minDist) { minDist = d; closest = node; }
        }
        return closest;
    }

    float DistXZ(Vector3 a, Vector3 b)
    {
        return Vector3.Distance(new Vector3(a.x, 0f, a.z), new Vector3(b.x, 0f, b.z));
    }

    GameObject GetBarForType(InteractionPoint.BarType barType)
    {
        switch (barType)
        {
            case InteractionPoint.BarType.BenchBar: return benchBar;
            case InteractionPoint.BarType.SquatBar: return squatBar;
            default: return null;
        }
    }
}