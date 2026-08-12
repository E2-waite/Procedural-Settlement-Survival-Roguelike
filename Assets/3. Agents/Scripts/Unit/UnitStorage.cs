using System.Collections.Generic;
using UnityEngine;

public class UnitStorage
{
    public List<Unit> units = new List<Unit>();

    public int Count => units.Count;

    // Start tracking unit
    public void Add(Unit unit) 
    {
        if (!units.Contains(unit))
        {
            units.Add(unit);
        }
    }

    // Stop tracking unit
    public void Remove(Unit unit) 
    {
        units.Remove(unit);
    }
}
