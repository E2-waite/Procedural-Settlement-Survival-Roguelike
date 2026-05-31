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


    [SerializeField] UnitSprite sprite = new UnitSprite();
    [SerializeField] public UnitMovement movement;
    [SerializeField] protected UnitCombat combat = new UnitCombat();

    const int pathRange = 50;

    public State state, lastState;
    public float chunkInterval = 1f, chunkTimer = 0f;
    public float swarmRadius = .5f;
    [HideInInspector] public bool pathRequested = false;

    private Camera cam;

    public Chunk chunk;

    protected float scanInterval = 1.0f, scanTimer = 0; // Timer for tracking when to next scan for nearby friendly units
    protected List<Unit> nearbyUnits;

    protected virtual void Start()
    {
        movement = new UnitMovement(this);

        // Face the camera
        cam = Camera.main;
        Vector3 forward = cam.transform.forward;
        forward.Normalize();
        transform.rotation = Quaternion.LookRotation(forward);

        health.Fill();

        state = State.Idle;
        lastState = State.Idle;
    }

    protected virtual void Update()
    {
        if (IsDead) return; // Dead

        StateHandling();
        UpdateChunk();

        sprite.SetDirection(movement.MoveDir());

        if (combat != null)
        {
            combat.Update();
        }
    }

    // Updates chunk state (adds unit to new chunk and removes unit from old chunk)
    void UpdateChunk()
    {
        if (chunkTimer <= 0)
        {
            chunkTimer = chunkInterval;

            Chunk newChunk = WorldHandler.grid.ChunkFromGridPos(GridPos());
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
    protected virtual void SetState(State newState)
    {
        lastState = state;
        state = newState;
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

    }

    protected virtual void FollowState()
    {

    }

    protected virtual void CombatState()
    {
        if (combat != null)
        {
            combat.UpdateState();
            combat.ExecuteState();
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

    // Delayed death
    IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        Debug.Log(name + " should die");
        Destroy(gameObject);
    }
    #endregion

    #region Targeting
    // Targets the passed unit and sets state to attacking
    protected virtual void TargetUnit(Unit unit)
    {
        if (combat != null && unit != null)
        {
            combat.Target(unit);

            SetState(State.Combat);

            Vector2Int playerPos = new Vector2Int(Mathf.FloorToInt(unit.transform.position.x), Mathf.FloorToInt(unit.transform.position.z));

            RequestPath(combat.TargetPos(), unit.transform.position);
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

    // Scans chunk area for all nearby valid units
    public virtual Unit ScanForHostile(bool onKill = false)
    {
        if (scanTimer > 0) scanTimer -= Time.deltaTime;

        Unit closestUnit = null;
        if (scanTimer <= 0 || onKill)
        {
            scanTimer = scanInterval;

            if (chunk != null)
            {
                nearbyUnits = GetNearbyHostile();

                if (nearbyUnits == null) return null;

                float closestDist = float.MaxValue;
                foreach (Unit unit in nearbyUnits)
                {
                    float dist = Vector3.Distance(transform.position, unit.transform.position);

                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        closestUnit = unit;
                    }
                }
            }
        }

        return closestUnit;
    }
    #endregion

    public void RequestPath(Vector2Int target, Vector3 worldPos)
    {
        Vector2Int start = GridPos();
        Vector3 targetPos = worldPos;

        pathRequested = true;
        int size = pathRange * 2;

        bool[,] pathable = new bool[size, size];

        Vector2Int origin = new Vector2Int(
                            start.x - pathRange,
                            start.y - pathRange);

        // Define pathing area
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                Vector2Int tilePos = new Vector2Int(origin.x + x, origin.y + y);

                GridTile tile = WorldHandler.grid.GetTile(tilePos);

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
                // Updates the path on callback
                movement.SetPath(path);
                pathRequested = false;
            }
        };

        // Send pathfinding request
        PathfindingHandler.Instance.RequestPath(request);
    }

    public bool WaitingForPath()
    {
        return pathRequested;
    }
}
