using UnityEngine;

public class FollowerSystem
{
    [SerializeField] private UnitStorage storage = new UnitStorage();

    public void AddUnit(FollowerUnit unit)
    {
        storage.Add(unit);
    }

    public void RemoveUnit(FollowerUnit unit)
    {
        storage.Remove(unit);
    }
}
