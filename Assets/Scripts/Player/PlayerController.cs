using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Damageable
{
    public override TargetType Type => TargetType.Player;
    [SerializeField] UnitSprite sprite = new UnitSprite();
    [SerializeField] Fire fire = new Fire(false);
    public float fireLightDist = 2f, fireCheckInterval = .5f;
    public float moveSpeed = 5f;
    private float fireCheckTimer = 0f;
    public Light fireLight;
    private Camera cam;
    private Chunk chunk;
    public SphereCollider col;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        col = GetComponent<SphereCollider>();
        // Face the camera
        Vector3 forward = Camera.main.transform.forward;
        forward.Normalize();
        transform.rotation = Quaternion.LookRotation(forward);
        health.Fill();

        fire.Init(fireLight);
    }

    void Update()
    {
        UpdateChunk();

        if (fireCheckTimer <= 0)
        {
            CheckFires();
        }
        else
        {
            fireCheckTimer -= Time.deltaTime;
        }
        fire.Update();
    }



    FireBuilding FindFire()
    {
        fireCheckTimer = fireCheckInterval;

        List<FireBuilding> fires = FireHandler.Instance.fireBuildings;
        FireBuilding closest = null;
        float closestDist = float.MaxValue;

        foreach (FireBuilding fire in fires)
        {
            float dist = Vector3.Distance(transform.position, fire.transform.position);
            if (dist < closestDist && dist < fireLightDist)
            {
                closestDist = dist;
                closest = fire;
            }
        }

        return closest;
    }

    void CheckFires()
    {
        FireBuilding fireBuilding = FindFire();

        if (fireBuilding != null)
        {
            fireBuilding.Light(fire);
        }
    }

    void UpdateChunk()
    {
        Chunk newChunk = WorldManager.grid.ChunkFromGridPos(GridPos());

        if (newChunk != chunk)
        {
            if (chunk != null) chunk.RemovePlayer();
            newChunk.AddPlayer(this);
            chunk = newChunk;

            // Update chunks (disable stale chunks and enable/create active chunks)
            WorldManager.Instance.HandleChunks(chunk);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        FollowerUnit unit = other.GetComponent<FollowerUnit>();

        UnitSystem.Instance.Storage.AddNearby(unit);
    }

    private void OnTriggerExit(Collider other)
    {
        FollowerUnit unit = other.GetComponent<FollowerUnit>();

        UnitSystem.Instance.Storage.RemoveNearby(unit);
    }

    public void Move(Vector2 moveInput)
    {
        sprite.SetDirection(moveInput);

        Vector3 move = CameraRelativeMove(moveInput, Camera.main.transform);

        Vector3 targetPos = transform.position + move * moveSpeed * Time.deltaTime;

        GridTile tile = WorldManager.grid.GetTile(targetPos);

        // Only move if tile is walkable
        if (tile.Walkable())
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
