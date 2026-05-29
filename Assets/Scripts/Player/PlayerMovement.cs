using UnityEngine;

public class PlayerMovement
{
    Animator anim;
    public float moveSpeed = 5f;
    [HideInInspector]public PlayerController player;
    private PlayerControls controls;
    public Vector2 moveInput;

    public void EnableControls(bool enable)
    {
        if (enable)
            controls.Enable();
        else
            controls.Disable();
    }

    public void Init(PlayerController player)
    {
        this.player = player;

        controls = new PlayerControls();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        anim = player.GetComponent<Animator>();
    }

    public void Update()
    {
        Vector3 move = CameraRelativeMove(moveInput, Camera.main.transform);

        Vector3 targetPos = player.transform.position + move * moveSpeed * Time.deltaTime;

        GridTile tile = WorldHandler.grid.GetTile(targetPos);

        // Only move if tile is walkable
        if (tile.Walkable())
        {
            player.transform.position = targetPos;
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
