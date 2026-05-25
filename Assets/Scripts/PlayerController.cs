using System.Collections.Generic;
using UnityEditor;
using UnityEditor.ShaderGraph;
using UnityEngine;
using static InteractionManager;
using UnityEngine.InputSystem;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    PlayerControls controls;
    private Camera cam;

    public List<FollowerUnit> nearbyUnits = new List<FollowerUnit>();
    public List<FollowerUnit> followingUnits = new List<FollowerUnit>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Face the camera
        cam = Camera.main;
        Vector3 forward = cam.transform.forward;
        forward.Normalize();
        transform.rotation = Quaternion.LookRotation(forward);
    }

    public float moveSpeed = 5f;

    private Vector2 moveInput;

    void Awake()
    {
        controls = new PlayerControls();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

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

    void Update()
    {
        MovePlayer();

        // Player controller needs to know which chunk it's in so it can handle chunk visibility

        GridTile currentTile = Grid.Instance.getTile(transform.position);
        if (currentTile != null)
        {
            // TODO: stop doing this each update (likely expensive)
            Grid.Instance.HandleChunks(currentTile.chunk.position);
        }


        if (Keyboard.current.fKey.wasReleasedThisFrame)
        {
            for (int i = 0; i < nearbyUnits.Count; i++)
            {
                nearbyUnits[i].StartFollowing(this);
            }

            UnitHandler.Instance.SetFollowing(nearbyUnits);
        }
    }

    void MovePlayer()
    {
        Vector3 move = CameraRelativeMove(moveInput, Camera.main.transform);

        Vector3 targetPos = transform.position + move * moveSpeed * Time.deltaTime;

        GridTile tile = Grid.Instance.getTile(targetPos);

        // Only move if tile is walkable
        if (tile.Walkable())
        {
            transform.position = targetPos;
        }
    }

    void LateUpdate()
    {
        // Face the camera
        Vector3 forward = cam.transform.forward;
        forward.Normalize();
        transform.rotation = Quaternion.LookRotation(forward);
    }

    private void OnTriggerEnter(Collider other)
    {
        FollowerUnit unit = other.GetComponent<FollowerUnit>();

        if (unit != null && !nearbyUnits.Contains(unit))
        {
            nearbyUnits.Add(unit);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        FollowerUnit unit = other.GetComponent<FollowerUnit>();

        if (unit != null)
        {
            nearbyUnits.Remove(unit);
        }
    }
}
