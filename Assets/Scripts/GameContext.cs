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
    public BuildingSpawner buildingSpawner;
    public BuildingCatalog buildingCatalog;
    public TileMarker tileMarker;
    public ResourcesPanel resourcePanel;
    public CommandPanel commandPanel;
    public BuildingSystem buildingSystem;
    public CommandSystem commandSystem;
    public ResourceSystem resourceSystem;
    public FollowerSystem followerSystem;
    public EnemySystem enemySystem;
    public FireSystem fireSystem;
    public InputManager inputManager;

    public LayerMask buildMask;
    public LayerMask commandMask;

}
