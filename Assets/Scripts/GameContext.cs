using UnityEngine;

[System.Serializable]
public class GameContext
{
    [HideInInspector] public Player player;
    public World world;
    public InteractionController interactionController;
    public CameraController cameraController;
    public UserInterface userInterface;
    public PathfindingHandler pathfinding;

    public TileMarker tileMarker;
    public MainFireBuilding mainFireBuilding;
    public ResourcesPanel resourcePanel;
    public CommandPanel commandPanel;
    public CommandSystem commandSystem;
    public ResourceSystem resourceSystem;
    public FollowerSystem followerSystem;
    public FireSystem fireSystem;
    public InputManager inputManager;
    public ChunkStreaming chunkStreaming;
    public GameManager gameManager;

    public DayNightSystem dayNightSystem;
    public DayNightHandler dayNightHandler;
    public GridTile spawnTile;

    [Header("Building System")]
    public BuildingSystem buildingSystem;
    public BuildingSpawner buildingSpawner;
    public BuildingCatalog buildingCatalog;

    [Header("Enemy System")]
    public EnemySystem enemySystem;
    public EnemySpawner enemySpawner;
    public EnemyCatalog enemyCatalog;

    public LayerMask buildMask;
    public LayerMask commandMask;

}
