using System.Collections.Generic;
using UnityEngine;

public class GameBootstrapper : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject unitPrefab;
    public GameObject firePrefab;
    //public GameObject fighterPrefab;
    public GameObject enemyPrefab;
    public GameObject enemySettlementPrefab;
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
    EnemySystem EnemySystem => context.enemySystem;
    DayNightSystem DayNightSystem => context.dayNightSystem;

    GameManager Manager => context.gameManager;

    private void Start()
    {
        World.Generate(context);
        context.spawnTile = FindSpawnTile();

        if (context.spawnTile == null) Debug.LogWarning("No valid spawn tile could be found");

        context.fireSystem = new FireSystem();

        SpawnPlayer(context.spawnTile);
        Pathfinding.Init();
        TileMarker.Init(context);
        CreateSystems();
        ResourceSystem.Init(context);
        InputManager.Init();
        InteractionController.Init(context);
        UserInterface.Init(context);
        InputManager.EnableGameplayInput();
        CameraController.Init(context);
        SpawnFire(context.spawnTile);
        SpawnUnit(context.spawnTile);
        SpawnEnemy(context.spawnTile);
        //SpawnFighter(context.spawnTile);
        ChunkStreaming.Init(context.world);
        World.InitStartChunks();
        Manager.Init(context);
        EnemySystem.Init(context);
        DayNightSystem.Init(context);
        //SpawnEnemySettlement(FindSettlementTile());

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

    // Spawns the initial fire
    private void SpawnFire(GridTile tile)
    {
        GameObject fireObj = Instantiate(firePrefab, tile.worldPosition, Quaternion.identity);
        MainFireBuilding fireBuilding = fireObj.GetComponent<MainFireBuilding>();
        fireBuilding.Init(context);
        tile.Build(fireBuilding);
        fireBuilding.AddTile(tile);
        context.mainFireBuilding = fireBuilding;
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

    // Spawns the initial enemy
    private void SpawnEnemy(GridTile tile)
    {
        GameObject enemyObj = Instantiate(enemyPrefab, tile.Center + new Vector3(-1f, 0.5f, 0f), Quaternion.identity);
        Enemy enemy = enemyObj.GetComponent<Enemy>();
        enemy.Init(context, null, tile);
    }

    //private void SpawnEnemySettlement(GridTile tile)
    //{
    //    GameObject settlementObj = Instantiate(enemySettlementPrefab, tile.worldPosition, Quaternion.identity);
    //    EnemySettlement settlement = settlementObj.GetComponent<EnemySettlement>();
    //    settlement.Init(context);
    //    settlement.AddTile(tile);
    //    tile.Build(settlement);
    //}

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
                if (neighbourTile == null || !neighbourTile.IsEmpty)
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

    GridTile FindSettlementTile()
    {
        // TODO: handle edge-case situation where no spawn tile was found
        List<GridTile> validTiles = new List<GridTile>();
        World world = context.world;
        WorldGrid grid = world.Context.grid;

        // Spawn on a tile with all neighbouring cells empty so the starting area is usable.
        foreach (GridTile tile in grid.Tiles())
        {
            bool valid = true;

            float dist = Vector2Int.Distance(tile.position, context.spawnTile.position);
            if (dist < 25 || dist > 60) continue;

            List<GridTile> neighbourTiles = new List<GridTile>();

            neighbourTiles.Add(tile);

            foreach (Vector2Int neighbourPos in Consts.ALL_NEIGHBOURS)
            {
                GridTile neighbourTile = grid.GetTile(tile.position + neighbourPos);
                if (neighbourTile == null || !neighbourTile.IsEmpty)
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
