using System.Collections.Generic;
using UnityEngine;

public class UnitSystem : MonoSingleton<UnitSystem>
{
    public List<FighterUnit> fighters = new List<FighterUnit>();
    public List<WorkerUnit> workers = new List<WorkerUnit>();

    public List<FollowerUnit> nearbyUnits = new List<FollowerUnit>();
    public List<FollowerUnit> followingUnits = new List<FollowerUnit>();

    public List<FollowerUnit> Nearby => nearbyUnits;
    public List<FollowerUnit> Followers => followingUnits;

    public void HandleHover(RaycastHit hit)
    {

    }

    public void Command(GridTile hoveringTile)
    {
        foreach (FollowerUnit unit in Followers)
        {
            unit.Command(hoveringTile);
        }
    }
    
    public void Command(Unit hoveringUnit)
    {
        foreach (FollowerUnit unit in Followers)
        {
            unit.Command(hoveringUnit);
        }
    }

    // Command following units to interact with the passed building
    public void Command(Building hoveringBuilding)
    {

    }

    public void AddUnit(Unit unit) // Start tracking unit
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

    public void RemoveUnit(Unit unit) // Stop tracking unit
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

    // Tells all nearby followers to start following the player
    public void StartFollowing()
    {
        // Tells nearby units to start following
        foreach (FollowerUnit nearby in nearbyUnits)
        {
            followingUnits.Add(nearby);
            nearby.StartFollowing(GameManager.Player);
        }

        nearbyUnits.Clear();
    }

    public void StopFollowing(FollowerUnit unit)
    {
        followingUnits.Remove(unit);
        unit.StopFollowing();
    }
}
