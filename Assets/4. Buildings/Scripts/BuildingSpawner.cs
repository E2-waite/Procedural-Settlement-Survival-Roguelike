using UnityEngine;

public class BuildingSpawner : MonoBehaviour
{
    public Building Spawn(BuildingObject selectedBuilding, GridTile tile)
    {
        GameObject buildingObj = Instantiate(selectedBuilding.prefab, new Vector3(tile.position.x, 0, tile.position.y), Quaternion.identity);
        buildingObj.transform.localScale = new Vector3(selectedBuilding.size.x, 1, selectedBuilding.size.y);
        Building building = buildingObj.GetComponent<Building>();

        if (building == null)
        {
            Destroy(buildingObj);
        }

        return building;
    }
}