using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;

[System.Serializable]
public class BuildingManager : MonoSingleton<BuildingManager>
{
    public TileMarker tileMarker;
    BuildingObject selectedBuilding = null;
    public List<BuildingObject> buildings = new List<BuildingObject>();

    GridTile currTile = null;
    Vector2Int gridPos = new Vector2Int(0, 0);
    Vector2Int lastPos;

    // Resource stores
    public List<ResourceBuilding>[] resourceStores = new List<ResourceBuilding>[(int)ResourceNode.Type.Max];


    private void Start()
    {
        BuildPanel.Instance.UpdateDisplay(buildings);
    }

    public void SelectBuilding(int index)
    {
        if (index < buildings.Count) 
            selectedBuilding = buildings[index];

        InteractionManager.Instance.SetState(InteractionManager.GameState.Build);
    }

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
            GridTile hitTile = WorldManager.grid.GetTile(gridPos);

            if (hitTile != null)
            {
                currTile = hitTile;
                hitTile.Hover(true);
                tileMarker.HighlightTiles(gridPos, selectedBuilding.size, selectedBuilding != null && selectedBuilding.CanAfford());
                tileMarker.transform.position = new Vector3(gridPos.x + 1.5f, 0, gridPos.y + 1.5f);
            }
        }
    }

    public void CreateBuilding()
    {
        if (CanBuild(gridPos, selectedBuilding.size) && selectedBuilding.CanAfford())
        {
            Vector2Int tilePos = new Vector2Int();

            GameObject buildingObj = Instantiate(selectedBuilding.prefab, new Vector3(currTile.position.x, 0, currTile.position.y), Quaternion.identity);
            buildingObj.transform.localScale = new Vector3(selectedBuilding.size.x, 1, selectedBuilding.size.y);
            Building building = buildingObj.GetComponent<Building>();

            if (building == null) return;

            selectedBuilding.ConsumeResources();

            // Add resource store to its appropriate list (based on type)
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
            for (int x = gridPos.x; x < gridPos.x + selectedBuilding.size.x; x++)
            {
                for (int y = gridPos.y; y < gridPos.y + selectedBuilding.size.y; y++)
                {
                    tilePos.x = x;
                    tilePos.y = y;
                    GridTile tile = WorldManager.grid.GetTile(tilePos);
                    tile.Build(building);
                }
            }

            tileMarker.HighlightTiles(gridPos, selectedBuilding.size, selectedBuilding != null && selectedBuilding.CanAfford());
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

#if UNITY_EDITOR
    [ContextMenu("Refresh Building List")]
    public void RefreshBuildings()
    {

        string buildingsPath = "Assets/Buildings/Objects";

        buildings.Clear();
        string[] guids = AssetDatabase.FindAssets("t:BuildingObject", new[] { buildingsPath });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            BuildingObject obj = AssetDatabase.LoadAssetAtPath<BuildingObject>(path);

            if (obj != null)
                buildings.Add(obj);
        }


    }
#endif
}
