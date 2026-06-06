using UnityEngine;
using System.Collections.Generic;

public class Equipment : MonoBehaviour
{
    private NPCBrain _owner = null;
    private List<NPCBrain> _queue = new List<NPCBrain>();

    private bool _occupiedByPlayer = false;

    public bool IsOccupied => _owner != null || _occupiedByPlayer;
    public bool IsOccupiedByPlayer => _occupiedByPlayer;
    public int QueueCount => _queue.Count;
    public NPCBrain Owner => _owner;

    public void PlayerAcquire()
    {
        _occupiedByPlayer = true;
    }

    public void PlayerRelease()
    {
        _occupiedByPlayer = false;

        if (_queue.Count > 0 && _owner == null)
        {
            NPCBrain next = _queue[0];
            _queue.RemoveAt(0);
            if (next != null)
            {
                _owner = next;
                NotifyQueuePositions();
                next.NotifyEquipmentAvailable();
            }
        }
    }

    public bool TryAcquire(NPCBrain npc)
    {
        if (_occupiedByPlayer || _owner != null)
        {
            if (_owner == npc)
            {
                Debug.LogError($"[Equipment:{gameObject.name}] DOUBLE ACQUIRE de același NPC: {npc.name}!");
                return true;
            }

            if (!_queue.Contains(npc))
                _queue.Add(npc);

            return false;
        }

        _owner = npc;
        return true;
    }

    public void Release(NPCBrain npc)
    {
        if (_owner != npc)
        {
            Debug.LogWarning($"[Equipment:{gameObject.name}] Release invalid de la {npc.name}, owner e {(_owner != null ? _owner.name : "NULL")}.");
            return;
        }

        _owner = null;

        if (_occupiedByPlayer) return;

        if (_queue.Count > 0)
        {
            NPCBrain next = _queue[0];
            _queue.RemoveAt(0);
            if (next != null)
            {
                _owner = next;
                NotifyQueuePositions();
                next.NotifyEquipmentAvailable();
            }
        }
    }

    public void RemoveFromQueue(NPCBrain npc)
    {
        _queue.Remove(npc);
        NotifyQueuePositions();
    }

    public bool IsOwnedBy(NPCBrain npc) => _owner == npc;

    public int GetQueuePosition(NPCBrain npc) => _queue.IndexOf(npc);

    void NotifyQueuePositions()
    {
        for (int i = 0; i < _queue.Count; i++)
        {
            if (_queue[i] != null)
                _queue[i].UpdateQueuePosition(i);
        }
    }
}