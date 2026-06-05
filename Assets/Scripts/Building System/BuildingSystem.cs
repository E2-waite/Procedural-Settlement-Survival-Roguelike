using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class BuildingSystem
{
   // public TileMarker tileMarker;
    //private BuildingList _buildingList;
    private BuildingStorage storage = new BuildingStorage();
    private BuildingSpawner _spawner;

    public BuildingSystem(BuildingSpawner spawner, BuildingCatalog buildingList)
    {
        _spawner = spawner;
    }

    // Try to place the selected building on the passed tile
    public bool TryPlace(GridTile tile, BuildingObject selected)
    {
        //BuildingObject selected = _buildingList.Selected;

        if (selected == null || tile == null) return false;

        // Placement, affordability, resource payment, and tile ownership are committed together.
        if (CanBuild(tile.position, selected.size) && selected.CanAfford())
        {
            Building building = _spawner.Spawn(selected, tile);

            selected.ConsumeResources();

            storage.Add(building);

            // Assign buildings to appropriate tiles
            for (int x = tile.position.x; x < tile.position.x + selected.size.x; x++)
            {
                for (int y = tile.position.y; y < tile.position.y + selected.size.y; y++)
                {
                    GridTile buildTile = WorldManager.grid.GetTile(new Vector2Int(x, y));
                    buildTile.Build(building);
                }
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    // Returns true if all tiles in pos to pos + size are buildable
    public bool CanBuild(Vector2Int pos, Vector2Int size)
    {
        Vector2Int tilePos = new Vector2Int();
        for (int x = pos.x; x < pos.x + size.x; x++)
        {
            for (int y = pos.y; y < pos.y + size.y; y++)
            {
                tilePos.x = x;
                tilePos.y = y;
                GridTile tile = WorldManager.grid.GetTile(tilePos);

                if (tile == null || !tile.Buildable())
                {
                    return false;
                }
            }
        }

        return true;
    }

    // Get the closest resource building to the passed position
    public ResourceBuilding GetClosestStore(ResourceNode.Type type, Vector2Int pos)
    {
        float lowestDist = float.MaxValue;
        ResourceBuilding store = null;

        List<ResourceBuilding> storeList = storage.ResourceBuildings(type);

        for (int i = 0; storeList != null && i < storeList.Count; i++)
        {
            ResourceBuilding current = storeList[i];

            if (!current.Built) continue; // Don't include non-built or broken stores

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
