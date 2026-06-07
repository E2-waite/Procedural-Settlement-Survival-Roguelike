using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : Damageable
{
    public enum State
    {
        Idle,
        Moving,
        Following,
        Combat,
        Work
    }

    public virtual UnitCombat Combat => null;

    [SerializeField] UnitSprite sprite = new UnitSprite();
    [SerializeField] public UnitMovement movement;
    public override TargetType Type => TargetType.Unit;
    public SpriteRenderer unitSprite;
    public SpriteRenderer markerSprite;
    //public SpriteRenderer hoverSprite;
    const int pathRange = 50;

    public State state, lastState;
    public float chunkInterval = 1f, chunkTimer = 0f;
    public float swarmRadius = .5f;
    [HideInInspector] public bool pathRequested = false;
    private Camera cam;
    public Chunk chunk;
    protected float scanInterval = 1.0f, scanTimer = 0; // Timer for tracking when to next scan for nearby friendly units
    protected List<Unit> nearbyUnits;
    private bool highlighted = false; // Is the mouse currently hovering over this unit
    private WorldGrid grid;
    protected FireSystem fireSystem;
    protected PathfindingHandler pathfinding;
    public virtual void Init(GameContext context)
    {
        World world = context.world;
        grid = world.Context.grid;
        fireSystem = context.fireSystem;
        pathfinding = context.pathfinding;

        movement.Init(this);
        sprite.Init(unitSprite);
        if (Combat != null) Combat.Init(this);

        health.Fill();

        state = State.Idle;
        lastState = State.Idle;
    }

    protected virtual void Start()
    {
        // Face the camera
        cam = Camera.main;
        Vector3 forward = cam.transform.forward;
        forward.Normalize();
        unitSprite.transform.rotation = Quaternion.LookRotation(forward);
    }

    protected virtual void Update()
    {
        if (IsDead) return; // Dead

        // Unit subclasses override the state hooks rather than replacing the whole update loop.
        StateHandling();
        UpdateChunk();

        sprite.SetDirection(movement.MoveDir());

        if (Combat != null)
        {
            Combat.Update();
        }
    }

    // Updates chunk state (adds unit to new chunk and removes unit from old chunk)
    void UpdateChunk()
    {
        if (chunkTimer <= 0)
        {
            chunkTimer = chunkInterval;

            // Chunk membership powers local enemy/friendly queries for combat and swarming.
            Chunk newChunk = grid.ChunkFromGridPos(GridPos());
            if (newChunk != null && newChunk != chunk)
            {
                if (chunk != null) chunk.RemoveUnit(this);

                newChunk.AddUnit(this);

                chunk = newChunk;
            }
        }
    }

    #region States
    // Generic SetState function for setting units' state to generic state (idle, moving, following and attacking)
    public virtual void SetState(State newState)
    {
        if (state != newState)
        {
            lastState = state;
            state = newState;
        }
    }

    // Generic GetState function for getting units' state as an int
    protected virtual State GetState()
    {
        return state;
    }

    public void SetIdle()
    {
        SetState(State.Idle);
    }

    public void StartCombat()
    {
        SetState(State.Combat);
    }

    void StateHandling()
    {
        switch(state)
        {
            case State.Idle:
                IdleState(); break;

            case State.Moving:
                MoveState(); break;

            case State.Following:
                FollowState(); break;

            case State.Combat:
                CombatState(); break;

            case State.Work:
                WorkingState(); break;
        }
    }

    protected virtual void IdleState()
    {
        // Do nothing
    }

    protected virtual void MoveState()
    {
        //// Becomes idle if unit reaches target tile
        //if (targetTile == null || Vector3.Distance(transform.position, targetTile.worldPosition) < .25f)
        //{
        //    SetIdle();
        //}
    }

    protected virtual void FollowState()
    {

    }

    protected virtual void CombatState()
    {
        if (Combat != null)
        {
            Combat.UpdateState();
            Combat.ExecuteState();
        }
    }

    protected virtual void WorkingState()
    {

    }
    #endregion

    #region Taking Damage
    // Handles receiving hits from another unit. Returns true if target is dead

    

    protected override void OnDeathStart()
    {
        if (chunk != null)
        {
            chunk.RemoveUnit(this);
        }

    }

    public override bool Hit(float damage, Damageable source)
    {
        bool died = base.Hit(damage, source);

        if (!died && Combat != null)
        {
            Combat.AddThreat(source, 5);
        }

        return died;
    }

    #endregion

    #region Targeting
    // Targets the passed unit and sets state to attacking
    protected virtual void TargetUnit(Unit unit)
    {
        if (Combat != null && unit != null)
        {
            Combat.Target(unit);

            SetState(State.Combat);

            Vector2Int playerPos = new Vector2Int(Mathf.FloorToInt(unit.transform.position.x), Mathf.FloorToInt(unit.transform.position.z));

            RequestPath(Combat.TargetPos(), unit.transform.position);
        }
    }
    #endregion


    #region Unit Detection

    // Gets all nearby units (in the chunk area) - Fighters get enemies, Enemies get followers
    public virtual List<Unit> GetNearbyHostile()
    {
        return null;
    }

    public virtual List<Unit> GetNearbyFriendly()
    {
        return null;
    }

    // TODO: get nearby when the chunk's units change, rather than continuously every second
    #endregion

    // Request a path to a position and set target tile
    public void RequestPath(GridTile tile)
    {
        movement.SetTargetTile(tile);
        RequestPath(tile.position, tile.worldPosition);
    }

    // Request a path to the position
    public void RequestPath(Vector2Int target, Vector3 worldPos)
    {
        movement.ClearPath();

        Vector2Int start = GridPos();
        Vector3 targetPos = worldPos;

        pathRequested = true;
        int size = pathRange * 2;

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

                if (tile != null && tile.Walkable())
                    pathable[x, y] = true;
                else
                    pathable[x, y] = false;
            }
        }

        // Construct the pathing request
        Pathfinding.PathRequest request = new Pathfinding.PathRequest
        {
            size = size,
            start = start,
            target = target,
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

    public bool WaitingForPath => pathRequested;


    public void Highlight(bool active)
    {
        highlighted = active;
        if (unitSprite != null)
            unitSprite.material.SetFloat("_OutlineThickness", highlighted ? 1f : 0f);
    }

    protected override IEnumerator HitCoroutine()
    {
        sprite.SetColor(Color.red);
        yield return new WaitForSeconds(0.1f);
        sprite.SetColor(Color.white);
    }
}
