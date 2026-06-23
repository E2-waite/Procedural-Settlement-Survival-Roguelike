using System.Collections.Generic;
using UnityEngine;
using static GameContext;
using static UnityEngine.Rendering.DebugUI.Table;
public class Player : Destructable
{
    PlayerController controller;
    public Light fireLight;
    public override TargetType Type => TargetType.Player;
    public AgentObject agentObject;
    AgentSprite sprite = new AgentSprite();
    [SerializeField] Fire fire = new Fire(false);
    private List<Unit> nearbyUnits = new List<Unit>();
    public List<Unit> NearbyUnits => nearbyUnits;
    public float fireLightDist = 2f, fireCheckInterval = .5f;
    private float fireCheckTimer = 0f;
    private Chunk chunk;
    //public SphereCollider col;
    private WorldGrid grid;
    private ChunkStreaming chunkStreaming;
    private FireSystem fireSystem;
    public GameObject body;
    public Vector3 hoverPos = Vector3.zero;

    public void Init(GameContext context)
    {
        chunkStreaming = context.chunkStreaming;
        World world = context.world;
        grid = world.Context.grid;
        if (context.gameType == GameType.Game)
            fireSystem = context.fireSystem;

        controller = GetComponent<PlayerController>();
        controller.Init(context);

        sprite = new AgentSprite();
        if (sprite != null && body != null)
            sprite.Init(body.transform, null, agentObject);

        fire.Init(fireLight);
        health.Fill();
    }

    void Update()
    {
        UpdateChunk();

        


        if (fireSystem != null)
        {
            if (fireCheckTimer <= 0)
            {
                CheckFires();
            }
            else
            {
                fireCheckTimer -= Time.deltaTime;
            }
        }

        
        fire?.Update();
    }

    public void OnLookChanged(Quaternion rot)
    {
        sprite?.SetDirection(rot * Vector3.forward);
    }

    public void LookTo(Vector3 vec)
    {
        sprite?.SetDirection(vec);
    }

    FireBuilding FindFire()
    {
        fireCheckTimer = fireCheckInterval;

        FireBuilding closest = null;
        float closestDist = float.MaxValue;

        foreach (FireBuilding fire in fireSystem.Buildings)
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
        Chunk newChunk = grid.ChunkFromGridPos(GridPos);

        if (newChunk != chunk)
        {
            if (chunk != null) chunk.RemovePlayer();
            newChunk.AddPlayer(this);
            chunk = newChunk;

            // Update chunks (disable stale chunks and enable/create active chunks)
            chunkStreaming.UpdateChunks(chunk);
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    Unit unit = other.GetComponent<Unit>();

    //    if (unit != null && !nearbyUnits.Contains(unit))
    //    {
    //        nearbyUnits.Add(unit);
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    Unit unit = other.GetComponent<Unit>();

    //    if (unit != null && nearbyUnits.Contains(unit))
    //    {
    //        nearbyUnits.Remove(unit);
    //    }
    //}

    public void OnMove(Vector2 moveInput)
    {
        controller?.MovePlayer(moveInput);
    }

    public void OnHover(RaycastHit hit)
    {
        hoverPos = hit.point;
    }

}
