using System.Collections.Generic;
using UnityEngine;
using static FighterUnit;
using static UnityEditorInternal.VersionControl.ListControl;
using static WorkerUnit;

public class EnemyUnit : Unit
{
    public enum EnemyState
    {
        Idle,
        Moving,
        Following,
        Fighting
    }

    public EnemyState state;
    EnemyState lastState;
    public float attackDist = 1f, attackDamage = 10f;
    float attackInterval = 0.5f, attackTimer = 0;
    float scanInterval = 1.0f, scanTimer = 0; // Timer for tracking when to next scan for nearby friendly units
    List<FollowerUnit> nearbyFollowers;
    Unit targetUnit;

    protected override void Start()
    {
        base.Start();
        state = EnemyState.Idle;
        lastState = EnemyState.Idle;

        EnemyHandler.Instance.AddEnemy(this);
    }

    protected override void Die()
    {
        EnemyHandler.Instance.RemoveEnemy(this);
        base.Die();
    }

    protected override int GetState()
    {
        return (int)state;
    }

    protected override void SetState(int newState)
    {
        lastState = state;
        state = (EnemyState)newState;
    }

    void SetState(EnemyState newState)
    {
        lastState = state;
        state = newState;
    }

    public override bool Hit(float damage, Unit source)
    {
        if (base.Hit(damage, source)) return true;

        if (source is FighterUnit)
        {
            // Targets the unit that hit this enemy
            TargetUnit(source);
        }

        return false;
    }

    protected override void Update()
    {
        ScanForFriendlies();
        base.Update();
    }

    void ScanForFriendlies()
    {
        if (scanTimer > 0) scanTimer -= Time.deltaTime;

        if (scanTimer <= 0)
        {
            scanTimer = scanInterval;

            if (chunk != null)
            {
                nearbyFollowers = chunk.GetFollowers();

                float closestDist = float.MaxValue;
                FollowerUnit followerUnit = null;
                foreach (FollowerUnit follower in nearbyFollowers)
                {
                    float dist = Vector3.Distance(transform.position, follower.transform.position);

                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        followerUnit = follower;
                    }
                }
            }
        }
    }
    protected override bool HandleStates()
    {
        if (attackTimer > 0) attackTimer -= Time.deltaTime;

        bool handled = base.HandleStates();
        if (handled) return true;

        if (state == EnemyState.Fighting)
        {
            if (targetUnit != null)
            {
                // Chase the enemy, or attack in range

                Vector2Int enemyPos = new Vector2Int(Mathf.FloorToInt(targetUnit.transform.position.x), Mathf.FloorToInt(targetUnit.transform.position.z));
                float dist = Vector3.Distance(transform.position, targetUnit.transform.position);

                if (dist <= attackDist)
                {
                    if (attackTimer <= 0)
                    {
                        // Gather if in range and timer has finished
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

    void TargetUnit(Unit unit)
    {
        targetUnit = unit;

        SetState(EnemyState.Fighting);

        Vector2Int playerPos = new Vector2Int(Mathf.FloorToInt(unit.transform.position.x), Mathf.FloorToInt(unit.transform.position.z));

        RequestPath(GridPos(), playerPos, unit.transform.position);
    }

    protected virtual void Attack()
    {
        if (targetUnit != null)
        {
            attackTimer = attackInterval;

            targetUnit.Hit(attackDamage, this);
        }
    }
}
