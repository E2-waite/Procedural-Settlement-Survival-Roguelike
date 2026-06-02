using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Damageable
{
    public override TargetType Type => TargetType.Player;
    [SerializeField] UnitSprite sprite = new UnitSprite();
    [SerializeField] PlayerMovement movement = new PlayerMovement();
    [SerializeField] Fire fire = new Fire(false);
    public float fireLightDist = 2f, fireCheckInterval = .5f;
    private float fireCheckTimer = 0f;
    public Light fireLight;
    private Camera cam;
    private Chunk chunk;


    public SphereCollider col;
    private void Awake()
    {
        movement.Init(this);
    }

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

    void OnEnable() => movement.EnableControls(true);
    void OnDisable() => movement.EnableControls(false);

    void Update()
    {
        UpdateChunk();
        movement.Update();
        sprite.SetDirection(movement.moveInput);

        

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

        UnitSystem.Instance.AddNearby(unit);
    }

    private void OnTriggerExit(Collider other)
    {
        FollowerUnit unit = other.GetComponent<FollowerUnit>();

        UnitSystem.Instance.RemoveNearby(unit);
    }

    // Sets a follower to start following this player
    //private void StartFollowing(FollowerUnit follower)
    //{
    //    followingUnits.Add(follower);
    //    follower.StartFollowing(this);
    //}

    // Stops a follower following this player
    //public void StopFollowing(FollowerUnit follower)
    //{
    //    followingUnits.Remove(follower);

    //    if (Vector3.Distance(transform.position, follower.transform.position) <= col.radius)
    //    {
    //        nearbyUnits.Add(follower);
    //    }

    //    follower.StopFollowing();
    //}

}
