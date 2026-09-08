using UnityEngine;
using Cinderwild.Core;
using Cinderwild.World.Runtime;

namespace Cinderwild.Gameplay.Agents
{
    [RequireComponent(typeof(PlayerController), typeof(AgentSprites))]
    public class Player : Destructable
    {
        public PlayerController Controller { get; private set; }
        private AgentSprites spriteController;

        public void Init(Context context)
        {
            Chunk chunk = WorldSystem.GetChunk(transform.position);
            WorldSystem.StreamChunks(chunk);

            Controller = GetComponent<PlayerController>();
            Controller.Init();

            spriteController = GetComponent<AgentSprites>();
            spriteController?.Init(context.Camera);

            health.Fill();
        }

        private void Update()
        {
            Chunk chunk = WorldSystem.GetChunk(transform.position);
            WorldSystem.StreamChunks(chunk);

            // TODO: remove update from player
            if (Controller != null)
                spriteController?.UpdateDirection(Controller.Facing);
        }
    }
}
