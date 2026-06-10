using UnityEngine;

[System.Serializable]
public class GameContext
{
    public enum GameType
    {
        Game,
        CombatPlayground
    }
    public GameType gameType = GameType.Game;

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
    public UnitSystem unitSystem;
    public FireSystem fireSystem;
    public InputManager inputManager;
    public ChunkStreaming chunkStreaming;
    public GameManager gameManager;

    public DayNightSystem dayNightSystem;
    public DayNightHandler dayNightHandler;
    public GridTile spawnTile;

    public AgentCatalog agentCatalog;

    [Header("Building System")]
    public BuildingSystem buildingSystem;
    public BuildingSpawner buildingSpawner;
    public BuildingCatalog buildingCatalog;

    [Header("Enemy System")]
    public EnemySystem enemySystem;
    public EnemyCatalog enemyCatalog;

    public LayerMask buildMask;
    public LayerMask commandMask;

}
