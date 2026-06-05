using UnityEngine;

[System.Serializable]
public class GameContext
{
    [HideInInspector] public Player player;
    public InteractionController interactionController;
    public CameraController cameraController;
    public UserInterface userInterface;
    public BuildingSpawner buildingSpawner;
    public BuildingCatalog buildingCatalog;
    public ResourceDefs resourceDefs;
    public TileMarker tileMarker;
    public CommandPanel commandPanel;
    public BuildingSystem buildingSystem;
    public CommandSystem commandSystem;
    public ResourceSystem resourceSystem;
    public InputManager inputManager;
}
