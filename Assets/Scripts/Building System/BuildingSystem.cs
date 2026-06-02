using System.Collections.Generic;
using UnityEngine;

public class BuildingSystem : MonoSingleton<BuildingSystem>
{
    public TileMarker tileMarker;
    BuildingList buildingList;
    public List<ResourceBuilding>[] resourceStores = new List<ResourceBuilding>[(int)ResourceNode.Type.Max];
    GridTile lastTile = null;

    private void Start()
    {
        buildingList = BuildingList.Instance;
    }

    public void SetEnabled(bool enabled)
    {
        tileMarker.gameObject.SetActive(enabled);
    }


    public void HandleHover(GridTile tile)
    {
        if (tile != lastTile)
        {
            lastTile = tile;

            BuildingObject selected = buildingList.Selected;

            tileMarker.HighlightTiles(tile.position, selected.size, selected != null && selected.CanAfford());
            tileMarker.transform.position = new Vector3(tile.position.x + 1.5f, 0, tile.position.y + 1.5f);
        }
    }

    public bool TryPlace(GridTile tile)
    {
        BuildingObject selected = buildingList.Selected;

        if (selected == null || tile == null) return false;

        if (CanBuild(tile.position, selected.size) && selected.CanAfford())
        {
            GameObject buildingObj = Instantiate(selected.prefab, new Vector3(tile.position.x, 0, tile.position.y), Quaternion.identity);
            buildingObj.transform.localScale = new Vector3(selected.size.x, 1, selected.size.y);
            Building building = buildingObj.GetComponent<Building>();

            if (building == null)
            {
                Destroy(buildingObj);
                return false;
            }

            selected.ConsumeResources();

            // Setup resource store building
            if (building is ResourceBuilding)
            {
                ResourceBuilding store = (ResourceBuilding)building;

                ResourceNode.Type type = store.type;

                if (resourceStores[(int)type] == null)
                {
                    resourceStores[(int)type] = new List<ResourceBuilding>();
                }

                resourceStores[(int)type].Add(store);
            }

            // Assign buildings to appropriate tiles
            for (int x = tile.position.x; x < tile.position.x + selected.size.x; x++)
            {
                for (int y = tile.position.y; y < tile.position.y + selected.size.y; y++)
                {
                    GridTile buildTile = WorldManager.grid.GetTile(new Vector2Int(x, y));
                    buildTile.Build(building);
                }
            }

            tileMarker.HighlightTiles(tile.position, selected.size, selected != null && selected.CanAfford());


            return true;
        }
        else
        {
            return false;
        }
    }

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

    public ResourceBuilding GetClosestStore(ResourceNode.Type type, Vector2Int pos)
    {
        float lowestDist = float.MaxValue;
        ResourceBuilding store = null;

        List<ResourceBuilding> storeList = resourceStores[(int)type];

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
