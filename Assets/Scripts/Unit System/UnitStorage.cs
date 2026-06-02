using System.Collections.Generic;
using UnityEngine;

public class UnitStorage
{
    public List<FighterUnit> fighters = new List<FighterUnit>();
    public List<WorkerUnit> workers = new List<WorkerUnit>();

    public List<FollowerUnit> nearbyUnits = new List<FollowerUnit>();
    public List<FollowerUnit> followingUnits = new List<FollowerUnit>();

    public List<FollowerUnit> Nearby => nearbyUnits;
    public List<FollowerUnit> Followers => followingUnits;

    public void Add(Unit unit) // Start tracking unit
    {
        if (unit is FighterUnit)
        {
            FighterUnit fighter = (FighterUnit)unit;
            if (!fighters.Contains(fighter))
            {
                fighters.Add(fighter);
            }
        }
        else if (unit is WorkerUnit)
        {
            WorkerUnit worker = (WorkerUnit)unit;
            if (!workers.Contains(worker))
            {
                workers.Add(worker);
            }
        }
    }

    public void Remove(Unit unit) // Stop tracking unit
    {
        if (unit is FighterUnit)
        {
            FighterUnit fighter = (FighterUnit)unit;
            if (fighters.Contains(fighter))
            {
                fighters.Remove(fighter);
            }
        }
        else if (unit is WorkerUnit)
        {
            WorkerUnit worker = (WorkerUnit)unit;
            if (workers.Contains(worker))
            {
                workers.Remove(worker);
            }
        }
    }

    public void AddNearby(FollowerUnit unit)
    {
        if (unit != null && !nearbyUnits.Contains(unit))
        {
            nearbyUnits.Add(unit);
        }
    }

    public void RemoveNearby(FollowerUnit unit)
    {
        if (unit != null && nearbyUnits.Contains(unit))
        {
            nearbyUnits.Remove(unit);
        }
    }

    public void ClearNearby()
    {
        nearbyUnits.Clear();
    }

    public void AddFollowing(FollowerUnit unit)
    {
        if (unit != null && !followingUnits.Contains(unit))
        {
            followingUnits.Add(unit);
        }
    }

    public void RemoveFollowing(FollowerUnit unit)
    {
        if (unit != null && followingUnits.Contains(unit))
        {
            followingUnits.Remove(unit);
        }
    }
}
