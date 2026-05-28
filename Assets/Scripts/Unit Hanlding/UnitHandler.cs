using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class UnitHandler : MonoSingleton<UnitHandler>
{
    public GridTile hoveringTile = null;
    public Unit hoveringUnit = null;
    public List<FollowerUnit> followingUnits = new List<FollowerUnit>();

    public List<FighterUnit> fighters = new List<FighterUnit>();
    public List<WorkerUnit> workers = new List<WorkerUnit>();

    public void AddUnit(Unit unit)
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

    public void RemoveUnit(Unit unit)
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


    public void Hover(RaycastHit hit)
    {
        if (hit.collider == null)
        {
            return;
        }

        // If hovering over tile
        // Else if hovering over unit
        if (hit.transform.CompareTag("Unit"))
        {
            hoveringUnit = hit.transform.GetComponent<Unit>();
            hoveringTile = null;
        }
        else
        {
            hoveringTile = WorldHandler.grid.GetTile(hit.point);
            hoveringUnit = null;
        }
    }

    public void SetFollowing(List<FollowerUnit> following)
    {
        followingUnits = new List<FollowerUnit>(following);
    }

    public void CommandUnits()
    {
        if (hoveringTile != null || hoveringUnit != null)
        {
            for (int i = 0; i < followingUnits.Count; i++)
            {
                if (hoveringTile != null)
                    followingUnits[i].Command(hoveringTile);
                else if (hoveringUnit != null)
                    followingUnits[i].Command(hoveringUnit);
            }
        }
    }
}
