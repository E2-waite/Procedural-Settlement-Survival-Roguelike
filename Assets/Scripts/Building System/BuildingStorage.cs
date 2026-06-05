using System.Collections.Generic;

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
}
