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
    World World => context.world;
    PathfindingHandler Pathfinding => context.pathfinding;
    ChunkStreaming ChunkStreaming => context.chunkStreaming;
    InputManager InputManager => context.inputManager;
    InteractionController InteractionController => context.interactionController;
    CameraController CameraController => context.cameraController;
    UserInterface UserInterface => context.userInterface;
    TileMarker TileMarker => context.tileMarker;
    ResourceSystem ResourceSystem => context.resourceSystem;

    private void Start()
    {
        World.Generate(context);
        GridTile spawnTile = FindSpawnTile();
        context.fireSystem = new FireSystem();

        SpawnPlayer(spawnTile);
        Pathfinding.Init();
        TileMarker.Init(context);
        CreateSystems();
        ResourceSystem.Init(context);
        InputManager.Init();
        InteractionController.Init(context);
        UserInterface.Init(context);
        InputManager.EnableGameplayInput();
        CameraController.Init(context);
        SpawnFire(spawnTile);
        SpawnWorker(spawnTile);
        SpawnFighter(spawnTile);
        SpawnEnemy(spawnTile);
        ChunkStreaming.Init(context.world);
        World.InitStartChunks();
    }

    // Creates the game systems
    private void CreateSystems()
    {
        context.followerSystem = new FollowerSystem();
        context.enemySystem = new EnemySystem();
        context.resourceSystem = new ResourceSystem();
        context.buildingSystem = new BuildingSystem(context);
        context.commandSystem = new CommandSystem(context);
    }

    // Spawns the initial fire
    private void SpawnFire(GridTile tile)
    {
        GameObject fireObj = Instantiate(firePrefab, tile.worldPosition, Quaternion.identity);
        FireBuilding fireBuilding = fireObj.GetComponent<FireBuilding>();
        fireBuilding.Init(context.fireSystem);
        tile.Build(fireBuilding);
    }

    // Spawns the player
    private void SpawnPlayer(GridTile tile)
    {
        GameObject playerObj = Instantiate(playerPrefab, tile.Center() + new Vector3(1f, 0.5f, 0), Quaternion.identity);
        context.player = playerObj.GetComponent<Player>();
        context.player.Init(context);
    }

    // Spawns the initial worker
    private void SpawnWorker(GridTile tile)
    {
        GameObject workerObj = Instantiate(workerPrefab, tile.Center() + new Vector3(1f, 0.5f, 1f), Quaternion.identity);
        WorkerUnit worker = workerObj.GetComponent<WorkerUnit>();
        worker.Init(context);
    }

    // Spawns the initial fighter
    private void SpawnFighter(GridTile tile)
    {
        GameObject fighterObj = Instantiate(fighterPrefab, tile.Center() + new Vector3(-1f, 0.5f, -1f), Quaternion.identity);
        FighterUnit fighter = fighterObj.GetComponent<FighterUnit>();
        fighter.Init(context);
    }

    // Spawns the initial enemy
    private void SpawnEnemy(GridTile tile)
    {
        GameObject enemyObj = Instantiate(enemyPrefab, tile.Center() + new Vector3(-1f, 0.5f, 0f), Quaternion.identity);
        EnemyUnit enemy = enemyObj.GetComponent<EnemyUnit>();
        enemy.Init(context);
    }

    // Finds an appropriate start tile
    GridTile FindSpawnTile()
    {
        // TODO: handle edge-case situation where no spawn tile was found
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
