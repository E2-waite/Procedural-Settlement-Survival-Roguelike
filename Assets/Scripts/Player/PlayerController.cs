using UnityEngine;

// Class for handling player character movement
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Camera cam;
    private WorldGrid grid;

    public void Init(GameContext context)
    {
        grid = context.world.Context.grid;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Face the camera
        Vector3 forward = Camera.main.transform.forward;
        forward.Normalize();
        transform.rotation = Quaternion.LookRotation(forward);
    }

    public void MovePlayer(Vector2 moveInput)
    {
        Vector3 move = CameraRelativeMove(moveInput, Camera.main.transform);

        Vector3 targetPos = transform.position + move * moveSpeed * Time.deltaTime;

        GridTile tile = grid.GetTile(targetPos);

        // Only move if tile is walkable
        if (tile != null && tile.IsEmpty)
        {
            transform.position = targetPos;
        }
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
