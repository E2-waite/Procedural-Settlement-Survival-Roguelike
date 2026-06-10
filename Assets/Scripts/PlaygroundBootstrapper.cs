using UnityEngine;

public class PlaygroundBootstrapper : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject unitSpawnerPrefab;
    public GameObject unitPrefab;

    [SerializeField] private GameContext context = new GameContext();
    World World => context.world;
    PathfindingHandler Pathfinding => context.pathfinding;
    ChunkStreaming ChunkStreaming => context.chunkStreaming;
    InputManager InputManager => context.inputManager;
    InteractionController InteractionController => context.interactionController;
    CameraController CameraController => context.cameraController;
    UserInterface UserInterface => context.userInterface;
    TileMarker TileMarker => context.tileMarker;

    private void Start()
    {
        World.GenerateEmpty(context);

        GridTile spawnTile = GetSpawnTile();
        SpawnPlayer(spawnTile);
        Pathfinding.Init();
        TileMarker.Init(context);
        CreateSystems();
        InputManager.Init();
        InteractionController.Init(context);
        UserInterface.Init(context);
        InputManager.EnableGameplayInput();
        CameraController.Init(context);
        ChunkStreaming.Init(context.world);
        World.InitStartChunks();

        SpawnUnit(context.spawnTile);
        SpawnUnitSpawner(context.spawnTile);



        // Disable this GameObject when finished init
        gameObject.SetActive(false);
    }

    // Creates the game systems
    private void CreateSystems()
    {
        context.unitSystem = new UnitSystem();
        context.enemySystem = new EnemySystem();
        context.resourceSystem = new ResourceSystem();
        context.buildingSystem = new BuildingSystem(context);
        context.commandSystem = new CommandSystem(context);
        context.dayNightSystem = new DayNightSystem();
    }

    // Spawns the player
    private void SpawnPlayer(GridTile tile)
    {
        GameObject playerObj = Instantiate(playerPrefab, tile.Center + new Vector3(1f, 0.5f, 0), Quaternion.identity);
        context.player = playerObj.GetComponent<Player>();
        context.player.Init(context);
    }

    // Spawns the initial unit
    private void SpawnUnit(GridTile tile)
    {
        GameObject unitObj = Instantiate(unitPrefab, tile.Center + new Vector3(1f, 0.5f, 1f), Quaternion.identity);
        Unit unit = unitObj.GetComponent<Unit>();
        unit.Init(context);
    }

    private void SpawnUnitSpawner(GridTile tile)
    {
        GameObject settlementObj = Instantiate(unitSpawnerPrefab, tile.worldPosition, Quaternion.identity);
        UnitSpawnerBuilding settlement = settlementObj.GetComponent<UnitSpawnerBuilding>();
        settlement.Init(context);
        settlement.AddTile(tile);
        tile.Build(settlement);
    }

    private GridTile GetSpawnTile()
    {
        World world = context.world;
        WorldGrid grid = world.Context.grid;

        return grid.GetTile(new Vector2Int(0,0));
    }
}
