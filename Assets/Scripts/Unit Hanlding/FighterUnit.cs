using System.Collections.Generic;
using UnityEngine;

public class FighterUnit : FollowerUnit
{
    // YOU MUST ENSURE CONSISTENCY 0 = idle, 1 = moving, 2 = following
    public enum FighterState
    {
        Idle,
        Moving,
        Following,
        Fighting
    }

    public FighterState state;
    FighterState lastState;

    EnemyUnit targetEnemy;

    public float attackDist = 1f, attackDamage = 10f;

    float attackInterval = 0.5f, attackTimer = 0;
    float scanInterval = 1.0f, scanTimer = 0; // Timer for tracking when to next scan for nearby enemies

    List<EnemyUnit> nearbyEnemies;

    protected override void Start()
    {
        base.Start();
        state = FighterState.Idle;
        lastState = FighterState.Idle;
    }

    protected override int GetState()
    {
        return (int)state;
    }

    protected override void SetState(int newState)
    {
        lastState = state;
        state = (FighterState)newState;
    }

    void SetState(FighterState newState)
    {
        lastState = state;
        state = newState;
    }

    public override bool Hit(float damage, Unit source)
    {
        if (base.Hit(damage, source)) return true;

        if (source is EnemyUnit)
        {
            // Update target?
            TargetEnemy((EnemyUnit)source);
        }

        return false;
    }

    protected override void Update()
    {
        //ScanForEnemies();

        base.Update();
    }

    // TODO: get nearby when the chunk's units change, rather than continuously
    void ScanForEnemies()
    {
        if (scanTimer > 0) scanTimer -= Time.deltaTime;

        if (scanTimer <= 0)
        {
            scanTimer = scanInterval;

            if (chunk != null)
            {
                nearbyEnemies = chunk.GetEnemies();

                float closestDist = float.MaxValue;
                EnemyUnit enemyUnit = null;
                foreach (EnemyUnit enemy in nearbyEnemies)
                {
                    float dist = Vector3.Distance(transform.position, enemy.transform.position);

                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        enemyUnit = enemy;
                    }
                }

                if (enemyUnit != null)
                {
                    TargetEnemy(enemyUnit);
                }
            }
        }
    }

    protected override bool HandleStates()
    {
        if (attackTimer > 0) attackTimer -= Time.deltaTime;

        bool handled = base.HandleStates();
        if (handled) return true;

        if (state == FighterState.Fighting)
        {
            if (targetEnemy != null)
            {
                // Chase the enemy, or attack in range

                Vector2Int enemyPos = new Vector2Int(Mathf.FloorToInt(targetEnemy.transform.position.x), Mathf.FloorToInt(targetEnemy.transform.position.z));
                float dist = Vector3.Distance(transform.position, targetEnemy.transform.position);

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

                    RequestPath(currentPos, enemyPos, targetEnemy.transform.position);
                }
            }

            FollowPath();

            handled = true;
        }

        return handled;
    }

    // Command to interact with tile
    public override void Command(GridTile tile)
    {
        if (tile == null) return;
        bool handled = false;

        if (!handled)
            base.Command(tile);
    }

    // Command to interact with unit
    public override void Command(Unit unit)
    {
        if (unit == null || unit == this) return;

        if (unit is EnemyUnit)
        {
            TargetEnemy((EnemyUnit)unit);
        }
    }

    void TargetEnemy(EnemyUnit enemy)
    {
        targetEnemy = enemy;

        SetState(FighterState.Fighting);

        Vector2Int playerPos = new Vector2Int(Mathf.FloorToInt(enemy.transform.position.x), Mathf.FloorToInt(enemy.transform.position.z));

        RequestPath(GridPos(), playerPos, enemy.transform.position);
    }

    void Attack()
    {
        if (targetEnemy != null)
        {
            attackTimer = attackInterval;

            if (targetEnemy.Hit(attackDamage, this))
            {
                // Scan for next target if current target died
                ScanForEnemies();
            }
        }
    }
}
