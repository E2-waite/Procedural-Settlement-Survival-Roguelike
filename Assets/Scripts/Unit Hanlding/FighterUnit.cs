using UnityEngine;
using static WorkerUnit;

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

    protected override void Update()
    {
        base.Update();
    }

    protected override bool HandleStates()
    {
        bool handled = base.HandleStates();
        if (handled) return true;

        if (state == FighterState.Fighting)
        {
            if (targetEnemy != null)
            {
                // Chase the enemy, or attack in range

                Vector2Int playerPos = new Vector2Int(Mathf.FloorToInt(targetEnemy.transform.position.x), Mathf.FloorToInt(targetEnemy.transform.position.z));

                if (!pathRequested && (currentPath == null || currentPath.Count == 0) && Vector3.Distance(transform.position, targetEnemy.transform.position) > followDist)
                {
                    Vector2Int currentPos = new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.z));

                    RequestPath(currentPos, playerPos, targetEnemy.transform.position);
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
        Debug.Log("Commanding to: " + unit.ToString());

        if (unit == null || unit == this) return;

        if (unit is EnemyUnit)
        {
            TargetEnemy((EnemyUnit)unit);
        }
    }

    void TargetEnemy(EnemyUnit enemy)
    {
        Debug.Log("Should target enemy");

        targetEnemy = enemy;

        SetState(FighterState.Fighting);

        Vector2Int playerPos = new Vector2Int(Mathf.FloorToInt(enemy.transform.position.x), Mathf.FloorToInt(enemy.transform.position.z));

        RequestPath(GridPos(), playerPos, enemy.transform.position);


    }
}
