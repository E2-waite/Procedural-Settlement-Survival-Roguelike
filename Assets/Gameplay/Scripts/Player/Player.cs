using UnityEngine;
using Cinderwild.Core.Data;
using Cinderwild.WorldBuilder.Runtime;
using Cinderwild.SpriteEditor.Data;
using Cinderwild.SpriteEditor.Runtime;

namespace Cinderwild.Gameplay.Agents
{
    [RequireComponent(typeof(PlayerController))]
    public class Player : Destructable
    {
        public PlayerController Controller { get; private set; }
        [SerializeField] private SpriteRig spriteRig;
        private Direction facing = Direction.North;
        private CameraController camera;

        public void Init(Context context)
        {
            Chunk chunk = WorldManager.GetChunk(transform.position);
            WorldManager.StreamChunks(chunk);

            camera = context.Camera;

            Controller = GetComponent<PlayerController>();
            Controller.Init();
            health.Fill();

            spriteRig?.Init();
        }

        private void Update()
        {
            Chunk chunk = WorldManager.GetChunk(transform.position);
            WorldManager.StreamChunks(chunk);

            if (spriteRig != null && camera != null)
            {
                facing = GetDirection(Controller.Facing);
                UpdateSpriteRig();
            }
        }

        private int lastCamIndex = -1, lastPlayerIntex = -1;

        public static Direction GetDirection(Vector3 forward)
        {
            if (forward.sqrMagnitude < 0.0001f)
                return Direction.Null;

            float angle = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
            angle = Mathf.Repeat(angle, 360f);

            int index = Mathf.RoundToInt(angle / 45f) % 8;

            return (Direction)index;
        }

        private void UpdateSpriteRig()
        {
            if (facing == Direction.Null ||
                camera.Facing == Direction.Null)
                return;

            int playerIndex = (int)facing;
            int cameraIndex = (int)camera.Facing;

            if (playerIndex == lastPlayerIntex && cameraIndex == lastCamIndex) return;

            lastPlayerIntex = playerIndex;
            lastCamIndex = cameraIndex;

            int relativeIndex = (playerIndex - cameraIndex) % 8;

            if (relativeIndex < 0)
                relativeIndex += 8;

            spriteRig.UpdateDirection((Direction)relativeIndex);
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
