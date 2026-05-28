using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public GameObject playerPrefab;
    public PlayerController player;
    Grid grid;

    private void Start()
    {
        grid = Grid.Instance;

        grid.GenerateGrid();

        GridTile spawnTile = FindSpawnTile();

        if (spawnTile != null)
        {
            GameObject playerObj = Instantiate(playerPrefab, spawnTile.worldPosition + new Vector3(0, 0.5f, 0), Quaternion.identity);
            player = playerObj.GetComponent<PlayerController>();
        }
    }

    GridTile FindSpawnTile()
    {
        List<GridTile> validTiles = new List<GridTile>();

        foreach (GridTile tile in grid.tileGrid.Values)
        {
            if (tile.Walkable())
            {
                validTiles.Add(tile);
            }
        }

        if (validTiles.Count == 0) return null;

        return validTiles[Random.Range(0, validTiles.Count)];
    }
}
