using System.Collections.Generic;
using UnityEngine;
using static InteractionController;

public class GameManager : MonoSingleton<GameManager>
{
    public GameObject playerPrefab;
    public GameObject workerPrefab;
    public GameObject firePrefab;
    public GameObject fighterPrefab;
    public GameObject enemyPrefab;
    public GameContext context = new GameContext();
    public LayerMask buildMask;
    public LayerMask commandMask;


    private void Start()
    {
        WorldManager.Instance.GenerateGrid();
        GridTile spawnTile = FindSpawnTile();

        SpawnPlayer(spawnTile);
        context.resourceSystem = new ResourceSystem();
        context.resourceSystem.Init();
        context.buildingSystem = new BuildingSystem(context);
        context.commandSystem = new CommandSystem(context.player);
        context.inputManager.Init();
        context.interactionController.Init(context);
        context.interactionController.SetLayerMasks(buildMask, commandMask);
        context.userInterface.Init(context);
        context.inputManager.EnableGameplayInput();
        context.cameraController.Init(context);
        SpawnFire(spawnTile);
        SpawnWorker(spawnTile);
        SpawnFighter(spawnTile);
        WorldManager.Instance.HandleChunks(spawnTile.chunk);
    }

    private void SpawnFire(GridTile tile)
    {
        GameObject fireObj = Instantiate(firePrefab, tile.worldPosition, Quaternion.identity);
        FireBuilding fireBuilding = fireObj.GetComponent<FireBuilding>();
        tile.Build(fireBuilding);
    }

    private void SpawnPlayer(GridTile tile)
    {
        GameObject playerObj = Instantiate(playerPrefab, tile.Center() + new Vector3(1f, 0.5f, 0), Quaternion.identity);
        context.player = playerObj.GetComponent<Player>();
    }

    private void SpawnWorker(GridTile tile)
    {
        GameObject workerObj = Instantiate(workerPrefab, tile.Center() + new Vector3(1f, 0.5f, 1f), Quaternion.identity);
        WorkerUnit worker = workerObj.GetComponent<WorkerUnit>();
        worker.Init(context);
    }

    private void SpawnFighter(GridTile tile)
    {
        Instantiate(fighterPrefab, tile.Center() + new Vector3(-1f, 0.5f, -1f), Quaternion.identity);
        Instantiate(enemyPrefab,  tile.Center() + new Vector3(-1f, 0.5f, 0f), Quaternion.identity);
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
