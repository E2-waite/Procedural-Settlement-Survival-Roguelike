using UnityEngine;

[System.Serializable]
public class GameContext
{
    [HideInInspector] public Player player;
    public World world;
    public InteractionController interactionController;
    public CameraController cameraController;
    public UserInterface userInterface;
    public BuildingSpawner buildingSpawner;
    public BuildingCatalog buildingCatalog;
    public ResourceDefs resourceDefs;
    public TileMarker tileMarker;
    public ResourcesPanel resourcePanel;
    public CommandPanel commandPanel;
    public BuildingSystem buildingSystem;
    public CommandSystem commandSystem;
    public ResourceSystem resourceSystem;
    public FireSystem fireSystem;
    public InputManager inputManager;

    public LayerMask buildMask;
    public LayerMask commandMask;

}
