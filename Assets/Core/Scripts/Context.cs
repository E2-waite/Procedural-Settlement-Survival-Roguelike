using UnityEngine;
using Cinderwild.Core.Input;
using Cinderwild.World.Runtime;
using Cinderwild.Gameplay.Agents;
using Cinderwild.Pathfinding.Runtime;

namespace Cinderwild.Core.Data
{
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
