using Cinderwild.Command.Data;
using Cinderwild.Command.UI;
using Cinderwild.Gameplay.Agents;
using Cinderwild.Pathfinding.Runtime;
using Cinderwild.World.Runtime;
using UnityEngine;

// Expect to use most namespaces here

namespace Cinderwild.Core
{
    /// <summary>
    /// The Game Context Containing All Required Resources
    /// </summary>
    [System.Serializable]
    public class Context
    {
        // Monobehaviours
        [SerializeField] private WorldManager world;
        [SerializeField] private InputManager input;
        [SerializeField] private Player player;
        [SerializeField] private Agent agent;
        [SerializeField] private CameraController camera;
        [SerializeField] private CommandOptionCatalog commandCatalog;

        public WorldManager World => world;
        public InputManager Input => input;
        public Player Player => player;
        public Agent Agent => agent;
        public CameraController Camera => camera;
        public CommandOptionCatalog CommandCatalog => commandCatalog;

        [Header("User Interface")]
        [SerializeField] private CommandWidget commandWidget;
        public CommandWidget CommandWidget => commandWidget;


        [Header("Managers")]
        [SerializeField] private PathfindingManager pathfinding;
        public PathfindingManager Pathfinding => pathfinding;


    }
}
