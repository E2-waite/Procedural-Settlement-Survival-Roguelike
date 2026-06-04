using System.Collections.Generic;
using UnityEngine;
using static InteractionController;

public class GameManager : MonoSingleton<GameManager>
{
    private static Player player = null;
    public static Player Player => player;

    public GameObject playerPrefab;
    public GameObject workerPrefab;
    public GameObject firePrefab;
    public GameObject fighterPrefab;
    public GameObject enemyPrefab;

    private void Start()
    {
        // Startup is coordinated here so systems do not depend on Unity's Start order.
        ResourceSystem.Instance.Init();
        UIHandler.Instance.Init();
        BuildingList.Instance.Init();

        // World data must exist before choosing a valid spawn tile.
        WorldManager.Instance.GenerateGrid();

        SpawnStartingFireAndUnits();

        // Input is created, then listeners subscribe, then gameplay input is enabled.
        InputManager.Instance.Init();
        InteractionController.Instance.Init();
        InputManager.Instance.EnableGameplayInput();

        // SetGameState(GameState.Playing);
    }

    private void SpawnStartingFireAndUnits()
    {
        GridTile spawnTile = FindSpawnTile();

        if (spawnTile != null)
        {
            GameObject fireObj = Instantiate(firePrefab, spawnTile.worldPosition, Quaternion.identity);
            FireBuilding fireBuilding = fireObj.GetComponent<FireBuilding>();
            spawnTile.Build(fireBuilding);

            GameObject playerObj = Instantiate(playerPrefab, spawnTile.Center() + new Vector3(1f, 0.5f, 0), Quaternion.identity);
            player = playerObj.GetComponent<Player>();

            Instantiate(workerPrefab, spawnTile.Center() + new Vector3(1f, 0.5f, 1f), Quaternion.identity);

            Instantiate(fighterPrefab, spawnTile.Center() + new Vector3(-1f, 0.5f, -1f), Quaternion.identity);
            Instantiate(enemyPrefab, spawnTile.Center() + new Vector3(-1f, 0.5f, 0f), Quaternion.identity);

            //Instantiate(workerPrefab, spawnTile.worldPosition + new Vector3(-.5f, 0.5f, 1.5f), Quaternion.identity);
            //Instantiate(workerPrefab, spawnTile.worldPosition + new Vector3(-.5f, 0.5f, -.5f), Quaternion.identity);
            //Instantiate(workerPrefab, spawnTile.worldPosition + new Vector3(-.5f, 0.5f, -.5f), Quaternion.identity);

            WorldManager.Instance.HandleChunks(spawnTile.chunk);
        }
    }

    GridTile FindSpawnTile()
    {
        List<GridTile> validTiles = new List<GridTile>();

        // Spawn on a tile with all neighbouring cells empty so the starting area is usable.
        foreach (GridTile tile in WorldManager.grid.Tiles())
        {
            bool valid = true;

            List<GridTile> neighbourTiles = new List<GridTile>();

            neighbourTiles.Add(tile);

            foreach (Vector2Int neighbourPos in Consts.ALL_NEIGHBOURS)
            {
                GridTile neighbourTile = WorldManager.grid.GetTile(tile.position + neighbourPos);
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
