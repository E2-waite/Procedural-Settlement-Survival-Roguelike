using UnityEngine;
using Cinderwild.WorldBuilder.Runtime;
using Cinderwild.WorldBuilder.Data;

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
    public FormationCatalog formationCatalog;
    public InteractionController interactionController;
    public CameraController cameraController;
    public UserInterface userInterface;
    public PathfindingHandler pathfinding;

    public TileMarker tileMarker;
    public MainFireBuilding mainFireBuilding;
    public ResourcesPanel resourcePanel;
    public ActionPanel actionPanel;
    public CommandSystem commandSystem;
    public ResourceSystem resourceSystem;
    public UnitSystem unitSystem;
    public FireSystem fireSystem;
    public InputManager inputManager;
    public GameManager gameManager;

    public DayNightSystem dayNightSystem;
    public DayNightHandler dayNightHandler;
    public TileData spawnTile;

    public AgentCatalog agentCatalog;

    [Header("Building System")]
    public BuildingSystem buildingSystem;
    public BuildingSpawner buildingSpawner;
    public BuildingCatalog buildingCatalog;

    [Header("Enemy System")]
    public EnemySystem enemySystem;
    public EnemyCatalog enemyCatalog;
    public EnemyManager enemyManager;
    public GameObject enemySettlement;

    [Header("Particle System")]
    public ParticleManager particleManager;

    [Header("Layer Masks")]
    public LayerMask buildMask;
    public LayerMask commandMask;

}
