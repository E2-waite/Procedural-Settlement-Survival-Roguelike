using UnityEngine;

public class FollowerSystem
{
    [SerializeField] private UnitStorage storage = new UnitStorage();

    public void AddUnit(FriendlyUnit unit)
    {
        storage.Add(unit);
    }

    public void RemoveUnit(FriendlyUnit unit)
    {
        storage.Remove(unit);
    }
}
