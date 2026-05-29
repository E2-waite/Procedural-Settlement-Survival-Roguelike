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
    [SerializeField] UnitSprite sprite = new UnitSprite();
    [SerializeField] Health health = new Health();
    [SerializeField] PlayerMovement movement = new PlayerMovement();
    private Camera cam;
    private Chunk chunk;

    private List<FollowerUnit> nearbyUnits = new List<FollowerUnit>();
    private List<FollowerUnit> followingUnits = new List<FollowerUnit>();

    private void Awake()
    {
        movement.Init(this);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Face the camera
        Vector3 forward = Camera.main.transform.forward;
        forward.Normalize();
        transform.rotation = Quaternion.LookRotation(forward);
        health.Fill();
    }

    void OnEnable() => movement.EnableControls(true);
    void OnDisable() => movement.EnableControls(false);

    // Converts world position to grid position
    public Vector2Int GridPos()
    {
        return new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.z));
    }

    void Update()
    {
        UpdateChunk();
        movement.Update();
        sprite.SetDirection(movement.moveInput);

        if (Keyboard.current.fKey.wasReleasedThisFrame)
        {
            for (int i = 0; i < nearbyUnits.Count; i++)
            {
                if (nearbyUnits[i] == null)
                    nearbyUnits.RemoveAt(i--);
                else
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

    void LateUpdate()
    {
        // Face the camera
        Vector3 forward = Camera.main.transform.forward;
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
