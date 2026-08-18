using UnityEngine;
using Cinderwild.Core.Data;
using Cinderwild.WorldBuilder.Runtime;

namespace Cinderwild.Gameplay.Agents
{
    [RequireComponent(typeof(PlayerController))]
    public class Player : Destructable
    {
        public PlayerController Controller { get; private set; }

        public void Init(Context context)
        {
            Controller = GetComponent<PlayerController>();
            health.Fill();
        }

        private void Update()
        {
            Chunk chunk = WorldManager.GetChunk(transform.position);
            WorldManager.StreamChunks(chunk);
        }
    }
}
