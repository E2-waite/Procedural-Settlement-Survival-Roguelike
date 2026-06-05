using System.Collections.Generic;
using UnityEngine;

public class UnitStorage
{
    public List<FighterUnit> fighters = new List<FighterUnit>();
    public List<WorkerUnit> workers = new List<WorkerUnit>();

    // Start tracking unit
    public void Add(Unit unit) 
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

    // Stop tracking unit
    public void Remove(Unit unit) 
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
}
