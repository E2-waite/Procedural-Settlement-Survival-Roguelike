using UnityEngine;
using Shadowvale.World.Runtime;
using Shadowvale.World.Data;

namespace Shadowvale.Gameplay.Agents
{
    [RequireComponent(typeof(PlayerController), typeof(AgentSprites))]
    public class Player : Destructable
    {
        public PlayerController Controller { get; private set; }
        public Transform Body => body;
        private AgentSprites spriteController;
        private WorldManager world;
        [SerializeField] private Transform body;

        public void Init(WorldManager world, CameraController camera)
        {
            this.world = world;

            TileData tile = world.Data.GetTile(body.position);
            ChunkData chunk = tile?.Chunk;
            world.System.StreamChunks(chunk);

            Controller = GetComponent<PlayerController>();
            Controller.Init(world, this);

            spriteController = GetComponent<AgentSprites>();
            spriteController?.Init(camera);

            health.Fill();
        }

        private void Update()
        {
            TileData tile = world.Data.GetTile(body.position);
            ChunkData chunk = tile?.Chunk;

            world.System.StreamChunks(chunk);

            // TODO: remove update from player
            if (Controller != null)
                spriteController?.UpdateDirection(Controller.Facing);
        }
    }
}
