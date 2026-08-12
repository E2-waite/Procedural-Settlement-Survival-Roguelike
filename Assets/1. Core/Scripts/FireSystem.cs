using System.Collections.Generic;
using UnityEngine;

public class FireSystem
{
    private List<FireBuilding> fireBuildings = new List<FireBuilding>();
    public List<FireBuilding> Buildings => fireBuildings;

    public void Add(FireBuilding building)
    {
        fireBuildings.Add(building);
    }

    public void Remove(FireBuilding building)
    {
        fireBuildings.Remove(building);
    }
}
