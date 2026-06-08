using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : Unit
{
    [SerializeField] public UnitCombat combat = new UnitCombat();
    public override UnitCombat Combat => combat;

    private EnemyTargetting targetting = new EnemyTargetting();
    public override Targetting Targetting => (Targetting)targetting;

    public float fireCheckInterval = .5f, fireDetectDist = 30f;
    private float fireCheckTimer = 0f;
    Player player;
    FireBuilding targetFire;
    EnemySystem enemySystem;
    MainFireBuilding mainFire;
    public EnemySettlement settlement;
    private GridTile spawnTile;
    public GridTile SpawnTile => spawnTile;

    public void Init(GameContext context, EnemySettlement settlement, GridTile spawnTile)
    {
        Init(context);
        enemySystem = context.enemySystem;
        enemySystem.AddEnemy(this);
        player = context.player;
        mainFire = context.mainFireBuilding;
        this.settlement = settlement;
        this.spawnTile = spawnTile;
        targetting.Init(context, this);
    }

    protected override void Update()
    {
        base.Update();

        targetting.Tick();
    }
    #region Taking Damage

    public override bool Hit(float damage, Damageable source)
    {
        if (base.Hit(damage, source)) return true;

        if (source is FighterUnit)
        {
            // Targets the unit that hit this enemy
            TargetUnit((FighterUnit)source);
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
    public override List<Unit> GetNearbyHostile()
    {
        if (chunk != null)
        {
            return chunk.GetFollowers();
        }

        return null;
    }

    // Gets nearby enemies units for swarming
    public override List<Unit> GetNearbyFriendly()
    {
        if (chunk != null)
        {
            return chunk.GetEnemies();
        }

        return null;
    }
    #endregion
}
