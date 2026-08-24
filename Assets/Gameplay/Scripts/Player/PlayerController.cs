using UnityEngine;
using Cinderwild.WorldBuilder.Data;
using Cinderwild.WorldBuilder.Runtime;

namespace Cinderwild.Gameplay.Agents
{
    // Class for handling player character movement
    public class PlayerController : MonoBehaviour
    {
        public float moveSpeed = 5f;
        private Vector3 facing = Vector3.zero;
        public Vector3 Facing => facing;
        public bool Rotating { get; private set; }

        public void Init()
        {
            TileData tile = WorldSystem.GetTile(transform.position);
            if (tile != null && tile.IsWalkable)
            {
                transform.position += new Vector3(0, tile.WorldPosition.y, 0);
            }
        }

        public void MovePlayer(Vector2 moveInput)
        {
            Vector3 move = CameraRelativeMove(moveInput, Camera.main.transform);
            if (move.magnitude > 0.1f)
            {
                facing = move.normalized;
            }

            Vector3 targetPos = transform.position + move * moveSpeed * Time.deltaTime;

            TileData tile = WorldSystem.GetTile(targetPos);

            // Only move if tile is walkable
            if (tile != null && tile.IsWalkable)
            {
                targetPos.y = tile.WorldPosition.y;
                transform.position = targetPos;
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