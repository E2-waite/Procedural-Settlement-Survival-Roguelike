using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyUnit;
using static UnityEditorInternal.VersionControl.ListControl;
using static WorkerUnit;

public class Unit : PathAgent
{
    private Camera cam;
    public float currentHealth, maxHealth = 100;
    public float chunkInterval = 1f, chunkTimer = 0f;
    protected Chunk chunk;

    public float attackDist = 1f, attackDamage = 10f;
    public bool combatUnit = false;
    protected float attackInterval = 0.5f, attackTimer = 0;

    protected float scanInterval = 1.0f, scanTimer = 0; // Timer for tracking when to next scan for nearby friendly units
    protected Unit targetUnit;
    protected List<Unit> nearbyUnits;


    protected virtual void Start()
    {
        // Face the camera
        cam = Camera.main;
        Vector3 forward = cam.transform.forward;
        forward.Normalize();
        transform.rotation = Quaternion.LookRotation(forward);

        currentHealth = maxHealth;
    }

    // Returns true if target is dead
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

    protected virtual void Die()
    {
        if (chunk != null)
        {
            chunk.RemoveUnit(this);
        }
        StartCoroutine(DeathRoutine());
    }

    IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        Debug.Log(name + " should die");
        Destroy(gameObject);
    }

    protected virtual void SetState(int newState)
    {
    }

    protected virtual int GetState()
    {
        return 0;
    }

    protected Vector2Int GridPos()
    {
        return new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.z));

    }

    protected virtual void Update()
    {
        if (currentHealth <= 0) return; // Dead

        HandleStates();

        UpdateChunk();

        if (combatUnit)
        {
            Unit nearbyUnit = ScanForUnits();
            //if (nearbyUnit != null)
            //{
            //    TargetUnit(nearbyUnit);
            //}
        }
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

    protected virtual bool HandleStates()
    {
        if (attackTimer > 0) attackTimer -= Time.deltaTime;

        bool handled = false;

        if (GetState() == Consts.ATTACKING_STATE)
        {
            if (targetUnit != null)
            {
                // Chase the target, or attack in range
                Vector2Int enemyPos = new Vector2Int(Mathf.FloorToInt(targetUnit.transform.position.x), Mathf.FloorToInt(targetUnit.transform.position.z));
                float dist = Vector3.Distance(transform.position, targetUnit.transform.position);

                if (dist <= attackDist)
                {
                    if (attackTimer <= 0)
                    {
                        // Attack if in range and timer has finished
                        Attack();
                    }
                }
                else if (!pathRequested && (currentPath == null || currentPath.Count == 0))
                {
                    Vector2Int currentPos = new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.z));

                    RequestPath(currentPos, enemyPos, targetUnit.transform.position);
                }
            }

            FollowPath();

            handled = true;
        }

        return handled;
    }

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

    // Targets the passed unit and sets state to attacking
    protected virtual void TargetUnit(Unit unit)
    {
        targetUnit = unit;

        SetState(Consts.ATTACKING_STATE);

        Vector2Int playerPos = new Vector2Int(Mathf.FloorToInt(unit.transform.position.x), Mathf.FloorToInt(unit.transform.position.z));

        RequestPath(GridPos(), playerPos, unit.transform.position);
    }

    // Attacks the target unit
    protected virtual void Attack()
    {
        if (targetUnit != null)
        {
            attackTimer = attackInterval;

            if (targetUnit.Hit(attackDamage, this))
            {
                Unit nearbyUnit = ScanForUnits(true);

                if (nearbyUnit != null)
                {
                    TargetUnit(nearbyUnit);
                }
            }
        }
    }
}
