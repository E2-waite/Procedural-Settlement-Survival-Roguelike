using UnityEngine;
using Cinderwild.Core.Input;
using Cinderwild.Core.Interaction;
using Cinderwild.Gameplay.Agents;
using Cinderwild.Pathfinding.Runtime;

// Expect to use most namespaces here

namespace Cinderwild.Core.Data
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
        [SerializeField] private InteractionController interaction;
        [SerializeField] private Player player;
        [SerializeField] private CameraController camera;
        [SerializeField] private PathfindingManager pathfinding;
        public World.Runtime.WorldManager World => world;
        public InputManager Input => input;
        public InteractionController Interaction => interaction;
        public Player Player => player;
        public CameraController Camera => camera;
        public PathfindingManager Pathfinding => pathfinding;
    }
}
