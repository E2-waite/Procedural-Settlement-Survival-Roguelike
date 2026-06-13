using System.Collections.Generic;
using UnityEngine;

public class Enemy : Agent
{
    public enum State
    {
        Idle,
        Moving,
        Combat
    }
    private State state = State.Idle;

    [SerializeField] public AgentCombat combat = new AgentCombat();
    public AgentCombat Combat => combat;

    private EnemyTargetting targetting = new EnemyTargetting();
    public EnemyTargetting Targeting => targetting;

    public float fireCheckInterval = .5f, fireDetectDist = 30f;
    private float fireCheckTimer = 0f;
    Player player;
    FireBuilding targetFire;
    EnemySystem enemySystem;
    MainFireBuilding mainFire;
    public EnemySpawnerBuilding settlement;
    private GridTile spawnTile;
    public GridTile SpawnTile => spawnTile;
    protected override Color HighlightColor => Color.red;

    public void Init(GameContext context, EnemySpawnerBuilding settlement, GridTile spawnTile)
    {
        Init(context);
        enemySystem = context.enemySystem;
        enemySystem.AddEnemy(this);
        player = context.player;
        mainFire = context.mainFireBuilding;
        this.settlement = settlement;
        this.spawnTile = spawnTile;
        targetting.Init(context, this);
        Sprite.Init(spriteRend, eyesRend, context.agentCatalog.enemy);
    }

    protected override void Update()
    {
        if (IsDead) return;
        base.Update();

        Targeting?.Tick();
        Combat?.Tick();
    }

    public void SetState(State state)
    {
        if (this.state != state)
        {
            this.state = state;
        }
    }

    protected override void HandleStates()
    {
        switch (state)
        {
            case State.Idle:
                IdleState(); break;

            case State.Moving:
                MovingState(); break;

            case State.Combat:
                CombatState(); break;
        }
    }

    protected virtual void IdleState()
    {
        // Do nothing
    }

    // Moves to target position
    protected virtual void MovingState()
    {
        movement.FollowPath();

        if (movement.HasPath && movement.TargetReached)
        {
            TargetTileReached();
        }
    }

    protected virtual void CombatState()
    {
        if (Combat != null)
        {
            Combat.UpdateState();
            Combat.ExecuteState();
        }
    }


    #region Taking Damage

    public override bool Hit(Destructable source, float damage, Vector3 dir)
    {
        if (base.Hit(source, damage, dir)) return true;

        if (source is Unit)
        {
            // Targets the unit that hit this enemy
            Targeting.AddTarget(source, 10);
        }

        return false;
    }

    protected override void OnDeathStart()
    {
        base.OnDeathStart();

        enemySystem.RemoveEnemy(this);
    }
    #endregion

    #region Detecting Units
    // Gets nearby follower units for targetting
    public override List<Agent> GetNearbyHostile()
    {
        if (chunk != null)
        {
            return chunk.GetUnits();
        }

        return null;
    }

    // Gets nearby enemies units for swarming
    public override List<Agent> GetNearbyFriendly()
    {
        if (chunk != null)
        {
            return chunk.GetEnemies();
        }

        return null;
    }
    #endregion
}
