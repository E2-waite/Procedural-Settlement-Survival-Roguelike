using UnityEngine;

public class UnitSystem
{
    [SerializeField] private UnitStorage storage = new UnitStorage();

    public void AddUnit(Unit unit)
    {
        storage.Add(unit);
    }

    public void RemoveUnit(Unit unit)
    {
        storage.Remove(unit);
    }
}
