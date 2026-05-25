using UnityEngine;

public class BuildHandler : MonoSingleton<BuildHandler>
{
    //public Vector2Int buildSize = new Vector2Int(1, 1);

    public TileMarker tileMarker;
    public BuildingObject selectedBuilding = null;

    GridTile currTile = null;
    Vector2Int gridPos = new Vector2Int(0, 0);
    Vector2Int lastPos;
    public void SetEnabled(bool enabled)
    {
        tileMarker.gameObject.SetActive(enabled);
    }

    // Handle the ray cast from the InteractionManager
    public void HandleRay(RaycastHit hit)
    {
        gridPos.x = Mathf.FloorToInt(hit.point.x);
        gridPos.y = Mathf.FloorToInt(hit.point.z);

        // Only check if hovering tile has changed
        if (gridPos != lastPos)
        {
            lastPos = gridPos;
            GridTile hitTile = Grid.Instance.getTile(gridPos);

            if (hitTile != null)
            {
                currTile = hitTile;
                hitTile.Hover(true);
                tileMarker.HighlightTiles(gridPos, selectedBuilding.size);
                tileMarker.transform.position = new Vector3(gridPos.x + 1.5f, 0, gridPos.y + 1.5f);
            }
        }
    }

    public void Build()
    {
        if (CanBuild(gridPos, selectedBuilding.size))
        {
            Vector2Int tilePos = new Vector2Int();

            GameObject buildingObj = Instantiate(selectedBuilding.prefab, new Vector3(currTile.position.x, 0, currTile.position.y), Quaternion.identity);
            buildingObj.transform.localScale = new Vector3(selectedBuilding.size.x, 1, selectedBuilding.size.y);
            Building building = buildingObj.GetComponent<Building>();

            if (building == null) return;

            // Assign buildings to appropriate tiles
            for (int x = gridPos.x; x < gridPos.x + selectedBuilding.size.x; x++)
            {
                for (int y = gridPos.y; y < gridPos.y + selectedBuilding.size.y; y++)
                {
                    tilePos.x = x;
                    tilePos.y = y;
                    GridTile tile = Grid.Instance.getTile(tilePos);
                    tile.Build(building);
                }
            }

            tileMarker.HighlightTiles(gridPos, selectedBuilding.size);
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
                GridTile tile = Grid.Instance.getTile(tilePos);

                if (tile == null || !tile.Buildable())
                {
                    return false;
                }
            }
        }

        return true;
    }
}
