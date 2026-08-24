using UnityEngine;
using Cinderwild.Core.Input;
using Cinderwild.WorldBuilder.Runtime;
using Cinderwild.Gameplay.Agents;
using Cinderwild.Pathfinding.Runtime;

namespace Cinderwild.Core.Data
{
    [System.Serializable]
    public class Context
    {
        // Monobehaviours
        [SerializeField] private WorldBuilder.Runtime.WorldBuilder world;
        [SerializeField] private InputManager input;
        [SerializeField] private InteractionController interaction;
        [SerializeField] private Player player;
        [SerializeField] private CameraController camera;
        [SerializeField] private PathfindingManager pathfinding;
        public WorldBuilder.Runtime.WorldBuilder World => world;
        public InputManager Input => input;
        public InteractionController Interaction => interaction;
        public Player Player => player;
        public CameraController Camera => camera;
        public PathfindingManager Pathfinding => pathfinding;
    }
}
