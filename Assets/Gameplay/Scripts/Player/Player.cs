using UnityEngine;
using Cinderwild.World.Runtime;

namespace Cinderwild.Gameplay.Agents
{
    [RequireComponent(typeof(PlayerController), typeof(AgentSprites))]
    public class Player : Destructable
    {
        public PlayerController Controller { get; private set; }
        public Transform Body => body;
        private AgentSprites spriteController;
        private WorldSystem world;
        [SerializeField] private Transform body;

        public void Init(WorldSystem world, CameraController camera)
        {
            this.world = world;
            Chunk chunk = world.GetChunk(transform.position);
            world.StreamChunks(chunk);

            Controller = GetComponent<PlayerController>();
            Controller.Init(world, this);

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
