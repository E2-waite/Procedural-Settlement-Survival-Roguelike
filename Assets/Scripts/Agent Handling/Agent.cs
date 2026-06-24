using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AgentObject;

public class Agent : Destructable
{
    protected RoleType roleType = RoleType.None;
    public RoleType AgentType => roleType;

    public int SquadIndex { get; set; } = -1; // Index in squad's agents list
    public int SquadTypeIndex { get; set; } = -1; // Index in squad's specific agent type list

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
    public GameObject spriteObj;
    public GameObject outlineObj;
    public Vector3 FormationPos { get; set; } = Vector3.zero;
    public override Vector2Int GridPos => new Vector2Int(Mathf.FloorToInt(body.transform.position.x), Mathf.FloorToInt(body.transform.position.z));
    public override Vector3 WorldPos => body.transform.position;
    public virtual bool Commanding => false;

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
        HandleStates();
    }

    public void RequestPath(GridTile tile)
    {
        Movement.SetTargetPos(tile.worldPosition);

        RequestPath(tile.position);
    }

    // Request a path to a position and set target tile
    public void RequestPath(GridTile tile, Vector3 worldPos)
    {
        Movement.SetTargetPos(worldPos);
        Movement.SetTargetTile(tile);

        RequestPath(tile.position);
    }

    public void RequestPath(Vector3 worldPos)
    {
        Movement.SetTargetPos(worldPos);
        Vector2Int gridPos = new Vector2Int(
            Mathf.FloorToInt(worldPos.x),
            Mathf.FloorToInt(worldPos.z));

        RequestPath(grid.GetTile(gridPos), worldPos);
    }

    // Request a path to the position
    public void RequestPath(Vector2Int gridPos, bool includeTarget = false)
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

                if (includeTarget && tilePos == gridPos)
                {
                    pathable[x, y] = true;
                }
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


    public void Highlight(Color color)
    {
        if (squad == null)
        {
            HighlightSelf(color);
        }
        else
        {
            if (squad.Faction == GlobalDefs.Faction.Friendly)
                squad.Highlight(color);
            else
                HighlightSelf(color);
        }
           
    }

    public void HighlightSelf(Color color)
    {
        highlighted = true;
        sprite.ShowOutline(true);
        sprite.OutlineColor(color);
    }

    public void ClearHighlight()
    {
        if (squad == null)
        {
            ClearHighlightSelf();
        }
        else
        {
            squad.ClearHighlight();
        }
    }

    public void ClearHighlightSelf()
    {
        highlighted = false;
        sprite.ShowOutline(false);    
    }

    protected override void OnDeathStart()
    {
        if (chunk != null)
        {
            chunk.RemoveAgent(this);
            squad?.RemoveAgent(this);
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
            body.transform.position = body.transform.position + dir * velocity * Time.deltaTime;
            velocity -= falloff * Time.deltaTime;

            yield return null;
        }
    }

    public void LaunchProjectile(Destructable target, float damage)
    {
        GameObject projectileObj = Instantiate(projectilePrefab, WorldPos + new Vector3(0, .5f, 0), Quaternion.identity);
        Projectile projectile = projectileObj.GetComponent<Projectile>();
        projectile.Launch(this, target, damage);
    }

    public void LookTo(Vector3 vec)
    {
        Sprite?.SetDirection(vec);
    }
}
