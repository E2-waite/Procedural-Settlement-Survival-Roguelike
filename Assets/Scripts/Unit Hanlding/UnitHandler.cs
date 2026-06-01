using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class UnitHandler
{
    public GameObject fighterPrefab;

    private GridTile hoveringTile = null;
    private Unit hoveringUnit = null;
    private Building hoveringBuilding = null;

    public List<FighterUnit> fighters = new List<FighterUnit>();
    public List<WorkerUnit> workers = new List<WorkerUnit>();


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


    public void HandleHovering(RaycastHit hit) // 
    {
        if (hit.collider == null)
        {
            return;
        }

        switch(hit.transform.tag)
        {
            case "Unit":
                SetHovering(hit.transform.GetComponent<Unit>());
                break;
            case "Tile":
                SetHovering(WorldManager.grid.GetTile(hit.point));
                break;
            case "Building":
                SetHovering(hit.transform.GetComponent<Building>());
                break;
        }
    }

    void SetHovering(GridTile tile)
    {
        hoveringTile = tile;
        hoveringBuilding = null;
        hoveringUnit = null;
    }

    void SetHovering(Unit unit)
    {
        hoveringUnit = unit;
        hoveringBuilding = null;
        hoveringTile = null;
    }

    void SetHovering(Building building)
    {
        hoveringBuilding = building;
        hoveringTile = null;
        hoveringUnit = null;
    }

    public void Command()
    {
        if (GameManager.Player != null)
        {
            List<FollowerUnit> units = GameManager.Player.Followers;
            foreach (FollowerUnit unit in units)
            {
                if (hoveringTile != null)
                    unit.Command(hoveringTile);
                else if (hoveringUnit != null)
                    unit.Command(hoveringUnit);
                //else if (hoveringBuilding != null)
                //    unit.Command(hoveringBuilding);
            }
        }
    }
}
