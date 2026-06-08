using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : Unit
{
    [SerializeField] public UnitCombat combat = new UnitCombat();
    public override UnitCombat Combat => combat;

    public float fireCheckInterval = .5f, fireDetectDist = 30f;
    private float fireCheckTimer = 0f;
    Player player;
    FireBuilding targetFire;
    EnemySystem enemySystem;
    MainFireBuilding mainFire;
    DayNightSystem dayNightSystem;
    public EnemySettlement settlement;
    private GridTile spawnTile;
    private bool returning = false, targetting = false;
    public void Init(GameContext context, EnemySettlement settlement, GridTile spawnTile)
    {
        Init(context);
        enemySystem = context.enemySystem;
        enemySystem.AddEnemy(this);
        player = context.player;
        mainFire = context.mainFireBuilding;
        dayNightSystem = context.dayNightSystem;
        this.settlement = settlement;
        this.spawnTile = spawnTile;
    }

    protected override void Update()
    {
        base.Update();

        if (dayNightSystem.Phase == DayNightSystem.DayPhase.Night && !targetting)
        {
            // Target fire
            TargetFire();
        }
        else if (dayNightSystem.Phase == DayNightSystem.DayPhase.Day && !returning)
        {
            ReturnToSpawn();
        }    

        //if ((targetFire == null || targetFire.IsDead) && fireCheckTimer <= 0)
        //{
        //    targetFire = FindFire();
        //    if (targetFire != null)
        //    {
        //        //SetState(State.Combat);
        //        //combat.Target(targetFire);
        //    }
        //}
        //else
        //{
        //    fireCheckTimer -= Time.deltaTime;
        //}
    }

    FireBuilding FindFire()
    {
        fireCheckTimer = fireCheckInterval;

        FireBuilding closest = null;
        float closestDist = float.MaxValue;

        foreach (FireBuilding fire in fireSystem.Buildings)
        {
            float dist = Vector3.Distance(transform.position, fire.transform.position);
            if (dist < closestDist && dist < fireDetectDist)
            {
                closestDist = dist;
                closest = fire;
            }
        }

        return closest;
    }

    private void ReturnToSpawn()
    {
        Debug.Log(name + " returning to spawn");
        RequestPath(spawnTile);
        SetState(State.Moving);
        returning = true;
        targetting = false;
    }

    private void TargetFire()
    {
        if (player == null) return;
        if (Combat != null && mainFire != null)
        {
            Debug.Log(name + " targetting fire");

            targetting = true;
            returning = false;
            Combat.Target(mainFire);

            SetState(State.Combat);

            RequestPath(Combat.TargetPos(), mainFire.transform.position);
        }
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
