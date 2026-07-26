using System.Collections.Generic;
using UnityEngine;
using static GlobalDefs;

public class Enemy : Agent
{
    public enum State
    {
        Idle,
        Moving,
        Combat
    }

    [SerializeField] private State state = State.Combat;
    public Faction faction = Faction.Enemy;
    [SerializeField] public AgentCombat combat = new AgentCombat();
    public AgentCombat Combat => combat;

    private EnemyTargetting targeting = new EnemyTargetting();
    public EnemyTargetting Targeting => targeting;

    public float fireCheckInterval = .5f, fireDetectDist = 30f;
    private float fireCheckTimer = 0f;
    Player player;
    FireBuilding targetFire;
    EnemySystem enemySystem;
    MainFireBuilding mainFire;
    public EnemySettlementBuilding settlement;
    private GridTile spawnTile;
    public GridTile SpawnTile => spawnTile;
    protected override Color HighlightColor => Color.red;
    public void Init(GameContext context, EnemySettlementBuilding settlement, GridTile spawnTile)
    {
        Init(context);
        enemySystem = context.enemySystem;
        enemySystem.AddEnemy(this);
        player = context.player;
        mainFire = context.mainFireBuilding;
        this.settlement = settlement;
        this.spawnTile = spawnTile;
        targeting.Init(context, this);
        roleType = AgentObject.RoleType.Melee;
        Sprite.Init(spriteObj.transform, outlineObj.transform, context.agentCatalog.enemy);
        Combat?.Init(this, Targeting, CombatType.Melee);
        health.Fill();
    }

    protected override void Update()
    {
        if (IsDead) return;
        base.Update();

        Combat?.Tick();
        targeting?.Tick();
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

    public void OrderTo(Building building)
    {
        Targeting?.AddTarget(building, 100f);
    }


    #region Taking Damage

    public override bool OnHit(Destructable source, float damage, Vector3 dir)
    {
        if (base.OnHit(source, damage, dir)) return true;

        if (source is Unit)
        {
            // Targets the unit that hit this enemy
            Targeting.AddThreat(source, damage);
            SetState(State.Combat);
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
    // Gets nearby follower units for targeting
    public override List<Agent> GetNearbyHostile()
    {
        if (chunk != null)
        {
            return chunk.Data.GetUnits();
        }

        return null;
    }

    // Gets nearby enemies units for swarming
    public override List<Agent> GetNearbyFriendly()
    {
        if (chunk != null)
        {
            return chunk.Data.GetEnemies();
        }

        return null;
    }
    #endregion
}
