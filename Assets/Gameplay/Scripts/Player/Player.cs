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
            Chunk chunk = WorldManager.GetChunk(transform.position);
            WorldManager.StreamChunks(chunk);

            Controller = GetComponent<PlayerController>();
            Controller.Init();
            health.Fill();
        }

        private void Update()
        {
            Chunk chunk = WorldManager.GetChunk(transform.position);
            WorldManager.StreamChunks(chunk);
        }

        private void LateUpdate()
        {
            Camera cam = Camera.main;

            if (cam == null)
                return;

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
