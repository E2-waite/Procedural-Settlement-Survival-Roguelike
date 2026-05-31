using System.Collections.Generic;
using UnityEngine;

public class FireHandler : MonoSingleton<FireHandler>
{
    public List<FireBuilding> fireBuildings = new List<FireBuilding>();

    public void Add(FireBuilding building)
    {
        fireBuildings.Add(building);
    }

    public void Remove(FireBuilding building)
    {
        fireBuildings.Remove(building);
    }
}
