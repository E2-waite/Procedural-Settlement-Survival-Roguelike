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
        [SerializeField] private Player player;
        [SerializeField] private InputManager input;
        [SerializeField] private InteractionController interaction;
        [SerializeField] private World world;


        public Player Player => player;
        public InputManager Input => input;
        public InteractionController Interaction => interaction;
        public World World => world;
    }
}
