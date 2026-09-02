using Cinderwild.Command.UI;
using Cinderwild.Gameplay.Agents;
using Cinderwild.Pathfinding.Runtime;
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
        [SerializeField] private World.Runtime.WorldManager world;
        [SerializeField] private InputManager input;
        [SerializeField] private Player player;
        [SerializeField] private CameraController camera;


        public World.Runtime.WorldManager World => world;
        public InputManager Input => input;
        public Player Player => player;
        public CameraController Camera => camera;

        [Header("User Interface")]
        [SerializeField] private CommandWidget commandWidget;
        public CommandWidget CommandWidget => commandWidget;


        [Header("Managers")]
        [SerializeField] private PathfindingManager pathfinding;
        public PathfindingManager Pathfinding => pathfinding;


    }
}
