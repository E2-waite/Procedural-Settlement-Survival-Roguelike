using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class BuildingStorage
{
    private List<ResourceBuilding>[] resourceBuildings = new List<ResourceBuilding>[(int)ResourceNode.Type.Max];
    public List<ResourceBuilding> ResourceBuildings(ResourceNode.Type type) => resourceBuildings[(int)type];

    public List<Building> buildings = new List<Building>();

    public void Add(Building building)
    {
        buildings.Add(building);

        if (building is ResourceBuilding)
        {
            ResourceBuilding resourceBuilding = (ResourceBuilding)building;

            ResourceNode.Type type = resourceBuilding.type;

            if (resourceBuildings[(int)type] == null)
            {
                resourceBuildings[(int)type] = new List<ResourceBuilding>();
            }

            resourceBuildings[(int)type].Add(resourceBuilding);
        }
    }

    // Get the closest resource building to the passed position
    public ResourceBuilding GetClosestStore(ResourceNode.Type type, Vector2Int pos)
    {
        float lowestDist = float.MaxValue;
        ResourceBuilding store = null;

        List<ResourceBuilding> storeList = ResourceBuildings(type);

        for (int i = 0; storeList != null && i < storeList.Count; i++)
        {
            ResourceBuilding current = storeList[i];

            if (!current.Built()) continue; // Don't include non-built or broken stores

            Vector2Int storePos = new Vector2Int((int)current.transform.position.x, (int)current.transform.position.z);

            float dist = Vector2Int.Distance(storePos, pos);
            if (dist < lowestDist)
            {
                lowestDist = dist;
                store = current;
            }
        }

        return store;
    }
}
