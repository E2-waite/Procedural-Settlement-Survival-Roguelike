using UnityEngine;

namespace Cinderwild.Gameplay.Agents
{
    // Class for handling player character movement
    public class PlayerController : MonoBehaviour
    {
        public float moveSpeed = 5f;
        private Vector3 facing = Vector3.zero;
        public Vector3 Facing => facing;

        public void MovePlayer(Vector2 moveInput)
        {
            Vector3 move = CameraRelativeMove(moveInput, Camera.main.transform);
            if (move.magnitude > 0.1f)
            {
                facing = move.normalized;
            }

            Vector3 targetPos = transform.position + move * moveSpeed * Time.deltaTime;

            //GridTile tile = grid.GetTile(targetPos);

            //// Only move if tile is walkable
            //if (tile != null && tile.IsEmpty)
            //{
                transform.position = targetPos;
            //}
        }

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