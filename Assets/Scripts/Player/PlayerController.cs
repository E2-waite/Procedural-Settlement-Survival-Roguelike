using System.Collections.Generic;
using UnityEditor;
using UnityEditor.ShaderGraph;
using UnityEngine;
using static InteractionManager;
using UnityEngine.InputSystem;
using System.Linq;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviour
{
    PlayerControls controls;
    private Camera cam;
    private Chunk chunk;

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
    Vector2Int gridPos = new Vector2Int();

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

    // Converts world position to grid position
    public Vector2Int GridPos()
    {
        return new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.z));
    }

    void Update()
    {
        UpdateChunk();
        MovePlayer();

        if (Keyboard.current.fKey.wasReleasedThisFrame)
        {
            for (int i = 0; i < nearbyUnits.Count; i++)
            {
                nearbyUnits[i].StartFollowing(this);
            }

            UnitHandler.Instance.SetFollowing(nearbyUnits);
        }
    }

    void UpdateChunk()
    {
        Chunk newChunk = WorldHandler.grid.ChunkFromGridPos(GridPos());

        if (newChunk != chunk)
        {
            if (chunk != null) chunk.RemovePlayer();
            newChunk.AddPlayer(this);
            chunk = newChunk;

            // Update chunks (disable stale chunks and enable/create active chunks)
            WorldHandler.Instance.HandleChunks(chunk);
        }
    }

    void MovePlayer()
    {
        Vector3 move = CameraRelativeMove(moveInput, Camera.main.transform);

        Vector3 targetPos = transform.position + move * moveSpeed * Time.deltaTime;

        GridTile tile = WorldHandler.grid.GetTile(targetPos);

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
