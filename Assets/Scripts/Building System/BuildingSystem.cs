using System.Collections.Generic;
using UnityEngine;

public class BuildingSystem : MonoSingleton<BuildingSystem>
{
    private BuildingStorage storage = new BuildingStorage();
    public BuildingStorage Storage => storage;

    public TileMarker tileMarker;
    BuildingList buildingList;
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
}
