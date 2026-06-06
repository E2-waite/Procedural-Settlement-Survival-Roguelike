using System.Collections.Generic;
using UnityEngine;

public class Player : Damageable
{
    PlayerController controller;
    public Light fireLight;
    public override TargetType Type => TargetType.Player;
    [SerializeField] UnitSprite sprite = new UnitSprite();
    [SerializeField] Fire fire = new Fire(false);
    private List<FollowerUnit> nearbyUnits = new List<FollowerUnit>();
    public List<FollowerUnit> NearbyUnits => nearbyUnits;
    public float fireLightDist = 2f, fireCheckInterval = .5f;
    private float fireCheckTimer = 0f;
    private Chunk chunk;
    public SphereCollider col;
    private World world;
    private WorldGrid grid;

    public void Init(GameContext context)
    {
        world = context.world;
        grid = world.Context.grid;

        controller = GetComponent<PlayerController>();
        controller.Init(context);

        fire.Init(fireLight);
        health.Fill();
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
        Chunk newChunk = grid.ChunkFromGridPos(GridPos());

        if (newChunk != chunk)
        {
            if (chunk != null) chunk.RemovePlayer();
            newChunk.AddPlayer(this);
            chunk = newChunk;

            // Update chunks (disable stale chunks and enable/create active chunks)
            world.HandleChunks(chunk);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        FollowerUnit unit = other.GetComponent<FollowerUnit>();

        if (unit != null && !nearbyUnits.Contains(unit))
        {
            unit.SetNearby();
            nearbyUnits.Add(unit);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        FollowerUnit unit = other.GetComponent<FollowerUnit>();

        if (unit != null && nearbyUnits.Contains(unit))
        {
            unit.ClearNearby();
            nearbyUnits.Remove(unit);
        }
    }

    public void OnMove(Vector2 moveInput)
    {
        if (controller != null)
        {
            controller.MovePlayer(moveInput);
        }

        //if (sprite != null)
        //{
        //    sprite.SetDirection(moveInput);
        //}
    }
}
