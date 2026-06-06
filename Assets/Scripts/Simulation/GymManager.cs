using System.Collections.Generic;
using UnityEngine;

public class GymManager : MonoBehaviour
{
    public static GymManager Instance;
    public List<Equipment> equipments = new List<Equipment>();

    void Awake()
    {
        Instance = this;
    }

    public Equipment GetFreeEquipment()
    {
        List<Equipment> free = equipments.FindAll(e => !e.IsOccupied);
        if (free.Count == 0) return null;
        return free[Random.Range(0, free.Count)];
    }
}
