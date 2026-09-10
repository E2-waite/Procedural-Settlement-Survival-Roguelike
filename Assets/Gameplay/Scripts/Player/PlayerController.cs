using UnityEngine;
using Cinderwild.World.Data;
using Cinderwild.World.Runtime;

namespace Cinderwild.Gameplay.Agents
{
    // Class for handling player character movement
    public class PlayerController : MonoBehaviour
    {
        private Player player;
        public float moveSpeed = 5f;
        private Vector3 facing = Vector3.zero;
        public Vector3 Facing => facing;
        public bool Rotating { get; private set; }
        private WorldManager world;

        public void Init(WorldManager world, Player player)
        {
            this.world = world;
            this.player = player;

            TileData tile = world.Data.GetTile(player.Body.position);
            if (tile != null && tile.IsWalkable)
            {
                player.Body.position += new Vector3(0, tile.WorldPosition.y, 0);
            }
        }

        public void MovePlayer(Vector2 moveInput)
        {
            Vector3 move = CameraRelativeMove(moveInput, Camera.main.transform);
            if (move.magnitude > 0.1f)
            {
                facing = move.normalized;
            }

            Vector3 targetPos = player.Body.position + move * moveSpeed * Time.deltaTime;

            TileData tile = world.Data.GetTile(targetPos);

            // Only move if tile is walkable
            if (tile != null && tile.IsWalkable)
            {
                targetPos.y = tile.WorldPosition.y;
                player.Body.position = targetPos;
            }
        }

        // Get movement vector relative to the camera rotation
        Vector3 CameraRelativeMove(Vector2 input, Transform cam)
        {
            Vector3 forward = cam.forward;
            Vector3 right = cam.right;

            // flatten so camera tilt doesn't affect movement
            forward.y = 0;
            right.y = 0;

            forward.Normalize();
            right.Normalize();

            return forward * input.y + right * input.x;
        }
    }
}