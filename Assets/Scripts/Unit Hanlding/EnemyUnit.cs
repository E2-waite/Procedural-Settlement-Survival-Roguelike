using System.Collections.Generic;
using UnityEngine;
using static FighterUnit;
using static UnityEditorInternal.VersionControl.ListControl;
using static WorkerUnit;

public class EnemyUnit : Unit
{
    protected override void Start()
    {
        base.Start();
        combat = new UnitCombat(this);

        EnemyHandler.Instance.AddEnemy(this);
    }

    protected override void Update()
    {
        base.Update();
    }

    #region HitHandling

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

    protected override void Die()
    {
        EnemyHandler.Instance.RemoveEnemy(this);
        base.Die();
    }
    #endregion

    #region CombatHandling
    // Gets nearby follower units for targetting
    protected override List<Unit> GetNearbyUnits()
    {
        if (chunk != null)
        {
            return chunk.GetFollowers();
        }

        return null;
    }
    #endregion
}
