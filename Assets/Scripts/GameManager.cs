using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public GameObject playerPrefab;
    public GameObject workerPrefab;
    public PlayerController player;

    private void Start()
    {
        WorldHandler.Instance.GenerateGrid();
        
        GridTile spawnTile = FindSpawnTile();

        if (spawnTile != null)
        {
            GameObject playerObj = Instantiate(playerPrefab, spawnTile.worldPosition + new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity);
            player = playerObj.GetComponent<PlayerController>();

            Instantiate(workerPrefab, spawnTile.worldPosition + new Vector3(1.5f, 0.5f, 1.5f), Quaternion.identity);
            //Instantiate(workerPrefab, spawnTile.worldPosition + new Vector3(-.5f, 0.5f, 1.5f), Quaternion.identity);
            //Instantiate(workerPrefab, spawnTile.worldPosition + new Vector3(-.5f, 0.5f, -.5f), Quaternion.identity);
            //Instantiate(workerPrefab, spawnTile.worldPosition + new Vector3(-.5f, 0.5f, -.5f), Quaternion.identity);

            WorldHandler.Instance.HandleChunks(spawnTile.chunk);
        }
    }

    GridTile FindSpawnTile()
    {
        List<GridTile> validTiles = new List<GridTile>();

        foreach (GridTile tile in WorldHandler.grid.Tiles())
        {
            bool valid = true;

            List<GridTile> neighbourTiles = new List<GridTile>();

            neighbourTiles.Add(tile);

            foreach (Vector2Int neighbourPos in Consts.ALL_NEIGHBOURS)
            {
                GridTile neighbourTile = WorldHandler.grid.GetTile(tile.position + neighbourPos);
                if (neighbourTile == null || !neighbourTile.IsEmpty())
                {
                    valid = false;
                    break;
                }
            }

            if (valid) validTiles.Add(tile);
        }

        if (validTiles.Count == 0) return null;

        return validTiles[Random.Range(0, validTiles.Count)];
    }
}
