using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : Destructable
{
    private PathfindingHandler pathfinding;
    public PathfindingHandler Pathfinding => pathfinding;
    [SerializeField] public AgentMovement movement;
    public AgentMovement Movement => movement;

    [HideInInspector] public bool pathRequested = false;
    protected WorldGrid grid;
    public float chunkInterval = 1f, chunkTimer = 0f;
    public Chunk chunk;
    private bool highlighted = false; // Is the mouse currently hovering over this agent
    [SerializeField] AgentSprite sprite = new AgentSprite();
    public AgentSprite Sprite => sprite;
    public bool WaitingForPath => pathRequested;
    protected AgentSquad squad;

    public bool HasSquad => squad != null;
    public void SetSquad(AgentSquad squad) { this.squad = squad; }
    public AgentSquad Squad => squad;
    public void ClearSquad() { squad = null; }
    public GameObject projectilePrefab;
    protected virtual Color HighlightColor => Color.white;
    public GameObject body;

    public virtual void Init(GameContext context)
    {
        movement.Init(this);

        World world = context.world;
        grid = world.Context.grid;
        pathfinding = context.pathfinding;
    }

    protected virtual void Update()
    {
        UpdateChunk();
        sprite.SetDirection(movement.MoveDir());
        HandleStates();
    }

    // Request a path to a position and set target tile
    public void RequestPath(GridTile tile)
    {
        movement.SetTargetTile(tile);
        RequestPath(tile.position, tile.worldPosition);
    }

    // Request a path to the position
    public void RequestPath(Vector2Int gridPos, Vector3 worldPos, bool includeResources = false)
    {
        movement.ClearPath();

        Vector2Int start = GridPos;
        int pathRange = Mathf.RoundToInt(Vector2Int.Distance(GridPos, gridPos));
        pathRequested = true;
        int size = pathRange * 4;

        bool[,] pathable = new bool[size, size];

        Vector2Int origin = new Vector2Int(
                            start.x - pathRange,
                            start.y - pathRange);

        // Snapshot walkability around the unit so the worker thread does not read Unity state.
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                Vector2Int tilePos = new Vector2Int(origin.x + x, origin.y + y);

                GridTile tile = grid.GetTile(tilePos);
                if (tile == null || !tile.IsEmpty)
                    pathable[x, y] = false;
                else
                    pathable[x, y] = true;
            }
        }

        // Construct the pathing request
        Pathfinding.PathRequest request = new Pathfinding.PathRequest
        {
            size = size,
            start = start,
            target = gridPos,
            origin = origin,
            pathable = pathable,
            callback = (path) =>
            {
                // Callback runs on the main thread via PathfindingHandler.Update.
                movement.SetPath(path);
                pathRequested = false;
            }
        };

        // Send pathfinding request
        pathfinding.RequestPath(request);
    }

    // Updates chunk state (adds unit to new chunk and removes unit from old chunk)
    void UpdateChunk()
    {
        if (chunkTimer <= 0 && grid != null)
        {
            chunkTimer = chunkInterval;

            // Chunk membership powers local enemy/friendly queries for combat and swarming.
            Chunk newChunk = grid.ChunkFromGridPos(GridPos);
            if (newChunk != null && newChunk != chunk)
            {
                if (chunk != null) chunk.RemoveAgent(this);

                newChunk.AddAgent(this);

                chunk = newChunk;
            }
        }
    }

    protected virtual void HandleStates()
    {

    }

    protected virtual void TargetTileReached() { }


    // Targets the passed unit and sets state to attacking
    protected virtual void TargetAgent(Agent agent)
    {
       
    }

    // Gets all nearby units (in the chunk area) - Fighters get enemies, Enemies get followers
    public virtual List<Agent> GetNearbyHostile()
    {
        return null;
    }

    public virtual List<Agent> GetNearbyFriendly()
    {
        return null;
    }


    public void Highlight(bool active)
    {
        highlighted = active;
        //if (spriteRend != null)
        //{
        //    spriteRend.material.SetColor("_OutlineColor", HighlightColor);
        //    spriteRend.material.SetFloat("_OutlineThickness", highlighted ? 3f : 0f);
        //}
    }

    protected override void OnDeathStart()
    {
        if (chunk != null)
        {
            chunk.RemoveAgent(this);
        }
    }

    protected override IEnumerator HitRoutine(Vector3 dir)
    {
        sprite.ShowOverlay(true);
        StartCoroutine(KnockbackRoutine(dir));
        yield return new WaitForSeconds(0.1f);
        sprite.ShowOverlay(false);
    }

    protected virtual IEnumerator KnockbackRoutine(Vector3 dir)
    {
        float velocity = 10f;
        float falloff = 50f;
        dir.y = 0;
        dir.Normalize();
        while (velocity > 0)
        {
            transform.position = transform.position + dir * velocity * Time.deltaTime;
            velocity -= falloff * Time.deltaTime;

            yield return null;
        }
    }

    public void LaunchProjectile(Destructable target, float damage)
    {
        GameObject projectileObj = Instantiate(projectilePrefab, transform.position + new Vector3(0, .5f, 0), Quaternion.identity);
        Projectile projectile = projectileObj.GetComponent<Projectile>();
        projectile.Launch(this, target, damage);
    }
}
