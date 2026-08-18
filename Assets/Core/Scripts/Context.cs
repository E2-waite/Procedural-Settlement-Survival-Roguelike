using UnityEngine;
using Cinderwild.Core.Input;
using Cinderwild.WorldBuilder.Runtime;
using Cinderwild.Gameplay.Agents;

namespace Cinderwild.Core.Data
{
    [System.Serializable]
    public class Context
    {
        // Monobehaviours
        [SerializeField] private World world;
        [SerializeField] private InputManager input;
        [SerializeField] private InteractionController interaction;
        [SerializeField] private Player player;
        [SerializeField] private CameraController camera;

        public World World => world;
        public InputManager Input => input;
        public InteractionController Interaction => interaction;
        public Player Player => player;
        public CameraController Camera => camera;
    }
}
