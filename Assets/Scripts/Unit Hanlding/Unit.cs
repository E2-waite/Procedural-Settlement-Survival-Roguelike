using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyUnit;
using static FighterUnit;
using static UnityEditorInternal.VersionControl.ListControl;
using static UnityEngine.GraphicsBuffer;
using static WorkerUnit;

public class Unit : PathAgent
{
    public enum State
    {
        Idle,
        Moving,
        Following,
        Fighting,
        Gathering,
        Storing,
        Building
    }

    public State state, lastState;

    private Camera cam;
    public float currentHealth, maxHealth = 100;
    public float chunkInterval = 1f, chunkTimer = 0f;

    public UnitCombat combat;

    protected Chunk chunk;

    protected float scanInterval = 1.0f, scanTimer = 0; // Timer for tracking when to next scan for nearby friendly units
    protected List<Unit> nearbyUnits;


    protected virtual void Start()
    {
        // Face the camera
        cam = Camera.main;
        Vector3 forward = cam.transform.forward;
        forward.Normalize();
        transform.rotation = Quaternion.LookRotation(forward);

        currentHealth = maxHealth;

        state = State.Idle;
        lastState = State.Idle;
    }

    protected virtual void Update()
    {
        if (currentHealth <= 0) return; // Dead

        StateHandling();

        UpdateChunk();

        if (combat != null)
        {
            combat.Update();
        }

        //if (combatUnit)
        //{
        //    Unit nearbyUnit = ScanForUnits();
        //}
    }

    // Converts world position to grid position
    public Vector2Int GridPos()
    {
        return new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.z));

    }

    // Updates chunk state (adds unit to new chunk and removes unit from old chunk)
    void UpdateChunk()
    {
        if (chunkTimer <= 0)
        {
            chunkTimer = chunkInterval;

            Chunk newChunk = Grid.Instance.ChunkFromGridPos(GridPos());
            if (newChunk != null && newChunk != chunk)
            {
                if (chunk != null) chunk.RemoveUnit(this);

                newChunk.AddUnit(this);

                chunk = newChunk;
            }
        }
    }

    #region StateHandling

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

            case State.Fighting:
                FightState(); break;

            case State.Gathering:
                GatherState(); break;

            case State.Storing:
                StoreState(); break;

            case State.Building:
                BuildState(); break;
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

    protected virtual void FightState()
    {
        if (combat != null)
        {
            // Chase the target, or attack in range
            if (combat.HasTarget())
            {
                if (combat.InRange())
                {
                    combat.AttackTarget();
                }
                else
                {
                    if (!pathRequested && (currentPath == null || currentPath.Count == 0))
                    {
                        RequestPath(GridPos(), combat.TargetPos(), combat.target.transform.position);
                    }
                    FollowPath();
                }
            }
            else
            {
                SetState(State.Idle);
            }
        }
    }

    protected virtual void GatherState()
    {

    }

    protected virtual void StoreState()
    {

    }

    protected virtual void BuildState()
    {

    }

    #endregion

    #region HitHandling

    // Handles receiving hits from another unit. Returns true if target is dead
    public virtual bool Hit(float damage, Unit source)
    {
        if (currentHealth <= 0) return true; // Already dead

        Debug.Log(name + " hit by " + source.name + "(" + damage + " dmg)");

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
            return true;
        }
        return false;
    }

    // Handle death
    protected virtual void Die()
    {
        if (chunk != null)
        {
            chunk.RemoveUnit(this);
        }
        StartCoroutine(DeathRoutine());
    }

    // Delayed death
    IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        Debug.Log(name + " should die");
        Destroy(gameObject);
    }

    #endregion

    #region TargetHandling

    // Targets the passed unit and sets state to attacking
    protected virtual void TargetUnit(Unit unit)
    {
        if (combat != null && unit != null)
        {
            combat.SetTarget(unit);

            SetState(State.Fighting);

            Vector2Int playerPos = new Vector2Int(Mathf.FloorToInt(unit.transform.position.x), Mathf.FloorToInt(unit.transform.position.z));

            RequestPath(GridPos(), combat.TargetPos(), unit.transform.position);
        }


    }
    #endregion

    #region CombatHandling

    // Gets all nearby units (in the chunk area) - Fighters get enemies, Enemies get followers
    protected virtual List<Unit> GetNearbyUnits()
    {
        return null;
    }

    // TODO: get nearby when the chunk's units change, rather than continuously every second

    // Scans chunk area for all nearby valid units
    protected virtual Unit ScanForUnits(bool onKill = false)
    {
        if (scanTimer > 0) scanTimer -= Time.deltaTime;

        Unit closestUnit = null;
        if (scanTimer <= 0 || onKill)
        {
            scanTimer = scanInterval;

            if (chunk != null)
            {
                nearbyUnits = GetNearbyUnits();

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

    // Attacks the target unit
    //protected virtual void Attack()
    //{
    //    if (targetUnit != null)
    //    {
    //        attackTimer = attackInterval;

    //        if (targetUnit.Hit(attackDamage, this))
    //        {
    //            Unit nearbyUnit = ScanForUnits(true);

    //            if (nearbyUnit == null)
    //            {
    //                // Become idle if no nearby valid units
    //                SetState(Consts.IDLE_STATE);
    //            }
    //            else
    //            {
    //                // Target unit if nearby unit was found
    //                TargetUnit(nearbyUnit);
    //            }
    //        }
    //    }
    //}
    #endregion
}
