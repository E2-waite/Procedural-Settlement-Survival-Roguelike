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
        private WorldSystem world;

        public void Init(WorldSystem world, CameraController camera)
        {
            this.world = world;
            Chunk chunk = world.GetChunk(transform.position);
            world.StreamChunks(chunk);

            Controller = GetComponent<PlayerController>();
            Controller.Init(world);

            spriteController = GetComponent<AgentSprites>();
            spriteController?.Init(camera);

            health.Fill();
        }

        private void Update()
        {
            Chunk chunk = world.GetChunk(transform.position);
            world.StreamChunks(chunk);

            // TODO: remove update from player
            if (Controller != null)
                spriteController?.UpdateDirection(Controller.Facing);
        }
    }
}
