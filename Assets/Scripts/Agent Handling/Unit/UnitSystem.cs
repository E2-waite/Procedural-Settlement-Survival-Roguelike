using UnityEngine;

public class UnitSystem
{
    [SerializeField] private UnitStorage storage = new UnitStorage();
    private int capacity = 5;

    public void IncreaseCapacity(int num)
    {
        capacity += num;

        Debug.Log("Capacity at " + capacity);
    }

    public bool AddUnit(Unit unit)
    {
        if (storage.Count >= capacity) return false;

        storage.Add(unit);
        return true;
    }

    public void RemoveUnit(Unit unit)
    {
        storage.Remove(unit);
    }
}
