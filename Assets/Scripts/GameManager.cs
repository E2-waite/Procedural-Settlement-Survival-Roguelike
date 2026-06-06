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
    [SerializeField] private GameContext context = new GameContext();


    private void Start()
    {
        context.world.GenerateGrid();
        GridTile spawnTile = FindSpawnTile();
        context.fireSystem = new FireSystem();

        SpawnPlayer(spawnTile);
        context.tileMarker.Init(context);
        context.resourceSystem = new ResourceSystem();
        context.resourceSystem.Init(context);
        context.buildingSystem = new BuildingSystem(context);
        context.commandSystem = new CommandSystem(context);
        context.inputManager.Init();
        context.interactionController.Init(context);
        context.userInterface.Init(context);
        context.inputManager.EnableGameplayInput();
        context.cameraController.Init(context);
        SpawnFire(spawnTile);
        SpawnWorker(spawnTile);
        SpawnFighter(spawnTile);
        SpawnEnemy(spawnTile);
        context.world.HandleChunks(spawnTile.chunk);
    }

    private void SpawnFire(GridTile tile)
    {
        GameObject fireObj = Instantiate(firePrefab, tile.worldPosition, Quaternion.identity);
        FireBuilding fireBuilding = fireObj.GetComponent<FireBuilding>();
        fireBuilding.Init(context.fireSystem);
        tile.Build(fireBuilding);
    }

    private void SpawnPlayer(GridTile tile)
    {
        GameObject playerObj = Instantiate(playerPrefab, tile.Center() + new Vector3(1f, 0.5f, 0), Quaternion.identity);
        context.player = playerObj.GetComponent<Player>();
        context.player.Init(context);
    }

    private void SpawnWorker(GridTile tile)
    {
        GameObject workerObj = Instantiate(workerPrefab, tile.Center() + new Vector3(1f, 0.5f, 1f), Quaternion.identity);
        WorkerUnit worker = workerObj.GetComponent<WorkerUnit>();
        worker.Init(context);
    }

    private void SpawnFighter(GridTile tile)
    {
        GameObject fighterObj = Instantiate(fighterPrefab, tile.Center() + new Vector3(-1f, 0.5f, -1f), Quaternion.identity);
        FighterUnit fighter = fighterObj.GetComponent<FighterUnit>();
        fighter.Init(context);
    }

    private void SpawnEnemy(GridTile tile)
    {
        GameObject enemyObj = Instantiate(enemyPrefab, tile.Center() + new Vector3(-1f, 0.5f, 0f), Quaternion.identity);
        EnemyUnit enemy = enemyObj.GetComponent<EnemyUnit>();
        enemy.Init(context);
    }

    GridTile FindSpawnTile()
    {
        List<GridTile> validTiles = new List<GridTile>();
        World world = context.world;
        WorldGrid grid = world.Context.grid;

        // Spawn on a tile with all neighbouring cells empty so the starting area is usable.
        foreach (GridTile tile in grid.Tiles())
        {
            bool valid = true;

            List<GridTile> neighbourTiles = new List<GridTile>();

            neighbourTiles.Add(tile);

            foreach (Vector2Int neighbourPos in Consts.ALL_NEIGHBOURS)
            {
                GridTile neighbourTile = grid.GetTile(tile.position + neighbourPos);
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
