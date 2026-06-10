using System.Collections.Generic;
using UnityEngine;
using static GameContext;
public class BuildingSystem
{
    // public TileMarker tileMarker;
    //private BuildingList _buildingList;
    private WorldGrid grid;

    private BuildingStorage storage = new BuildingStorage();
    private BuildingSpawner spawner;
    private ResourceSystem resourceSystem;
    private BuildingObject selected;
    public BuildingObject Selected => selected;
    private GameContext gameContext;
    GameType gameType;

    public BuildingSystem(GameContext context)
    {
        gameType = context.gameType;
        spawner = context.buildingSpawner;
        if (gameType == GameType.Game)
            resourceSystem = context.resourceSystem;
        World world = context.world;
        grid = world.Context.grid;
        gameContext = context;

    }

    public void Select(BuildingObject selection)
    {
        selected = selection;
    }

    public bool CanAfford()
    {
        if (gameType != GameType.Game) return true;

        if (selected != null)
        {
            for (ResourceNode.Type i = ResourceNode.Type.Wood; i < ResourceNode.Type.Max; i++)
            {
                if (resourceSystem.GetResourceCount(i) < selected.Cost.Get(i))
                    return false;
            }

            return true;
        }
        return false;
    }

    // Try to place the selected building on the passed tile
    public bool TryPlace(GridTile tile, BuildingObject selected)
    {
        if (selected == null || tile == null) return false;

        // Placement, affordability, resource payment, and tile ownership are committed together.
        if (CanBuild(tile.position, selected.size) && CanAfford(selected))
        {
            Building building = spawner.Spawn(selected, tile);
            if (building == null) return false;
            building.Init(gameContext);

            ConsumeResources(selected);
            storage.Add(building);

            if (building is ResourceBuilding)
            {
                ((ResourceBuilding)building).Init(gameContext);
            }
            else if (building is FireBuilding)
            {
                ((FireBuilding)building).Init(gameContext);
            }

            // Assign buildings to appropriate tiles
            for (int x = tile.position.x; x < tile.position.x + selected.size.x; x++)
            {
                for (int y = tile.position.y; y < tile.position.y + selected.size.y; y++)
                {
                    GridTile buildTile = grid.GetTile(new Vector2Int(x, y));
                    building.AddTile(buildTile);
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

    bool CanAfford(BuildingObject building)
    {
        if (gameType != GameType.Game) return true;

        for (ResourceNode.Type i = ResourceNode.Type.Wood; i < ResourceNode.Type.Max; i++)
        {
            if (resourceSystem.GetResourceCount(i) < building.Cost.Get(i))
                return false;
        }

        return true;
    }

    void ConsumeResources(BuildingObject building)
    {
        if (gameType != GameType.Game) return;

        for (ResourceNode.Type i = ResourceNode.Type.Wood; i < ResourceNode.Type.Max; i++)
        {
            resourceSystem.ConsumeResource(i, building.Cost.Get(i));
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
                GridTile tile = grid.GetTile(tilePos);

                if (tile == null || !tile.Buildable)
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
