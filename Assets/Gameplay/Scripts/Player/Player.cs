using UnityEngine;
using Cinderwild.Core.Data;
using Cinderwild.World.Runtime;
using Cinderwild.SpriteSytem.Runtime;

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
            spriteController?.Init();

            health.Fill();
        }

        private void Update()
        {
            Chunk chunk = WorldSystem.GetChunk(transform.position);
            WorldSystem.StreamChunks(chunk);

            if (Controller != null)
                spriteController?.UpdateDirection(Controller.Facing);
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
