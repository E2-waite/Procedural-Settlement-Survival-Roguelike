using UnityEngine;
using Cinderwild.Core.Data;
using Cinderwild.WorldBuilder.Runtime;

namespace Cinderwild.Gameplay.Agents
{
    [RequireComponent(typeof(PlayerController), typeof(AgentSpriteController))]
    public class Player : Destructable
    {
        public PlayerController Controller { get; private set; }
        public AgentSpriteController SpriteController { get; private set; }

        public void Init(Context context)
        {
            Chunk chunk = WorldManager.GetChunk(transform.position);
            WorldManager.StreamChunks(chunk);

            Controller = GetComponent<PlayerController>();
            Controller.Init();

            SpriteController = GetComponent<AgentSpriteController>();
            SpriteController?.Init(context);

            health.Fill();
        }

        private void Update()
        {
            Chunk chunk = WorldManager.GetChunk(transform.position);
            WorldManager.StreamChunks(chunk);
            if (Controller != null)
                SpriteController?.UpdateDirection(Controller.Facing);
        }

        private void LateUpdate()
        {
            Camera cam = Camera.main;

            if (cam == null) return;

            Vector3 direction = cam.transform.position - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                transform.rotation =
                    Quaternion.LookRotation(direction) *
                    Quaternion.Euler(0f, 180f, 0f);
            }
        }
    }
}
